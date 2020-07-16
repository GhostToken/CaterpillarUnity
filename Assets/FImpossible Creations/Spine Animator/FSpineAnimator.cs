using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.FSpine
{
    /// <summary>
    /// FM: Main component for spine-chain-follow procedural animation
    /// </summary>
    [AddComponentMenu("FImpossible Creations/Spine Animator/FSpine Animator")]
    public class FSpineAnimator : MonoBehaviour, UnityEngine.EventSystems.IDropHandler, IFHierarchyIcon
    {
        public string EditorIconPath { get { return "Spine Animator/Spine Animator Icon"; } }
        public void OnDrop(UnityEngine.EventSystems.PointerEventData data) { }

        [Tooltip("Spine bones chain, always go from parent to children.")]
        public List<Transform> SpineTransforms;
        [Tooltip("Reference to main object which is looking forward towards orientation (blue Z axis)")]
        public Transform ForwardReference;

        #region MAIN CALCULATIONS DATA CONTINERS 

        /// <summary> List of invisible for editor points which represents ghost animation for spine </summary>
        private List<FSpine_Point> proceduralPoints;
        private List<FSpine_Point> ProceduralReferencePose;

        /// <summary> Helping points to support connecting spine animator's with unity animator and for debug view </summary>
        private List<FSpine_Point> helperProceduralPoints;

        /// <summary> Remember initial distances between tail transforms for right placement between tail segments during animation (will be also used in next versions to support object scalling) </summary>
        private List<float> initialBoneDistances;

        #endregion

        [Tooltip("Blend between procedural and animator's keyframed animation. 0 -> Full spine animator motion  1 -> No spine animator motion", order = 0)]
        [Range(0f, 1f)]
        public float BlendToOriginal = 0f;

        [Tooltip("If your spine lead bone is in beggining of your hierarchy chain, toggle it. Component's gizmos can help you out to define which bone should be leading (head gizmo when you switch this toggle).")]
        public bool LastBoneLeading = true;
        private bool reversedChangeFlag = false;
        [Tooltip("Depending on your setup, you should toggle this to true when changing default 'Last bone leading' value to support walking backwards for animals for example")]
        public bool BackwardMovement = false;

        private Vector3 lookUp = Vector3.up;

        public enum EFChainMethod { Universal, InversedVertical, AxisBased, AxisBasedInversed, Deprecated }
        private EFChainMethod chainMethodChangeFlag;
        public bool ReverseOrder = false;
        [Tooltip("Depending on your skeleton bones' axes there can be needed other chain method for the algorithm to avoid wrong spine motion.")]
        public EFChainMethod ChainMethod = EFChainMethod.Universal;

        [Tooltip("If you want tail animator motion to be connected with keyframed animation motion, don't use this when your object isn't animated (red highlight signalize that there is not enabled Unity's animator on the character)")]
        public bool ConnectWithAnimator = true;

        [Tooltip("If you want tail animator to stop computing when choosed mesh is not visible in any camera view (editor's scene camera is detecting it too)")]
        public bool OptimizeWithMesh = false;
        public Renderer VisibilityRenderer = null;

        [Tooltip("In most cases it work just right with this option disabled, but in some situations it can give you better results when you enable animating leading bone")]
        public bool AnimateLeadingBone = false;
        [Tooltip("If animated leading bone translation should be added after spine motion - to use in case model is using position frames animation's grounding")]
        public bool LeadingAnimateAfterMotion = false;

        [Tooltip("If you using animate physics on animator you should set this variable enabled")]
        public bool PhysicalUpdate = false;

        [Tooltip("Safe delta can eliminate some chopping when framerate isn't stable")]
        public bool SafeDeltaTime = true;
        protected float deltaTime = 0.016f;

        [Tooltip("When animation should be hardly precise to main transform motion, untoggle it when you using it only on part of chractesr body, not on main movement transform")]
        public bool AnchorToThis = true;
        [Tooltip("Finding correction rotation automatically by algorithm")]
        public bool AutoAnchor = false;
        [Tooltip("Connecting root translation to given transform, useful when you use limb, then you should put here parent of first bone and probably enable QueueToLastUpdate")]
        public Transform AnchorRoot = null;
        [Tooltip("If you find out that debug tail pose have wrong rotation")]
        public Vector3 CustomAnchorRotationOffset = Vector3.zero;
        [Tooltip("If you need to offset leading bone rotation")]
        public Vector3 LeadBoneRotationOffset = Vector3.zero;

        [Tooltip("Useful when you use few spine animators and want to rely on animated position and rotation by other spine animator")]
        public bool QueueToLastUpdate = false;

        [Tooltip("When your keyframed animation don't have keys on position track (common case) or you using not animated object with spine animator")]
        public bool PositionsNotAnimated = true;
        public List<bool> SelectivePosNotAnimated;
        [Tooltip("When your keyframed animation don't have keys on rotation track (rare case) or you using not animated object with spine animator")]
        public bool RotationsNotAnimated = false;
        public List<bool> SelectiveRotNotAnimated;
        public List<bool> SegmentCollision;

        #region MAIN CALCULATIONS VARIABLES

        /// <summary> Depends of bones' hierarchy structure, sometimes you will need leading bone in reversed position than default, this variable will help handling it in code </summary>
        private int leadingBoneIndex;

        /// <summary> Helper variable to reverse some variables when using reversed spine feature </summary>
        private int reverser = 1;
        private int adjuster = 1;

        /// <summary> Initial or frame freeze coordinates for bones </summary>
        private List<FSpine_Point> staticCoordinates;

        /// <summary> Variables which are detected at Init() and allowing to configure tail follow rotations to look correctly independently of how bones rotations orientations are set </summary>
        public FSpine_FixingSet spineRotationFixingSet;
        public FSpine_FixingSet.FSpine_Axis customAnchorAxis;
        private Vector3 customAnchorInitLocPos = Vector3.zero;
        //private Quaternion customAnchorInitLocRot;

        [Tooltip("Advanced option, when some of your spine bones going in opposite way that they should, you can adjust it here sometimes")]
        public List<Vector3> lookDirectionsAdditiveCorrection;

        #endregion

        [Tooltip("If corrections should affect spine chain")]
        public bool ManualAffectChain = false;
        [Tooltip("Component is made to work universally on many sets of skeletons, but there can exist small offsets which you can correct using this variables")]
        public List<Vector3> ManualRotationOffsets;
        [Tooltip("Component is made to work universally on many sets of skeletons, but there can exist small offsets which you can correct using this variables")]
        public List<Vector3> ManualPositionOffsets;

        [Tooltip("When start we doing precise calculations for fixing rotations, but in some cases rounding this values can do job better")]
        public bool RoundCorrection = false;
        private bool wasRoundCorrection = false;
        [Tooltip("Using only one axis correction direction for all segments")]
        public bool UnifyCorrection = false;
        private bool wasUnified = false;

        [Tooltip("Often when you drop model to scene, it's initial pose is much different than animations, which causes problems, this toggle solves it at start")]
        public bool StartAfterTPose = true;


        #region Extra helper variables

        /// <summary> Initial rotation of base transform, important to calculating hard static referene poses </summary>
        private Vector3 previousScale;

        /// <summary> Generated transforms to help connecting motion with animator and support model scalling </summary>
        private Transform[] anchorHelpers;
        private Transform anchorsContainer;

        /// <summary> Flag to detect if we was animating object with full blend to source animation </summary>
        private bool wasSourceAnimation = false;

        /// <summary> Flag to define if component was initialized already for more controll </summary>
        private bool initialized = false;

        /// <summary> Variable to calculate difference in last frame position to current, needed for some straigtening calculations when bone is in move</summary>
        private Vector3 previousPos;

        /// <summary> If we want to sync some wrong hierarched bones </summary>
        private List<FSpineBoneConnector> connectors;

        #endregion


        #region Animation settings and limitations

        [Range(0f, 1f)]
        [Tooltip("If animation of changing segments position should be smoothed - creating a little gumy effect")]
        public float PosSmoother = 0f;
        [Range(0f, 1f)]
        [Tooltip("If animation of changing segments rotation should be smoothed - making it more soft, but don't overuse it")]
        public float RotSmoother = 0f;
        [Range(0f, 1f)]
        [Tooltip("We stretching segments to bigger value than bones are by default to create some extra effect which looks good but sometimes it can stretch to much if you using position smoothing, you can adjust it here")]
        public float MaxStretching = 1f;
        [Tooltip("Making algorithm referencing back to more stable rotation if value = 0f | at 1 motion have more range and is more slithery")]
        [Range(0f, 1f)]
        public float Slithery = 1f;

        [Range(1f, 90f)]
        [Tooltip("Limiting rotation angle difference between each segment of spine but consider using StraightenSpeed variable for smoother effect")]
        public float AngleLimit = 90f;
        [Range(0f, 1f)]
        [Tooltip("Smoothing how fast limiting should make segments go back to marginal pose")]
        public float LimitSmoother = .35f;
        [Range(0f, 15f)]
        [Tooltip("How fast spine should be rotated to straight pose when it moves, higher angle limit - straigtening should be lower (behave different than GoBackSpeed)")]
        public float StraightenSpeed = 0f;
        public bool TurboStraighten = false;

        [Tooltip("Spine going back to straight position with choosed speed intensity")]
        [Range(0f, 1f)]
        public float GoBackSpeed = 0f;

        [Tooltip("Elastic spring effect good for tails to make them more 'meaty'")]
        [Range(0f, 1f)]
        public float Springiness = 0.0f;


        // V1.0.1
        [Tooltip("Sometimes offsetting model's pivot position gives better results using spine animator, offset forward axis so front legs are in centrum and see the difference (generating additional transform inside hierarchy)")]
        public Vector3 MainPivotOffset = new Vector3(0f, 0f, 0f);
        [HideInInspector] // As public because there was troubles with loosing reference when entering playmode and going back
        public Transform mainPivotOffsetTransform;

        [Tooltip("<! Most models can not need this !> Offset for bones rotations, thanks to that animation is able to rotate to segments in a correct way, like from center of mass")]
        public Vector3 SegmentsPivotOffset = new Vector3(0f, 0f, 0f);

        [Tooltip("Multiplies distance value between bones segments - can be useful for use with humanoid skeletons")]
        public float DistancesMultiplier = 1f;

        #endregion


        #region Incorrection detection
#if UNITY_EDITOR
        [HideInInspector]
        public int getReminders = 0;
        [HideInInspector]
        public bool wasIncorrectRemind = false;
        [HideInInspector]
        public bool incorrectionWarning = false;
        private int incorrectionCounter = 5;
        private float incorrectionSum = 0f;
        private float incorrectionSumPos = 0f;
        private Quaternion preIncorrect = Quaternion.identity;
#endif

        #endregion


        #region Initialization methods

        void Reset()
        {
            ForwardReference = FindForwardReference();
        }

        void Init()
        {
            if (initialized) return;

            if (ForwardReference == null) ForwardReference = FindForwardReference();

            // Getting bones transforms which will be animated by component
            ConfigureBonesTransforms();

            // Generating animation points for ghost translations
            PrepareSpinePoints();

            // Computing variables needed to hold motion
            ComputePredefinedVariables();

            CollidersDataToCheck = new List<FImp_ColliderData_Base>();

            // Flag for refresging variables if we do changes in playmode for tweaking
            reversedChangeFlag = LastBoneLeading;
            previousScale = transform.localScale;
            chainMethodChangeFlag = ChainMethod;

            initialized = true;

            RefreshRefDirsOnReverse();

            // Straightening spine pose to desired positions and rotations
            ReposeSpine();

            SpineMotion();
        }


        /// <summary>
        /// Precomputing spine look directions for more correct rotations of bones
        /// </summary>
        protected void ComputePredefinedVariables()
        {
            staticCoordinates = new List<FSpine_Point>();

            for (int i = 0; i < SpineTransforms.Count; i++)
                staticCoordinates.Add(new FSpine_Point { Index = i, Transform = SpineTransforms[i], Position = SpineTransforms[i].localPosition, Rotation = SpineTransforms[i].localRotation });

            RefreshDistances();

            spineRotationFixingSet = new FSpine_FixingSet().Init(SpineTransforms.Count);

            // Finding Up Left and Right Vector in bone orientation
            for (int i = 0; i < SpineTransforms.Count; i++)
            {
                Transform parentTr, childTr;
                Vector3 childPos, parentPos;

                #region Detecting parent and child at the end of the array

                if (i == SpineTransforms.Count - 1)
                {
                    if (SpineTransforms[i].childCount == 1)
                    {
                        parentTr = SpineTransforms[i];
                        childTr = SpineTransforms[i].GetChild(0);
                        parentPos = parentTr.position;
                        childPos = childTr.position;
                    }
                    else
                    {
                        parentTr = SpineTransforms[i];
                        childTr = SpineTransforms[i];

                        parentPos = SpineTransforms[i - 1].position;
                        childPos = SpineTransforms[i].position;
                    }
                }
                else
                {
                    parentTr = SpineTransforms[i];
                    childTr = SpineTransforms[i + 1];
                    parentPos = parentTr.position;
                    childPos = childTr.position;
                }

                #endregion

                Vector3 forwardInBoneOrientation = parentTr.InverseTransformDirection(childPos) - parentTr.InverseTransformDirection(parentPos);
                Vector3 projectedUp = Vector3.ProjectOnPlane(ForwardReference.up, SpineTransforms[i].TransformDirection(forwardInBoneOrientation).normalized).normalized;
                Vector3 upInBoneOrientation = parentTr.InverseTransformDirection(parentPos + projectedUp) - parentTr.InverseTransformDirection(parentPos);
                Vector3 crossRight = Vector3.Cross(SpineTransforms[i].TransformDirection(forwardInBoneOrientation), SpineTransforms[i].TransformDirection(upInBoneOrientation));
                Vector3 rightInBoneOrientation = parentTr.InverseTransformDirection(parentPos + crossRight) - parentTr.InverseTransformDirection(parentPos);

                FSpine_FixingSet.FSpine_Axis axis = new FSpine_FixingSet.FSpine_Axis(rightInBoneOrientation, upInBoneOrientation, forwardInBoneOrientation);
                spineRotationFixingSet.Axes.Add(axis);

                //forwardInBoneOrientation = parentTr.InverseTransformDirection(parentPos) - parentTr.InverseTransformDirection(childPos);

                // Reversed Stage
                #region Detecting parent and child at the end of the array

                if (i == 0)
                {
                    childTr = SpineTransforms[i];
                    parentTr = SpineTransforms[i + 1];
                    parentPos = parentTr.position;
                    childPos = childTr.position;
                }
                else
                {
                    childTr = SpineTransforms[i - 1];
                    parentTr = SpineTransforms[i];
                    parentPos = parentTr.position;
                    childPos = childTr.position;
                }

                #endregion

                forwardInBoneOrientation = parentTr.InverseTransformDirection(childPos) - parentTr.InverseTransformDirection(parentPos);
                projectedUp = Vector3.ProjectOnPlane(ForwardReference.up, SpineTransforms[i].TransformDirection(forwardInBoneOrientation).normalized).normalized;
                upInBoneOrientation = parentTr.InverseTransformDirection(parentPos + projectedUp) - parentTr.InverseTransformDirection(parentPos);
                crossRight = Vector3.Cross(SpineTransforms[i].TransformDirection(forwardInBoneOrientation), SpineTransforms[i].TransformDirection(upInBoneOrientation));
                rightInBoneOrientation = parentTr.InverseTransformDirection(parentPos + crossRight) - parentTr.InverseTransformDirection(parentPos);

                axis = new FSpine_FixingSet.FSpine_Axis(rightInBoneOrientation, upInBoneOrientation, forwardInBoneOrientation);
                spineRotationFixingSet.AxesReversed.Add(axis);
            }

            #region Fixing set for anchor

            if (AnchorRoot)
            {
                Transform parentTr, childTr;
                Vector3 childPos, parentPos;
                parentTr = AnchorRoot;
                childTr = SpineTransforms[0];
                childPos = childTr.position;
                parentPos = parentTr.position;

                Vector3 forwardInBoneOrientation = parentTr.InverseTransformDirection(childPos) - parentTr.InverseTransformDirection(parentPos);
                Vector3 projectedUp = Vector3.ProjectOnPlane(ForwardReference.up, childTr.TransformDirection(forwardInBoneOrientation).normalized).normalized;
                Vector3 upInBoneOrientation = parentTr.InverseTransformDirection(parentPos + projectedUp) - parentTr.InverseTransformDirection(parentPos);
                Vector3 crossRight = Vector3.Cross(childTr.TransformDirection(forwardInBoneOrientation), childTr.TransformDirection(upInBoneOrientation));
                Vector3 rightInBoneOrientation = parentTr.InverseTransformDirection(parentPos + crossRight) - parentTr.InverseTransformDirection(parentPos);

                customAnchorAxis = new FSpine_FixingSet.FSpine_Axis(rightInBoneOrientation, upInBoneOrientation, forwardInBoneOrientation);
            }
            else
                customAnchorAxis = new FSpine_FixingSet.FSpine_Axis();

            #endregion

            RefreshManualPosOffs();
            RefreshManualRotOffs();
            RefreshSelectivePosNotAnimated();
            RefreshSelectiveRotNotAnimated();
            RefreshDisabledSegmentsCollisions();
            RefreshCollidersOffsets();

            spineRotationFixingSet.Rounded.Clear();
            spineRotationFixingSet.RoundedReversed.Clear();

            // Look directions defined
            for (int i = 0; i < spineRotationFixingSet.Axes.Count; i++)
            {
                spineRotationFixingSet.Rounded.Add(new FSpine_FixingSet.FSpine_Axis());
                spineRotationFixingSet.Rounded[i].Right = RoundToBiggestValue(spineRotationFixingSet.Axes[i].Right);
                spineRotationFixingSet.Rounded[i].Up = RoundToBiggestValue(spineRotationFixingSet.Axes[i].Up);
                spineRotationFixingSet.Rounded[i].Forward = RoundToBiggestValue(spineRotationFixingSet.Axes[i].Forward);
                spineRotationFixingSet.RoundedReversed.Add(new FSpine_FixingSet.FSpine_Axis());
                spineRotationFixingSet.RoundedReversed[i].Right = RoundToBiggestValue(spineRotationFixingSet.AxesReversed[i].Right);
                spineRotationFixingSet.RoundedReversed[i].Up = RoundToBiggestValue(spineRotationFixingSet.AxesReversed[i].Up);
                spineRotationFixingSet.RoundedReversed[i].Forward = RoundToBiggestValue(spineRotationFixingSet.AxesReversed[i].Forward);
            }

            lookDirectionsAdditiveCorrection = new List<Vector3>();

            for (int i = 0; i < SpineTransforms.Count; i++) lookDirectionsAdditiveCorrection.Add(Vector3.zero);

            wasRoundCorrection = !RoundCorrection;
        }


        /// <summary>
        /// Generating ghost points for animating spine segments
        /// </summary>
        protected virtual void PrepareSpinePoints()
        {
            helperProceduralPoints = new List<FSpine_Point>();
            proceduralPoints = new List<FSpine_Point>();
            ProceduralReferencePose = new List<FSpine_Point>();

            for (int i = 0; i < SpineTransforms.Count; i++)
            {
                helperProceduralPoints.Add(new FSpine_Point
                {
                    Position = SpineTransforms[i].position,
                    Rotation = SpineTransforms[i].rotation
                });

                proceduralPoints.Add(new FSpine_Point
                {
                    Index = i,
                    Transform = SpineTransforms[i],
                    Position = SpineTransforms[i].position,
                    Rotation = SpineTransforms[i].rotation
                });

                ProceduralReferencePose.Add(new FSpine_Point
                {
                    Position = proceduralPoints[i].Position,
                    Rotation = proceduralPoints[i].Rotation
                });
            }

            anchorsContainer = new GameObject(name + "-SpineAnimator-AnchorsContainer").transform;
            anchorsContainer.SetParent(transform, true);
            anchorsContainer.localPosition = Vector3.zero;
            anchorsContainer.localRotation = Quaternion.identity;
            anchorsContainer.localScale = Vector3.one;

            anchorHelpers = new Transform[SpineTransforms.Count];
            for (int i = 0; i < SpineTransforms.Count; i++)
            {
                Transform anchorHelper;
                anchorHelper = new GameObject(name + "-Spine Helper [" + i + "] - " + SpineTransforms[i].name).transform;
                anchorHelper.localScale = SpineTransforms[i].lossyScale;
                anchorHelper.SetParent(anchorsContainer, true);
                anchorHelper.position = SpineTransforms[i].position;
                anchorHelper.rotation = SpineTransforms[i].rotation;
                anchorHelpers[i] = anchorHelper;
            }
        }


        /// <summary>
        /// Auto collect spine transforms if they're not defined from inspector
        /// also this is place for override and configure more
        /// </summary>
        protected virtual void ConfigureBonesTransforms()
        {
            if (SpineTransforms == null) SpineTransforms = new List<Transform>();

            if (SpineTransforms.Count < 2)
            {
                Transform lastParent = transform;

                bool boneDefined = true;

                if (SpineTransforms.Count == 0)
                {
                    boneDefined = false;
                    lastParent = transform;
                }
                else lastParent = SpineTransforms[0];

                Transform rootTransform = lastParent;

                // 100 iterations because I am scared of while() loops :O so limit to 100 or 1000 if anyone would ever need
                for (int i = SpineTransforms.Count; i < 100; i++)
                {
                    if (boneDefined)
                        if (lastParent == rootTransform)
                        {
                            lastParent = lastParent.GetChild(0);
                            continue;
                        }

                    SpineTransforms.Add(lastParent);

                    if (lastParent.childCount > 0) lastParent = lastParent.GetChild(0); else break;
                }
            }
        }


        void OnEnable()
        {
            if (PhysicalUpdate)
                StartCoroutine("LateFixedUpdate");
        }

        void OnDisable()
        {
            StopCoroutine("LateFixedUpdate");
        }

        #endregion


        #region After initial helper methods

        /// <summary>
        /// Method to initialize component, to have more controll than waiting for Start() method, init can be executed before or after start, as programmer need it.
        /// </summary>
        protected void Start()
        {
            if (QueueToLastUpdate)
            {
                enabled = false;
                enabled = true;
            }

            if (!StartAfterTPose) Init(); else StartCoroutine(InitTPoseStartOffset());
        }

        /// <summary>
        /// Skipping few first frames to reference static poses not from TPose but from first played animation frame (in most cases important)
        /// </summary>
        private IEnumerator InitTPoseStartOffset()
        {
            int counter = 1;
            while (counter > -5)
            {
                if (!initialized)
                    if (StartAfterTPose)
                    {
                        counter--;
                        if (counter < -1) Init();
                        yield return null;
                    }
                    else
                    {
                        counter--;
                        if (counter == -3) ReposeSpine();
                    }

                yield return null;
            }
        }

        /// <summary>
        /// Updating pointers for reversed and basic spine lead direction
        /// </summary>
        private void UpdateReverseHelpVariables()
        {
            if (LastBoneLeading)
            {
                leadingBoneIndex = SpineTransforms.Count - 1;
                reverser = -1;
                adjuster = 1;
            }
            else
            {
                leadingBoneIndex = 0;
                reverser = 1;
                adjuster = 0;
            }
        }

        /// <summary>
        /// Restraightening spine motion pose
        /// </summary>
        private void ReposeSpine()
        {
            UpdateReverseHelpVariables();
            RefreshDifferenceReference();

            for (int i = 1 - adjuster; i < proceduralPoints.Count - adjuster; i++)
            {
                proceduralPoints[i].Position = ProceduralReferencePose[i].Position;
                proceduralPoints[i].PreviousPosition = ProceduralReferencePose[i].Position;
                proceduralPoints[i].Rotation = ProceduralReferencePose[i].Rotation;
            }

            proceduralPoints[leadingBoneIndex] = GetLeadingBoneCoordinates();
        }

        #endregion


        private void LateUpdate()
        {
            if (!PhysicalUpdate) SpineMotion();
        }

        IEnumerator LateFixedUpdate()
        {
            while (true) { yield return new WaitForFixedUpdate(); SpineMotion(); }
        }

        //public float UpdateRate = 60f;
        //public float updateRateElapsed = 0f;

        private void SpineMotion()
        {
            #region Initial procedures before calculating motion

#if UNITY_EDITOR
            if (DrawDebug) if (Input.GetKey(KeyCode.BackQuote)) return; // Turning off component motion for debug purposes using "~" key
#endif

            if (!initialized) return;

            if (SpineTransforms.Count == 0)
            {
                Debug.LogError("No spine bones defined in " + name + " !");
                return;
            }

            // Defining delta time
            if (SafeDeltaTime)
                deltaTime = Mathf.Lerp(deltaTime, GetClampedSmoothDelta(), 0.05f);
            else
                deltaTime = Time.smoothDeltaTime;

            // Updating bone connectors before spine motion
            if (connectors != null) for (int i = 0; i < connectors.Count; i++) connectors[i].RememberAnimatorState();

            if (AnchorRoot)
                if (customAnchorInitLocPos == Vector3.zero)
                {
                    customAnchorInitLocPos = AnchorRoot.localPosition;
                    //customAnchorInitLocRot = AnchorRoot.localRotation;
                }

            // Switching current correction list for calculations
            RefreshRefDirsOnReverse();

            #endregion

            #region Blending and animator definition

            Vector3 leadingOrigPos = SpineTransforms[leadingBoneIndex].position;
            Quaternion leadingOrigRot = SpineTransforms[leadingBoneIndex].rotation;

            if (ConnectWithAnimator)
            {
                if (PositionsNotAnimated)
                    for (int i = 0; i < SpineTransforms.Count; i++)
                        if (SelectivePosNotAnimated[i])
                            SpineTransforms[i].localPosition = staticCoordinates[i].Position;

                if (RotationsNotAnimated)
                    for (int i = 0; i < SpineTransforms.Count; i++)
                        if (SelectiveRotNotAnimated[i])
                            SpineTransforms[i].localRotation = staticCoordinates[i].Rotation;
            }
            else
            {
                for (int i = 0; i < SpineTransforms.Count; i++)
                {
                    SpineTransforms[i].localPosition = staticCoordinates[i].Position;
                    SpineTransforms[i].localRotation = staticCoordinates[i].Rotation;
                }
            }

            // No spine animator blend, just keyframed animation
            if (BlendToOriginal >= 1f)
            {
                for (int i = 0; i < SpineTransforms.Count; i++)
                {
                    helperProceduralPoints[i].Position = SpineTransforms[i].position;
                    helperProceduralPoints[i].Rotation = SpineTransforms[i].rotation;
                }

                wasSourceAnimation = true;

                // Returning so nothing more is animated
                return;
            }

            if (OptimizeWithMesh)
                if (VisibilityRenderer)
                {
                    if (!VisibilityRenderer.isVisible) return;
                }

            // Update on change for this variables
            if (reversedChangeFlag != LastBoneLeading) wasRoundCorrection = !RoundCorrection;
            if (chainMethodChangeFlag != ChainMethod)
            {
                chainMethodChangeFlag = ChainMethod;
                wasRoundCorrection = !RoundCorrection;
            }
            if (wasUnified != UnifyCorrection) wasRoundCorrection = !RoundCorrection;

            // If we switched back from full blend to source animation we repose spine locomotion
            if (wasSourceAnimation)
            {
                ReposeSpine();
                wasSourceAnimation = false;
            }

            UpdateReverseHelpVariables();

            if (ManualAffectChain)
                for (int i = 1 - adjuster; i < proceduralPoints.Count - adjuster; i++)
                {
                    SpineTransforms[i].position += helperProceduralPoints[i].TransformDirection(ManualPositionOffsets[i]);
                    SpineTransforms[i].position += helperProceduralPoints[i].TransformDirection(SegmentsPivotOffset * (initialBoneDistances[i] * DistancesMultiplier));
                    SpineTransforms[i].rotation *= Quaternion.Euler(ManualRotationOffsets[i]);
                }

            // Updating distance variables only when main object is rescaled
            if (previousScale != transform.localScale) RefreshDistances();

            // Leading bone positioning and rotation
            proceduralPoints[leadingBoneIndex] = GetLeadingBoneCoordinates();

            #endregion

            #region Physics Experimental Stuff Support

            if (UseCollisions)
            {
                if (!collisionInitialized) AddColliders();

                RefreshCollidersDataList();

                // Letting every tail segment check only enabled colliders by game object
                CollidersDataToCheck.Clear();

                for (int i = 0; i < IncludedCollidersData.Count; i++)
                {
                    if (IncludedCollidersData[i].Collider == null) { forceRefreshCollidersData = true; break; }
                    if (IncludedCollidersData[i].Collider.gameObject.activeInHierarchy)
                    {
                        IncludedCollidersData[i].RefreshColliderData();
                        CollidersDataToCheck.Add(IncludedCollidersData[i]);
                    }
                }
            }

            #endregion

            RefreshDifferenceReference();
            CalculateMotion();

            #region Spine Chain Coordinates Calculations

            // Calculating difference values in reference to static pose
            FSpine_Point[] diffs = new FSpine_Point[SpineTransforms.Count];
            Vector3[] eulDiffs = new Vector3[diffs.Length];

            float revX = 1f;
            if (ChainMethod == EFChainMethod.AxisBasedInversed || ChainMethod == EFChainMethod.InversedVertical) revX = -1f;

            if (ChainMethod == EFChainMethod.Universal)
            {
                for (int i = 0; i < SpineTransforms.Count; i++)
                {
                    diffs[i] = new FSpine_Point
                    {
                        Position = proceduralPoints[i].Position - ProceduralReferencePose[i].Position,
                    };

                    var axis = spineRotationFixingSet.Current[i];
                    Quaternion fixedBendRotation = proceduralPoints[i].Rotation * Quaternion.FromToRotation(axis.Up, Vector3.up) * Quaternion.FromToRotation(axis.Right, Vector3.right);
                    Quaternion fixedRefRotation = ProceduralReferencePose[i].Rotation * Quaternion.FromToRotation(axis.Up, Vector3.up) * Quaternion.FromToRotation(axis.Right, Vector3.right);

                    diffs[i].Rotation = fixedBendRotation * Quaternion.Inverse(fixedRefRotation);
                }

                for (int i = 1 - adjuster; i < proceduralPoints.Count - adjuster; i++)
                {
                    helperProceduralPoints[i].Position = SpineTransforms[i].position + diffs[i].Position;
                    helperProceduralPoints[i].Rotation = diffs[i].Rotation * SpineTransforms[i].rotation;
                }
            }
            else
            if (ChainMethod == EFChainMethod.Deprecated || ChainMethod == EFChainMethod.InversedVertical) // Deprecated Oldest
            {
                for (int i = 0; i < SpineTransforms.Count; i++)
                {
                    diffs[i] = new FSpine_Point
                    {
                        Position = proceduralPoints[i].Position - ProceduralReferencePose[i].Position,
                        // Going to euler is solving here some problems with rotating in reference to base transorm's rotation
                        Rotation = proceduralPoints[i].Rotation * Quaternion.Inverse(ProceduralReferencePose[i].Rotation)
                    };

                    // Converting axis orientation to skeleton space rotation
                    Quaternion fromTo = Quaternion.FromToRotation((spineRotationFixingSet.Current[i].Forward * (revX) + lookDirectionsAdditiveCorrection[i]), Vector3.forward);
                    eulDiffs[i] = (proceduralPoints[i].Rotation * fromTo).eulerAngles - (ProceduralReferencePose[i].Rotation * fromTo).eulerAngles;
                    diffs[i].Rotation = Quaternion.Euler(eulDiffs[i]);
                }

                // Applying variables to helper list
                for (int i = 1 - adjuster; i < proceduralPoints.Count - adjuster; i++)
                {
                    helperProceduralPoints[i].Position = SpineTransforms[i].position + diffs[i].Position;
                    helperProceduralPoints[i].Rotation = Quaternion.Euler(SpineTransforms[i].eulerAngles + eulDiffs[i]);
                }
            }
            else // Axis based
            {
                for (int i = 0; i < SpineTransforms.Count; i++)
                {
                    diffs[i] = new FSpine_Point
                    {
                        Position = proceduralPoints[i].Position - ProceduralReferencePose[i].Position,
                        // Going to euler is solving here some problems with rotating in reference to base transorm's rotation
                        Rotation = proceduralPoints[i].Rotation * Quaternion.Inverse(ProceduralReferencePose[i].Rotation)
                    };

                    // Converting axis orientation to skeleton space rotation
                    Quaternion fromTo = Quaternion.FromToRotation(spineRotationFixingSet.Current[i].Right * revX, Vector3.forward);
                    eulDiffs[i] = (proceduralPoints[i].Rotation * fromTo).eulerAngles - (ProceduralReferencePose[i].Rotation * fromTo).eulerAngles;
                    diffs[i].Rotation = Quaternion.Euler(eulDiffs[i]);
                }

                // Applying variables to helper list
                for (int i = 1 - adjuster; i < proceduralPoints.Count - adjuster; i++)
                {
                    helperProceduralPoints[i].Position = SpineTransforms[i].position + diffs[i].Position;
                    helperProceduralPoints[i].Rotation = Quaternion.Euler(SpineTransforms[i].eulerAngles + eulDiffs[i]);
                }
            }

            #endregion


            #region Manual Offsets, Leading Bone Operations

            // Giving possibility to manual correction for bones rotations and positions in spine chain
            if (!ManualAffectChain)
                for (int i = 1 - adjuster; i < proceduralPoints.Count - adjuster; i++)
                {
                    helperProceduralPoints[i].Position += helperProceduralPoints[i].TransformDirection(ManualPositionOffsets[i]);
                    helperProceduralPoints[i].Position += helperProceduralPoints[i].TransformDirection(SegmentsPivotOffset * (initialBoneDistances[i] * DistancesMultiplier));
                    helperProceduralPoints[i].Rotation *= Quaternion.Euler(ManualRotationOffsets[i]);
                }

            // Applying fixing parameters to leading bone
            if (!AnimateLeadingBone)
                helperProceduralPoints[leadingBoneIndex].Position = SpineTransforms[leadingBoneIndex].position + helperProceduralPoints[leadingBoneIndex].TransformDirection(ManualPositionOffsets[leadingBoneIndex]);
            else
                helperProceduralPoints[leadingBoneIndex].Position = proceduralPoints[leadingBoneIndex].Position + helperProceduralPoints[leadingBoneIndex].TransformDirection(ManualPositionOffsets[leadingBoneIndex]);

            helperProceduralPoints[leadingBoneIndex].Position += helperProceduralPoints[leadingBoneIndex].TransformDirection(SegmentsPivotOffset * (initialBoneDistances[leadingBoneIndex] * DistancesMultiplier));
            helperProceduralPoints[leadingBoneIndex].Rotation = SpineTransforms[leadingBoneIndex].rotation * Quaternion.Euler(ManualRotationOffsets[leadingBoneIndex]);

            SpineTransforms[leadingBoneIndex].position = Vector3.Lerp(helperProceduralPoints[leadingBoneIndex].Position, SpineTransforms[leadingBoneIndex].position, BlendToOriginal);
            SpineTransforms[leadingBoneIndex].rotation = Quaternion.Slerp(helperProceduralPoints[leadingBoneIndex].Rotation, SpineTransforms[leadingBoneIndex].rotation, BlendToOriginal);

            #endregion


            #region Applying new coordinates for transforms and refreshing hierarchy

            for (int i = 1 - adjuster; i < proceduralPoints.Count - adjuster; i++)
            {
                SpineTransforms[i].position = Vector3.Lerp(helperProceduralPoints[i].Position, SpineTransforms[i].position, BlendToOriginal);
                SpineTransforms[i].rotation = Quaternion.Slerp(helperProceduralPoints[i].Rotation, SpineTransforms[i].rotation, BlendToOriginal);
            }

            if (AnimateLeadingBone)
                if (LeadingAnimateAfterMotion)
                {
                    SpineTransforms[leadingBoneIndex].position = leadingOrigPos;
                    SpineTransforms[leadingBoneIndex].rotation = leadingOrigRot;
                }

            #endregion


            #region Editor Mode Specific Stuff
#if UNITY_EDITOR
            // Detecting if something wrong is going on with animator in playmode -> one time for each added component if something wrong occurs
            if (!wasIncorrectRemind)
            {
                if (incorrectionCounter > 0)
                {
                    if (previousPos != RoundPosDiff(proceduralPoints[leadingBoneIndex].Position)) incorrectionCounter--;
                }
                else
                {
                    if (incorrectionCounter > -30)
                    {
                        int ind = Mathf.Max(0, SpineTransforms.Count / 4);
                        if (incorrectionCounter != 0)
                        {
                            incorrectionSum += Quaternion.Angle(preIncorrect, SpineTransforms[ind].localRotation);
                            for (int i = 0; i < SpineTransforms.Count; i++) incorrectionSumPos += Vector3.Distance(SpineTransforms[i].localPosition, staticCoordinates[i].Position);
                        }

                        preIncorrect = SpineTransforms[ind].localRotation;
                        incorrectionCounter--;
                    }
                    else
                    {
                        if (incorrectionCounter != -100)
                        {
                            if (incorrectionSum > 750 || incorrectionSumPos > (55 * (Mathf.Max(1f, SpineTransforms.Count / 4))))
                            {
                                incorrectionWarning = true;
                                Debug.LogWarning("[Spine Animator] There is something wrong going on with your bones in " + name + ". Check now inspector window of SpineAnimator then exit playmode. (" + incorrectionSum + " : " + incorrectionSumPos + ")");
                            }

                            incorrectionCounter = -100;
                        }
                    }
                }
            }
#endif
            #endregion

            previousPos = RoundPosDiff(proceduralPoints[leadingBoneIndex].Position);
            previousScale = transform.localScale;

            // Syncing bone connectors after spine motion (V1.0.5)
            if (connectors != null) for (int i = 0; i < connectors.Count; i++) connectors[i].RefreshAnimatorState();
        }


        /// <summary>
        /// Refreshing position and rotation of leading bone according to settings
        /// </summary>
        private FSpine_Point GetLeadingBoneCoordinates()
        {
            FSpine_Point newPoint = new FSpine_Point();
            newPoint.Transform = SpineTransforms[leadingBoneIndex];

            if (AnchorToThis)
            {
                transform.rotation *= Quaternion.Euler(LeadBoneRotationOffset);

                if (!AnimateLeadingBone)
                {
                    newPoint.Position = anchorHelpers[leadingBoneIndex].position;
                    newPoint.Rotation = transform.rotation;
                }
                else
                {
                    if (!LeadingAnimateAfterMotion)
                    {
                        newPoint.Position = SpineTransforms[leadingBoneIndex].position;
                        newPoint.Rotation = SpineTransforms[leadingBoneIndex].rotation;
                        newPoint.Rotation = transform.rotation;
                    }
                    else
                    {
                        newPoint.Position = anchorHelpers[leadingBoneIndex].position;
                        newPoint.Rotation = transform.rotation;
                    }
                }

                if (!LastBoneLeading && AutoAnchor)
                {
                    var axis = spineRotationFixingSet.Current[leadingBoneIndex];
                    Quaternion fixedRefRotation = Quaternion.FromToRotation(axis.Right, Vector3.right) * Quaternion.FromToRotation(axis.Up, Vector3.up);
                    newPoint.Rotation *= fixedRefRotation;
                }
            }
            else
            {
                if (AnchorRoot)
                {
                    newPoint.Position = staticCoordinates[0].Transform.parent.position;
                    newPoint.Position += staticCoordinates[0].Transform.parent.TransformVector(staticCoordinates[0].Position);

                    newPoint.Rotation = AnchorRoot.rotation;

                    if (!LastBoneLeading && AutoAnchor)
                    {
                        var axis = customAnchorAxis;
                        Quaternion fixedRefRotation = Quaternion.FromToRotation(axis.Right, Vector3.right) * Quaternion.FromToRotation(axis.Up, Vector3.up);
                        newPoint.Rotation *= fixedRefRotation;
                    }

                    newPoint.Rotation *= Quaternion.Euler(CustomAnchorRotationOffset);
                }
                else
                {
                    //transform.rotation *= Quaternion.Euler(LeadBoneRotationOffset);

                    //Vector3 prePos = SpineTransforms[leadingBoneIndex].localPosition;
                    //Quaternion preRot = SpineTransforms[leadingBoneIndex].localRotation;

                    //SpineTransforms[leadingBoneIndex].localPosition = staticCoordinates[leadingBoneIndex].Position;
                    //newPoint.Position = SpineTransforms[leadingBoneIndex].position;

                    //SpineTransforms[leadingBoneIndex].localRotation = staticCoordinates[leadingBoneIndex].Rotation;
                    ////newPoint.Rotation = transform.rotation;
                    //if ((int)ChainMethod <= 1)
                    //    newPoint.Rotation = AnchorRoot.rotation;
                    //else
                    //    newPoint.Rotation = AnchorRoot.rotation * Quaternion.FromToRotation(staticCoordinates[leadingBoneIndex].Rotation * Vector3.forward, spineRotationFixingSet.Current[leadingBoneIndex].Forward);

                    //newPoint.Rotation *= Quaternion.Euler(CustomAnchorRotationOffset);

                    //SpineTransforms[leadingBoneIndex].localPosition = prePos;
                    //SpineTransforms[leadingBoneIndex].localRotation = preRot;

                    //newPoint.Position = -staticCoordinates[leadingBoneIndex].Position;
                    //newPoint.Rotation = staticCoordinates[leadingBoneIndex].Rotation;

                    //if (!LastBoneLeading)
                    //{
                    //    newPoint.Position += staticCoordinates[leadingBoneIndex].Transform.parent.position;
                    //    newPoint.Rotation *= staticCoordinates[leadingBoneIndex].Transform.parent.rotation;
                    //}

                    //if (!LastBoneLeading && AutoAnchor)
                    //{
                    //    var axis = customAnchorAxis;
                    //    Quaternion fixedRefRotation = Quaternion.FromToRotation(axis.Right, Vector3.right) * Quaternion.FromToRotation(axis.Up, Vector3.up);
                    //    newPoint.Rotation *= fixedRefRotation;
                    //}

                    //newPoint.Rotation *= Quaternion.Euler(CustomAnchorRotationOffset);

                    newPoint.Position = staticCoordinates[leadingBoneIndex].Transform.parent.position;
                    newPoint.Position += staticCoordinates[leadingBoneIndex].Transform.parent.TransformVector(staticCoordinates[leadingBoneIndex].Position);

                    newPoint.Rotation = staticCoordinates[leadingBoneIndex].Transform.parent.rotation * staticCoordinates[leadingBoneIndex].Rotation;

                    if (!LastBoneLeading && AutoAnchor)
                    {
                        var axis = customAnchorAxis;
                        Quaternion fixedRefRotation = Quaternion.FromToRotation(axis.Right, Vector3.right) * Quaternion.FromToRotation(axis.Up, Vector3.up);
                        newPoint.Rotation *= fixedRefRotation;
                    }

                    newPoint.Rotation *= Quaternion.Euler(CustomAnchorRotationOffset);
                }
            }

            return newPoint;
        }


        /// <summary>
        /// Calculating spine-like movement animation logic for given transforms list
        /// </summary>
        protected virtual void CalculateMotion()
        {
            // Optimizing to avoid unneccesarry multiple use of if ( UseCollisions ) inside for loops

            gravityScale = GravityPower * deltaTime;

            if (UseCollisions) // With use of collision
            {
                if (LastBoneLeading)
                {
                    // Predicted position -> rotation to predicted position then final position calculations? with assigning?
                    for (int i = proceduralPoints.Count - 2; i >= 0; i--)
                    {
                        CalculateSpineBehaviourRotation(i);
                        CalculateSpineBehaviourPosition(i);
                    }
                }
                else
                {
                    for (int i = 1; i < proceduralPoints.Count; i++)
                    {
                        CalculateSpineBehaviourRotation(i);
                        CalculateSpineBehaviourPosition(i);
                    }
                }
            }
            else // Without collisions
            {
                //gravityScale = Vector3.zero;

                if (LastBoneLeading)
                {
                    // Predicted position -> rotation to predicted position then final position calculations? with assigning?
                    for (int i = proceduralPoints.Count - 2; i >= 0; i--)
                    {
                        CalculateSpineBehaviourRotation(i);

                        Vector3 targetPosition = CalculateTargetPosition(i);
                        CalculateStretchingLimiting(targetPosition, i);
                        AssignPosition(targetPosition, i);
                    }
                }
                else
                {
                    for (int i = 1; i < proceduralPoints.Count; i++)
                    {
                        CalculateSpineBehaviourRotation(i);

                        Vector3 targetPosition = CalculateTargetPosition(i);
                        CalculateStretchingLimiting(targetPosition, i);
                        AssignPosition(targetPosition, i);
                    }
                }
            }


        }

        /// <summary>
        /// Calculating position for single segment
        /// </summary>
        private void CalculateSpineBehaviourPosition(int index)
        {
            Vector3 targetPosition = CalculateTargetPosition(index);

            CalculateStretchingLimiting(targetPosition, index);

            if (SegmentCollision[index]) PushIfSegmentInsideCollider(proceduralPoints[index], ref targetPosition);

            AssignPosition(targetPosition, index);
        }


        protected void AssignPosition(Vector3 targetPosition, int index)
        {
            if (PosSmoother == 0f)
                proceduralPoints[index].Position = targetPosition;
            else
                proceduralPoints[index].Position = Vector3.Lerp(proceduralPoints[index].Position, targetPosition, deltaTime * SmootherValue(PosSmoother));
        }


        protected Vector3 CalculateTargetPosition(int index)
        {
            Vector3 targetPosition = (proceduralPoints[index - reverser].Position) - (proceduralPoints[index].TransformDirection(Vector3.forward) * (initialBoneDistances[index] * DistancesMultiplier));

            if (SegmentCollision[index]) targetPosition += gravityScale;

            #region Springiness Stuff

            if (Springiness > 0f)
            {
                if (!LastBoneLeading)
                {
                    FSpine_Point otherPoint = proceduralPoints[index - reverser]; FSpine_Point currentPoint = proceduralPoints[index];
                    Vector3 backPosDiff = currentPoint.Position - currentPoint.PreviousPosition;
                    Vector3 newPos = currentPoint.Position;
                    currentPoint.PreviousPosition = currentPoint.Position;
                    newPos += backPosDiff * (1 - Mathf.Lerp(.05f, .25f, Springiness));
                    //newPos += backPosDiff * (1 - Mathf.Lerp(0.3f, 0.05f, Springiness));

                    float restDistance = (otherPoint.Position - newPos).magnitude;

                    Matrix4x4 otherLocalToWorld = otherPoint.Transform.localToWorldMatrix;
                    otherLocalToWorld.SetColumn(3, otherPoint.Position);
                    Vector3 restPos = otherLocalToWorld.MultiplyPoint3x4(currentPoint.Transform.localPosition);

                    Vector3 diffPosVector = restPos - newPos;
                    newPos += diffPosVector * Mathf.Lerp(0.05f, 0.2f, Springiness);
                    //newPos += diffPosVector * Mathf.Lerp(0.5f, 0.2f, Springiness);

                    diffPosVector = restPos - newPos;
                    float distance = diffPosVector.magnitude;
                    float maxDistance = restDistance * (1 - Mathf.Lerp(0.0f, 0.2f, Springiness)) * 2;
                    //float maxDistance = restDistance * (1 - Mathf.Lerp(0.4f, 0.1f, Springiness)) * 2;
                    if (distance > maxDistance) newPos += diffPosVector * ((distance - maxDistance) / distance);

                    if (MaxStretching < 1f)
                    {
                        float dist = Vector3.Distance(proceduralPoints[index].Position, newPos);
                        if (dist > 0f)
                        {
                            float maxDist = initialBoneDistances[index] * 4 * MaxStretching;
                            if (dist > maxDist) newPos = Vector3.Lerp(newPos, targetPosition, Mathf.InverseLerp(dist, 0f, maxDist));
                        }
                    }

                    targetPosition = Vector3.Lerp(targetPosition, newPos, Mathf.Lerp(0.3f, 0.9f, Springiness));
                }
            }

            #endregion

            return targetPosition;
        }


        protected void CalculateStretchingLimiting(Vector3 targetPosition, int index)
        {
            if (MaxStretching < 1f)
            {
                float dist = Vector3.Distance(proceduralPoints[index].Position, targetPosition);
                if (dist > 0f)
                {
                    float maxDist = initialBoneDistances[index] * 4 * MaxStretching;
                    if (dist > maxDist) proceduralPoints[index].Position = Vector3.Lerp(proceduralPoints[index].Position, targetPosition, Mathf.InverseLerp(dist, 0f, maxDist));
                }
            }
        }

        /// <summary>
        /// Calculating rotation with limitations and stuff for single segment
        /// </summary>
        private void CalculateSpineBehaviourRotation(int index)
        {
            FSpine_Point otherSpinePoint = proceduralPoints[index - reverser];
            FSpine_Point currentSpinePoint = proceduralPoints[index];

            Quaternion targetLookRotation;
            Quaternion backRotationRef = ProceduralReferencePose[index].Rotation;

            if (Slithery >= 1f) backRotationRef = otherSpinePoint.Rotation;
            else
            if (Slithery > 0f) backRotationRef = Quaternion.Slerp(ProceduralReferencePose[index].Rotation, otherSpinePoint.Rotation, Slithery);

            targetLookRotation = Quaternion.LookRotation
                (
                    otherSpinePoint.Position - currentSpinePoint.Position,
                    otherSpinePoint.TransformDirection(lookUp)
                );

            #region Calculations to limit rotations in order to make custom animation behaviour

            float lookDiff = Quaternion.Angle(targetLookRotation, backRotationRef);

            // Limiting rotation to correct state with elastic range
            if (lookDiff > AngleLimit)
            {
                float limiting = 0f;
                limiting = Mathf.InverseLerp(0f, lookDiff, lookDiff - AngleLimit);

                Quaternion limitRange = Quaternion.Slerp(targetLookRotation, backRotationRef, limiting);

                float elasticPush = Mathf.Min(1f, lookDiff / (AngleLimit / 0.75f));
                elasticPush = Mathf.Sqrt(Mathf.Pow(elasticPush, 4)) * elasticPush; // sqrt and power will make this value increase slower but reaching 1f at the end

                if (LimitSmoother == 0f)
                    targetLookRotation = Quaternion.Slerp(targetLookRotation, limitRange, elasticPush);
                else
                    targetLookRotation = Quaternion.Slerp(targetLookRotation, limitRange, deltaTime * SmootherValue(LimitSmoother) * elasticPush);
            }

            if (GoBackSpeed <= 0f)
            {
                // When position in previous frame was different, we straigtening a little rotation of spine
                if (StraightenSpeed > 0f)
                {
                    if (previousPos != RoundPosDiff(proceduralPoints[leadingBoneIndex].Position))
                        spineRotationFixingSet.TargetStraighteningFactor[index] = 1f;
                    else
                        if (spineRotationFixingSet.TargetStraighteningFactor[index] > 0f)
                        spineRotationFixingSet.TargetStraighteningFactor[index] -= deltaTime * (5f + StraightenSpeed);

                    spineRotationFixingSet.StraighteningFactor[index] = Mathf.Lerp(spineRotationFixingSet.StraighteningFactor[index], spineRotationFixingSet.TargetStraighteningFactor[index], deltaTime * (1f + StraightenSpeed));

                    if (spineRotationFixingSet.StraighteningFactor[index] > 0.025f)
                        targetLookRotation = Quaternion.Lerp(targetLookRotation, backRotationRef, deltaTime * spineRotationFixingSet.StraighteningFactor[index] * StraightenSpeed * (TurboStraighten ? 6f : 1f));
                }
            }
            else // When we set GoBackSpeed variable spine is going back to straight pose continously so diff would be detected all the time and we don't want this
            {
                // If we use straigtening at the same time when using GoBack variable
                float straightenVal = 0f;

                // When position in previous frame was different, we straigtening a little rotation of spine
                if (StraightenSpeed > 0f)
                {
                    if (previousPos != RoundPosDiff(proceduralPoints[leadingBoneIndex].Position))
                        spineRotationFixingSet.TargetStraighteningFactor[index] = 1f;
                    else
                        if (spineRotationFixingSet.TargetStraighteningFactor[index] > 0f)
                        spineRotationFixingSet.TargetStraighteningFactor[index] -= deltaTime * (5f + StraightenSpeed);

                    spineRotationFixingSet.StraighteningFactor[index] = Mathf.Lerp(spineRotationFixingSet.StraighteningFactor[index], spineRotationFixingSet.TargetStraighteningFactor[index], deltaTime * (1f + StraightenSpeed));

                    if (spineRotationFixingSet.StraighteningFactor[index] > 0.025f)
                        straightenVal = spineRotationFixingSet.StraighteningFactor[index] * StraightenSpeed * (TurboStraighten ? 6f : 1f);
                }

                targetLookRotation = Quaternion.Lerp(targetLookRotation, backRotationRef, deltaTime * (Mathf.Lerp(0f, 40f, GoBackSpeed) + straightenVal));
            }

            #endregion

            // If we want some smooth motion for follower
            if (RotSmoother == 0f)
                currentSpinePoint.Rotation = targetLookRotation;
            else
                currentSpinePoint.Rotation = Quaternion.Slerp(currentSpinePoint.Rotation, targetLookRotation, deltaTime * SmootherValue(RotSmoother));
        }


        #region Helpers

        /// <summary>
        /// Assigning correction lists for calculations, only executed when change on flag occurs
        /// </summary>
        private void RefreshRefDirsOnReverse()
        {
            if (wasRoundCorrection != RoundCorrection)
            {
                wasUnified = UnifyCorrection;

                bool reversed = false;
                if (ChainMethod == EFChainMethod.Deprecated || ChainMethod == EFChainMethod.InversedVertical) reversed = true;
                if (LastBoneLeading) reversed = !reversed;
                if (ReverseOrder) reversed = !reversed;

                if (reversed)
                    spineRotationFixingSet.Current = spineRotationFixingSet.AxesReversed;
                else
                    spineRotationFixingSet.Current = spineRotationFixingSet.Axes;

                if (RoundCorrection)
                {
                    if (reversed)
                        spineRotationFixingSet.Current = spineRotationFixingSet.RoundedReversed;
                    else
                        spineRotationFixingSet.Current = spineRotationFixingSet.Rounded;
                }

                if (UnifyCorrection)
                {
                    spineRotationFixingSet.Current = spineRotationFixingSet.SetUnified(spineRotationFixingSet.Current);
                }

                wasRoundCorrection = RoundCorrection;
            }
        }

        /// <summary>
        /// Supporting scaling in update
        /// </summary>
        private void RefreshDistances()
        {
            initialBoneDistances = new List<float>();

            int c = SpineTransforms.Count - 1;

            for (int i = 0; i < SpineTransforms.Count - 1; i++)
            {
                initialBoneDistances.Add(Vector3.Distance(anchorHelpers[i].position, anchorHelpers[i + 1].position));
            }

            // Adding last variable in different way
            initialBoneDistances.Add(Vector3.Distance(anchorHelpers[c - 1].transform.position, anchorHelpers[c].transform.position));
        }

        /// <summary>
        /// Returning motion reference object
        /// </summary>
        private Transform GetAnchor()
        {
            if (AnchorToThis) return transform;
            else if (AnchorRoot) return AnchorRoot;
            return transform;
        }

        /// <summary>
        /// Adding connector component for spine animator for it to try sync bones after spine motion
        /// </summary>
        public void AddConnector(FSpineBoneConnector connector)
        {
            if (connectors == null) connectors = new List<FSpineBoneConnector>();
            if (!connectors.Contains(connector)) connectors.Add(connector);
        }

        /// <summary>
        /// Helper class to animate spine bones
        /// </summary>
        //[System.Serializable]
        public class FSpine_Point
        {
            public int Index;
            public Transform Transform;
            public Vector3 Position = Vector3.zero;
            public Vector3 PreviousPosition = Vector3.zero;
            public Vector3 SettedPosition = Vector3.zero;
            public Quaternion Rotation = Quaternion.identity;

            public float CollisionRadius = 1f;

            public float GetRadiusScaled()
            {
                return CollisionRadius * Transform.lossyScale.x;
            }

            public Vector3 TransformDirection(Vector3 dir)
            {
                return Rotation * dir;
            }
        }

        /// <summary>
        /// Making translations more smooth for more elastic effect
        /// </summary>
        private float SmootherValue(float val)
        {
            return Mathf.Lerp(60f, 0.1f, val);
        }

        /// <summary>
        /// Helper class to hold some calculation variables more learly in code
        /// </summary>
        //[System.Serializable]
        public class FSpine_FixingSet
        {
            public List<FSpine_Axis> Current;
            public List<FSpine_Axis> Axes;
            public List<FSpine_Axis> AxesReversed;
            public List<FSpine_Axis> Rounded;
            public List<FSpine_Axis> RoundedReversed;
            public List<FSpine_Axis> Unified;

            /// <summary> Variable to hold straigtening lerp smooth value </summary>
            public List<float> StraighteningFactor;
            public List<float> TargetStraighteningFactor;

            public List<FSpine_Axis> SetUnified(List<FSpine_Axis> toUnify)
            {
                List<FSpine_Axis> unified = new List<FSpine_Axis>();
                for (int i = 0; i < toUnify.Count; i++)
                {
                    FSpine_Axis axis = new FSpine_Axis(toUnify[toUnify.Count / 2].Right, toUnify[toUnify.Count / 2].Up, toUnify[toUnify.Count / 2].Forward);
                    unified.Add(axis);
                }

                return unified;
            }


            internal FSpine_FixingSet Init(int spineCount)
            {
                Current = new List<FSpine_Axis>();
                Axes = new List<FSpine_Axis>();
                AxesReversed = new List<FSpine_Axis>();
                Rounded = new List<FSpine_Axis>();
                RoundedReversed = new List<FSpine_Axis>();
                Unified = new List<FSpine_Axis>();

                StraighteningFactor = new List<float>();
                TargetStraighteningFactor = new List<float>();

                for (int i = 0; i < spineCount; i++)
                {
                    StraighteningFactor.Add(0f);
                    TargetStraighteningFactor.Add(0f);
                }

                return this;
            }

            //[System.Serializable]
            public class FSpine_Axis
            {
                public Vector3 Right;
                public Vector3 Up;
                public Vector3 Forward;

                public FSpine_Axis() { }

                public FSpine_Axis(Vector3 right, Vector3 up, Vector3 forward)
                {
                    Right = right.normalized;
                    Up = up.normalized;
                    Forward = forward.normalized;
                }
            }
        }

        /// <summary>
        /// Rounding position used in calculating difference for straightening
        /// </summary>
        protected Vector3 RoundPosDiff(Vector3 pos, int digits = 1)
        {
            return new Vector3((float)System.Math.Round(pos.x, digits), (float)System.Math.Round(pos.y, digits), (float)System.Math.Round(pos.z, digits));
        }

        /// <summary>
        /// Rounding fix correction angles to nearest values, we calculate axes directions in precise way, but in most cases rounded are doing job much better
        /// </summary>
        private Vector3 RoundToBiggestValue(Vector3 vec)
        {
            int biggest = 0;
            if (Mathf.Abs(vec.y) > Mathf.Abs(vec.x))
            {
                biggest = 1;
                if (Mathf.Abs(vec.z) > Mathf.Abs(vec.y)) biggest = 2;
            }
            else
                if (Mathf.Abs(vec.z) > Mathf.Abs(vec.x)) biggest = 2;

            if (biggest == 0) vec = new Vector3(Mathf.Round(vec.x), 0f, 0f);
            else
            if (biggest == 1) vec = new Vector3(0f, Mathf.Round(vec.y), 0f);
            else
                vec = new Vector3(0f, 0f, Mathf.Round(vec.z));

            return vec;
        }

        /// <summary>
        /// Method for drawing rays to debug in more visible way
        /// </summary>
        private void DrawFatRay(Vector3 origin, Vector3 dir)
        {
            float off = 0.01f;
            Gizmos.DrawRay(origin + Vector3.forward * off, dir);
            Gizmos.DrawRay(origin - Vector3.forward * off, dir);
            Gizmos.DrawRay(origin - Vector3.right * off, dir);
            Gizmos.DrawRay(origin + Vector3.right * off, dir);
            Gizmos.DrawRay(origin + Vector3.up * off, dir);
            Gizmos.DrawRay(origin - Vector3.up * off, dir);
            Gizmos.DrawRay(origin, dir);
        }

        /// <summary>
        /// Method for drawing lines to debug in more visible way
        /// </summary>
        private void DrawBoneLine(Vector3 origin, Vector3 dir)
        {
            float off = 0.05f;
            Gizmos.DrawLine(origin + Vector3.forward * off, dir);
            Gizmos.DrawLine(origin - Vector3.forward * off, dir);
            Gizmos.DrawLine(origin - Vector3.right * off, dir);
            Gizmos.DrawLine(origin + Vector3.right * off, dir);
            Gizmos.DrawLine(origin + Vector3.up * off, dir);
            Gizmos.DrawLine(origin - Vector3.up * off, dir);
            Gizmos.DrawLine(origin, dir);
        }


        /// <summary>
        /// Refresh selective variables values
        /// </summary>
        public void RefreshSelectivePosNotAnimated()
        {
            if (SelectivePosNotAnimated == null || SelectivePosNotAnimated.Count != SpineTransforms.Count)
            {
                SelectivePosNotAnimated = new List<bool>();
                for (int i = 0; i < SpineTransforms.Count; i++) SelectivePosNotAnimated.Add(true);
            }
        }

        /// <summary>
        /// Refresh collider offsets
        /// </summary>
        public void RefreshCollidersOffsets()
        {
            if (CollidersOffsets == null || CollidersOffsets.Count != SpineTransforms.Count)
            {
                CollidersOffsets = new List<Vector3>();
                for (int i = 0; i < SpineTransforms.Count; i++) CollidersOffsets.Add(Vector3.zero);
            }
        }

        /// <summary>
        /// Refresh selective variables values
        /// </summary>
        public void RefreshSelectiveRotNotAnimated()
        {
            if (SelectiveRotNotAnimated == null || SelectiveRotNotAnimated.Count != SpineTransforms.Count)
            {
                SelectiveRotNotAnimated = new List<bool>();
                for (int i = 0; i < SpineTransforms.Count; i++) SelectiveRotNotAnimated.Add(true);
            }
        }

        public void RefreshDisabledSegmentsCollisions()
        {
            if (SpineTransforms != null)
                if (SegmentCollision == null || SegmentCollision.Count != SpineTransforms.Count)
                {
                    SegmentCollision = new List<bool>();
                    for (int i = 0; i < SpineTransforms.Count; i++) SegmentCollision.Add(true);
                }
        }

        /// <summary>
        /// Refresh manual offset variables values
        /// </summary>
        public void RefreshManualPosOffs()
        {
            if (ManualPositionOffsets == null || ManualPositionOffsets.Count != SpineTransforms.Count)
            {
                ManualPositionOffsets = new List<Vector3>();
                for (int i = 0; i < SpineTransforms.Count; i++) ManualPositionOffsets.Add(Vector3.zero);
            }
        }

        /// <summary>
        /// Refresh manual offset variables values
        /// </summary>
        public void RefreshManualRotOffs()
        {
            if (ManualRotationOffsets == null || ManualRotationOffsets.Count != SpineTransforms.Count)
            {
                ManualRotationOffsets = new List<Vector3>();
                for (int i = 0; i < SpineTransforms.Count; i++) ManualRotationOffsets.Add(Vector3.zero);
            }
        }

        /// <summary>
        /// Destroying objects generated by component
        /// </summary>
        public void OnDestroy()
        {
            if (Application.isPlaying)
            {
                if (anchorsContainer) Destroy(anchorsContainer.gameObject);
                if (anchorHelpers != null) for (int i = 0; i < anchorHelpers.Length; i++) if (anchorHelpers[i]) Destroy(anchorHelpers[i].gameObject);

                // V1.0.1
                if (mainPivotOffsetTransform)
                {
                    RestoreBasePivotChildren();
                    Destroy(mainPivotOffsetTransform.gameObject);
                }
            }
            else
            {
                if (mainPivotOffsetTransform)
                {
                    RestoreBasePivotChildren();
                    DestroyImmediate(mainPivotOffsetTransform.gameObject);
                }
            }
        }

        // V1.0.1
        /// <summary>
        /// Restoring generated pivot transform children to initial state
        /// </summary>
        private void RestoreBasePivotChildren()
        {
            for (int i = mainPivotOffsetTransform.childCount - 1; i >= 0; i--)
            {
                mainPivotOffsetTransform.GetChild(i).SetParent(mainPivotOffsetTransform.parent, true);
            }
        }

        /// <summary>
        /// Refreshing difference pose for spine animator motion
        /// </summary>
        private void RefreshDifferenceReference()
        {
            ProceduralReferencePose[leadingBoneIndex] = GetLeadingBoneCoordinates();

            if (LastBoneLeading)
            {
                // Setting spine straight forward in hierarchy space
                for (int i = ProceduralReferencePose.Count - 2; i >= 0; i--)
                {
                    ProceduralReferencePose[i].Rotation = ProceduralReferencePose[i - reverser].Rotation;
                    ProceduralReferencePose[i].Position = ProceduralReferencePose[i - reverser].Position - (ProceduralReferencePose[i].TransformDirection(Vector3.forward) * (initialBoneDistances[i] * DistancesMultiplier));
                }
            }
            else
            {
                for (int i = 1; i < ProceduralReferencePose.Count; i++)
                {
                    ProceduralReferencePose[i].Rotation = ProceduralReferencePose[i - reverser].Rotation;
                    ProceduralReferencePose[i].Position = ProceduralReferencePose[i - reverser].Position - (ProceduralReferencePose[i].TransformDirection(Vector3.forward) * (initialBoneDistances[i] * DistancesMultiplier));
                }
            }
        }

        #region Editor Helping Stuff

        // V1.0.1
        /// <summary>
        /// Supporting generating additional pivot transform
        /// </summary>
        private void OnValidate()
        {
            RefreshDisabledSegmentsCollisions();

            if ( OptimizeWithMesh)
                if( VisibilityRenderer == null)
                {
                    VisibilityRenderer = GetComponent<Renderer>();
                    if (!VisibilityRenderer) VisibilityRenderer = GetComponentInChildren<Renderer>();
                }

            if (MainPivotOffset == Vector3.zero)
            {
                if (mainPivotOffsetTransform)
                {
                    if (mainPivotOffsetTransform.childCount > 0)
                    {
                        mainPivotOffsetTransform.localPosition = MainPivotOffset;
                        RestoreBasePivotChildren();
                    }

                    // We can't destroy objects in OnValidate so transform will be left here in case you will use it again
                    //if (Application.isPlaying) Destroy(mainPivotOffsetTransform.gameObject); else DestroyImmediate(mainPivotOffsetTransform.gameObject);
                }
            }
            else
            {
                // Generating pivot offset transform - quickest way without defining other stuff
                if (!mainPivotOffsetTransform)
                {
                    mainPivotOffsetTransform = new GameObject("Main Pivot Offset-Spine Animator-" + name).transform;
                    mainPivotOffsetTransform.SetParent(transform, false);
                    mainPivotOffsetTransform.localPosition = Vector3.zero;
                    mainPivotOffsetTransform.localRotation = Quaternion.identity;
                    mainPivotOffsetTransform.localScale = Vector3.one;
                }

                if (mainPivotOffsetTransform.childCount == 0)
                {
                    for (int i = transform.childCount - 1; i >= 0; i--)
                    {
                        if (transform.GetChild(i) == mainPivotOffsetTransform) continue;
                        transform.GetChild(i).SetParent(mainPivotOffsetTransform, true);
                    }
                }

                mainPivotOffsetTransform.localPosition = MainPivotOffset;
            }
        }



        /// <summary>
        /// Trying to predict some correction variables values
        /// </summary>
        public void TryAutoCorrect(Transform head = null, bool checkAnimator = true)
        {
            if (!head)
            {
                if (SpineTransforms[0].parent == transform) LastBoneLeading = true;
                else
                {
                    Transform p = SpineTransforms[0].parent;
                    for (int i = 0; i < 100; i++)
                    {
                        if (p.parent == null) break;
                        if (p.childCount == 1)
                        {
                            p = p.parent;
                            if (p == transform)
                            {
                                LastBoneLeading = true;
                                break;
                            }
                        }
                        else break;
                    }
                }

                if (SpineTransforms[SpineTransforms.Count - 1].childCount == 0) LastBoneLeading = false;
            }
            else
            {
                Vector2 distances = Vector2.zero;
                distances.x = Vector3.Distance(SpineTransforms[0].position, head.position);
                distances.y = Vector3.Distance(SpineTransforms[1].position, head.position);
                if (distances.x > distances.y) LastBoneLeading = true; else LastBoneLeading = false;
            }

            if (checkAnimator)
            {
                ConnectWithAnimator = false;

                Animator animator = GetComponentInChildren<Animator>();
                if (animator) if (animator.runtimeAnimatorController) ConnectWithAnimator = true;

                Animation animation = GetComponentInChildren<Animation>();
                if (animation) ConnectWithAnimator = true;
            }

            float dot = Vector3.Dot(SpineTransforms[0].forward, transform.forward);
            if (dot < 0.8f && dot > -0.8f) ChainMethod = EFChainMethod.Universal;
        }

        public void DevLog()
        {
            Debug.Log("No need for now");
        }


        #endregion


        private float GetClampedSmoothDelta()
        {
            return Mathf.Clamp(Time.smoothDeltaTime, 0f, 0.1f);
        }


        #region Drawing Gizmos

#if UNITY_EDITOR

        public bool DrawDebug = false;
        [Range(0f, 1f)] public float DebugAlpha = 1f;
        [Range(0f, 1f)] public float AdditionalDebugAlpha = 1f;

        // Set it to false if you don't want any gizmo
        public static bool drawMainGizmo = true;
        public bool drawGizmos = true;

        protected virtual void OnDrawGizmos()
        {
            if (drawMainGizmo)
            {
                Gizmos.DrawIcon(transform.position, "FIMSpace/FSpine/SPR_SpineFollowerGizmo.png", true);

                if (drawGizmos)
                {
                    if (SpineTransforms != null)
                    {
                        if (SpineTransforms.Count != 0)
                        {
                            if (SpineTransforms.Count > 0) if (!LastBoneLeading) Gizmos.DrawIcon(SpineTransforms[0].position, "FIMSpace/FSpine/SPR_SpineFollowerGizmoHead.png"); else Gizmos.DrawIcon(SpineTransforms[0].position, "FIMSpace/FSpine/SPR_SpineFollowerGizmoSegment.png");

                            for (int i = 0; i < SpineTransforms.Count - 1; i++)
                            {
                                if (i == 0) if (SpineTransforms[0] == transform) continue;
                                Gizmos.DrawIcon(SpineTransforms[i].position, "FIMSpace/FSpine/SPR_SpineFollowerGizmoSegment.png");
                            }

                            if (LastBoneLeading) Gizmos.DrawIcon(SpineTransforms[SpineTransforms.Count - 1].position, "FIMSpace/FSpine/SPR_SpineFollowerGizmoHead.png");
                            else
                                Gizmos.DrawIcon(SpineTransforms[SpineTransforms.Count - 1].position, "FIMSpace/FSpine/SPR_SpineFollowerGizmoSegment.png");
                        }
                    }
                }
            }

            if (!DrawDebug || !enabled) return;

            if (!initialized)
            {
                if (LastBoneLeading)
                {
                    for (int i = SpineTransforms.Count - 2; i >= 0; i--)
                    {
                        Gizmos.color = Color.HSVToRGB((float)i / (float)(SpineTransforms.Count), 0.5f, DebugAlpha);
                        DrawBoneLine(SpineTransforms[i].position, SpineTransforms[i + 1].position);
                    }

                }
                else
                    for (int i = 1; i < SpineTransforms.Count; i++)
                    {
                        Gizmos.color = Color.HSVToRGB((float)i / (float)(SpineTransforms.Count), 0.5f, DebugAlpha);
                        DrawBoneLine(SpineTransforms[i].position, SpineTransforms[i - 1].position);
                    }

                return;
            }

            Gizmos.color = new Color(0.5f, 1f, 0.4f, DebugAlpha);
            Gizmos.DrawSphere(anchorHelpers[leadingBoneIndex].position, 0.25f);

            for (int i = 0; i < proceduralPoints.Count; i++)
            {
                Gizmos.color = FColorMethods.ChangeColorAlpha(Color.HSVToRGB((float)i / (float)(proceduralPoints.Count), 0.7f, 0.9f), DebugAlpha);
                DrawFatRay(proceduralPoints[i].Position, proceduralPoints[i].Rotation * Vector3.right);
                //DrawFatRay(proceduralPoints[i].Position, proceduralPoints[i].Rotation * Vector3.up / 2f);
            }

            if (AdditionalDebugAlpha > 0f)
            {
                for (int i = 0; i < SpineTransforms.Count; i++)
                {
                    Vector3 forwardInBoneOrientation = spineRotationFixingSet.Current[i].Forward;
                    Vector3 projectedUp = spineRotationFixingSet.Current[i].Up;
                    Vector3 upInBoneOrientation = spineRotationFixingSet.Current[i].Up;
                    Vector3 crossRight = spineRotationFixingSet.Current[i].Right;
                    Vector3 rightInBoneOrientation = spineRotationFixingSet.Current[i].Right;

                    Vector3 o = SpineTransforms[i].position + ForwardReference.up / 2f;
                    Gizmos.color = Color.red; Gizmos.color *= new Color(1f, 1f, 1f, AdditionalDebugAlpha);
                    Gizmos.DrawRay(o + ForwardReference.up / 2f, crossRight.normalized * 0.1f);
                    Gizmos.color = Color.blue; Gizmos.color *= new Color(1f, 1f, 1f, AdditionalDebugAlpha);
                    Gizmos.DrawRay(o + ForwardReference.up / 2f, (forwardInBoneOrientation).normalized * 0.1f);
                    Gizmos.color = Color.green; Gizmos.color *= new Color(1f, 1f, 1f, AdditionalDebugAlpha);
                    Gizmos.DrawRay(o + ForwardReference.up / 2f, projectedUp.normalized * 0.1f);


                    Gizmos.color = Color.green; Gizmos.color *= new Color(1f, 1f, 1f, AdditionalDebugAlpha);
                    Vector3 upTip = SpineTransforms[i].TransformDirection(upInBoneOrientation).normalized * 0.2f;

                    Gizmos.DrawLine(o, o + upTip);
                    Vector3 rightInBone = SpineTransforms[i].TransformDirection(rightInBoneOrientation).normalized;
                    Gizmos.DrawLine(o + upTip, o + upTip * 0.8f + rightInBone * 0.03f);
                    Gizmos.DrawLine(o + upTip, o + upTip * 0.8f + rightInBone * -0.03f);


                    Gizmos.color = Color.red; Gizmos.color *= new Color(1f, 1f, 1f, AdditionalDebugAlpha);
                    Vector3 rightTip = SpineTransforms[i].TransformDirection(rightInBoneOrientation).normalized * 0.25f;
                    Vector3 forwInBone = SpineTransforms[i].TransformDirection(forwardInBoneOrientation).normalized;
                    Gizmos.DrawLine(o, o + rightTip);
                    Gizmos.DrawLine(o + rightTip, o + rightTip * 0.8f + forwInBone * 0.03f);
                    Gizmos.DrawLine(o + rightTip, o + rightTip * 0.8f + forwInBone * -0.03f);


                    Gizmos.color = Color.red; Gizmos.color *= new Color(1f, 1f, 1f, AdditionalDebugAlpha);
                    Gizmos.DrawRay(o + ForwardReference.up / 3f, SpineTransforms[i].right * 0.1f);
                    Gizmos.color = Color.blue; Gizmos.color *= new Color(1f, 1f, 1f, AdditionalDebugAlpha);
                    Gizmos.DrawRay(o + ForwardReference.up / 3f, SpineTransforms[i].forward * 0.1f);
                    Gizmos.color = Color.green; Gizmos.color *= new Color(1f, 1f, 1f, AdditionalDebugAlpha);
                    Gizmos.DrawRay(o + ForwardReference.up / 3f, SpineTransforms[i].up * 0.1f);
                }


                if (spineRotationFixingSet != null)
                {
                    if (spineRotationFixingSet.Axes != null)
                    {
                        for (int j = 0; j < spineRotationFixingSet.Axes.Count; j++)
                        {
                            Vector3 o = SpineTransforms[j].position + ForwardReference.up * 1.25f;
                            Gizmos.color = Color.green; Gizmos.color *= new Color(1f, 1f, 1f, AdditionalDebugAlpha);
                            Gizmos.DrawRay(o, SpineTransforms[j].TransformDirection(spineRotationFixingSet.Axes[j].Up).normalized * 0.3f);

                            Gizmos.color = Color.blue; Gizmos.color *= new Color(1f, 1f, 1f, AdditionalDebugAlpha);
                            Gizmos.DrawRay(o, SpineTransforms[j].TransformDirection(spineRotationFixingSet.Axes[j].Forward).normalized * 0.3f);

                            Gizmos.color = Color.red; Gizmos.color *= new Color(1f, 1f, 1f, AdditionalDebugAlpha);
                            Gizmos.DrawRay(o, SpineTransforms[j].TransformDirection(spineRotationFixingSet.Axes[j].Right).normalized * 0.3f);


                            o = ProceduralReferencePose[j].Position + ForwardReference.up * 1.8f;
                            Gizmos.color = Color.green; Gizmos.color *= new Color(1f, 1f, 1f, AdditionalDebugAlpha);
                            Gizmos.DrawRay(o, ProceduralReferencePose[j].TransformDirection(spineRotationFixingSet.Axes[j].Up).normalized * 0.3f);

                            Gizmos.color = Color.blue; Gizmos.color *= new Color(1f, 1f, 1f, AdditionalDebugAlpha);
                            Gizmos.DrawRay(o, ProceduralReferencePose[j].TransformDirection(spineRotationFixingSet.Axes[j].Forward).normalized * 0.3f);

                            Gizmos.color = Color.red; Gizmos.color *= new Color(1f, 1f, 1f, AdditionalDebugAlpha);
                            Gizmos.DrawRay(o, ProceduralReferencePose[j].TransformDirection(spineRotationFixingSet.Axes[j].Right).normalized * 0.3f);
                        }
                    }
                }
            }

            //if (ProceduralReferencePose != null)
            //    for (int i = 0; i < ProceduralReferencePose.Count; i++)
            //    {
            //        Gizmos.color = FColorMethods.ChangeColorAlpha(Color.HSVToRGB((float)i / (float)(proceduralPoints.Count), 0.7f, 0.9f), DebugAlpha);
            //        Gizmos.DrawRay(ProceduralReferencePose[i].Position + Vector3.up * 2.1f, ProceduralReferencePose[i].Rotation * Vector3.right);
            //    }

            if (LastBoneLeading)
            {
                for (int i = proceduralPoints.Count - 2; i >= 0; i--)
                {
                    Gizmos.color = Color.HSVToRGB((float)i / (float)(proceduralPoints.Count), 0.75f, DebugAlpha);
                    DrawBoneLine(proceduralPoints[i].Position, proceduralPoints[i + 1].Position);
                }
            }
            else
            {
                for (int i = 1; i < proceduralPoints.Count; i++)
                {
                    Gizmos.color = Color.HSVToRGB((float)i / (float)(proceduralPoints.Count), 0.75f, DebugAlpha);
                    DrawBoneLine(proceduralPoints[i].Position, proceduralPoints[i - 1].Position);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            DrawColliders();
        }

#endif

        #endregion

        #endregion


        #region Physics Experimental Stuff

        [Tooltip("[Experimental] Using some simple calculations to make tail bend on colliders")]
        public bool UseCollisions = false;

        public List<Collider> IncludedColliders;
        protected List<FImp_ColliderData_Base> IncludedCollidersData;
        protected List<FImp_ColliderData_Base> CollidersDataToCheck;
        public List<Vector3> CollidersOffsets;

        [Tooltip("If disabled Colliders can be offsetted a bit in wrong way - check pink spheres in scene view (playmode, with true positions disabled colliders are fitting to stiff reference pose) - but it gives more stable collision projection! But to avoid stuttery you can inrease position smoothing")]
        public bool UseTruePosition = false;
        public Vector3 OffsetAllColliders = Vector3.zero;

        public AnimationCurve CollidersScale = AnimationCurve.Linear(0, 1, 1, 1);
        public float CollidersScaleMul = 6.5f;
        [Range(0f, 1f)]
        public float DifferenceScaleFactor = 1f;

        [Tooltip("If you want to continue checking collision if segment collides with one collider (very useful for example when you using gravity power with ground)")]
        public bool DetailedCollision = false;

        [Tooltip("Pushing segments in world direction (should have included ground collider to collide with)")]
        public Vector3 GravityPower = Vector3.zero;
        protected Vector3 gravityScale = Vector3.zero;

        protected bool collisionInitialized = false;
        protected bool forceRefreshCollidersData = false;

        /// <summary>
        /// Calculating automatically scale for colliders on tail, which will be automatically assigned after initialization
        /// </summary>
        protected float GetColliderSphereRadiusFor(int i)
        {
            int backBone = i - 1;
            if (LastBoneLeading)
            {
                if (i == SpineTransforms.Count - 1) return 0f;
                backBone = i + 1;
            }
            else
                if (i == 0) return 0f;

            float refDistance = 1f;
            if (SpineTransforms.Count > 1) refDistance = Vector3.Distance(SpineTransforms[1].position, SpineTransforms[0].position);

            float singleScale = Mathf.Lerp(refDistance, (SpineTransforms[i].transform.position - SpineTransforms[backBone].transform.position).magnitude * 0.5f, DifferenceScaleFactor);
            float div = SpineTransforms.Count - 1;
            if (div <= 0f) div = 1f;
            float step = 1f / div;

            return 0.5f * singleScale * CollidersScaleMul * CollidersScale.Evaluate(step * (float)i);
        }


        protected void DrawColliders()
        {
            if (UseCollisions)
            {
                RefreshCollidersOffsets();
                RefreshDisabledSegmentsCollisions();

                Color preCol = Gizmos.color;
                Color sphColor;
                if (Application.isPlaying) { sphColor = new Color(0.4f, 1f, 0.4f, 0.9f); } else sphColor = new Color(0.4f, 1f, 0.4f, 1f);
                if (!UseTruePosition) { sphColor *= new Color(1f, 1f, 1f, 0.45f); }

                Gizmos.color = sphColor;

                if (LastBoneLeading)
                {
                    for (int i = SpineTransforms.Count - 1; i >= 0; i--)
                    {
                        if (!SegmentCollision[i]) Gizmos.color *= new Color(1f, 1f, 1f, 0.4f);
                        Vector3 pos = SpineTransforms[i].position + SpineTransforms[i].TransformVector(CollidersOffsets[i] + OffsetAllColliders);
                        Gizmos.DrawWireSphere(pos, GetColliderSphereRadiusFor(i));
                        Gizmos.color = sphColor;
                    }

                    sphColor = new Color(1f, 0f, 1f, 1f);
                    if (UseTruePosition) sphColor *= new Color(1f, 1f, 1f, 0.4f);

                    if (proceduralPoints != null)
                    {
                        Gizmos.color = new Color(0.9f, .2f, 0.5f, 0.5f);
                        for (int i = proceduralPoints.Count - 1; i >= 0; i--)
                        {
                            if (!SegmentCollision[i]) Gizmos.color *= new Color(1f, 1f, 1f, 0.4f);
                            Vector3 pos = proceduralPoints[i].Position + SpineTransforms[i].TransformVector(CollidersOffsets[i] + OffsetAllColliders);
                            Gizmos.DrawWireSphere(pos, GetColliderSphereRadiusFor(i));
                            Gizmos.color = sphColor;
                        }
                    }
                }
                else
                {
                    for (int i = 1; i < SpineTransforms.Count; i++)
                    {
                        if (!SegmentCollision[i]) Gizmos.color *= new Color(1f, 1f, 1f, 0.4f);
                        Vector3 pos = SpineTransforms[i].position + SpineTransforms[i].TransformVector(CollidersOffsets[i] + OffsetAllColliders);
                        Gizmos.DrawWireSphere(pos, GetColliderSphereRadiusFor(i));
                        Gizmos.color = sphColor;
                    }

                    sphColor = new Color(1f, 0f, 1f, 1f);
                    if (UseTruePosition) sphColor *= new Color(1f, 1f, 1f, 0.4f);

                    if (proceduralPoints != null)
                    {
                        for (int i = 1; i < SpineTransforms.Count; i++)
                        {
                            if (!SegmentCollision[i]) Gizmos.color *= new Color(1f, 1f, 1f, 0.4f);
                            Vector3 pos = proceduralPoints[i].Position + SpineTransforms[i].TransformVector(CollidersOffsets[i] + OffsetAllColliders);
                            Gizmos.DrawWireSphere(pos, GetColliderSphereRadiusFor(i));
                            Gizmos.color = sphColor;
                        }
                    }
                }

                Gizmos.color = preCol;
            }
        }


        protected void AddColliders()
        {
            for (int i = 0; i < SpineTransforms.Count; i++)
            {
                proceduralPoints[i].CollisionRadius = GetColliderSphereRadiusFor(i);
            }

            IncludedCollidersData = new List<FImp_ColliderData_Base>();
            RefreshCollidersDataList();
        }


        /// <summary>
        /// Refreshing colliders data for included colliders
        /// </summary>
        public void RefreshCollidersDataList()
        {
            if (IncludedColliders.Count != IncludedCollidersData.Count || forceRefreshCollidersData)
            {
                IncludedCollidersData.Clear();

                for (int i = IncludedColliders.Count - 1; i >= 0; i--)
                {
                    if (IncludedColliders[i] == null)
                    {
                        IncludedColliders.RemoveAt(i);
                        continue;
                    }

                    FImp_ColliderData_Base colData = FImp_ColliderData_Base.GetColliderDataFor(IncludedColliders[i]);
                    IncludedCollidersData.Add(colData);
                }

                forceRefreshCollidersData = false;
            }
        }

        public void PushIfSegmentInsideCollider(FSpine_Point spinePoint, ref Vector3 targetPoint)
        {
            // We must translate phantom/reference skeleton positions to true position in world space for collisions
            Vector3 offset;

            if (UseTruePosition)
            {
                Vector3 theTarget = targetPoint;
                Vector3 truePosition = helperProceduralPoints[spinePoint.Index].Position;
                offset = truePosition - theTarget + spinePoint.Transform.TransformVector(CollidersOffsets[spinePoint.Index] + OffsetAllColliders);
            }
            else
                offset = spinePoint.Transform.TransformVector(CollidersOffsets[spinePoint.Index] + OffsetAllColliders);

            if (!DetailedCollision)
            {
                for (int i = 0; i < CollidersDataToCheck.Count; i++)
                    if (CollidersDataToCheck[i].PushIfInside(ref targetPoint, spinePoint.GetRadiusScaled(), offset)) return;
            }
            else
            {
                for (int i = 0; i < CollidersDataToCheck.Count; i++)
                    CollidersDataToCheck[i].PushIfInside(ref targetPoint, spinePoint.GetRadiusScaled(), offset);
            }
        }

        public Transform FindForwardReference()
        {
            Transform target = transform;

            Transform p;
            Transform c = transform.parent;
            FSpineAnimator mySpine = null;

            if (c != null)
                for (int i = 0; i < 18; i++)
                {
                    p = c.parent;
                    mySpine = c.GetComponent<FSpineAnimator>();
                    if (mySpine) break;
                    c = p;
                    if (p == null) break;
                }

            if (mySpine != null)
            {
                if (mySpine.ForwardReference != null) target = mySpine.ForwardReference; else target = mySpine.transform;
            }

            return target;
        }

        #endregion
    }
}