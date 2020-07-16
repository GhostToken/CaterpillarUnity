using FIMSpace.FEditor;
using FIMSpace.FSpine;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[UnityEditor.CustomEditor(typeof(FSpineAnimator))]
/// <summary>
/// FM: Editor class component to enchance controll over component from inspector window
/// </summary>
[CanEditMultipleObjects]
public class FSpineAnimator_Editor : UnityEditor.Editor
{
    static bool drawDefaultInspector = false;
    static bool drawMain = true;
    static bool drawAnimationOptions = true;
    static bool drawQuickCorrection = false;
    static bool drawAdvancedCorrection = false;
    static bool drawDebug = false;
    static bool drawPreciseAutoCorr = false;
    static bool drawSpineTransforms = false;

    private Transform startBone;
    private Transform endBone;
    private Transform headBone;

    private bool incorrection = false;
    private bool incorrectionClicked = false;

    #region Editor to component stuff

    protected SerializedProperty sp_spines;
    protected SerializedProperty sp_forw;
    protected SerializedProperty sp_drawg;

    protected SerializedProperty sp_BlendToOriginal;
    protected SerializedProperty sp_ReversedLeadBone;
    protected SerializedProperty sp_ConnectWithAnimator;
    protected SerializedProperty sp_Optim;
    protected SerializedProperty sp_Visibil;
    protected SerializedProperty sp_BackwardMovement;
    protected SerializedProperty sp_PhysicalClock;
    protected SerializedProperty sp_SafeDeltaTime;
    protected SerializedProperty sp_AnchoredSpine;
    protected SerializedProperty sp_AutoAnchor;
    protected SerializedProperty sp_AnchorRoot;
    protected SerializedProperty sp_QueueToLastUpdate;
    protected SerializedProperty sp_PositionsNotAnimated;
    protected SerializedProperty sp_SelectivePosNotAnimated;
    protected SerializedProperty sp_RotationsNotAnimated;
    protected SerializedProperty sp_SelectiveRotNotAnimated;
    protected SerializedProperty sp_ManualAffects;
    protected SerializedProperty sp_ManualRotationOffsets;
    protected SerializedProperty sp_ManualPositionOffsets;
    protected SerializedProperty sp_RoundCorrection;
    protected SerializedProperty sp_UnifyCorrection;
    protected SerializedProperty sp_StartAfterTPose;
    protected SerializedProperty sp_InversedVerticalRotation;
    protected SerializedProperty sp_ChainMethod;
    protected SerializedProperty sp_OrientationReference;
    //protected SerializedProperty sp_InverseX;
    //protected SerializedProperty sp_InverseZ;

    protected SerializedProperty sp_PositionsSmoother;
    protected SerializedProperty sp_RotationsSmoother;
    protected SerializedProperty sp_MaxStretching;
    protected SerializedProperty sp_Slithery;

    protected SerializedProperty sp_AngleLimit;
    protected SerializedProperty sp_LimitingAngleSmoother;
    protected SerializedProperty sp_StraighteningSpeed;
    protected SerializedProperty sp_TurboStraighten;
    protected SerializedProperty sp_GoBackSpeed;
    protected SerializedProperty sp_Springiness;
    protected SerializedProperty sp_SegmentsPivotOffset;
    protected SerializedProperty sp_DistancesMul;
    protected SerializedProperty sp_AnimateLeadingBone;
    protected SerializedProperty sp_LeadingAnimateAfterMotion;
    protected SerializedProperty sp_LeadBoneRotationOffset;
    protected SerializedProperty sp_CustomAnchorRotationOffset;


    protected SerializedProperty sp_DrawDebug;
    protected SerializedProperty sp_DebugAlpha;
    protected SerializedProperty sp_AddDebugAlpha;
    protected SerializedProperty sp_MainPivotOffset;

    protected SerializedProperty sp_UseCollisions;
    protected SerializedProperty sp_IncludedColliders;
    protected SerializedProperty sp_CollidersScale;
    protected SerializedProperty sp_CollidersScaleMul;
    protected SerializedProperty sp_CollidersAutoCurve;
    protected SerializedProperty sp_CollidersOffsets;
    protected SerializedProperty sp_GravityPower;

    protected SerializedProperty sp_AllCollidersOffset;
    protected SerializedProperty sp_UseTruePosition;
    protected SerializedProperty sp_DetailedCollision;
    protected SerializedProperty sp_SegmentCollision;




    #endregion

