using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// A short demonstration of the key AccountSystem Elements and workflow.
/// 
/// When Designing this asset, we had in mind the following order:
/// 
/// 0) Download the registration Form as "accountInfo"
/// 	Done by accountInfo.TryToDownloadRegistrationForm (callback)
/// 
/// 1) Add the user's input to the accountInfo fields
/// 
/// 2) Attempt to Register	- 
/// 	Done by accountInfo.TryToRegister (callback)
/// 
/// 3) Prompt the user to input their username and password
/// 
/// 4) Attempt to Login 
/// 	Done by username.TryToLogin (password, callback )
/// 
/// 5) Download their account Info 
/// 	Done by accountInfo.TryToDownload ( id, callback)
/// 
/// 6) Alter their account Info (change username, custom fields, ..)
/// 
/// 7) Upload the account Info back to the database
/// 	Done by accountInfo.TryToUpload (id, callback)
/// 
/// Below is a short script - under 100 lines of code - to show you how to do just that :)
/// 
/// TECHNICAL NOTE: 
/// Most of our functions communicate with the database server,
/// meaning they do not execute instantly. Instead of waiting for them to 
/// finish, we have implemented them as Co-Routines, so we can call them,
/// they execute on their own and our program flow continues uninterrupted.
/// 
/// But Co-Routines are IEnumerators, so they can not return a value,
/// nor take ref/out parameters. So in order to "extract" our userId,
/// or the account Info we just downloaded, we use a method called Callback.

/// That is, we  pass another function as a parameter, and we call it from 
/// within the Co-Routine when an error occurs or when it has successfully executed
/// 
/// </summary>
public class AS_ShortDemo : MonoBehaviour
{

    // Get these from the user
    public string username = "Mr_Wanna_Be_Awesome", password = "secretPassword";
    public string newUsername = "Mr_Awesome";

    // Stores the account id
    public int accountId = -1;
    int randomNum;

    // Stores the registration field names and weather they are required or not
    public AS_AccountInfo accountInfo = new AS_AccountInfo();

    void Start()
    {

        // This simulates different user inputs
        randomNum = UnityEngine.Random.Range(1000, 9999);
        username += "_" + randomNum.ToString();
        newUsername += "_" + randomNum.ToString();

        // 0) Download the registration Form as "accountInfo" - this is done by calling DownloadedRegistrationForm when the download is done..!
        accountInfo.TryToDownloadRegistrationForm(RegistrationFormDownloaded);

    }

    // Used by the Register when it's finished executing
    void RegistrationFormDownloaded(string callbackMessage)
    {

        // Check for Errors
        if (callbackMessage.IsAnError())
        {
            this.Log( LogType.Error, callbackMessage);
            return;
        }

        // 1) Add the user's input to the accountInfo fields
        foreach (AS_MySQLField field in accountInfo.fields)
        {

            // At this step you would normally prompt your user for input via the GUI)
            accountInfo.SetFieldValue(field.name, "somethingFromOurUsers@_" + randomNum.ToString());
        }

        // We will also use the provided username and password
        accountInfo.SetFieldValue("username", username);
        accountInfo.SetFieldValue("password", password);

        // 2) Attempt to Register
        accountInfo.TryToRegister(RegistrationAttempted);

    }

    // Use this as a callback when the login attempt is finished
    void RegistrationAttempted(string message)
    {

        // Check for Errors
        if (message.IsAnError())
        {
            this.Log( LogType.Error, message);
            return;
        }

        // Visualize the Registered account
        this.Log(LogType.Log, "Attempting to Register the following account info:\n" + accountInfo.ToReadableString());

        // 3) Prompt the user to input their username and password
        // (Here we will use the provided username and password, normally you would prompt your user for input via the GUI)

        /// 4) Attempt to Login
        username.TryToLogin(password, LoginAttempted);

    }

    // Use this as a callback when the login attempt is finished
    void LoginAttempted(string message)
    {

        // Check for Errors
        if (message.IsAnError())
        {
            this.Log( LogType.Error, message);
            return;
        }

        // Try to get the accountID
        accountId = System.Convert.ToInt32(message);

        // 5) Download their account Info
        accountInfo.TryToDownload(accountId, OnDownload);

    }

    // This is called when the download has finished
    void OnDownload(string message)
    {

        this.Log(LogType.Log, "Downloaded Successfully the following account info:\n" + accountInfo.ToReadableString());

        // 6) Alter their account Info (change username, custom fields, ..)
        accountInfo.fields.SetFieldValue("username", newUsername);

        // 7) Upload the account Info back to the database
        accountInfo.TryToUpload(accountId, OnUpload);

    }

    // This is called when the upload has finished
    void OnUpload(string message)
    {

        this.Log(LogType.Log, "Uploaded Successfully the following account info:\n" + accountInfo.ToReadableString());

    }
}