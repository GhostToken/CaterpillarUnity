using FIMSpace.FSpine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public Ingredient Ingredient;
    public float DownScaleSpeed = 1.5f;
    bool DejaMange = false;

    public void Mange()
    {
        if (DejaMange == false)
        {
            DejaMange = true;
            StartCoroutine(Mangeage());
        }
    }

    private IEnumerator Mangeage()
    {
        while (transform.localScale.sqrMagnitude > 0.25f)
        {
            transform.localScale -= Vector3.one * DownScaleSpeed * Time.deltaTime;
            yield return null;
        }
        
        Partie.Mange(Ingredient);
        GameObject.Destroy(gameObject);
    }
}
