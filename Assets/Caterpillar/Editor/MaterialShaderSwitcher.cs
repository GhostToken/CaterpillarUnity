using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;

[Serializable]
public struct ShaderUpgrade
{
    public Shader ShaderARemplacer;
    public Material NoveauxReglagesMaterial;
}

public class MaterialShaderSwitcher : ScriptableWizard
{
    public List<ShaderUpgrade> Remplacements;
    public List<Material> A_Upgrader;

    [MenuItem("Caterpillar/Material shader switcher")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Switch shader on materials", typeof(MaterialShaderSwitcher), "Fermer", "Fait le Job !");
    }

    void OnWizardCreate()
    {
    }

    void OnWizardOtherButton()
    {
        for (int index = 0; index < A_Upgrader.Count; ++index)
        {
            if (UpgradeMaterial(A_Upgrader[index]) == true)
            {
                EditorUtility.SetDirty(A_Upgrader[index]);
            }
        }
    }

    bool UpgradeMaterial(Material Material)
    {
        for (int index = 0; index < Remplacements.Count; ++index)
        {
            if (Material.shader == Remplacements[index].ShaderARemplacer)
            {
                Material.shader = Remplacements[index].NoveauxReglagesMaterial.shader;
                Material.CopyPropertiesFromMaterial(Remplacements[index].NoveauxReglagesMaterial);
                return true;
            }
        }
        return false;
    }
}