    protected virtual void OnEnable()
    {
        sp_spines = serializedObject.FindProperty("SpineTransforms");
        sp_forw = serializedObject.FindProperty("ForwardReference");
        sp_drawg = serializedObject.FindProperty("drawGizmos");

        sp_BlendToOriginal = serializedObject.FindProperty("BlendToOriginal");
        sp_ReversedLeadBone = serializedObject.FindProperty("LastBoneLeading");
        sp_ConnectWithAnimator = serializedObject.FindProperty("ConnectWithAnimator");
        sp_BackwardMovement = serializedObject.FindProperty("BackwardMovement");
        sp_PhysicalClock = serializedObject.FindProperty("PhysicalUpdate");
        sp_SafeDeltaTime = serializedObject.FindProperty("SafeDeltaTime");
        sp_AnchoredSpine = serializedObject.FindProperty("AnchorToThis");
        sp_AutoAnchor = serializedObject.FindProperty("AutoAnchor");
        sp_AnchorRoot = serializedObject.FindProperty("AnchorRoot");
        sp_QueueToLastUpdate = serializedObject.FindProperty("QueueToLastUpdate");
        sp_PositionsNotAnimated = serializedObject.FindProperty("PositionsNotAnimated");
        sp_SelectivePosNotAnimated = serializedObject.FindProperty("SelectivePosNotAnimated");
        sp_RotationsNotAnimated = serializedObject.FindProperty("RotationsNotAnimated");
        sp_SelectiveRotNotAnimated = serializedObject.FindProperty("SelectiveRotNotAnimated");
        sp_ManualAffects = serializedObject.FindProperty("ManualAffectChain");
        sp_ManualRotationOffsets = serializedObject.FindProperty("ManualRotationOffsets");
        sp_ManualPositionOffsets = serializedObject.FindProperty("ManualPositionOffsets");
        sp_RoundCorrection = serializedObject.FindProperty("RoundCorrection");
        sp_UnifyCorrection = serializedObject.FindProperty("UnifyCorrection");
        sp_StartAfterTPose = serializedObject.FindProperty("StartAfterTPose");
        //sp_InverseX = serializedObject.FindProperty("InverseX");
        //sp_InverseZ = serializedObject.FindProperty("InverseZ");
        sp_InversedVerticalRotation = serializedObject.FindProperty("InversedVerticalRotation");
        sp_ChainMethod = serializedObject.FindProperty("ChainMethod");
        sp_OrientationReference = serializedObject.FindProperty("OrientationReference");

        sp_PositionsSmoother = serializedObject.FindProperty("PosSmoother");
        sp_RotationsSmoother = serializedObject.FindProperty("RotSmoother");
        sp_MaxStretching = serializedObject.FindProperty("MaxStretching");
        sp_Slithery = serializedObject.FindProperty("Slithery");
        sp_AngleLimit = serializedObject.FindProperty("AngleLimit");
        sp_LimitingAngleSmoother = serializedObject.FindProperty("LimitSmoother");
        sp_StraighteningSpeed = serializedObject.FindProperty("StraightenSpeed");
        sp_TurboStraighten = serializedObject.FindProperty("TurboStraighten");
        sp_GoBackSpeed = serializedObject.FindProperty("GoBackSpeed");
        sp_Springiness = serializedObject.FindProperty("Springiness");
        sp_SegmentsPivotOffset = serializedObject.FindProperty("SegmentsPivotOffset");
        sp_MainPivotOffset = serializedObject.FindProperty("MainPivotOffset");
        sp_DistancesMul = serializedObject.FindProperty("DistancesMultiplier");
        sp_AnimateLeadingBone = serializedObject.FindProperty("AnimateLeadingBone");
        sp_LeadingAnimateAfterMotion = serializedObject.FindProperty("LeadingAnimateAfterMotion");
        sp_LeadBoneRotationOffset = serializedObject.FindProperty("LeadBoneRotationOffset");
        sp_CustomAnchorRotationOffset = serializedObject.FindProperty("CustomAnchorRotationOffset");

        sp_DrawDebug = serializedObject.FindProperty("DrawDebug");
        sp_DebugAlpha = serializedObject.FindProperty("DebugAlpha");
        sp_AddDebugAlpha = serializedObject.FindProperty("AdditionalDebugAlpha");


        sp_UseCollisions = serializedObject.FindProperty("UseCollisions");
        sp_IncludedColliders = serializedObject.FindProperty("IncludedColliders");
        sp_CollidersScale = serializedObject.FindProperty("CollidersScale");
        sp_CollidersScaleMul = serializedObject.FindProperty("CollidersScaleMul");
        sp_CollidersAutoCurve = serializedObject.FindProperty("DifferenceScaleFactor");
        sp_CollidersOffsets = serializedObject.FindProperty("CollidersOffsets");
        sp_GravityPower = serializedObject.FindProperty("GravityPower");

        sp_AllCollidersOffset = serializedObject.FindProperty("OffsetAllColliders");
        sp_UseTruePosition = serializedObject.FindProperty("UseTruePosition");
        sp_DetailedCollision = serializedObject.FindProperty("DetailedCollision");
        sp_SegmentCollision = serializedObject.FindProperty("SegmentCollision");

        sp_Optim = serializedObject.FindProperty("OptimizeWithMesh");
        sp_Visibil = serializedObject.FindProperty("VisibilityRenderer");

        FSpineAnimator spineA = (FSpineAnimator)target;

        if (spineA.SpineTransforms != null)
        {
            if (spineA.SpineTransforms.Count > 0)
                startBone = FindStartBone(spineA);

            if (spineA.SpineTransforms.Count > 1)
                endBone = FindEndBone(spineA);

            if (startBone == null) startBone = spineA.SpineTransforms[0];
            if (endBone == null) endBone = spineA.SpineTransforms[spineA.SpineTransforms.Count - 1];
        }

        EditorUtility.SetDirty(target);
    }

