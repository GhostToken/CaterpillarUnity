using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ETypeIngredient
{
    Legume,
    Fruit,
    Viande,
    Poisson,
    Cereales,
    Boisson,
    Cremerie,
    Epicerie,
    Autres
}

[CreateAssetMenu(fileName = "NewIngredient", menuName = "Caterpillar/Ingredient")]
public class Ingredient : ScriptableObject
{
    #region Properties

    public string Nom;
    public ETypeIngredient Type;
    public Sprite Icon;
    
    #endregion

    #region Static Accessor

    private static List<Ingredient> AllIngredients;

    public static List<Ingredient> GetAllIngredients()
    {
        if (AllIngredients == null)
        {
            AllIngredients = new List<Ingredient>(Resources.LoadAll<Ingredient>("Ingredients"));
        }

        return AllIngredients;
    }

    public static Ingredient GetIngredient(string Ingredient)
    {
        return GetAllIngredients().Find(T => T.Nom == Ingredient);
    }

    #endregion
}
