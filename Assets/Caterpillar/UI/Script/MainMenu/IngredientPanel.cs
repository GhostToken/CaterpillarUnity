using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientPanel : MonoBehaviour
{
    #region Properties

    public Image Icon;
    public TextMeshProUGUI NomIngredient;

    #endregion

    #region Public Methods

    public void SetIngredient( Ingredient Ingredient)
    {
        NomIngredient.text = Ingredient.Nom;
        Icon.sprite = Ingredient.Icon;
    }

    #endregion
}
