using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Data Types


public enum ETypeRecette
{
    Entree,
    Plat,
    Dessert
}
#endregion

[CreateAssetMenu(fileName = "NewRecette", menuName = "Caterpillar/Recette")]
public class Recette : ScriptableObject
{
    #region Properties

    public string Nom;
    public ETypeRecette TypeDeRecette;
    public string Description;
    public Texture2D Polaroid;

    public List<Ingredient> IngredientsDeBase;
    public List<Ingredient> IngredientsOptionnels;
    public List<Ingredient> IngredientsCachés;

    #endregion

    #region Static Accessor

    private static List<Recette> AllRecettes;

    public static List<Recette> GetAllRecettes()
    {
        if (AllRecettes == null)
        {
            AllRecettes = new List<Recette>(Resources.LoadAll<Recette>("Recettes"));
        }

        return AllRecettes;
    }

    public static Recette GetRecette(string Recette)
    {
        return GetAllRecettes().Find(T => T.Nom == Recette);
    }

    #endregion
}