    public override void OnInspectorGUI()
    {
        Undo.RecordObject(target, "Spine Animator Inspector");

        // Update component from last changes
        serializedObject.Update();

        FSpineAnimator spineA = (FSpineAnimator)target;

        #region Incorrection handling

        if (Application.isPlaying)
        {
            if (!incorrectionClicked)
                if (!spineA.wasIncorrectRemind)
                    if (spineA.incorrectionWarning)
                    {
                        if (!incorrection)
                        {
                            incorrection = true;
                            drawAnimationOptions = false;
                            drawAdvancedCorrection = false;
                            drawQuickCorrection = true;
                            EditorPrefs.SetInt(spineA.name + "-" + spineA.GetInstanceID(), 1);
                        }
                    }
        }
        else
        {
            if (!incorrectionClicked)
                if (!spineA.wasIncorrectRemind)
                    if (EditorPrefs.GetInt(spineA.name + "-" + spineA.GetInstanceID()) == 1)
                    {
                        EditorPrefs.SetInt(spineA.name + "-" + spineA.GetInstanceID(), 0);
                        spineA.wasIncorrectRemind = true;
                        drawAnimationOptions = false;
                        drawAdvancedCorrection = false;
                        drawQuickCorrection = true;
                        incorrection = true;
                    }
        }

        #endregion

        //if (GUILayout.Button(new GUIContent("Dev Log"))) spineA.DevLog();

        #region Default Inspector
        if (drawDefaultInspector)
        {
            EditorGUILayout.BeginVertical(FEditor_Styles.GrayBackground);
            drawDefaultInspector = GUILayout.Toggle(drawDefaultInspector, "Default inspector");

            #region Exluding from view not needed properties

            List<string> excludedVars = new List<string>();

            if (!spineA.PositionsNotAnimated) excludedVars.Add("SelectivePosNotAnimated");
            if (!spineA.RotationsNotAnimated) excludedVars.Add("SelectiveRotNotAnimated");

            #endregion

            EditorGUILayout.EndVertical();

            // Draw default inspector without not needed properties
            DrawPropertiesExcluding(serializedObject, excludedVars.ToArray());
        }
        else
        #endregion
        {
            if (!incorrection)
            {
                EditorGUILayout.BeginVertical(FEditor_Styles.GrayBackground);
            }
            else
            {
                EditorGUILayout.BeginVertical(FEditor_Styles.Style(new Color(0.9f, 0.44f, 0.33f, 0.95f)));
                EditorGUILayout.HelpBox("There was detected strange behaviour of your spine in playmode, check highlighted parameters for solution guidement.", MessageType.Error);

                if (spineA.ConnectWithAnimator) EditorGUILayout.HelpBox("Are you sure you have enabled animator? If you not using animator, disable 'ConnectWithAnimator' option.", MessageType.Warning);

                bool was = incorrection;
                incorrection = GUILayout.Toggle(incorrection, "Show highlighting");
                if (was && !incorrection) incorrectionClicked = true;
            }

            EditorGUILayout.BeginHorizontal();
            drawDefaultInspector = GUILayout.Toggle(drawDefaultInspector, "Default inspector");

            GUILayout.FlexibleSpace();
            EditorGUIUtility.labelWidth = 88;
            EditorGUILayout.PropertyField(sp_drawg);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            GUILayout.Space(5f);
            Color preCol = GUI.color;

            EditorGUILayout.BeginVertical(FEditor_Styles.Style(FColorMethods.ChangeColorAlpha(Color.white, 0.25f)));
            EditorGUI.indentLevel++;

            drawMain = EditorGUILayout.Foldout(drawMain, "Main Parameters", true);

            #region Main Tab

            if (drawMain)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                if (spineA.SpineTransforms == null || spineA.SpineTransforms.Count < 1)
                {
                    GUILayout.BeginHorizontal(FEditor_Styles.YellowBackground);
                    EditorGUILayout.HelpBox("Put here two marginal bones from hierarchy and click 'Get' to create spine chain of section you want to animate with spine animator", MessageType.Info);
                    GUILayout.EndHorizontal();


                }

                EditorGUILayout.BeginVertical(FEditor_Styles.LGrayBackground);

                if (spineA.SpineTransforms == null || spineA.SpineTransforms.Count < 1)
                {
                    GUIStyle smallStyle = new GUIStyle(EditorStyles.miniLabel) { fontStyle = FontStyle.Italic, fontSize = 9 };
                    GUI.color = new Color(1f, 1f, 1f, 0.7f);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label(new GUIContent("             Enter for tooltip", "If you rigging quadroped or other animal, start bone should be pelvis bone with back legs and tail inside the hierarchy"), smallStyle);
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(new GUIContent("Enter for tooltip                       ", "If you rigging quadroped or other animal, end bone should be chest bone / neck bone or head bone, depends of your needs and model structure"), smallStyle);
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(1f);
                }

                EditorGUILayout.BeginHorizontal();
                int wrong = 0;
                if (spineA.SpineTransforms != null)
                {
                    if (spineA.SpineTransforms.Count < 2) wrong = 2;
                    else
                    {
                        if (startBone != spineA.SpineTransforms[0] || endBone != spineA.SpineTransforms[spineA.SpineTransforms.Count - 1])
                        {
                            wrong = 3;
                        }
                    }
                }
                else wrong = 1;

                if (wrong == 1) GUI.color = new Color(1f, 0.3f, 0.3f, 0.85f);
                if (wrong == 2) GUI.color = new Color(1f, 0.7f, 0.2f, 0.85f);

                EditorGUI.indentLevel--;

                EditorGUIUtility.labelWidth = 42f;
                if (startBone == null) startBone = FindStartBone(spineA);
                if (endBone == null) endBone = FindEndBone(spineA);
                startBone = (Transform)EditorGUILayout.ObjectField(new GUIContent("Start", "Put here first bone in hierarchy depth for automatically get chain of bones to end one"), startBone, typeof(Transform), true);
                endBone = (Transform)EditorGUILayout.ObjectField(new GUIContent("End", "Put here last bone in hierarchy depth for automatically get chain of bones from start one"), endBone, typeof(Transform), true);
                EditorGUIUtility.labelWidth = 0f;

                if (GUILayout.Button(new GUIContent("L", "Automatically get last bone in hierarchy - it depends of children placement, then sometimes last bone can be found wrong, whne you have arms/legs bones inside, if they're higher, algorithm will go through them"), new GUILayoutOption[2] { GUILayout.MaxWidth(24), GUILayout.MaxHeight(14) })) GetLastBoneInHierarchy();

                if (wrong == 3) GUI.color = new Color(0.2f, 1f, 0.4f, 0.85f);
                if (startBone != null && endBone != null)
                {
                    GUI.color = new Color(0.3f, 1f, 0.4f, 0.8f);

                    if (spineA.SpineTransforms != null)
                    {
                        if (spineA.SpineTransforms.Count > 0)
                        {
                            if (startBone != spineA.SpineTransforms[0] || endBone != spineA.SpineTransforms[spineA.SpineTransforms.Count - 1]) wrong = 3; else GUI.color = FColorMethods.ChangeColorAlpha(preCol, 0.7f);
                        }
                    }
                }

                if (GUILayout.Button(new GUIContent("Get"), new GUILayoutOption[2] { GUILayout.MaxWidth(36), GUILayout.MaxHeight(14) }))
                {
                    GetBonesChainFromStartToEnd();
                    spineA.TryAutoCorrect(null, false);
                    EditorUtility.SetDirty(target);
                }


                GUI.color = preCol;

                EditorGUILayout.EndHorizontal();


                if (spineA.SpineTransforms == null || spineA.SpineTransforms.Count < 1)
                {
                    GUIStyle smallStyle = new GUIStyle(EditorStyles.miniLabel) { fontStyle = FontStyle.Italic, fontSize = 9 };
                    GUI.color = new Color(1f, 1f, 1f, 0.7f);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label(new GUIContent("        Pelvis bone with Legs", "If you rigging quadroped or other animal, start bone should be pelvis bone with back legs and tail inside the hierarchy"), smallStyle);
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(new GUIContent("Chest/Neck/Head bone             ", "If you rigging quadroped or other animal, end bone should be chest bone / neck bone or head bone, depends of your needs and model structure"), smallStyle);
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(1f);
                }


                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel++;

                if (spineA.SpineTransforms == null || spineA.SpineTransforms.Count < 1)
                {
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndVertical();
                    return;
                }

                EditorGUIUtility.labelWidth = 148f;
                EditorGUILayout.PropertyField(sp_forw);
                EditorGUIUtility.labelWidth = 0;

                if (spineA.SpineTransforms.Count < 1)
                    EditorGUILayout.BeginVertical(FEditor_Styles.GrayBackground);
                //EditorGUILayout.BeginHorizontal(FEditor_Styles.Style(new Color32(99, 50, 166, 45)));
                else
                    EditorGUILayout.BeginVertical(FEditor_Styles.LGrayBackground);
                //EditorGUILayout.BeginHorizontal(FEditor_Styles.Style(new Color32(10, 66, 175, 25)));

                //EditorGUILayout.PropertyField(sp_spines, true);

                drawSpineTransforms = EditorGUILayout.Foldout(drawSpineTransforms, new GUIContent("Spine Transforms", "Spine chain transforms"), true, new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold });

                if (drawSpineTransforms)
                {
                    EditorGUIUtility.labelWidth = 120;
                    for (int i = 0; i < spineA.SpineTransforms.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();

                        GUI.enabled = false;
                        EditorGUILayout.ObjectField("Spine Bone [" + i + "]", spineA.SpineTransforms[i], typeof(Transform), true);
                        if (i != 0 && i != spineA.SpineTransforms.Count - 1) GUI.enabled = true;

                        if (GUILayout.Button("X", new GUILayoutOption[2] { GUILayout.Width(20), GUILayout.Height(14) }))
                        {
                            spineA.SpineTransforms.RemoveAt(i);
                            EditorUtility.SetDirty(target);
                            break;
                        }

                        GUI.enabled = true;
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUIUtility.labelWidth = 0;
                }

                EditorGUILayout.EndVertical();

                EditorGUIUtility.labelWidth = 124f;
                EditorGUI.indentLevel--;

                EditorGUILayout.BeginVertical(FEditor_Styles.Style(new Color32(0, 200, 100, 22)));
                EditorGUILayout.BeginVertical(FEditor_Styles.Style(new Color32(0, 200, 100, 0)));
                GUILayout.Space(2f);
                EditorGUILayout.PropertyField(sp_BlendToOriginal, true);
                GUILayout.Space(3f);

                EditorGUIUtility.labelWidth = 128f;

                GUI.color = new Color(0.3f, 1f, 0.4f, 0.8f);
                EditorGUILayout.PropertyField(sp_ReversedLeadBone, true);
                GUI.color = preCol;

                GUILayout.Space(2f);

                bool animatorDetected = CheckForAnimator(spineA);
                if (!animatorDetected && spineA.ConnectWithAnimator) GUI.color = new Color(1f, 0.2f, 0.2f, 0.8f);
                else if (animatorDetected && spineA.ConnectWithAnimator == false) GUI.color = new Color(1f, 1f, 0.35f, 0.8f);

                EditorGUIUtility.labelWidth = 163f;
                EditorGUILayout.PropertyField(sp_ConnectWithAnimator, true);
                GUI.color = preCol;

                GUILayout.Space(3f);
                EditorGUILayout.PropertyField(sp_Optim, true);

                if (spineA.OptimizeWithMesh)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(sp_Visibil, true);
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();
                GUILayout.Space(2f);

                EditorGUIUtility.labelWidth = 0f;

                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel++;
            }

            #endregion

            #region Animation Options

            EditorGUILayout.BeginVertical(FEditor_Styles.LGrayBackground);
            drawAnimationOptions = EditorGUILayout.Foldout(drawAnimationOptions, "Animation Options", true);

            if (drawAnimationOptions)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUI.indentLevel--;
                GUI.color = new Color(0.55f, 0.75f, 0.9f, 0.85f);

                GUILayout.BeginVertical(FEditor_Styles.Style(new Color32(33, 130, 200, 24)));

                EditorGUIUtility.labelWidth = 105f;
                GUILayout.Space(4f);

                Color preCc = GUI.color;

                if (spineA.UseCollisions) if (spineA.UseTruePosition)
                    {
                        if (spineA.PosSmoother < 0.075f) GUI.color = new Color(0.9f, 0.5f, 0.5f); else if (spineA.PosSmoother < 0.225f) GUI.color = Color.Lerp(new Color(0.9f, 0.6f, 0.6f), preCc, Mathf.InverseLerp(0.075f, 0.225f, spineA.PosSmoother));
                    }

                EditorGUILayout.PropertyField(sp_PositionsSmoother, true);
                GUI.color = preCc;

                EditorGUILayout.PropertyField(sp_RotationsSmoother, true);
                if (spineA.PosSmoother > 0.1f || spineA.Springiness > 0f) EditorGUILayout.PropertyField(sp_MaxStretching, true);
                GUILayout.Space(5f);

                EditorGUILayout.EndVertical();

                GUILayout.BeginVertical(FEditor_Styles.Style(new Color32(33, 130, 230, 58)));

                EditorGUILayout.PropertyField(sp_AngleLimit, true);
                EditorGUILayout.PropertyField(sp_LimitingAngleSmoother, true);

                EditorGUILayout.EndVertical();

                GUILayout.BeginVertical(FEditor_Styles.Style(new Color32(33, 130, 230, 58)));

                EditorGUIUtility.labelWidth = 120f;
                EditorGUILayout.PropertyField(sp_StraighteningSpeed, true);
                EditorGUIUtility.labelWidth = 0f;

                if (spineA.StraightenSpeed > 0f)
                {
                    EditorGUI.indentLevel++;
                    EditorGUIUtility.labelWidth = 121f;
                    EditorGUILayout.PropertyField(sp_TurboStraighten, true);
                    EditorGUI.indentLevel--;
                    GUILayout.Space(3f);
                    EditorGUIUtility.labelWidth = 105f;
                }

                EditorGUILayout.EndVertical();

                GUILayout.BeginVertical(FEditor_Styles.LBlueBackground);

                EditorGUIUtility.labelWidth = 105f;

                if (!spineA.LastBoneLeading)
                    if (spineA.Springiness > 0)
                        if (spineA.GoBackSpeed <= 0)
                            GUI.color = new Color(0.55f, 0.9f, 1f, 1f);

                EditorGUILayout.PropertyField(sp_GoBackSpeed, true);
                EditorGUILayout.PropertyField(sp_Slithery, true);

                GUI.color = new Color(0.55f, 0.75f, 0.9f, 0.85f);


                if (!spineA.LastBoneLeading)
                {
                    if (spineA.Springiness <= 0)
                        if (spineA.GoBackSpeed > 0.15f)
                            GUI.color = new Color(0.55f, 0.9f, 1f, 0.95f);

                    EditorGUILayout.PropertyField(sp_Springiness, true);
                }


                EditorGUILayout.EndVertical();

                EditorGUIUtility.labelWidth = 0f;
                EditorGUILayout.EndVertical();
                GUILayout.Space(5f);
                EditorGUI.indentLevel++;

                GUI.color = preCol;
            }

