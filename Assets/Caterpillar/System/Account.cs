using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
//using Steamworks;
#endif

public class Account : MonoBehaviour
{
    public object SteamUser { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        //var request = new LoginWithSteamRequest() { CreateAccount = true };
        //PlayFabClientAPI.LoginWithSteam(request, OnLoginSuccess, OnLoginFailure);
#elif UNITY_ANDROID
        var request = new LoginWithAndroidDeviceIDRequest() { AndroidDeviceId = SystemInfo.deviceUniqueIdentifier, CreateAccount = true };
        PlayFabClientAPI.LoginWithAndroidDeviceID(request, OnLoginSuccess, OnLoginFailure);
#elif UNITY_IOS
        var request = new LoginWithIOSDeviceIDRequest() { DeviceId = SystemInfo.deviceUniqueIdentifier, CreateAccount = true };
        PlayFabClientAPI.LoginWithIOSDeviceID(request, OnLoginSuccess, OnLoginFailure);
#endif
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, you made your first successful API call!");
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your first API call.  :(");
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }

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
