using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AfterLevelPopup : LevelPopup
{
    override protected void SetupCompletion(int LevelId)
    {
        UpdateStars(Partie.Stars);
        UpdateTextColors(Partie.Stars);
        UpdateScore(Partie.Score);
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
}