            EditorGUILayout.EndVertical();

            #endregion

            #region Basic Correction

            if (incorrection)
                EditorGUILayout.BeginVertical(FEditor_Styles.Style(new Color(1f, 0.3f, 0.3f, 0.4f)));
            else
                EditorGUILayout.BeginVertical(FEditor_Styles.LGrayBackground);

            EditorGUILayout.BeginHorizontal();
            drawQuickCorrection = EditorGUILayout.Foldout(drawQuickCorrection, "Main Tuning Options", true);
            //GUILayout.FlexibleSpace();

            //if (!drawPreciseAutoCorr)
            //    GUI.color = new Color(0.7f, 1f, 0.9f, 0.9f);
            //else
            //    GUI.color = new Color(0.95f, 1f, 0.95f, 0.85f);

            //GUI.color = new Color(0.7f, 1f, 0.9f, 0.9f);
            //if (GUILayout.Button(new GUIContent("Auto", "Algorithm will analyze your skeleton and will try to find correct options, this correction is done automatically when you add coponent, but when you make some changes you can reset them by clicking here again."), new GUILayoutOption[2] { GUILayout.MaxWidth(44), GUILayout.MaxHeight(14) }))
            //{
            //    spineA.TryAutoCorrect();
            //    EditorUtility.SetDirty(target);
            //}

