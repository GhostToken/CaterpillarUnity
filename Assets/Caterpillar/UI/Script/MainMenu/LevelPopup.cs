using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelPopup : MonoBehaviour
{
    #region Property

    public TextMeshProUGUI Id;
    public Image Title_Star1;
    public Image Title_Star2;
    public Image Title_Star3;
    public TextMeshProUGUI Type;
    public TextMeshProUGUI Duration;
    public TextMeshProUGUI BestScore;
    public Image Star_Objective1;
    public Image Star_Objective2;
    public Image Star_Objective3;
    public TextMeshProUGUI Star_Objective_Description_1;
    public TextMeshProUGUI Star_Objective_Description_2;
    public TextMeshProUGUI Star_Objective_Description_3;
    public Color Achieved_Objective_Color;
    public Color Failed_Objective_Color;

    public GameObject PrefabRecette;

    public RectTransform ListeRecette;

    private int CurrentLevelId;
    private List<RecettePanel> CurrentRecettes = new List<RecettePanel>();

    #endregion

    #region Methods

    public void Open(int LevelId)
    {
        CurrentLevelId = LevelId;
        Level ThisLevel = Level.GetLevel(LevelId);
        if(ThisLevel != null)
        {
            SetupLevel(ThisLevel);
            SetupRecette(ThisLevel);
            SetupCompletion(LevelId);
        }
        else
        {
            Close();
        }
    }

    public void Close()
    {
        Menu.Instance.CloseAllPopups();
    }

    public void Launch()
    {
        Level.CurrentLevel = Level.GetLevel(CurrentLevelId);
        ScreenFader.Launch_FadeIn(() =>
        {
            SceneManager.LoadSceneAsync(Level.CurrentLevel.Scene, LoadSceneMode.Single);
        });
    }

    private void SetupLevel(Level ThisLevel)
    {
        Id.text = (ThisLevel.Id).ToString();
        Type.text = ThisLevel.GetTypeDeNiveau();
        Duration.text = ThisLevel.Duration.TotalMinutes.ToString() + " MIN.";
        Star_Objective_Description_1.text = ThisLevel.StarOne.Description;
        Star_Objective_Description_2.text = ThisLevel.StarTwo.Description;
        Star_Objective_Description_3.text = ThisLevel.StarThree.Description;
    }

    private void SetupRecette(Level ThisLevel)
    {
        foreach (RecettePanel oldRecette in CurrentRecettes)
        {
            GameObject.Destroy(oldRecette.gameObject);
        }

        CurrentRecettes.Clear();

        foreach ( Recette recette in ThisLevel.RecetteAFaire)
        {
            GameObject panelObject = GameObject.Instantiate(PrefabRecette);
            RecettePanel panel = panelObject.GetComponent<RecettePanel>();
            panel.SetRecette(recette);
            panel.rectTransform().SetParent(ListeRecette, false);
            CurrentRecettes.Add(panel);
        }
    }

    virtual protected void SetupCompletion(int LevelId)
    {
        int Score = SaveGame.GetScore(LevelId);
        int Stars = SaveGame.GetStars(LevelId);
        UpdateStars(Stars);
        UpdateTextColors(Stars);
        UpdateScore(Score);
    }

    protected void UpdateStars(int Stars)
    {
        Title_Star1.enabled = (Stars >= 1);
        Star_Objective1.enabled = (Stars >= 1);
        Title_Star2.enabled = (Stars >= 2);
        Star_Objective2.enabled = (Stars >= 2);
        Title_Star3.enabled = (Stars == 3);
        Star_Objective3.enabled = (Stars == 3);
    }

    protected void UpdateTextColors(int Stars)
    {
        Star_Objective_Description_1.color = (Stars >= 1 ? Achieved_Objective_Color : Failed_Objective_Color);
        Star_Objective_Description_2.color = (Stars >= 2 ? Achieved_Objective_Color : Failed_Objective_Color);
        Star_Objective_Description_3.color = (Stars == 3 ? Achieved_Objective_Color : Failed_Objective_Color);
    }

    protected void UpdateScore(int Score)
    {
        BestScore.text = Score.ToString();
    }

    #endregion
}
