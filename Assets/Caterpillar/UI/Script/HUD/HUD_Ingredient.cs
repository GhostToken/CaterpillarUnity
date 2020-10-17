using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Ingredient : MonoBehaviour
{
    #region Properties

    public Sprite IngredientMystere;
    public Sprite IngredientOptionnel;
    public Image Icon;
    public Image Done;
    private Ingredient ThisIngredient;

    #endregion

    #region Unity Methods

    private void Update()
    {
        Done.enabled = Partie.Estomac.Contains(ThisIngredient);
    }

    #endregion

    #region Public Methods

    public void SetIngredient(Ingredient Ingredient, bool caché = false)
    {
        ThisIngredient = Ingredient;
        Icon.sprite = ( caché ? IngredientMystere : Ingredient.Icon);
        Done.enabled = false;
    }

    #endregion
}