            //if (GUILayout.Button(new GUIContent("Precise", "Opening auto correction field with more precise option"), new GUILayoutOption[2] { GUILayout.MaxWidth(57), GUILayout.MaxHeight(14) }))
            //{
            //    drawPreciseAutoCorr = !drawPreciseAutoCorr;
            //}

            GUI.color = preCol;

            EditorGUILayout.EndHorizontal();

            if (drawPreciseAutoCorr)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUI.indentLevel--;
                EditorGUIUtility.labelWidth = 74f;

                if (!headBone) GUI.color = new Color(0.9f, 0.3f, 0.3f, 0.9f);
                headBone = (Transform)EditorGUILayout.ObjectField(new GUIContent("Head bone", "Head bone or some bone before, it's important to be in front of spine and not included in spine animator's chain"), headBone, typeof(Transform), true);
                GUI.color = preCol;

                if (headBone)
                {
                    if (GUILayout.Button(new GUIContent("Try Correct", "Auto correcting in reference to head bone position"), new GUILayoutOption[2] { GUILayout.MaxWidth(88), GUILayout.MaxHeight(14) }))
                    {
                        spineA.TryAutoCorrect(headBone);
                        EditorUtility.SetDirty(target);
                    }
                }

                EditorGUI.indentLevel++;
                EditorGUIUtility.labelWidth = 0f;

