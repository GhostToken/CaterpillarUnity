using DG.Tweening;
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
    public float AnimationDuration = 0.25f;
    private Ingredient ThisIngredient;
    private bool DoneLaunched = false;

    #endregion

    #region Unity Methods

    private void Update()
    {
        if(Done.enabled)
        {
            if( EstTrouvé() == false)
            {
                DoneLaunched = false;
                Done.enabled = false;
            }
        }
        else if (DoneLaunched == false)
        {
            if (EstTrouvé() == true)
            {
                Done.enabled = true;
                Done.transform.localScale = Vector3.zero;
                DoneLaunched = true;
                Sequence Sequence = DOTween.Sequence();
                Sequence.Append(Done.transform.DOScale(1.0f, AnimationDuration));
                Sequence.Append(Done.transform.DOPunchScale(Vector3.one, AnimationDuration / 2.0f));
                Sequence.onComplete = () =>
                {
                    DoneLaunched = false;
                };

                Icon.transform.DOPunchScale(Vector3.one * 0.5f, AnimationDuration * 2.0f);
            }
        }
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

    private bool EstTrouvé()
    {
        return Partie.Estomac.Contains(ThisIngredient);
    }
}
