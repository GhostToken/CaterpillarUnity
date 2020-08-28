using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuOverlay : MonoBehaviour
{
    public Color SelectedTextColor;
    public float FadeDuration = 0.1f;
    public Sprite Selection_Maison;
    public Sprite Selection_Inventaire;
    public Sprite Selection_CarnetDeNotes;
    public Sprite Selection_Succes;
    public Sprite Selection_Boutique;
    public Sprite Selection_Parametres;

    public enum Menu
    {
        Maison,
        Inventaire,
        CarnetDeNotes,
        Succes,
        Boutique,
        Parametres
    }

    public void OnMaison(GameObject Button)
    {
        StartCoroutine(StartButtonEffect(Button, Menu.Maison));
    }

    public void OnInventaire(GameObject Button)
    {
        StartCoroutine(StartButtonEffect(Button, Menu.Inventaire));
    }

    public void OnCarnetDeNotes(GameObject Button)
    {
        StartCoroutine(StartButtonEffect(Button, Menu.CarnetDeNotes));
    }

    public void OnSucces(GameObject Button)
    {
        StartCoroutine(StartButtonEffect(Button, Menu.Succes));
    }

    public void OnBoutique(GameObject Button)
    {
        StartCoroutine(StartButtonEffect(Button, Menu.Boutique));
    }

    public void OnParametres(GameObject Button)
    {
        StartCoroutine(StartButtonEffect(Button, Menu.Parametres));
    }

    private IEnumerator StartButtonEffect(GameObject Button, Menu Menu)
    {
        Image image = Button.GetComponentInChildren<Image>();
        switch (Menu)
        {
            case Menu.Maison:
                {
                    image.overrideSprite = Selection_Maison;
                    break;
                }
            case Menu.Inventaire:
                {
                    image.overrideSprite = Selection_Inventaire;
                    break;
                }
            case Menu.CarnetDeNotes:
                {
                    image.overrideSprite = Selection_CarnetDeNotes;
                    break;
                }
            case Menu.Succes:
                {
                    image.overrideSprite = Selection_Succes;
                    break;
                }
            case Menu.Boutique:
                {
                    image.overrideSprite = Selection_Boutique;
                    break;
                }
            case Menu.Parametres:
                {
                    image.overrideSprite = Selection_Parametres;
                    break;
                }
        }

        TextMeshProUGUI TextMesh = Button.GetComponentInChildren<TextMeshProUGUI>();
        TextMesh.CrossFadeColor(SelectedTextColor, FadeDuration, true, true);

        yield return new WaitForSeconds(FadeDuration);

        switch (Menu)
        {
            case Menu.Maison:
                {
                    Debug.Log("Ouvre Maison");
                    break;
                }
            case Menu.Inventaire:
                {
                    Debug.Log("Ouvre Inventaire");
                    break;
                }
            case Menu.CarnetDeNotes:
                {
                    Debug.Log("Ouvre CarnetDeNotes");
                    break;
                }
            case Menu.Succes:
                {
                    Debug.Log("Ouvre Succes");
                    break;
                }
            case Menu.Boutique:
                {
                    Debug.Log("Ouvre Boutique");
                    break;
                }
            case Menu.Parametres:
                {
                    Debug.Log("Ouvre Parametres");
                    break;
                }
        }

        image.overrideSprite = null;
        TextMesh.color = Color.white;

        gameObject.SetActive(false);
    }
}
