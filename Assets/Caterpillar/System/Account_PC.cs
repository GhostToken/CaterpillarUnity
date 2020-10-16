using UnityEngine;
using System.Collections;
using PlayFab;
using PlayFab.ClientModels;
using System;

#if UNITY_STANDALONE_WIN || (UNITY_EDITOR && !UNITY_ANDROID)

public partial class Account : MonoBehaviour
{
    #region Properties

    private bool LoggedWithEmail = false;

    #endregion

    #region Login

    void InitializeLogin()
    {
        SignInWithEmail();
    }

    private void SignInWithEmail()
    {
        if (LoggedWithEmail == false)
        {
            LoginWithEmailAddressRequest LoginRequest = new LoginWithEmailAddressRequest()
            {
                TitleId = PlayFabSettings.TitleId,
                Email = "test@test.com",
                Password = "password"
            };

            PlayFabClientAPI.LoginWithEmailAddress(LoginRequest, OnPlayfabEmailLoginSuccess, OnPlayfabEmailLoginError);
        }
    }

    private void OnPlayfabEmailLoginSuccess(LoginResult Result)
    {
        OnPlayfabLoginSuccess(Result);
        LoggedWithEmail = true;
        Debug.LogWarning("Playfab : Success to Login with TEST Email !");
    }

    private void OnPlayfabEmailLoginError(PlayFabError Error)
    {
        Debug.LogError("Playfab PC Device Login Failed -> Error : " + Error.ToString());
    }

    #endregion

    #region Notifications

    public static void SetNotificationActive(bool Active)
    {
    }

    public static bool GetNotificationEnabled()
    {
        return false;
    }

    #endregion
}

#endif