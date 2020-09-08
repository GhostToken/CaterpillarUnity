using UnityEngine;
using UnityEditor;
using System.Collections;
// CopyComponents - by Michael L. Croswell for Colorado Game Coders, LLC
// March 2010
 
//Modified by Kristian Helle Jespersen
//June 2011
 
public class ReplaceGameObjects : ScriptableWizard
{
    public bool copyValues = true;
    public GameObject NouveauPrefab;
    public Transform NouveauParent;

    public GameObject[] AncienObjets;
 
    [MenuItem("Caterpillar/Replace GameObjects")]
 
 
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Replace GameObjects", typeof(ReplaceGameObjects), "Replace");
    }
 
    void OnWizardCreate()
    {             
        //Get path to nearest (in case of nested) prefab from this gameObject in the scene
        string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(NouveauPrefab);

        //Get prefab object from path
        Object prefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(Object));

        foreach (GameObject go in AncienObjets)
        {
            GameObject newObject;
            newObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab, (NouveauParent != null ? NouveauParent : go.transform.parent));
            newObject.transform.position = go.transform.position;
            newObject.transform.rotation = go.transform.rotation;

            GameObject.DestroyImmediate(go);
        }
    }
}