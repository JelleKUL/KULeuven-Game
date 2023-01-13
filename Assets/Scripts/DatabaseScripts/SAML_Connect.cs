using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Reflection;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;
using AS;

// connects the saml te request an authenticationCode

public class SAML_Connect : MonoBehaviour
{ 
    [SerializeField]
    private Text debugText;

    [SerializeField]
    private string phpFilesLocation = "https://iiw.kuleuven.be/serious-game-topografie/accountsystem/";
    [SerializeField]
    private string authenticationFile = "simplesamlconnect.php";

    private string errorCode = "MySQL_ERROR";
    const string fieldsSeparator = "$#@(_fields_separator_*&%^";
    const string fieldNameValueSeparator = "$#@(_field_name_value_separator_*&%^";

    AS_AccountInfo accountInfo = new AS_AccountInfo();
    bool clickedWebsite = false;


    [DllImport("__Internal")]
    private static extern void openWindow(string url);

    void Awake()
    {
        if (debugText) debugText.text = "";
    }

    public void LoginWithSaml()
    {
        clickedWebsite = true;
        Debug.Log("Clicked, going to website");
        openWindow("https://iiw.kuleuven.be/serious-game-topografie/accountsystem/simplesamlredirect.php/");
        TryLoginSaml();
    }

    void TryLoginSaml()
    {
        Debug.Log("sending a request to the Saml server to get the key of the user");
        TryAuthenticate(SamlAttempted);
    }

    void SamlAttempted(string key)
    {
        Debug.Log("Got data: " + key + " trying to get the ID now");
        key.TryGetId(LoginAttempted);
    }

    // Called by the AttemptLogin coroutine when it's finished executing
    public void LoginAttempted(string callbackMessage)
    {
        // If our log in failed,
        if (callbackMessage.IsAnError())
        {
            this.Log(LogType.Error, callbackMessage);
            return;
        }
        // Otherwise,
        int accountId = Convert.ToInt32(callbackMessage);
        OnSuccessfulLogin(accountId);
    }

    public void OnSuccessfulLogin(int id)
    {
        this.Log(LogType.Log, "Successfully Logged In User with id: " + id);
        accountInfo.TryToDownload(id, AccountInfoDownloaded);
    }

    void AccountInfoDownloaded(string message)
    {
        if (message.ToLower().Contains("error"))
        {
            this.Log(LogType.Error, "Account System: " + message);
        }
        else
        {
            this.Log(LogType.Log, "Account System: " + message);
            int.TryParse(accountInfo.GetFieldValue("id"), out GameManager.loginID);
            GameManager.playerScore = accountInfo.customInfo.totalScore;
            GameManager.userName = accountInfo.GetFieldValue("username");
            GameManager.isLoggedIn = true;
            GameManager.chaptersInfos = accountInfo.customInfo.chapters;
            SceneManager.LoadScene("MainMenu");
        }

    }


    /// <summary>
    /// Send an authentication request to the server
    /// </summary>
    public void TryAuthenticate(Action<string> resultCallback)
    {
        LogMessage("Starting authenticationRequest @ " + phpFilesLocation);
        StartCoroutine(SendAuthenticationRequest(resultCallback));
        
    }

    IEnumerator SendAuthenticationRequest(Action<string> resultCallback)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(phpFilesLocation + authenticationFile);
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError) // check for any network errors
        {
            LogMessage("<b><color=red>DB_Connect:</color></b>: " + webRequest.error);
        }
        else if (webRequest.downloadHandler.text.Contains(errorCode)) // check for any database error
        {
            LogMessage("<b><color=red>DB_Connect:</color></b>: " + webRequest.downloadHandler.text);
        }
        else
        {
            LogMessage("<b><color=green>DB_Connect:</color></b>: " + "Data Downloaded Successfully with contents: " + webRequest.downloadHandler.text);
            resultCallback(webRequest.downloadHandler.text);
        }
    }


    /// <summary>
    /// log a debug message to the console and an optional debug text
    /// </summary>
    /// <param name="mss"></param>
    /// <param name="level">0:Log, 1:Warning, 2:Error</param>
    void LogMessage(string mss, int level = 0)
    {
        switch (level)
        {
            case 0:
                Debug.Log(mss);
                if (debugText) debugText.color = Color.white;
                break;
            case 1:
                Debug.LogWarning(mss);
                if (debugText) debugText.color = Color.yellow;
                break;
            case 2:
                Debug.LogError(mss);
                if (debugText) debugText.color = Color.red;
                break;
            default:
                break;
        }
        if (debugText) debugText.text = mss;
    }

}


