using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Ingredient : MonoBehaviour
{
    #region Properties

    public Image Icon;
    public Image Done;
    private Ingredient ThisIngredient;

    #endregion

    #region Unity Methods

    private void Update()
    {
        Done.enabled = Partie.Repas.Contains(ThisIngredient);
    }

    #endregion

    #region Public Methods

    public void SetIngredient(Ingredient Ingredient)
    {
        ThisIngredient = Ingredient;
        Icon.sprite = Ingredient.Icon;
        Done.enabled = false;
    }

    #endregion
}
