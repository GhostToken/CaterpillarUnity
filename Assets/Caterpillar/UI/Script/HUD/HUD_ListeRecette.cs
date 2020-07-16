using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD_ListeRecette : MonoBehaviour
{
    #region Properties

    public int BaseWidth = 150;
    public int IngredientWidth = 100;
    public int ParentWidth = 80;

    public GameObject PrefabRecette;

    private int CurrentWidth;

    #endregion

    #region Public Methods

    public void SetLevel(Level Level)
    {
        CurrentWidth = BaseWidth;
        foreach (Recette recette in Level.RecetteAFaire)
        {
            CurrentWidth += AddRecette(recette);
        }

        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CurrentWidth);
    }

    public int AddRecette(Recette Recette)
    {
        GameObject hudObject = GameObject.Instantiate(PrefabRecette);
        HUD_Recette hud = hudObject.GetComponent<HUD_Recette>();
        hud.rectTransform().SetParent(GetComponent<RectTransform>(), false);
        return hud.SetRecette(Recette);
    }

    #endregion
}
