using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Data Types

public enum ETypeRecette
{
    Entree,
    Plat,
    Dessert,
    Breuvage
}

[System.Flags]
public enum EIngredientFlags
{
    Necessaire = 1,
    Caché = 2
}

[Serializable]
public struct Composants
{
    [EnumAsFlags]
    public EIngredientFlags Flags;

    public Ingredient Ingredient;
}

#endregion

[CreateAssetMenu(fileName = "NewRecette", menuName = "Caterpillar/Recette")]
public class Recette : ScriptableObject
{
    #region Properties

    public string Nom;
    public ETypeRecette TypeDeRecette;
    public string Description;
    public Sprite Polaroid;

    public List<Composants> Ingredients;

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
