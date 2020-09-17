using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeshOccluder : MonoBehaviour
{
    #region Const

    const float DUREE_MESH_INVISIBLE = 0.35f;
    const float MIN_OPACITY = 0.05f;
    const float FADE_DURATION = 0.35f;

    #endregion

    #region Data Structure

    private class MeshQuiGene
    {
        public GameObject Reference;
        public Renderer Renderer;
        public float FadeTimer;
        public float GeneEncore;

        private Material OriginalMaterial;
        private Material TransparentMaterial;

        public MeshQuiGene(GameObject ReferenceGeneur, Material TransparentMaterialTemplate)
        {
            Reference = ReferenceGeneur;
            Renderer = ReferenceGeneur.GetComponentInChildren<Renderer>();
            OriginalMaterial = Renderer.sharedMaterial;
            TransparentMaterial = new Material(TransparentMaterialTemplate);
            TransparentMaterial.mainTexture = OriginalMaterial.mainTexture;
            Renderer.material = TransparentMaterial;
            GeneEncore = DUREE_MESH_INVISIBLE;
            FadeTimer = 0.0f;
        }

        public void Update()
        {
            if ((GeneEncore > 0.0f) && (FadeTimer < FADE_DURATION))
            {
                FadeTimer += Time.deltaTime;
                UpdateAlpha();
            }
            else if (FadeTimer > 0.0f)
            {
                FadeTimer -= Time.deltaTime;
                if (FadeTimer > 0.0f)
                {
                    UpdateAlpha();
                }
            }
            GeneEncore -= Time.deltaTime;
        }

        private void UpdateAlpha()
        {
            float alpha = Mathf.Lerp(1.0f, MIN_OPACITY, FadeTimer / FADE_DURATION);
            TransparentMaterial.SetFloat("Alpha", alpha);
        }

        public void Terminate()
        {
            Renderer.sharedMaterial = OriginalMaterial;
            GameObject.Destroy(TransparentMaterial);
            TransparentMaterial = null;
        }

        public bool CanBeTerminated
        {
            get
            {
                return (GeneEncore <= 0.0f && (FadeTimer <= 0.0f));
            }
        }
    }

    #endregion

    #region Properties

    public Material TransparentMaterialTemplate;
    static private List<MeshQuiGene> TouslesMeshGenants = new List<MeshQuiGene>();
    static float LastUpdateTime = 0.0f;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateOccluding());
    }

    void Update()
    {
        if( Time.time > LastUpdateTime)
        {
            LastUpdateTime = Time.time;

            foreach (MeshQuiGene Geneur in TouslesMeshGenants)
            {
                Geneur.Update();

                if(Geneur.CanBeTerminated)
                {
                    Geneur.Terminate();
                }
            }

            TouslesMeshGenants.RemoveAll(T => T.CanBeTerminated);
        }
    }

    private bool ShouldOcclude(Renderer Renderer)
    {
        if(Renderer == null)
        {
            return false;
        }
        if (Renderer.sharedMaterial == null)
        {
            return false;
        }
        return Renderer.sharedMaterial.shader.name.Contains("Glass") == false;
    }

    private GameObject GetObjectToOcclude(RaycastHit Hit)
    {
        GameObject HitObject = Hit.transform.gameObject;
        if ( PrefabUtility.IsPartOfNonAssetPrefabInstance(HitObject))
        {
            return PrefabUtility.GetOutermostPrefabInstanceRoot(HitObject) as GameObject;
        }

        if( HitObject.GetComponent<Renderer>() == true )
        {
            return HitObject;
        }

        if(HitObject.GetComponentInParent<Renderer>() == true )
        {
            return HitObject.transform.parent.gameObject;
        }

        return null;
    }

    private bool ShouldOcclude(GameObject Geneur)
    {
        return (Geneur.GetComponentInChildren<Food>() == null);
    }

    // Update is called once per frame
    private IEnumerator UpdateOccluding()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.15f);

            Vector3 Direction = -Camera.main.transform.forward;
            Vector3 Start = transform.position + Direction;
            Ray ray = new Ray(Start, Direction);
            RaycastHit[] hits = Physics.SphereCastAll(ray, 0.25f, 100.0f, (LayerMask.NameToLayer("Default") | LayerMask.NameToLayer("Ingredient")));

            Debug.DrawLine(Start, Start + Direction * 100.0f, Color.green, 0.15f, false);

            if (hits.Length > 0)
            {
                foreach(RaycastHit hit in hits)
                {
                    GameObject ObjectGeneur = GetObjectToOcclude(hit);

                    if (ObjectGeneur == null)
                    {
                        continue;
                    }
                    if (ShouldOcclude(ObjectGeneur) == false)
                    {
                        continue;
                    }

                    Debug.Log("Occlude " + ObjectGeneur.name);

                    MeshQuiGene Geneur = TouslesMeshGenants.Find(T => T.Reference == ObjectGeneur);

                    Debug.DrawLine(Start, hit.point, Color.red, 0.15f, false);

                    if (Geneur == null)
                    {
                        TouslesMeshGenants.Add(new MeshQuiGene(ObjectGeneur, TransparentMaterialTemplate));
                    }
                    else
                    {
                        Geneur.GeneEncore = DUREE_MESH_INVISIBLE;
                    }
                }
            }
        }
    }
}
