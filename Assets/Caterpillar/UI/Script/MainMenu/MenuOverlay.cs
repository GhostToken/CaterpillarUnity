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

    public GameObject Menu_Maison;
    public GameObject Menu_Inventaire;
    public GameObject Menu_CarnetDeNotes;
    public GameObject Menu_Succes;
    public GameObject Menu_Boutique;
    public GameObject Menu_Parametres;

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
                    if(Menu_Maison != null)
                    {
                        Menu_Maison.SetActive(true);
                    }
                    else
                    {
                        Debug.Log("Ouvre Maison");
                    }
                    break;
                }
            case Menu.Inventaire:
                {
                    if (Menu_Inventaire != null)
                    {
                        Menu_Inventaire.SetActive(true);
                    }
                    else
                    {
                        Debug.Log("Ouvre Inventaire");
                    }
                    break;
                }
            case Menu.CarnetDeNotes:
                {
                    if (Menu_CarnetDeNotes != null)
                    {
                        Menu_CarnetDeNotes.SetActive(true);
                    }
                    else
                    {
                        Debug.Log("Ouvre CarnetDeNotes");
                    }
                    break;
                }
            case Menu.Succes:
                {
                    if (Menu_Succes != null)
                    {
                        Menu_Succes.SetActive(true);
                    }
                    else
                    {
                        Debug.Log("Ouvre Succes");
                    }
                    break;
                }
            case Menu.Boutique:
                {
                    if (Menu_Boutique != null)
                    {
                        Menu_Boutique.SetActive(true);
                    }
                    else
                    {
                        Debug.Log("Ouvre Boutique");
                    }
                    break;
                }
            case Menu.Parametres:
                {
                    if (Menu_Parametres != null)
                    {
                        Menu_Parametres.SetActive(true);
                    }
                    else
                    {
                        Debug.Log("Ouvre Parametres");
                    }
                    break;
                }
        }

        image.overrideSprite = null;
        TextMesh.color = Color.white;

        gameObject.SetActive(false);
    }
}
