using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct RecetteTypeSetttings
{
    public ETypeRecette Type;
    public Color Color;
}

public class RecettePanel : MonoBehaviour
{
    #region Properties

    public List<RecetteTypeSetttings> RecetteTypeSetttings;
    public int BaseWidth = 175;
    public int IngredientWidth = 250;
    public int ParentWidth = 80;

    public Image BackGround;
    public TextMeshProUGUI NomRecette;
    public RectTransform ListeIngredient;
    public GameObject PrefabIngredient;

    private int CurrentWidth;

    #endregion

    #region Public Methods

    public void SetRecette(Recette Recette)
    {
        BackGround.color = RecetteTypeSetttings.Find(T => T.Type == Recette.TypeDeRecette).Color;
        NomRecette.text = Recette.Nom;

        CurrentWidth = BaseWidth + IngredientWidth * (Recette.Ingredients.Count);
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CurrentWidth);

        foreach (Composants composant in Recette.Ingredients)
        {
            if (composant.Flags.HasFlag(EIngredientFlags.Necessaire) == true)
            {
                if (composant.Flags.HasFlag(EIngredientFlags.Caché) == false)
                {
                    AddIngredient(composant.Ingredient);
                }
            }
        }

        foreach (Composants composant in Recette.Ingredients)
        {
            if (composant.Flags.HasFlag(EIngredientFlags.Necessaire) == false)
            {
                if (composant.Flags.HasFlag(EIngredientFlags.Caché) == false)
                {
                    AddIngredient(composant.Ingredient);
                }
            }
        }

        // Later : ajouter ingredients cachés
        //foreach (Composants composant in Recette.Ingredients)
        //{
        //    if (composant.Flags.HasFlag(EIngredientFlags.Caché) == true)
        //    {
        //        AddIngredient(composant.Ingredient);
        //    }
        //}
    }

    public void AddIngredient(Ingredient ingredient, bool caché = false)
    {
        GameObject panelObject = GameObject.Instantiate(PrefabIngredient);
        IngredientPanel panel = panelObject.GetComponent<IngredientPanel>();
        panel.SetIngredient(ingredient);
        panel.rectTransform().SetParent(ListeIngredient, false);
        ListeIngredient.SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal, Mathf.Max(ListeIngredient.rect.width, CurrentWidth + ParentWidth));
    }

    #endregion
}
