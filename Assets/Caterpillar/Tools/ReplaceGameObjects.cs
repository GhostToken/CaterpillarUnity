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
    public GameObject NewType;
    public Transform NouveauParent;

    public GameObject[] OldObjects;
 
    [MenuItem("Caterpillar/Replace GameObjects")]
 
 
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Replace GameObjects", typeof(ReplaceGameObjects), "Replace");
    }
 
    void OnWizardCreate()
    { 
        foreach (GameObject go in OldObjects)
        {
            GameObject newObject;
            newObject = (GameObject)PrefabUtility.InstantiatePrefab(NewType, (NouveauParent != null ? NouveauParent : go.transform.parent));
            newObject.transform.position = go.transform.position;
            newObject.transform.rotation = go.transform.rotation;
 
            DestroyImmediate(go);
 
        }
 
    }
}