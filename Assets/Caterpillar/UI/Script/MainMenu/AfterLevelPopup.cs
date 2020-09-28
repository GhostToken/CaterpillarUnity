using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AfterLevelPopup : MonoBehaviour
{
    #region Property

    public TextMeshProUGUI Id;
    public Image Title_Star1;
    public Image Title_Star2;
    public Image Title_Star3;
    public TextMeshProUGUI Score;
    public Image Star_Objective1;
    public Image Star_Objective2;
    public Image Star_Objective3;
    public TextMeshProUGUI Star_Objective_Description_1;
    public TextMeshProUGUI Star_Objective_Description_2;
    public TextMeshProUGUI Star_Objective_Description_3;
    public Color Achieved_Objective_Color;
    public Color Failed_Objective_Color;

    public TextMeshProUGUI TitreScoreEtoile;
    public TextMeshProUGUI TitreScoreAliments;
    public TextMeshProUGUI TitreScoreTemps;
    public TextMeshProUGUI TitreScoreArtefact;

    public TextMeshProUGUI DetailScoreEtoile;
    public TextMeshProUGUI DetailScoreAliments;
    public TextMeshProUGUI DetailScoreTemps;
    public TextMeshProUGUI DetailScoreArtefact;

    public TextMeshProUGUI TotalScoreEtoile;
    public TextMeshProUGUI TotalScoreAliments;
    public TextMeshProUGUI TotalScoreTemps;
    public TextMeshProUGUI TotalScoreArtefact;

    public Image[] Polaroids;

    public float AnimationDuration = 0.25f;

    public RectTransform ExpandArea;
    public RectTransform ExpandAreaIcon;
    public float ExpandAreaMinHeight = 120;
    public float ExpandAreaMaxHeight = 736;
    public RectTransform PolaroidArea;
    public float PolaroidAreaMinScale = 0.5f;
    public float PolaroidAreaMaxScale = 1.0f;

    private int CurrentLevelId;
    private bool DetailVisible = false;

    #endregion

    #region Methods
    public void Open(int LevelId)
    {
        CurrentLevelId = LevelId;
        Level ThisLevel = Level.GetLevel(LevelId);
        if (ThisLevel != null)
        {
            SetupLevel(ThisLevel);
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

    private void SetupLevel(Level ThisLevel)
    {
        Id.text = (ThisLevel.Id).ToString();
        Star_Objective_Description_1.text = ThisLevel.StarOne.Description;
        Star_Objective_Description_2.text = ThisLevel.StarTwo.Description;
        Star_Objective_Description_3.text = ThisLevel.StarThree.Description;
        UpdatePolaroids(ThisLevel);
    }

    virtual protected void SetupCompletion(int LevelId)
    {
        UpdateStars(Partie.Stars);
        UpdateResultats();
        StartCoroutine(UpdateScore(Partie.Score));
    }

    protected void UpdatePolaroids(Level ThisLevel)
    {
        for(int index = 0; index < Polaroids.Length; ++index)
        {
            if(index < ThisLevel.RecetteAFaire.Count)
            {
                Polaroids[index].sprite = ThisLevel.RecetteAFaire[index].Polaroid;
            }
            else
            {

            }
            Recette recette = ThisLevel.RecetteAFaire[index];
        }
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
            Sequence.Play();
        }
        Title_Star2.transform.localScale = Vector3.zero;
        if (Stars >= 2)
        {
            Sequence Sequence = DOTween.Sequence();
            Sequence.Append(Title_Star2.transform.DOScale(1.0f, AnimationDuration));
            Sequence.Append(Title_Star2.transform.DOPunchScale(Vector3.one * 0.25f, AnimationDuration / 2.0f));
            Sequence.PrependInterval(AnimationDuration * 2.0f);
            Sequence.Play();
        }
        Title_Star3.transform.localScale = Vector3.zero;
        if (Stars == 3)
        {
            Sequence Sequence = DOTween.Sequence();
            Sequence.Append(Title_Star3.transform.DOScale(1.0f, AnimationDuration));
            Sequence.Append(Title_Star3.transform.DOPunchScale(Vector3.one * 0.25f, AnimationDuration / 2.0f));
            Sequence.PrependInterval(AnimationDuration * 3.0f);
            Sequence.Play();
        }
    }

    protected IEnumerator UpdateScore(int Points)
    {
        Score.text = "0";
        yield return new WaitForSeconds(AnimationDuration);
        float Timer = 0.0f;
        float TimerDuration = AnimationDuration * 3.0f;
        while (Timer < TimerDuration)
        {
            int Value = (int)Mathf.Lerp(0, Points, Timer / TimerDuration);
            Score.text = Value.ToString();
            Timer += Time.deltaTime;
            yield return null;
        }
        Score.text = Points.ToString();
    }

    protected void UpdateResultats()
    {
        TitreScoreEtoile.text       = Partie.Stars + " étoiles";
        TitreScoreAliments.text     = Partie.ToutLeRepas.Count + " aliments";
        TitreScoreTemps.text        = Mathf.FloorToInt(Partie.TempsRestant) + " sec. bonus";
        TitreScoreArtefact.text     = "0 Artefact";

        DetailScoreEtoile.text      = Partie.Stars + " x500 pts";
        DetailScoreAliments.text    = Partie.ToutLeRepas.Count + " x50 pts";
        DetailScoreTemps.text       = Mathf.FloorToInt(Partie.TempsRestant) + " x200 pts";
        DetailScoreArtefact.text    = "0x1000 pts";

        TotalScoreEtoile.text       = (Partie.Stars * 500).ToString() + "PTS";
        TotalScoreAliments.text     = (Partie.ToutLeRepas.Count * 50).ToString() + "PTS";
        TotalScoreTemps.text        = (Mathf.FloorToInt(Partie.TempsRestant)*200).ToString() + "PTS";
        TotalScoreArtefact.text     = "0PTS";
    }

    protected void UpdateDetailsStars(int Stars)
    {
        Star_Objective1.transform.localScale = Vector3.zero;
        if (Stars >= 1)
        {
            Sequence Sequence = DOTween.Sequence();
            Sequence.Append(Star_Objective1.transform.DOScale(1.0f, AnimationDuration));
            Sequence.Append(Star_Objective1.transform.DOPunchScale(Vector3.one * 0.25f, AnimationDuration / 2.0f));
            Sequence.PrependInterval(AnimationDuration);
            Sequence.Play();
        }
        Star_Objective2.transform.localScale = Vector3.zero;
        if (Stars >= 2)
        {
            Sequence Sequence = DOTween.Sequence();
            Sequence.Append(Star_Objective2.transform.DOScale(1.0f, AnimationDuration));
            Sequence.Append(Star_Objective2.transform.DOPunchScale(Vector3.one * 0.25f, AnimationDuration / 2.0f));
            Sequence.PrependInterval(AnimationDuration * 2.0f);
            Sequence.Play();
        }
        Star_Objective3.transform.localScale = Vector3.zero;
        if (Stars == 3)
        {
            Sequence Sequence = DOTween.Sequence();
            Sequence.Append(Star_Objective3.transform.DOScale(1.0f, AnimationDuration));
            Sequence.Append(Star_Objective3.transform.DOPunchScale(Vector3.one * 0.25f, AnimationDuration / 2.0f));
            Sequence.PrependInterval(AnimationDuration * 3.0f);
            Sequence.Play();
        }
    }

    protected void UpdateTextColors(int Stars)
    {
        Star_Objective_Description_1.color = Failed_Objective_Color;
        if (Stars >= 1)
        {
            Star_Objective_Description_1.DOBlendableColor(Achieved_Objective_Color, AnimationDuration).SetDelay(AnimationDuration);
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

    #endregion


    #region Events

    public void OpenDetails()
    {
        int Stars = Partie.Stars;
        DetailVisible = !DetailVisible;
        ExpandAreaIcon.DOScale(new Vector3(1.0f, (DetailVisible ? -1.0f : 1.0f), 1.0f), AnimationDuration);
        if (DetailVisible)
        {
            ExpandArea.DOSizeDelta(new Vector2(ExpandArea.sizeDelta.x, ExpandAreaMaxHeight), AnimationDuration);
            PolaroidArea.DOScale(Vector3.one * PolaroidAreaMinScale, AnimationDuration);
            UpdateDetailsStars(Stars);
            UpdateTextColors(Stars);
        }
        else
        {
            ExpandArea.DOSizeDelta(new Vector2(ExpandArea.sizeDelta.x, ExpandAreaMinHeight), AnimationDuration);
            PolaroidArea.DOScale(Vector3.one * PolaroidAreaMaxScale, AnimationDuration);
        }
    }

    public void Rejouer()
    {
        SceneManager.LoadSceneAsync(Level.CurrentLevel.Scene, LoadSceneMode.Single);
    }

    public void NiveauSuivant()
    {
        int CurrentLevelId = Level.CurrentLevel.Id;
        Level.CurrentLevel = Level.GetLevel(CurrentLevelId + 1);
        if (Level.CurrentLevel == null)
        {
            Level.CurrentLevel = Level.GetLevel(CurrentLevelId);
        }
        Menu.Instance.CloseAllPopups();
        Menu.Instance.OpenLevelPopup(Level.CurrentLevel.Id);
    }

    #endregion
}
