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

        if (webRequest.isNetworkError || webRequest.isHttpError) // check for any network errors
        {
            LogMessage("<b><color=blue>DB_Connect:</color></b>: " + webRequest.error);
        }
        else if (webRequest.downloadHandler.text.Contains(errorCode)) // check for any database error
        {
            LogMessage("<b><color=blue>DB_Connect:</color></b>: " + webRequest.downloadHandler.text);
        }
        else
        {
            LogMessage("<b><color=blue>DB_Connect:</color></b>: " + "Data Downloaded Successfully with contents: " + webRequest.downloadHandler.text);
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


