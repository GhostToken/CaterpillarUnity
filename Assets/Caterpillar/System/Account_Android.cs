using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System;

#if UNITY_ANDROID

// These using statements are required.
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Firebase.Messaging;

public partial class Account : MonoBehaviour
{
    #region Properties

    private bool LoggedWithGooglePlay = false;
    private string AndroidPushToken = null;

    #endregion


    #region static Properties

    private static string DeviceLinkedFlag
    {
        get
        {
            return "AndroidDeviceLinkedFlag";
        }
    }

    #endregion


    #region Login

    void InitializeLogin()
    {
        InitializeAndroidPlatform();
        SignInWithGooglePlay();
    }

    private void InitializeAndroidPlatform()
    {
        // The following grants profile access to the Google Play Games SDK.
        // Note: If you also want to capture the player's Google email, be sure to add
        // .RequestEmail() to the PlayGamesClientConfiguration
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        .AddOauthScope("profile")
        .RequestServerAuthCode(false)
        .RequestIdToken()
        .Build();
        PlayGamesPlatform.InitializeInstance(config);

        // recommended for debugging:
        PlayGamesPlatform.DebugLogEnabled = true;

        // we activate only if player has opt in
        InitializeAndroidNotification();

        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();
    }

    public void SignInWithGooglePlay()
    {
        if (LoggedWithGooglePlay == true)
        {
            Debug.LogWarning("SignInWithGooglePlay : Already logged with Google Play !");
            return;
        }
        Social.localUser.Authenticate(OnAuthentificateWithGoogle);
    }

    private void OnAuthentificateWithGoogle(bool Success)
    {
        if (Success)
        {
            LoggedWithGooglePlay = true;

            var serverAuthCode = PlayGamesPlatform.Instance.GetServerAuthCode();
            LoginWithGoogleAccountRequest LoginRequest = new LoginWithGoogleAccountRequest()
            {
                TitleId = PlayFabSettings.TitleId,
                ServerAuthCode = serverAuthCode,
                CreateAccount = true
            };

            PlayFabClientAPI.LoginWithGoogleAccount(LoginRequest, OnPlayfabGooglePlayLoginSuccess, OnPlayfabGooglePlayLoginError);
        }
        else
        {
            Debug.LogWarning("Google : Failed to Login with Google Play : Fallback to Android Device Login");
            SignInWithAndroidDevice();
        }
    }

    private void OnPlayfabGooglePlayLoginSuccess(LoginResult Result)
    {
        OnPlayfabLoginSuccess(Result);
        LinkAndroidDevice();
    }

    private void OnPlayfabGooglePlayLoginError(PlayFabError Error)
    {
        Debug.LogError("Playfab GooglePlay Login Failed -> Error : " + Error.ToString());
        SignInWithAndroidDevice();
    }

    public void SignInWithAndroidDevice()
    {
        if (LoggedOnPlayfab == true)
        {
            Debug.LogWarning("SignInWithAndroidDevice : Already logged with Playfab !");
            return;
        }
        LoginWithAndroidDeviceIDRequest LoginRequest = new LoginWithAndroidDeviceIDRequest()
        {
            CreateAccount = true,
            AndroidDeviceId = SystemInfo.deviceUniqueIdentifier,
            OS = "Android"
        };
        PlayFabClientAPI.LoginWithAndroidDeviceID(LoginRequest, OnPlayfabAndroidDeviceLoginResult, OnPlayfabAndroidDeviceLoginError);
    }

    private void OnPlayfabAndroidDeviceLoginResult(LoginResult Result)
    {
        OnPlayfabLoginSuccess(Result);
    }

    private void OnPlayfabAndroidDeviceLoginError(PlayFabError Error)
    {
        Debug.LogError("Playfab Android Device Login Failed -> Error : " + Error.ToString());
        SignInWithAndroidDevice();
        Loginfail.Invoke();
    }

    public void LinkAndroidDevice()
    {
        if( PlayerPrefs.HasKey(DeviceLinkedFlag) == false )
        {
            LinkAndroidDeviceIDRequest LinkRequest = new LinkAndroidDeviceIDRequest()
            {
                AndroidDeviceId = SystemInfo.deviceUniqueIdentifier,
                AndroidDevice = SystemInfo.deviceName,
                OS = "Android",
                ForceLink = true
            };
            PlayFabClientAPI.LinkAndroidDeviceID(LinkRequest, 
            OnLinkDeviceSuccess,
            (error) =>
            {
                Debug.LogError("Got error Linking Android Device ID");
                Debug.LogError(error.GenerateErrorReport());
            });
        }
    }

