using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HUD : MonoBehaviour
{
    #region Properties

    public TextMeshProUGUI LevelId;
    public Image Portrait;

    public Image StarOn_1;
    public Image StarOn_2;
    public Image StarOn_3;
    public TextMeshProUGUI Timer;
    public Image BackGroundTimer;
    public TextMeshProUGUI Points;
    public HUD_ListeRecette ListeRecette;
    public HUD_Pause Pause;

    #endregion

    #region Unity Methods

    private void Start()
    {
        if (Level.CurrentLevel == null)
        {
            return;
        }

        Partie.Start();
        InitializeLevel();
    }

    private void Update()
    {
        if(Level.CurrentLevel == null)
        {
            return;
        }

        Partie.Update(Time.deltaTime);

        int minutes = Mathf.FloorToInt(Partie.TempsRestant / 60.0f);
        int seconds = Mathf.FloorToInt(Partie.TempsRestant % 60.0f);

        Timer.SetText(minutes.ToString("00") + ":" + seconds.ToString("00"));
        BackGroundTimer.fillAmount = (Partie.TempsRestant / (float)Level.CurrentLevel.Duration.TotalSeconds);

        Points.SetText(Partie.Score.ToString());

        StarOn_1.enabled = (Partie.Stars > 0);
        StarOn_2.enabled = (Partie.Stars > 1);
        StarOn_3.enabled = (Partie.Stars > 2);
    }

    #endregion

    #region Public Methods

    public void InitializeLevel()
    {
        LevelId.SetText(Level.CurrentLevel.Id.ToString());
        ListeRecette.SetLevel(Level.CurrentLevel);
        ListeRecette.gameObject.SetActive(Options.MenuAffiché);

        Points.SetText("0");

        BackGroundTimer.fillAmount = 1.0f; ;
        Timer.SetText(Level.CurrentLevel.Duration.Minutes.ToString() + ":" + Level.CurrentLevel.Duration.Seconds.ToString());

        StarOn_1.enabled = false;
        StarOn_2.enabled = false;
        StarOn_3.enabled = false;
    }

    public void OuvrePauseMenu()
    {
        if(Pause != null)
        {
            Pause.gameObject.SetActive(true);
            Pause.Open(Level.CurrentLevel.Id);
        }
    }

    #endregion
}
