using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

#region Data Types

public enum ETypeNiveau
{
    GloutonDebutant = 0,
    GloutonExplorateur = 1,
    ApprentisSorcier = 2,
    Maestro = 3,
    ChefDePartie = 4,
    SousChef = 5,
    Chef = 6,
    ChefEtoilé = 7,
    DieuDesFourneaux = 8
}

[System.Flags]
public enum EConditionEtoile
{
    PasDeGrignotage = 1,
    Recettes = 2,
    RecetteUniques = 4,
    Score = 8,
}

[Serializable]
public struct StarStep
{
    public string Description;
    [EnumAsFlags]
    public EConditionEtoile Conditions;
    [AnyFlagRequirement("Conditions", (int)EConditionEtoile.Recettes)]
    public int NombreDeRecette;
    [AnyFlagRequirement("Conditions", (int)EConditionEtoile.RecetteUniques)]
    public int NombreDeRecetteUnique;
    [AnyFlagRequirement("Conditions", (int)EConditionEtoile.Score)]
    public int ScoreRequis;

    public bool EstReussie(int RecetteValidées, int RecetteUniqueValidées, bool IngredientRestants, int Score)
    {
        if(Conditions.HasFlag(EConditionEtoile.PasDeGrignotage) == true)
        {
            if(IngredientRestants == true)
            {
                return false;
            }
        }

        if (Conditions.HasFlag(EConditionEtoile.Recettes) == true)
        {
            if (RecetteValidées < NombreDeRecette)
            {
                return false;
            }
        }

        if (Conditions.HasFlag(EConditionEtoile.RecetteUniques) == true)
        {
            if (RecetteUniqueValidées < NombreDeRecetteUnique)
            {
                return false;
            }
        }

        if (Conditions.HasFlag(EConditionEtoile.Score) == true)
        {
            if (Score < ScoreRequis)
            {
                return false;
            }
        }

        return true;
    }
}

#endregion

[CreateAssetMenu(fileName = "Level_", menuName = "Caterpillar/Level")]
public class Level : ScriptableObject
{
    #region Properties

    public int Id;
    [SerializeField]
    private ETypeNiveau TypeDeNiveau = ETypeNiveau.GloutonDebutant;
    public int Duree = 180;

    public string Scene;
    public List<Recette> RecetteAFaire;
    public StarStep StarOne;
    public StarStep StarTwo;
    public StarStep StarThree;

    #endregion

    #region Accessors

    public string GetTypeDeNiveau()
    {
        switch (TypeDeNiveau)
        {
            default:
            case ETypeNiveau.GloutonDebutant:
                {
                    return "Glouton débutant";
                }
            case ETypeNiveau.GloutonExplorateur:
                {
                    return "Glouton explorateur";
                }
            case ETypeNiveau.ApprentisSorcier:
                {
                    return "Apprentis sorcier";
                }
            case ETypeNiveau.Maestro:
                {
                    return "Maestro";
                }
            case ETypeNiveau.ChefDePartie:
                {
                    return "Chef de partie";
                }
            case ETypeNiveau.SousChef:
                {
                    return "Sous-chef";
                }
            case ETypeNiveau.Chef:
                {
                    return "Chef";
                }
            case ETypeNiveau.ChefEtoilé:
                {
                    return "Chef étoilé";
                }
            case ETypeNiveau.DieuDesFourneaux:
                {
                    return "Dieu des fourneaux";
                }
        }
    }

    public TimeSpan Duration
    {
        get
        {
            return new TimeSpan(0, 0, Duree);
        }
    }

    #endregion

    #region Static Accessor

    private static List<Level> AllLevels;

    public static Level CurrentLevel;

    public static List<Level> GetAllLevels()
    {
        if (AllLevels == null)
        {
            AllLevels = new List<Level>(Resources.LoadAll<Level>("Levels"));
        }

        return AllLevels;
    }

    public static Level GetLevel(int LevelId)
    {
        return GetAllLevels().Find(T => T.Id == LevelId);
    }

    #endregion
}