    private void OnLinkDeviceSuccess(LinkAndroidDeviceIDResult Result)
    {
        PlayerPrefs.SetString(DeviceLinkedFlag, "true");
    }

    #endregion


    #region Notifications

    public static void SetNotificationActive(bool Active)
    {
        Instance.SetAndroidNotificationEnabled(Active);
        Debug.Log("Notification activation is now : " + Active);
    }

    public static bool GetNotificationEnabled()
    {
        bool result = false;
        result = Instance.GetAndroidNotificationEnabled();
        Debug.Log("Notification activation is : " + result);
        return result;
    }

    private void InitializeAndroidNotification()
    {
        if (FirebaseMessaging.TokenRegistrationOnInitEnabled == true)
        {
            FirebaseMessaging.TokenReceived += OnTokenReceived;
            FirebaseMessaging.MessageReceived += OnMessageReceived;
        }
    }

    public void SetAndroidNotificationEnabled(bool Enabled)
    {
        bool WasEnabled = GetAndroidNotificationEnabled();

        if (WasEnabled != Enabled)
        {
            FirebaseMessaging.TokenRegistrationOnInitEnabled = Enabled;
            if (Enabled)
            {
                FirebaseMessaging.TokenReceived += OnTokenReceived;
                //FirebaseMessaging.MessageReceived += OnMessageReceived;
            }
            else
            {
                FirebaseMessaging.TokenReceived -= OnTokenReceived;
                //FirebaseMessaging.MessageReceived -= OnMessageReceived;

                // TO DO : Do something to stop notification
                // https://community.playfab.com/questions/25697/enabledisable-push-notifications.html
                // FirebaseMessaging.getInstance().unsubscribeFromTopic("YourTopic");
            }

            ConnectPlayfabNotification(Enabled);
        }
    }

    public bool GetAndroidNotificationEnabled()
    {
        return FirebaseMessaging.TokenRegistrationOnInitEnabled;
    }

    private void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {
        Debug.Log("FirebaseMessaging: OnTokenReceived : " + token.Token);
        AndroidPushToken = token.Token;
        ConnectPlayfabNotification(true);
    }

    private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        Debug.Log("PlayFab: Received a notification:" + e.Message.Notification.Title);
        Debug.Log("PlayFab: Received a new message from: " + e.Message.From);
        if (e.Message.Data != null)
        {
            Debug.Log("PlayFab: Received a message with data:");
            foreach (var pair in e.Message.Data)
                Debug.Log("PlayFab data element: " + pair.Key + "," + pair.Value);
        }
        if (e.Message.Notification != null)
        {
            Debug.Log("PlayFab: Received a notification:" + e.Message.Notification.Body);
        }
    }

    private void ConnectPlayfabNotification(bool Enabled)
    {
        if (LoggedOnPlayfab == false)
        {
            Debug.LogWarning("PlayFab: ConnectPlayfabNotification : Playfab not logged yet");
            return;
        }

        if (Enabled && AndroidPushToken == null)
        {
            Debug.LogWarning("PlayFab: ConnectPlayfabNotification : Android Push token not received yet");
            return;
        }
        Debug.Log("PlayFab: ConnectPlayfabNotification : " + Enabled);

        AndroidDevicePushNotificationRegistrationRequest request = new AndroidDevicePushNotificationRegistrationRequest
        {
            DeviceToken = Enabled ? AndroidPushToken : "None",
            SendPushNotificationConfirmation = Enabled,
            ConfirmationMessage = "Push notifications registered successfully",
        };
        if (Enabled)
        {
            PlayFabClientAPI.AndroidDevicePushNotificationRegistration(request, OnAndroidDevicePushNotificationUpdate, OnAndroidDevicePushNotification_Activate_Error);
        }
        else
        {
            PlayFabClientAPI.AndroidDevicePushNotificationRegistration(request, OnAndroidDevicePushNotificationUpdate, OnAndroidDevicePushNotification_Deactivate_Error);
        }
    }

    private void OnAndroidDevicePushNotificationUpdate(AndroidDevicePushNotificationRegistrationResult result)
    {
        Debug.Log("PlayFab: Push Registration Successful");
    }

    private void OnAndroidDevicePushNotification_Activate_Error(PlayFabError Error)
    {
        Debug.LogError("Playfab failed to register notification -> Error : " + Error.ToString());
    }

    private void OnAndroidDevicePushNotification_Deactivate_Error(PlayFabError Error)
    {
        Debug.LogWarning("Playfab failed to register notification -> Error : " + Error.ToString());
    }

    #endregion
}

#endif