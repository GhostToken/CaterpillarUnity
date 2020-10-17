using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientPanel : MonoBehaviour
{
    #region Properties

    public Sprite IngredientMystere;
    public Sprite IngredientOptionnel;
    public Image Icon;
    public TextMeshProUGUI NomIngredient;

    #endregion

    #region Public Methods

    public void SetIngredient( Ingredient Ingredient, bool caché = false)
    {
        NomIngredient.text = (caché ? "?" : Ingredient.Nom);
        Icon.sprite = (caché ? IngredientMystere : Ingredient.Icon);
    }

    #endregion
}
