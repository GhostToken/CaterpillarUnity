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

    #endregion


    #region Unity Handlers

    void Start()
    {
        Notifications.isOn = Account.GetNotificationEnabled();
        MeshOccluding.isOn = Options.MeshOccluding;
        MenuAffiché.isOn = Options.MenuAffiché;
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

    #endregion
}
