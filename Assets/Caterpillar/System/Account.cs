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
using Firebase.Messaging;
#endif

public class Account : MonoBehaviour
{
    #region Singleton
    // Singleton
    private static Account Instance;

    #endregion

    public UnityEvent LoginSuccess;
    public UnityEvent Loginfail;

    private bool LoggedOnPlayfab = false;
    private bool LoggedWithGooglePlay = false;
    private LoginResult PlayfabLogins = null;
    private string AndroidPushToken = null;

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
    void InitializeLogin()
    {
#if UNITY_ANDROID
        InitializeAndroidPlatform();
        SignInWithGooglePlay();
#endif
    }

    private void OnPlayfabLoginResult(LoginResult Result)
    {
        LoggedOnPlayfab = true;
        PlayfabLogins = Result;
        Debug.Log("Playfab : Signed In as " + Result.PlayFabId);
        LoginSuccess.Invoke();

        //PlayFabClientAPI.AddGenericID();
        //PlayFabClientAPI.AddOrUpdateContactEmail();
        //PlayFabClientAPI.AndroidDevicePushNotificationRegistration();
        //PlayFabClientAPI.UpdateAvatarUrl();
        //PlayFabClientAPI.UpdateUserTitleDisplayName();
        //PlayFabClientAPI.GrantCharacterToUser();
    }

    private void OnPlayfabLoginError(PlayFabError Error)
    {
        Debug.LogError("Playfab Login Failed -> Error : " + Error.ToString());
#if UNITY_ANDROID
        SignInWithAndroidDevice();
#endif
        Loginfail.Invoke();
    }

    #endregion


    #region Static Methods

    public static void SetNotificationActive(bool Active)
    {
#if UNITY_ANDROID
        Instance.SetAndroidNotificationEnabled(Active);
#endif
        Debug.Log("Notification activation is now : " + Active);
    }

    public static bool GetNotificationEnabled()
    {
        bool result = false;
#if UNITY_ANDROID
        result = Instance.GetAndroidNotificationEnabled();
#endif
        Debug.Log("Notification activation is : " + result);
        return result;
    }

    #endregion


    #region Android Handlers

#if UNITY_ANDROID

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
        PlayFabClientAPI.LoginWithAndroidDeviceID(LoginRequest, OnPlayfabLoginResult, OnPlayfabLoginError);
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

            PlayFabClientAPI.LoginWithGoogleAccount(LoginRequest, OnPlayfabLoginResult, OnPlayfabLoginError);
        }
        else
        {
            Debug.LogWarning("Google : Failed to Login with Google Play : Fallback to Device Login");
            SignInWithAndroidDevice();
        }
    }

    private void InitializeAndroidNotification()
    {
        if(FirebaseMessaging.TokenRegistrationOnInitEnabled == true)
        {
            FirebaseMessaging.TokenReceived += OnTokenReceived;
            FirebaseMessaging.MessageReceived += OnMessageReceived;
        }
    }

    public void SetAndroidNotificationEnabled(bool Enabled)
    {
        bool WasEnabled = GetAndroidNotificationEnabled();

        if( WasEnabled != Enabled)
        {
            FirebaseMessaging.TokenRegistrationOnInitEnabled = Enabled;
            if ( Enabled )
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

        if(Enabled && AndroidPushToken == null)
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
        if(Enabled)
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

#endif

    #endregion
}