                EditorGUILayout.EndHorizontal();
            }


            if (drawQuickCorrection)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUIUtility.labelWidth = 171f;

                GUILayout.Space(3f);

                //EditorGUILayout.PropertyField(sp_InversedVerticalRotation, true);
                EditorGUILayout.PropertyField(sp_ChainMethod, true);

                //EditorGUILayout.PropertyField(sp_InverseX, true);
                //EditorGUILayout.PropertyField(sp_InverseZ, true);
                //GUILayout.Space(3f);
                //EditorGUILayout.PropertyField(sp_BackwardMovement, true);
                GUILayout.Space(3f);

                if (spineA.ConnectWithAnimator)
                {
                    if (!incorrection)
                        GUILayout.BeginVertical(FEditor_Styles.Style(new Color32(33, 200, 130, 24)));
                    else
                        GUILayout.BeginVertical(FEditor_Styles.Style(new Color32(255, 111, 111, 55)));

                    EditorGUILayout.PropertyField(sp_PositionsNotAnimated, true);

                    if (spineA.PositionsNotAnimated)
                    {
                        spineA.RefreshSelectivePosNotAnimated();

                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(sp_SelectivePosNotAnimated, true);
                        EditorGUI.indentLevel--;
                    }
                    GUILayout.EndVertical();


                    if (incorrection)
                        GUILayout.BeginVertical(FEditor_Styles.Style(new Color32(255, 111, 111, 55)));
                    else
                        GUILayout.BeginVertical(FEditor_Styles.Style(new Color32(33, 130, 200, 24)));

                    EditorGUIUtility.labelWidth = 174f;
                    GUILayout.Space(3f);
                    EditorGUILayout.PropertyField(sp_RotationsNotAnimated, true);

                    if (spineA.RotationsNotAnimated)
                    {
                        spineA.RefreshSelectiveRotNotAnimated();

                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(sp_SelectiveRotNotAnimated, true);
                        EditorGUI.indentLevel--;
                    }
                    GUILayout.EndVertical();
                    GUILayout.Space(5f);

                    //if (incorrection) GUILayout.EndVertical();
                }

                //if (incorrection) GUILayout.EndVertical();

                EditorGUIUtility.labelWidth = 146f;
                //EditorGUILayout.PropertyField(sp_RefinedCorrection, true);

                if (!Application.isPlaying)
                {
                    //GUILayout.BeginVertical(FEditor_Styles.Style(new Color32(33, 200, 130, 24)));
                    EditorGUILayout.PropertyField(sp_StartAfterTPose, true);
                    //GUILayout.EndVertical();
                }

                GUILayout.Space(3f);
                EditorGUILayout.PropertyField(sp_MainPivotOffset, true);

                //EditorGUILayout.PropertyField(sp_RoundCorrection, true);
                //EditorGUILayout.PropertyField(sp_UnifyCorrection, true);

                EditorGUIUtility.labelWidth = 0f;
                GUILayout.Space(5f);

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();

            #endregion

            #region Advanced correction

            EditorGUILayout.BeginVertical(FEditor_Styles.LGrayBackground);
            drawAdvancedCorrection = EditorGUILayout.Foldout(drawAdvancedCorrection, "Advanced Options", true);

            if (drawAdvancedCorrection)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                GUILayout.Space(4f);
                EditorGUIUtility.labelWidth = 144f;
                if (!Application.isPlaying) EditorGUILayout.PropertyField(sp_PhysicalClock, true);
                EditorGUILayout.PropertyField(sp_SafeDeltaTime, true);

                EditorGUIUtility.labelWidth = 166f;

                if (spineA.AnchorRoot && !spineA.AnchorToThis && !spineA.QueueToLastUpdate) GUI.color = new Color(0.5f, 1f, 0.65f, 0.85f);

                if (!Application.isPlaying) EditorGUILayout.PropertyField(sp_QueueToLastUpdate, true);
                GUI.color = preCol;

                FEditor_Styles.DrawUILine(new Color(0.6f, 0.6f, 0.6f, 0.4f));

                GUILayout.BeginVertical(FEditor_Styles.Style(new Color32(33, 200, 130, 24)));
                if (spineA.AnchorToThis) EditorGUILayout.PropertyField(sp_LeadBoneRotationOffset, true);

                bool was = spineA.AnchorToThis;
                EditorGUILayout.PropertyField(sp_AnchoredSpine, true);
                serializedObject.ApplyModifiedProperties();

                if (was && spineA.AnchorToThis == false)
                {
                    spineA.QueueToLastUpdate = true;
                }

                GUILayout.EndVertical();

                if (!spineA.LastBoneLeading)
                {
                    EditorGUILayout.PropertyField(sp_AutoAnchor, true);
                }

                if (!spineA.AnchorToThis)
                {
                    EditorGUI.indentLevel++;

                    if (spineA.AnchorRoot == null)
                    {
                        EditorGUILayout.HelpBox("If you connecting tail to spine animated by SpineAnimator, try to put into 'Anchor Root' same bone as first bone for the spine, enable 'QueueToLastUpdate' and set some 'GoBack' value", MessageType.Info);

                        EditorGUILayout.BeginHorizontal();
                    }

                    EditorGUILayout.PropertyField(sp_AnchorRoot, true);

                    if (spineA.AnchorRoot == null)
                    {
                        if (GUILayout.Button(new GUIContent("Parent", "Putting this transform's parent onto field"), new GUILayoutOption[2] { GUILayout.MaxWidth(88), GUILayout.MaxHeight(14) }))
                        {
                            spineA.AnchorRoot = spineA.transform.parent;
                            EditorUtility.SetDirty(target);
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.PropertyField(sp_CustomAnchorRotationOffset, true);

                    EditorGUI.indentLevel--;
                }

                GUILayout.Space(4f);

                EditorGUILayout.PropertyField(sp_AnimateLeadingBone, true);

                if (spineA.AnimateLeadingBone)
                {
                    EditorGUI.indentLevel++;
                    EditorGUIUtility.labelWidth = 216f;
                    EditorGUILayout.PropertyField(sp_LeadingAnimateAfterMotion, true);
                    EditorGUIUtility.labelWidth = 0f;
                    EditorGUI.indentLevel--;
                }

                FEditor_Styles.DrawUILine(new Color(0.6f, 0.6f, 0.6f, 0.4f));

                //GUILayout.Space(5f);
                EditorGUILayout.PropertyField(sp_SegmentsPivotOffset, true);
                EditorGUILayout.PropertyField(sp_DistancesMul, true);

                FEditor_Styles.DrawUILine(new Color(0.6f, 0.6f, 0.6f, 0.4f));

                GUILayout.Space(3f);
                EditorGUI.indentLevel++;

                spineA.RefreshManualPosOffs();
                spineA.RefreshManualRotOffs();

                EditorGUILayout.PropertyField(sp_ManualAffects, true);
                EditorGUILayout.PropertyField(sp_ManualPositionOffsets, true);
                EditorGUILayout.PropertyField(sp_ManualRotationOffsets, true);
                GUILayout.Space(5f);
                EditorGUI.indentLevel--;

                EditorGUIUtility.labelWidth = 0f;
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();

            #endregion


            #region Debug Options

            EditorGUILayout.BeginVertical(FEditor_Styles.LGrayBackground);
            drawDebug = EditorGUILayout.Foldout(drawDebug, "Debugging", true);

            if (drawDebug)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                GUILayout.BeginHorizontal(FEditor_Styles.LBlueBackground);
                EditorGUILayout.HelpBox("When 'DrawDebug' is toggled, you can use button '~' to instantly deactivate SpineAnimator's motion for time you hold this button (not on build)", MessageType.None);
                GUILayout.EndHorizontal();

                GUILayout.Space(5f);
                EditorGUILayout.PropertyField(sp_DrawDebug, true);
                EditorGUILayout.PropertyField(sp_DebugAlpha, true);

                if (spineA.DrawDebug)
                    EditorGUILayout.PropertyField(sp_AddDebugAlpha, true);

                EditorGUILayout.PropertyField(sp_drawg, true);
                GUILayout.Space(5f);

                EditorGUIUtility.labelWidth = 0f;
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();

            #endregion


            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;

            DrawPhysicalOptionsTab(spineA);
        }

        // Apply changed parameters variables
        serializedObject.ApplyModifiedProperties();
    }

    private Transform FindEndBone(FSpineAnimator spineA)
    {
        Transform target = null;

        SkinnedMeshRenderer[] skins = spineA.GetComponentsInChildren<SkinnedMeshRenderer>();
        Transform[] children;

        foreach (var s in skins)
        {
            children = s.bones;

            // Search for neck
            foreach (Transform t in children)
            {
                if (t.name.ToLower().Contains("neck")) return t;
                if (t.name.ToLower().Contains("head")) return t;
                if (t.name.ToLower().Contains("chest")) return t;
            }
        }

        // Search for neck
        if (spineA.SpineTransforms != null)
        {
            if (spineA.SpineTransforms.Count > 0) children = spineA.SpineTransforms[0].GetComponentsInChildren<Transform>();
            else children = spineA.GetComponentsInChildren<Transform>();
        }
        else children = spineA.GetComponentsInChildren<Transform>();

        foreach (Transform t in children)
        {
            if (t.name.ToLower().Contains("neck")) return t;
            if (t.name.ToLower().Contains("head")) return t;
            if (t.name.ToLower().Contains("chest")) return t;
        }

        return target;
    }

    private Transform FindStartBone(FSpineAnimator spineA)
    {
        Transform target = spineA.transform;

        // Search for neck
        SkinnedMeshRenderer[] skins = spineA.GetComponentsInChildren<SkinnedMeshRenderer>();
        Transform[] children;

        foreach (var s in skins)
        {
            children = s.bones;

            // Search for neck
            foreach (Transform t in children)
            {
                if (t.name.ToLower().Contains("pelv")) return t;
                if (t.name.ToLower().Contains("root")) return t;
                if (t.name.ToLower().Contains("spine")) return t;
            }
        }

        children = spineA.GetComponentsInChildren<Transform>();

        foreach (Transform t in children)
        {
            if (t.name.ToLower().Contains("pelv")) return t;
            if (t.name.ToLower().Contains("root")) return t;
            if (t.name.ToLower().Contains("spine")) return t;
        }

        return target;
    }

    /// <summary>
    /// Getting last bone in hierarhy going up by first children
    /// </summary>
    void GetLastBoneInHierarchy()
    {
        if (startBone == null)
        {
            Debug.LogWarning("Start bone is not defined in " + target.name);
            return;
        }

        Transform c = startBone;

        // Try to find bones with spine word in it and go through deepest found with this name
        Transform spine = null;
        foreach (Transform t in c.GetComponentsInChildren<Transform>())
        {
            if (t.name.ToLower().Contains("spine"))
            {
                spine = t;
                break;
            }
        }

        if (spine != null) c = spine;

        // I'm scared of while() loops so I just put here iterator to limit in some case
        for (int i = 0; i < 1000; i++)
        {
            if (c.childCount > 0)
            {
                for (int j = 0; j < c.childCount; j++)
                    if (c.GetChild(j).name.ToLower().Contains("spine"))
                    {
                        c = c.GetChild(j);
                        break;
                    }

                c = c.GetChild(0);
            }
            else break;
        }

        endBone = c;
    }

    /// <summary>
    /// Getting bones automatically by defining start and end in hierarchy
    /// </summary>
    private void GetBonesChainFromStartToEnd()
    {
        if (startBone == null)
        {
            Debug.LogWarning("Start bone is not defined in " + target.name);
            return;
        }

        if (endBone == null)
        {
            Debug.LogWarning("End bone is not defined in " + target.name);
            return;
        }

        List<Transform> bones = new List<Transform>();
        Transform p = endBone;
        bool wrong = false;

        // I'm scared of while() loops so I just put here iterator to limit in some case
        for (int i = 0; i < 1000; i++)
        {
            bool willStop = false;
            if (p == startBone) willStop = true;

            if (p == null)
            {
                wrong = true;
                break;
            }

            bones.Add(p);
            p = p.parent;

            if (willStop) break;
        }

        if (wrong)
        {
            Debug.LogError("Something went wrong during getting bones automatically for " + target.name + ". Did you assigned start and end bone correctly? It should go only up in hierarchy (end bone should be nested child of start bone).");
            return;
        }

        FSpineAnimator spineA = target as FSpineAnimator;
        if (spineA)
        {
            bones.Reverse();
            spineA.SpineTransforms = bones;
        }

        if (spineA.SpineTransforms.Count > 2)
        {
            // Checking if we should warn user about incorrect rotation of main object
            Transform main = spineA.transform;

            Animator anim = spineA.GetComponentInChildren<Animator>();

            if (!anim)
                if (main.parent != null)
                {
                    anim = main.parent.GetComponent<Animator>();
                    if (!anim) if (main.parent.parent != null) anim = main.parent.parent.GetComponent<Animator>();
                }

            bool shouldViewIfWillIncorrect = false;

            if (!anim)
            {
                if (main.parent == null) shouldViewIfWillIncorrect = true;
                else
                    if (main.parent.parent == null) shouldViewIfWillIncorrect = true;
            }
            else shouldViewIfWillIncorrect = true;

            if (shouldViewIfWillIncorrect)
            {
                Vector3 averageDirection = Vector3.zero;

                for (int i = 1; i < spineA.SpineTransforms.Count - 1; i++)
                    averageDirection += spineA.SpineTransforms[i + 1].position - spineA.SpineTransforms[i].position;

                averageDirection.Normalize();

                float diffDot = Vector3.Dot(main.forward, averageDirection);

                if (diffDot < 0.75f && diffDot > -0.75f)
                {
                    if (spineA.getReminders < 2)
                    {
                        EditorUtility.DisplayDialog("It seems setup can be wrong!", "Algorithm detected that your main object is not facing Z axis (blue:forward) which can cause SpineAnimator to act wrong, also please check user manual if you will have other troubles. (Assets/FImpossible Creations/Spine Animator/Spine Animator User Manual.pdf)", "Ok I will check User Manual in SpineAnimator directory");
                        spineA.getReminders++;
                    }
                    else
                    {
                        Debug.LogWarning("(1/2) It seems setup for " + spineA.name + " can be wrong!");
                        Debug.LogWarning("(2/2) Algorithm detected that your main object is not facing Z axis (blue: forward) which can cause SpineAnimator to act wrong, also please check user manual if you will have other troubles. (Assets / FImpossible Creations / Spine Animator / Spine Animator User Manual.pdf)");
                    }
                }

                //Debug.Log("Dot = " + Vector3.Dot(main.forward, averageDirection ) + " av = " + averageDirection + " trs " + main.TransformDirection(averageDirection) + " forw = " + main.forward);
            }
        }
    }

    protected bool CheckForAnimator(FSpineAnimator spineAnimator)
    {
        Animation animation = null;
        Animator animator = spineAnimator.GetComponentInChildren<Animator>();
        if (!animator) if (spineAnimator.transform.parent) if (spineAnimator.transform.parent.parent) animator = spineAnimator.transform.parent.parent.GetComponentInChildren<Animator>(); else animator = spineAnimator.transform.parent.GetComponent<Animator>();

        if (!animator)
        {
            animation = spineAnimator.GetComponentInChildren<Animation>();
            if (!animation) if (spineAnimator.transform.parent) if (spineAnimator.transform.parent.parent) animation = spineAnimator.transform.parent.parent.GetComponentInChildren<Animation>(); else animation = spineAnimator.transform.parent.GetComponent<Animation>();
        }

        if (!animator && !animation)
        {
            if (spineAnimator.ForwardReference)
            {
                animator = spineAnimator.ForwardReference.GetComponentInChildren<Animator>();
                if (!animator) animation = spineAnimator.ForwardReference.GetComponentInChildren<Animation>();
            }

            if (!animator && !animation)
                if (spineAnimator.SpineTransforms.Count > 0)
                    if (spineAnimator.SpineTransforms[0].parent)
                    {
                        animator = spineAnimator.SpineTransforms[0].parent.GetComponentInChildren<Animator>();
                        if (!animator) animation = spineAnimator.SpineTransforms[0].parent.GetComponentInChildren<Animation>();
                    }
        }

        if (spineAnimator.ConnectWithAnimator)
            if (animator)
            {
                if (animator.runtimeAnimatorController == null)
                {
                    EditorGUILayout.HelpBox("No 'Animator Controller' inside Animator", MessageType.Warning);
                    animator = null;
                }
            }


        if (animator != null || animation != null)
        {
            if (animator) if (!animator.enabled) return false;
            if (animation) if (!animation.enabled) return false;
            return true;
        }
        else return false;
    }



    #region Physical Experimental Stuff

    protected static bool drawCollisionParams = false;

    protected virtual void DrawPhysicalOptionsTab(FSpineAnimator spine)
    {
        EditorGUIUtility.labelWidth = 130;

        EditorGUILayout.BeginVertical(FEditor_Styles.Style(new Color(0.9f, 0.9f, 0.9f, 0.15f)));

        EditorGUI.indentLevel++;

        GUILayout.BeginHorizontal(FEditor_Styles.LGrayBackground);
        drawCollisionParams = EditorGUILayout.Foldout(drawCollisionParams, "Collisions (Experimental)", true);
        GUILayout.EndHorizontal();

        if (drawCollisionParams)
        {
            GUILayout.Space(3f);
            DrawPhysicsStuff(spine);
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();

        EditorGUIUtility.labelWidth = 0;
    }

    private void DrawPhysicsStuff(FSpineAnimator spine)
    {
        EditorGUIUtility.labelWidth = 140;

        EditorGUILayout.BeginVertical(FEditor_Styles.GrayBackground);

        EditorGUILayout.PropertyField(sp_UseCollisions);

        if (spine.UseCollisions)
        {
            spine.RefreshCollidersOffsets();

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Collision support is experimental and not working fully correct yet.", MessageType.Info);
                //EditorGUI.indentLevel++;

                if (spine.IncludedColliders.Count == 0)
                    EditorGUILayout.BeginVertical(FEditor_Styles.RedBackground);
                else
                    EditorGUILayout.BeginVertical(FEditor_Styles.Emerald);

                Color c = GUI.color;
                GUILayout.BeginVertical();
                if (ActiveEditorTracker.sharedTracker.isLocked) GUI.color = new Color(0.44f, 0.44f, 0.44f, 0.8f); else GUI.color = new Color(0.95f, 0.95f, 0.99f, 0.9f);
                if (GUILayout.Button(new GUIContent("Lock Inspector for Drag & Drop Colliders", "Drag & drop colliders to 'Included Colliders' List from the hierarchy"), EditorStyles.toolbarButton)) ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
                GUI.color = c;
                GUILayout.EndVertical();

                EditorGUILayout.PropertyField(sp_IncludedColliders, true);
                EditorGUILayout.EndVertical();
                // EditorGUI.indentLevel--;

                FEditor_Styles.DrawUILine(new Color(0.5f, 0.5f, 0.5f, 0.6f));

                EditorGUILayout.PropertyField(sp_CollidersScaleMul, new GUIContent("Scale Multiplier"));
                EditorGUILayout.PropertyField(sp_CollidersScale, new GUIContent("Scale Curve"));
                EditorGUILayout.PropertyField(sp_CollidersAutoCurve, new GUIContent("Auto Curve"));
                FEditor_Styles.DrawUILine(new Color(0.6f, 0.6f, 0.6f, 0.4f));
                EditorGUILayout.PropertyField(sp_AllCollidersOffset, true);

                if (spine.UseTruePosition)
                    EditorGUILayout.BeginVertical(FEditor_Styles.RedBackground);
                else
                    EditorGUILayout.BeginVertical(FEditor_Styles.YellowBackground);

                EditorGUILayout.PropertyField(sp_UseTruePosition, true);
                EditorGUILayout.EndVertical();

                EditorGUILayout.PropertyField(sp_SegmentCollision, true);
                Color preCol = GUI.color;
                if (spine.GravityPower != Vector3.zero) if (!spine.DetailedCollision) GUI.color = new Color(1f, 1f, 0.35f, 0.8f);
                GUI.color = preCol;

                EditorGUILayout.PropertyField(sp_CollidersOffsets, true);
                EditorGUILayout.PropertyField(sp_DetailedCollision, true);


                FEditor_Styles.DrawUILine(new Color(0.6f, 0.6f, 0.6f, 0.4f));
                EditorGUILayout.PropertyField(sp_GravityPower, true);

                GUILayout.Space(3f);
            }
            else // In Playmode
            {
                EditorGUILayout.BeginVertical(FEditor_Styles.Emerald);

                Color c = GUI.color;
                GUILayout.BeginVertical();
                if (ActiveEditorTracker.sharedTracker.isLocked) GUI.color = new Color(0.44f, 0.44f, 0.44f, 0.8f); else GUI.color = new Color(0.95f, 0.95f, 0.99f, 0.9f);
                if (GUILayout.Button(new GUIContent("Lock Inspector for Drag & Drop Colliders", "Drag & drop colliders to 'Included Colliders' List from the hierarchy"), EditorStyles.toolbarButton)) ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
                GUI.color = c;
                GUILayout.EndVertical();

                EditorGUILayout.PropertyField(sp_IncludedColliders, true);
                EditorGUILayout.EndVertical();
                // EditorGUI.indentLevel--;

                EditorGUILayout.HelpBox("Rescalling in playmode available only in editor not in build", MessageType.Warning);
                EditorGUILayout.PropertyField(sp_CollidersScaleMul, new GUIContent("Scale Multiplier"));
                EditorGUILayout.PropertyField(sp_CollidersScale, new GUIContent("Scale Curve"));
                EditorGUILayout.PropertyField(sp_CollidersAutoCurve, new GUIContent("Auto Curve"));
                FEditor_Styles.DrawUILine(new Color(0.6f, 0.6f, 0.6f, 0.4f));
                EditorGUILayout.PropertyField(sp_AllCollidersOffset, true);

                if (spine.UseTruePosition)
                    EditorGUILayout.BeginVertical(FEditor_Styles.RedBackground);
                else
                    EditorGUILayout.BeginVertical(FEditor_Styles.YellowBackground);

                EditorGUILayout.PropertyField(sp_UseTruePosition, true);
                EditorGUILayout.EndVertical();

                EditorGUILayout.PropertyField(sp_SegmentCollision, true);
                Color preCol = GUI.color;
                if (spine.GravityPower != Vector3.zero) if (!spine.DetailedCollision) GUI.color = new Color(1f, 1f, 0.35f, 0.8f);
                GUI.color = preCol;

                EditorGUILayout.PropertyField(sp_CollidersOffsets, true);
                EditorGUILayout.PropertyField(sp_DetailedCollision, true);

                FEditor_Styles.DrawUILine(new Color(0.6f, 0.6f, 0.6f, 0.4f));
                EditorGUILayout.PropertyField(sp_GravityPower, true);

                GUILayout.Space(3f);
            }
        }
        else
        {
            FEditor_Styles.DrawUILine(new Color(0.6f, 0.6f, 0.6f, 0.4f));
            EditorGUILayout.PropertyField(sp_GravityPower, true);
            GUILayout.Space(3f);
        }

        EditorGUILayout.EndVertical();
    }

    #endregion

}
