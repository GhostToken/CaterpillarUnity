using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_ANDROID
// These using statements are required.
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
//using Steamworks;
#endif

public class Account : MonoBehaviour
{
    public UnityEvent LoginSuccess;
    public UnityEvent Loginfail;
    public TMPro.TextMeshProUGUI Output;

    //public object SteamUser { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        //var request = new LoginWithSteamRequest() { CreateAccount = true };
        //PlayFabClientAPI.LoginWithSteam(request, OnLoginSuccess, OnLoginFailure);
#elif UNITY_ANDROID
        // The following grants profile access to the Google Play Games SDK.
        // Note: If you also want to capture the player's Google email, be sure to add
        // .RequestEmail() to the PlayGamesClientConfiguration
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        .AddOauthScope("profile")
        .RequestServerAuthCode(false)
        .Build();
        PlayGamesPlatform.InitializeInstance(config);

        // recommended for debugging:
        PlayGamesPlatform.DebugLogEnabled = true;

        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();
#elif UNITY_IOS
        var request = new LoginWithIOSDeviceIDRequest() { DeviceId = SystemInfo.deviceUniqueIdentifier, CreateAccount = true };
        PlayFabClientAPI.LoginWithIOSDeviceID(request, OnLoginSuccess, OnLoginFailure);
#endif
        OnSignInButtonClicked();
    }

#if UNITY_ANDROID

    public void OnSignInButtonClicked()
    {
        Social.localUser.Authenticate(OnAuthentificate);
    }

    private void OnAuthentificate(bool Success)
    {
        if (Success)
        {
            Debug.Log("Google : Signed In");
            Output.text = "Google : Signed In";

            var serverAuthCode = PlayGamesPlatform.Instance.GetServerAuthCode();
            Debug.Log("Google : Server Auth Code: " + serverAuthCode);

            LoginWithGoogleAccountRequest LoginRequest = new LoginWithGoogleAccountRequest()
            {
                TitleId = PlayFabSettings.TitleId,
                ServerAuthCode = serverAuthCode,
                CreateAccount = true
            };

            PlayFabClientAPI.LoginWithGoogleAccount(LoginRequest, OnPlayfabLoginResult, OnPlayfabLoginError);
        }
        else
        {
            Loginfail.Invoke();
            Debug.Log("Google : Failed to Authorize your login");
            Output.text = "Google : Failed to Authorize your login";
        }
    }

    private void OnPlayfabLoginResult(LoginResult Result)
    {
        Debug.Log("Playfab : Signed In as " + Result.PlayFabId);
        LoginSuccess.Invoke();
        Output.text = "Playfab : Signed In as " + Result.PlayFabId;
    }

    private void OnPlayfabLoginError(PlayFabError Error)
    {
        Debug.Log("Playfab : Error : " + Error.ToString());
        Loginfail.Invoke();
        Output.text = "Playfab : Error : " + Error.ToString();
    }

#endif

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    //public string GetSteamAuthTicket()
    //{
    //    byte[] ticketBlob = new byte[1024];
    //    uint ticketSize;

    //    // Retrieve ticket; hTicket should be a field in the class so you can use it to cancel the ticket later
    //    // When you pass an object, the object can be modified by the callee. This function modifies the byte array you've passed to it.
    //    HAuthTicket hTicket = SteamUser.GetAuthSessionTicket(ticketBlob, ticketBlob.Length, out ticketSize);

    //    // Resize the buffer to actual length
    //    Array.Resize(ref ticketBlob, (int)ticketSize);

    //    // Convert bytes to string
    //    StringBuilder sb = new StringBuilder();
    //    foreach (byte b in ticketBlob)
    //    {
    //        sb.AppendFormat("{0:x2}", b);
    //    }
    //    return sb.ToString();
    //}
#endif // UNITY_EDITOR || UNITY_STANDALONE_WIN
}
