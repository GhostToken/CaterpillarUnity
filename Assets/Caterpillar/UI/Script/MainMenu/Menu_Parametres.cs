﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_Parametres : MonoBehaviour
{
    #region Properties

    public Toggle Notifications;
    public Toggle MeshOccluding;
    public Toggle MenuAffiché;
    public Toggle MoveMarker;
    public Toggle TapToMove;

    #endregion


    #region Unity Handlers

    void OnEnable()
    {
        Notifications.isOn = Account.GetNotificationEnabled();
        MeshOccluding.isOn = Options.MeshOccluding;
        MenuAffiché.isOn = Options.MenuAffiché;
        MoveMarker.isOn = Options.MoveMarker;
        TapToMove.isOn = Options.UseTapToMove;
        MoveMarker.enabled = Options.UseTapToMove;
    }

    #endregion

    #region Public Handlers

    public void SetNotificationActive(bool Active)
    {
        Account.SetNotificationActive(Active);
    }

    public void SetMeshOccluding(bool Active)
    {
        Options.MeshOccluding = Active;
    }

    public void SetMenuAffiché(bool Active)
    {
        Options.MenuAffiché = Active;
    }

    public void SetMoveMarker(bool Active)
    {
        Options.MoveMarker = Active;
    }

    public void SetTapToMove(bool Active)
    {
        Options.UseTapToMove = Active;
    }

    public void RevoirTutoriel()
    {
        Menu.Instance.OpenTutoriel();
    }

    #endregion
}
