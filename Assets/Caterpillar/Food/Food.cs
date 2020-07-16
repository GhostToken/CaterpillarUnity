using FIMSpace.FSpine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public Ingredient Ingredient;
    public float DownScaleSpeed = 1.5f;
    bool Mange = false;

    private void OnCollisionStay(Collision collision)
    {
        if( collision.gameObject.GetComponent<Caterpillar>() )
        {
            if (!Mange)
            {
                Mange = true;
                StartCoroutine(Mangeage());
            }
        }
    }

    private IEnumerator Mangeage()
    {
        while (transform.localScale.sqrMagnitude > 0.25f)
        {
            transform.localScale -= Vector3.one * DownScaleSpeed * Time.deltaTime;
            yield return null;
        }

        Mange = true;
        Partie.Mange(Ingredient);
        GameObject.Destroy(gameObject);
    }
}
