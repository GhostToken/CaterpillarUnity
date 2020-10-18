using HedgehogTeam.EasyTouch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD_Pause : LevelPopup
{
    #region Methods

    public override void Open(int LevelId)
    {
        base.Open(LevelId);
        Partie.SetPause(true);
    }

    public override void Close()
    {
        gameObject.SetActive(false);
        Partie.SetPause(false);
    }

    public void GoToMenu()
    {
        Partie.SetPause(false);
        Menu.Open();
    }

    #endregion
}
