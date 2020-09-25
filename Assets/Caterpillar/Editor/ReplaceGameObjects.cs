using UnityEngine;
using UnityEditor;
 
public class ReplaceGameObjects : ScriptableWizard
{
    public bool copyValues = true;
    public GameObject NouveauPrefab;
    public Transform NouveauParent;

    public bool EffaceAnciensObjets = true;

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

            if( EffaceAnciensObjets )
            {
                GameObject.DestroyImmediate(go);
            }
        }
    }
}