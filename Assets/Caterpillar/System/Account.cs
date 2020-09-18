using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine;
using UnityEngine.Events;

public partial class Account : MonoBehaviour
{
    #region Singleton

    // Singleton
    private static Account Instance;

    #endregion

    public UnityEvent LoginSuccess;
    public UnityEvent Loginfail;

    private bool LoggedOnPlayfab = false;
    private LoginResult PlayfabLogins = null;

    //public object SteamUser { get; private set; }

    #region Unity Handlers

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeLogin();
    }

    #endregion


    #region Global Handlers

    private void OnPlayfabLoginSuccess(LoginResult Result)
    {
        LoggedOnPlayfab = true;
        PlayfabLogins = Result;
        Debug.Log("Playfab : Signed In as " + Result.PlayFabId);
        LoginSuccess.Invoke();

        GetLocalPlayerUserDatas();
        GetLocalPlayerStatistics();
    }

    #endregion

    #region Playfab Data Pull

    void GetLocalPlayerStatistics()
    {
        PlayFabClientAPI.GetPlayerStatistics( new GetPlayerStatisticsRequest(),
            OnGetLocalPlayerStatistics,
            (error) =>
            {
                Debug.Log("Got error retrieving player statistics");
                Debug.LogError(error.GenerateErrorReport());
            }
      );
    }

    private void OnGetLocalPlayerStatistics(GetPlayerStatisticsResult Result)
    {
        SaveGame.OnGetLocalPlayerStatistics(Result);
    }

    void GetLocalPlayerUserDatas()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            OnGetLocalUserData,
            (error) =>
            {
                Debug.Log("Got error retrieving user data:");
                Debug.Log(error.GenerateErrorReport());
            }
        );
    }

    private void OnGetLocalUserData(GetUserDataResult Result)
    {
        SaveGame.OnGetLocalUserData(Result);
    }

    #endregion
}
