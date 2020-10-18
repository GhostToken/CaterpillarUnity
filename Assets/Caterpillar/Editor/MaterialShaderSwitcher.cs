using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor.Presets;

public class MaterialShaderSwitcher : ScriptableWizard
{

    public List<GameObject> A_Upgrader;
    public List<Shader> ExcludedShaders;
    public Shader GlassShader;

    public Preset Preset_Decor;
    public Preset Preset_Food;
    public Preset Preset_Glass;

    public List<Material> Materiaux_Decor;
    public List<Material> Materiaux_Food;

    static int AlbedoId = Shader.PropertyToID("Albedo");

    [MenuItem("Caterpillar/Material shader switcher")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Switch shader settings on materials", typeof(MaterialShaderSwitcher), "Appliquer", "Fait le tri !");
    }

    void OnWizardCreate()
    {
        AlbedoId = Shader.PropertyToID("_AlbedoMap");
        AppliquePresets();
    }

    void OnWizardOtherButton()
    {
        CollecteLesMateriaux();
    }

    private void AppliquePresets()
    {
        if(Materiaux_Decor.Count > 0)
        {
            Undo.RecordObjects(Materiaux_Decor.ToArray(), "Materiaux Decor");
            foreach (Material material in Materiaux_Decor)
            {
                if(material.shader == GlassShader )
                {
                    ApplyPreset(material, Preset_Glass);
                }
                else
                {
                    ApplyPreset(material, Preset_Decor);
                }
            }
        }

        if (Materiaux_Food.Count > 0)
        {
            Undo.RecordObjects(Materiaux_Food.ToArray(), "Materiaux Food");
            foreach (Material material in Materiaux_Food)
            {
                if (material.shader == GlassShader)
                {
                    ApplyPreset(material, Preset_Glass);
                }
                else
                {
                    ApplyPreset(material, Preset_Food);
                }
            }
        }
    }

    private void CollecteLesMateriaux()
    {
        foreach(GameObject gameobject in A_Upgrader)
        {
            ChercheLesMateriaux(gameobject.transform);
        }
    }

    private void ChercheLesMateriaux(Transform GameObject)
    {
        if(ShouldExcludeObject(GameObject) == true)
        {
            return;
        }

        // Grere les food et tous les sous objets
        if(GameObject.GetComponent<Food>() != null )
        {
            MeshRenderer[] foodRenderer = GameObject.GetComponentsInChildren<MeshRenderer>();

            foreach(MeshRenderer renderer in foodRenderer)
            {
                AddMaterial(renderer.sharedMaterials, Materiaux_Food);
            }
            return;
        }

        // sinon c'est du decor
        MeshRenderer[] decorRenderer = GameObject.GetComponents<MeshRenderer>();
        foreach (MeshRenderer renderer in decorRenderer)
        {
            AddMaterial(renderer.sharedMaterials, Materiaux_Decor);
        }

        // on gere les childs
        for (int index = 0; index < GameObject.transform.childCount; ++index)
        {
            ChercheLesMateriaux(GameObject.transform.GetChild(index));
        }
    }

    private void ApplyPreset(Material Material, Preset Preset)
    {
        Texture texture = Material.mainTexture;
        Preset.ApplyTo(Material);
        Material.SetTexture(AlbedoId, texture);
        EditorUtility.SetDirty(Material);
    }

    private void AddMaterial(Material[] ToAdd, List<Material> ListeCible)
    {
        foreach(Material toAdd in ToAdd)
        {
            if(ShouldExclude(toAdd) == false)
            {
                if(ListeCible.Contains(toAdd) == false )
                {
                    ListeCible.Add(toAdd);
                }
            }
        }
    }

    private bool ShouldExclude(Material Material)
    {
        if(Material == null)
        {
            return true;
        }
        return ExcludedShaders.Contains(Material.shader);
    }

    private bool ShouldExcludeObject(Transform GameObject)
    {
        if (GameObject == null)
        {
            return true;
        }
        return (GameObject.GetComponent<Caterpillar>() != null);
    }
}
