using DG.Tweening;
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
    public float AnimationDuration = 0.25f;
    public RectTransform[] ScaleRoot;

    public GameObject PrefabRecette;

    public RectTransform ListeRecette;

    private int CurrentLevelId;
    private List<RecettePanel> CurrentRecettes = new List<RecettePanel>();

    #endregion

    #region Methods

    public void Open(int LevelId)
    {
        foreach(RectTransform root in ScaleRoot)
        {
            root.localScale = Vector3.zero;
            root.DOScale(Vector3.one, 0.25f);
        }
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
        StartCoroutine(UpdateScore(Score));
    }

    protected void UpdateStars(int Stars)
    {
        Title_Star1.transform.localScale = Vector3.zero;
        if (Stars >= 1)
        {
            Sequence Sequence = DOTween.Sequence();
            Sequence.Append(Title_Star1.transform.DOScale(1.0f, AnimationDuration));
            Sequence.Append(Title_Star1.transform.DOPunchScale(Vector3.one * 0.25f, AnimationDuration / 2.0f));
            Sequence.PrependInterval(AnimationDuration);
        }
        Title_Star2.transform.localScale = Vector3.zero;
        if (Stars >= 2)
        {
            Sequence Sequence = DOTween.Sequence();
            Sequence.Append(Title_Star2.transform.DOScale(1.0f, AnimationDuration));
            Sequence.Append(Title_Star2.transform.DOPunchScale(Vector3.one * 0.25f, AnimationDuration / 2.0f));
            Sequence.PrependInterval(AnimationDuration * 2.0f);
        }
        Title_Star3.transform.localScale = Vector3.zero;
        if (Stars == 3)
        {
            Sequence Sequence = DOTween.Sequence();
            Sequence.Append(Title_Star3.transform.DOScale(1.0f, AnimationDuration));
            Sequence.Append(Title_Star3.transform.DOPunchScale(Vector3.one * 0.25f, AnimationDuration / 2.0f));
            Sequence.PrependInterval(AnimationDuration * 3.0f);
        }
    }

    protected void UpdateTextColors(int Stars)
    {
        Star_Objective_Description_1.color = Failed_Objective_Color;
        if (Stars >= 1)
        {
            Star_Objective_Description_1.DOBlendableColor(Achieved_Objective_Color, AnimationDuration).SetDelay(AnimationDuration * 1.0f);
        }
        Star_Objective_Description_2.color = Failed_Objective_Color;
        if (Stars >= 2)
        {
            Star_Objective_Description_2.DOBlendableColor(Achieved_Objective_Color, AnimationDuration).SetDelay(AnimationDuration * 2.0f);
        }
        Star_Objective_Description_3.color = Failed_Objective_Color;
        if (Stars == 3)
        {
            Star_Objective3.DOBlendableColor(Achieved_Objective_Color, AnimationDuration).SetDelay(AnimationDuration * 3.0f);
        }
    }

    protected IEnumerator UpdateScore(int Score)
    {
        BestScore.text = "0";
        yield return new WaitForSeconds(AnimationDuration);
        float Timer = 0.0f;
        float TimerDuration = AnimationDuration * 3.0f;
        while (Timer < TimerDuration)
        {
            int Value = (int)Mathf.Lerp(0, Score, Timer/ TimerDuration);
            BestScore.text = Value.ToString();
            Timer += Time.deltaTime;
            yield return null;
        }
        BestScore.text = Score.ToString();
    }

    #endregion
}
