using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Recette : MonoBehaviour
{
    #region Properties

    public int BaseWidth = 150;
    public int IngredientWidth = 100;
    public int ParentWidth = 80;

    public TextMeshProUGUI Count;

    public GameObject PrefabIngredient;

    private int CurrentWidth;
    private Recette ThisRecette;

    #endregion

    #region Unity Methods

    private void Update()
    {
        Count.text = Partie.CompteRecetteCompletees(ThisRecette).ToString();
    }

    #endregion

    #region Public Methods

    public int SetRecette(Recette Recette)
    {
        ThisRecette = Recette;
        Count.text = "";
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
        return CurrentWidth;
    }

    public void AddIngredient(Ingredient ingredient)
    {
        GameObject ingredientObject = GameObject.Instantiate(PrefabIngredient);
        HUD_Ingredient hud = ingredientObject.GetComponent<HUD_Ingredient>();
        hud.SetIngredient(ingredient);
        RectTransform liste = GetComponent<RectTransform>();
        hud.rectTransform().SetParent(liste, false);
        liste.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Max(liste.rect.width, CurrentWidth + ParentWidth));
    }

    #endregion
}
