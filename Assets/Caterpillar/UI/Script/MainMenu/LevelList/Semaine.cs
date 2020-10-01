using Assets.Caterpillar.UI.Script.MainMenu.LevelList;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Semaine : MonoBehaviour
{
    #region Properties

    public Jour[] Jours;

    #endregion

    #region Methods

    public void CacheJour(EDay Jour)
    {
        int index = (int)Jour;
        Jours[(int)Jour].Cache();
    }

    public void ConfigueJour(EDay Jour, int _LevelOfMonth, int Day)
    {
        int index = (int)Jour;
        Jours[(int)Jour].Configure(_LevelOfMonth, Day);
    }

    #endregion
}
