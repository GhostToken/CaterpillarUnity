using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeteDeCaterpillar : MonoBehaviour
{
    public List<GameObject> TouteLesTetes = new List<GameObject>();

    public Transform TeteActuelle;

    [Range(0.15f, 15.0f)]
    public float DelaisEntreTete = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ChangeTete());
    }

    // Update is called once per frame
    IEnumerator ChangeTete()
    {
        while(true)
        {
            float attente = UnityEngine.Random.Range(0.15f, DelaisEntreTete);
            Debug.Log("Nouvelle attente : " + attente);
            yield return new WaitForSecondsRealtime(attente);
            Debug.Log("attente finie ! ");

            GameObject nouvelleTete = TouteLesTetes[UnityEngine.Random.Range(0, TouteLesTetes.Count)];
            AppliqueTete(nouvelleTete);
        }
    }

    void AppliqueTete(GameObject _NouvelleTete)
    {
        Vector3 scale = TeteActuelle.localScale;
        GameObject tete = GameObject.Instantiate<GameObject>(_NouvelleTete, TeteActuelle.position, TeteActuelle.rotation, TeteActuelle.parent);
        GameObject.Destroy(TeteActuelle.gameObject);
        TeteActuelle = tete.transform;
        TeteActuelle.localScale = scale;
    }
}
