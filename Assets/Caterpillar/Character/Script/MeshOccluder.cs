using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeshOccluder : MonoBehaviour
{
    #region Const

    const float C_DUREE_MESH_INVISIBLE = 0.35f;
    const float C_MIN_OPACITY = 0.15f;
    const float C_FADE_DURATION = 0.35f;
    const string C_GLASS_SHADER_KEYWORD = "Glass";
    const string C_ALPHA_SHADER_PROPERTY = "Alpha";

    #endregion

    #region Data Structure

    private class MeshQuiGene
    {
        public Renderer Renderer;
        public float FadeTimer;
        public float GeneEncore;

        bool UseGeneratedMaterial = false;
        private Material OriginalMaterial;
        private float OriginalAlpha = 1.0f;
        private Material TransparentMaterial;

        public MeshQuiGene(Renderer RendererGenant, Material TransparentMaterialTemplate)
        {
            Renderer = RendererGenant;
            OriginalMaterial = Renderer.sharedMaterial;
            if (OriginalMaterial.shader.name.Contains(C_GLASS_SHADER_KEYWORD) == false)
            {
                TransparentMaterial = new Material(TransparentMaterialTemplate);
                TransparentMaterial.mainTexture = OriginalMaterial.mainTexture;
                Renderer.material = TransparentMaterial;
                UseGeneratedMaterial = true;
            }
            else
            {
                TransparentMaterial = OriginalMaterial;
                OriginalAlpha = OriginalMaterial.GetFloat(C_ALPHA_SHADER_PROPERTY);
            }
            GeneEncore = C_DUREE_MESH_INVISIBLE;
            FadeTimer = 0.0f;
        }

        public void Update()
        {
            if ((GeneEncore > 0.0f) && (FadeTimer < C_FADE_DURATION))
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
            float alpha = Mathf.Lerp(OriginalAlpha, C_MIN_OPACITY, FadeTimer / C_FADE_DURATION);
            TransparentMaterial.SetFloat(C_ALPHA_SHADER_PROPERTY, alpha);
        }

        public void Terminate()
        {
            if(UseGeneratedMaterial == true)
            {
                Renderer.sharedMaterial = OriginalMaterial;
                GameObject.Destroy(TransparentMaterial);
                TransparentMaterial = null;
            }
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
    public LayerMask LayerMask;
    private Coroutine OccludingCoroutine = null;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        if( Options.MeshOccluding == true)
        {
            OccludingCoroutine = StartCoroutine(UpdateOccluding());
        }
    }

    void Update()
    {
        if (Options.MeshOccluding == true)
        {
            if (OccludingCoroutine == null)
            {
                OccludingCoroutine = StartCoroutine(UpdateOccluding());
            }
        }
        else
        {
            if (OccludingCoroutine != null)
            {
                StopCoroutine(OccludingCoroutine);
                OccludingCoroutine = null;
            }
        }

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

    private Renderer GetObjectToOcclude(RaycastHit Hit)
    {
        GameObject HitObject = Hit.transform.gameObject;
        Renderer rendererGenant = null;

        rendererGenant = HitObject.GetComponent<Renderer>();
        if (rendererGenant == null )
        {
            rendererGenant = HitObject.GetComponentInParent<Renderer>();
        }
        if (rendererGenant == null)
        {
            rendererGenant = HitObject.GetComponentInChildren<Renderer>();
        }
        if(rendererGenant == null)
        {
            return null;
        }
        if (rendererGenant.sharedMaterial == null)
        {
            return null;
        }

        return rendererGenant;
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
            RaycastHit[] hits = Physics.SphereCastAll(ray, 0.25f, 100.0f, LayerMask);

            Debug.DrawLine(Start, Start + Direction * 100.0f, Color.green, 0.15f, false);

            if (hits.Length > 0)
            {
                foreach(RaycastHit hit in hits)
                {
                    Renderer RendererGeneur = GetObjectToOcclude(hit);

                    if (RendererGeneur == null)
                    {
                        continue;
                    }

                    MeshQuiGene Geneur = TouslesMeshGenants.Find(T => T.Renderer == RendererGeneur);
                    Debug.DrawLine(Start, hit.point, Color.red, 0.15f, false);

                    if (Geneur == null)
                    {
                        TouslesMeshGenants.Add(new MeshQuiGene(RendererGeneur, TransparentMaterialTemplate));
                    }
                    else
                    {
                        Geneur.GeneEncore = C_DUREE_MESH_INVISIBLE;
                    }
                }
            }
        }
    }
}
