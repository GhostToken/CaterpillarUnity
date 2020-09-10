using System.Collections;
using UnityEngine;
using UnityEditor;

public class UpgradeGameObjects : ScriptableWizard
{
    public GameObject[] Modeles;
    public GameObject[] A_Upgrader;

    [MenuItem("Caterpillar/Upgrade GameObjects")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Upgrade GameObjects", typeof(UpgradeGameObjects), "Upgrade");
    }

    void OnWizardCreate()
    {
        if(Modeles.Length != A_Upgrader.Length)
        {
            Debug.LogError("les liste de gameobjects ont des longueurs differentes" );
            return;
        }

        for (int index = 0; index < A_Upgrader.Length; ++index)
        {
            UpgradeGameObject(Modeles[index], A_Upgrader[index]);
        }
    }

    void UpgradeGameObject(GameObject Modele, GameObject ToUpgrade)
    {
        MeshFilter[] meshFilters_Modele = Modele.GetComponents<MeshFilter>();
        MeshFilter[] meshFilters_ToUpgrade = ToUpgrade.GetComponents<MeshFilter>();

        MeshRenderer[] meshRenderer_Modele = Modele.GetComponents<MeshRenderer>();
        MeshRenderer[] meshRenderer_ToUpgrade = ToUpgrade.GetComponents<MeshRenderer>();

        if ((meshFilters_Modele.Length != meshFilters_ToUpgrade.Length)
            || (meshRenderer_Modele.Length != meshRenderer_ToUpgrade.Length))
        {
            Debug.LogError("la composition du GameObject " + Modele.name + " differe de celle de " + ToUpgrade.name);
            return;
        }

        for ( int index = 0; index < meshFilters_ToUpgrade.Length; ++index )
        {
            meshFilters_ToUpgrade[index].sharedMesh = meshFilters_Modele[index].sharedMesh;
        }

        for (int index = 0; index < meshRenderer_ToUpgrade.Length; ++index)
        {
            meshRenderer_ToUpgrade[index].sharedMaterials = meshRenderer_Modele[index].sharedMaterials;
        }

        if(Modele.transform.childCount != ToUpgrade.transform.childCount)
        {
            Debug.LogError("la hierarchie des enfants du GameObject " + Modele.name + " differe de celle de " + ToUpgrade.name);
            return;
        }

        for (int index = 0; index < ToUpgrade.transform.childCount; ++index)
        {
            UpgradeGameObject(Modele.transform.GetChild(index).gameObject, ToUpgrade.transform.GetChild(index).gameObject);
        }
    }
}
