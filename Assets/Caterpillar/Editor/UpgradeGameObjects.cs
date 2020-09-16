using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class UpgradeGameObjects : ScriptableWizard
{
    public List<GameObject> ModelesEnVrac;
    public List<GameObject> Modeles;
    public List<GameObject> A_Upgrader;

    [MenuItem("Caterpillar/Upgrade GameObjects")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Upgrade GameObjects", typeof(UpgradeGameObjects), "Upgrade !", "Prepare tout !" );
    }

    void OnWizardCreate()
    {
        if(Modeles.Count != A_Upgrader.Count)
        {
            Debug.LogError("les liste de gameobjects ont des longueurs differentes" );
            return;
        }

        for (int index = 0; index < A_Upgrader.Count; ++index)
        {
            if( UpgradeGameObject(Modeles[index], A_Upgrader[index]) == true )
            {
                PrefabUtility.ApplyPrefabInstance(A_Upgrader[index], InteractionMode.UserAction);
            }
        }
    }

    void OnWizardOtherButton()
    {
        for (int index = 0; index < A_Upgrader.Count; ++index)
        {
            if(TestObjetValide(A_Upgrader[index]) == true )
            {
                A_Upgrader.RemoveAt(index);
                index--;
                Debug.Log("Retire " + A_Upgrader[index].name + " : objet conforme !");
                continue;
            }
        }

        EnleveLesDuplicats();

        TrouveLesModeles();
    }

    int CompareTo(GameObject A, GameObject B)
    {
        return A.name.CompareTo(B.name);
    }

    bool TestObjetValide(GameObject ToCheck)
    {
        MeshFilter[] meshFilters_ToUpgrade = ToCheck.GetComponents<MeshFilter>();

        for (int index = 0; index < meshFilters_ToUpgrade.Length; ++index)
        {
            if ( meshFilters_ToUpgrade[index].sharedMesh == null )
            {
                Debug.LogWarning(ToCheck.name + " a un probleme de mesh");
                return false;
            }
        }

        MeshRenderer[] meshRenderer_ToUpgrade = ToCheck.GetComponents<MeshRenderer>();

        for (int index = 0; index < meshRenderer_ToUpgrade.Length; ++index)
        {
            if (meshRenderer_ToUpgrade[index].sharedMaterials.Contains(null))
            {
                Debug.LogWarning(ToCheck.name + " a un probleme de material");
                return false;
            }
        }

        for (int index = 0; index < ToCheck.transform.childCount; ++index)
        {
            if ( TestObjetValide(ToCheck.transform.GetChild(index).gameObject) == false )
            {
                Debug.LogWarning(ToCheck.name + " a un probleme de sous objet sur  " + ToCheck.transform.GetChild(index).gameObject.name);
                return false;
            }
        }

        return true;
    }

    void EnleveLesDuplicats()
    {
        A_Upgrader.Sort(CompareTo);

        for (int index = 1; index < A_Upgrader.Count; ++index)
        {
            if(A_Upgrader[index].name.Contains(A_Upgrader[index-1].name) )
            {
                A_Upgrader.RemoveAt(index);
                index--;
                Debug.Log("Retire " + A_Upgrader[index].name + " duplicat de " + A_Upgrader[index -1].name);
                continue;
            }
        }
    }

    void TrouveLesModeles()
    {
        for (int index = 0; index < A_Upgrader.Count; ++index)
        {
            Modeles.Add(TrouveLeModele(A_Upgrader[index]));
        }

        ModelesEnVrac.Clear();
    }

    GameObject TrouveLeModele(GameObject A_Upgrader)
    {
        int meshAUpgraderCount = A_Upgrader.GetComponentsInChildren<MeshFilter>().Length;
        for (int index = 0; index < ModelesEnVrac.Count; ++index)
        {
            bool NameMatch = (ModelesEnVrac[index].name == A_Upgrader.name);
            int meshCount = ModelesEnVrac[index].GetComponentsInChildren<MeshFilter>().Length;
            bool ChildCountMatch = (meshCount == meshAUpgraderCount);

            if (NameMatch && ChildCountMatch)
            {
                return ModelesEnVrac[index];
            }
        }

        return null;
    }

    bool UpgradeGameObject(GameObject Modele, GameObject ToUpgrade)
    {
        MeshFilter[] meshFilters_Modele = Modele.GetComponents<MeshFilter>();
        MeshFilter[] meshFilters_ToUpgrade = ToUpgrade.GetComponents<MeshFilter>();

        MeshRenderer[] meshRenderer_Modele = Modele.GetComponents<MeshRenderer>();
        MeshRenderer[] meshRenderer_ToUpgrade = ToUpgrade.GetComponents<MeshRenderer>();

        if ((meshFilters_Modele.Length != meshFilters_ToUpgrade.Length)
            || (meshRenderer_Modele.Length != meshRenderer_ToUpgrade.Length))
        {
            Debug.LogError("la composition du GameObject " + Modele.name + " differe de celle de " + ToUpgrade.name);
            return false;
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
            return false;
        }

        for (int index = 0; index < ToUpgrade.transform.childCount; ++index)
        {
            if( UpgradeGameObject(Modele.transform.GetChild(index).gameObject, ToUpgrade.transform.GetChild(index).gameObject) == false )
            {
                return false;
            }
        }

        return true;
    }
}
