using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Partie
{
    #region Properties

    public static int Score;
    public static int Stars;
    public static bool JustTerminated = false;

    public static float TempsRestant;
    public static List<Ingredient> Estomac = new List<Ingredient>();

    public static List<Recette> RecetteValidées = new List<Recette>();
    public static List<Recette> RecetteComplètes = new List<Recette>();

    public static List<Ingredient> ToutLeRepas = new List<Ingredient>();

    #endregion

    #region Public Methods

    public static void Start()
    {
        Score = 0;
        Stars = 0;
        TempsRestant = (float)Level.CurrentLevel.Duration.TotalSeconds;
        Estomac = new List<Ingredient>();
        ToutLeRepas = new List<Ingredient>();
        RecetteValidées = new List<Recette>();
        RecetteComplètes = new List<Recette>();


    }

    public static void Mange(Ingredient Ingredient)
    {
        Estomac.Add(Ingredient);
        ToutLeRepas.Add(Ingredient);
        Score += 50;

        Debug.Log("Ingredient mange : " + Ingredient.Nom);

        if (Level.CurrentLevel == null)
        {
            return;
        }

        CheckRecette();
        CheckRecetteAOptions();
        CheckStars();

        if(Stars == 3)
        {
            Terminate();
        }
    }

    public static void Update(float DeltaTime)
    {
        if( Level.CurrentLevel == null)
        {
            return;
        }

        if( !JustTerminated )
        {
            TempsRestant -= DeltaTime;

            if ((TempsRestant < 0.0f) || Input.GetKeyDown(KeyCode.Backspace))
            {
                Terminate();
                TempsRestant = 0.0f;
            }
        }
    }

    public static void Terminate()
    {
        Score += Mathf.FloorToInt(TempsRestant) * 200;
        SaveGame.RecordCurrentGame();
        JustTerminated = true;
        ScreenFader.Launch_FadeIn( () =>
        {
            SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
        });
    }

    public static int CompteRecetteCompletees(Recette Recette)
    {
        int Result = 0;

        Result += RecetteValidées.Count(T => T == Recette);
        Result += RecetteComplètes.Count(T => T == Recette);

        return Result;
    }

    #endregion

    #region Private Methods

    private static void CheckRecette()
    {
        foreach (Recette recetteAFaire in Level.CurrentLevel.RecetteAFaire)
        {
            bool recetteValidée = true;

            foreach (Composants composant in recetteAFaire.Ingredients)
            {
                if(composant.Flags.HasFlag(EIngredientFlags.Necessaire) == true)
                {
                    if (!Estomac.Contains(composant.Ingredient))
                    {
                        recetteValidée = false;
                        break;
                    }
                }
            }

            if (recetteValidée)
            {
                foreach (Composants composant in recetteAFaire.Ingredients)
                {
                    if (composant.Flags.HasFlag(EIngredientFlags.Necessaire) == true)
                    {
                        Estomac.Remove(composant.Ingredient);
                    }
                }

                RecetteValidées.Add(recetteAFaire);
                Debug.Log("Recette valide : " + recetteAFaire.Nom);
            }

            break;
        }
    }

    private static void CheckRecetteAOptions()
    {
        foreach (Recette recetteIncomplete in RecetteValidées)
        {
            bool recetteComplete = true;

            foreach (Composants composant in recetteIncomplete.Ingredients)
            {
                if (composant.Flags.HasFlag(EIngredientFlags.Necessaire) == false)
                {
                    if (!Estomac.Contains(composant.Ingredient))
                    {
                        recetteComplete = false;
                        break;
                    }
                }
            }

            if(recetteComplete)
            {
                foreach (Composants composant in recetteIncomplete.Ingredients)
                {
                    if (composant.Flags.HasFlag(EIngredientFlags.Necessaire) == false)
                    {
                        Estomac.Remove(composant.Ingredient);
                    }
                }

                RecetteComplètes.Add(recetteIncomplete);
                RecetteValidées.Remove(recetteIncomplete);

                Debug.Log("Recette Recette Complete : " + recetteIncomplete.Nom);
            }
            break;
        }
    }

    private static void CheckStars()
    {
        int TotalRecetteValidées = RecetteComplètes.Count + RecetteValidées.Count;
        int TotalRecetteUniqueValidées = CompteRecettesUniques();
        bool IngredientRestants = Estomac.Count > 0;

        switch (Stars)
        {
            default:
                {
                    break;
                }
            case 0:
                {
                    if( Level.CurrentLevel.StarOne.EstReussie(TotalRecetteValidées, TotalRecetteUniqueValidées, IngredientRestants, Score))
                    {
                        Stars++;
                        Score += 500;
                    }
                    break;
                }
            case 1:
                {
                    if (Level.CurrentLevel.StarTwo.EstReussie(TotalRecetteValidées, TotalRecetteUniqueValidées, IngredientRestants, Score))
                    {
                        Stars++;
                        Score += 500;
                    }
                    break;
                }
            case 2:
                {
                    if (Level.CurrentLevel.StarThree.EstReussie(TotalRecetteValidées, TotalRecetteUniqueValidées, IngredientRestants, Score))
                    {
                        Stars++;
                        Score += 500;
                    }
                    break;
                }
        }
    }

    private static int CompteRecettesUniques()
    {
        List<Recette> RecettesUniques = new List<Recette>();
        foreach (Recette recette in RecetteValidées)
        {
            if(!RecettesUniques.Contains(recette))
            {
                RecettesUniques.Add(recette);
            }
        }
        foreach (Recette recette in RecetteComplètes)
        {
            if (!RecettesUniques.Contains(recette))
            {
                RecettesUniques.Add(recette);
            }
        }
        return RecettesUniques.Count;
    }

    #endregion
}
