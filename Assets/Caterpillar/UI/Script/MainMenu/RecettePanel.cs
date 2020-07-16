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

        CurrentWidth = BaseWidth + IngredientWidth * (Recette.IngredientsDeBase.Count + Recette.IngredientsOptionnels.Count);
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CurrentWidth);

        foreach (Ingredient ingredient in Recette.IngredientsDeBase)
        {
            AddIngredient(ingredient);
        }
        foreach (Ingredient ingredient in Recette.IngredientsOptionnels)
        {
            AddIngredient(ingredient);
        }
    }

    public void AddIngredient(Ingredient ingredient)
    {
        GameObject panelObject = GameObject.Instantiate(PrefabIngredient);
        IngredientPanel panel = panelObject.GetComponent<IngredientPanel>();
        panel.SetIngredient(ingredient);
        panel.rectTransform().SetParent(ListeIngredient, false);
        ListeIngredient.SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal, Mathf.Max(ListeIngredient.rect.width, CurrentWidth + ParentWidth));
    }

    #endregion
}
