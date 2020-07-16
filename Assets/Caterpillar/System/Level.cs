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

[Serializable]
public struct StarStep
{
    public bool PasDeGrignotage;
    public int NombreDeRecette;
    public int NombreDeRecetteUnique;
    public string Description;

    public bool EstReussie(int RecetteValidées, int RecetteUniqueValidées, bool IngredientRestants)
    {
        return ((NombreDeRecette <= RecetteValidées) && (NombreDeRecetteUnique <= RecetteUniqueValidées) && (PasDeGrignotage ? !IngredientRestants : true));
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
    public TimeSpan Duration = new TimeSpan(0, 1, 0);

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
