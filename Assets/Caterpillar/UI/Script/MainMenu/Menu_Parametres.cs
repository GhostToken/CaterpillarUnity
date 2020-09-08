using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_Parametres : MonoBehaviour
{
    #region Properties

    public Toggle Notifications;

    #endregion


    #region Unity Handlers

    void Start()
    {
        Notifications.isOn = Account.GetNotificationEnabled();
    }

    #endregion

    #region Public Handlers

    public void SetNotificationActive(bool Active)
    {
        Account.SetNotificationActive(Active);
    }

    #endregion
}
