using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
public static class AS_Login
{


    const string loginPhp = "/checklogin.php?";
    const string getRegFormPhP = "/getregistrationform.php?";
    const string registerPhp = "/register.php?";
    const string passResetPhp = "/requestpasswordreset.php?";

    /// <summary>
    /// Downloads the registration form.
    /// </summary>
    /// <returns>The registration form.</returns>
    /// <param name="credentials">Credentials.</param>
    /// <param name="resultCallback">Result callback.</param>
    public static IEnumerator TryToDownloadRegistrationForm(AS_AccountInfo registrationForm, Action<string> resultCallback, string hostUrl = null)
    {
        if (hostUrl == null)
            hostUrl = AS_Credentials.phpScriptsLocation;

        if (hostUrl == "")
        {
            Log( LogType.Error, "Host URL not set..! Please load the Account System Setup window.");
            yield break;
        }

        // Location of the registration script
        string url = hostUrl + getRegFormPhP;
		
		
		url += "&requiredForMobile=" + UnityEngine.Random.Range(0, int.MaxValue).ToString();
	
		
		
        // Connect to the script
        WWW www = new WWW(url);

        // Wait for it to respond
        yield return www;
				

        // Check for WWW Errors
        if (www.error != null && www.error != "")
        {
            Log( LogType.Error, "WWW Error:\n" + www.error);
            resultCallback(www.error);
            yield break;
        }
        // Check for PHP / MySQL Errors
        if (www.text.ToLower().Contains("error"))
        {
            Log( LogType.Error, "PHP/MySQL Error:\n" + www.text);
            resultCallback(www.text.HandleError());
            yield break;
        }
		if (www.text.ToLower().Contains("warning"))
        {
            Log(LogType.Warning, "PHP/MySQL Warning:\n" + www.text);
            resultCallback(www.text.HandleError());
            yield break;
        }
		
		
		
        Log(LogType.Log, "Received serialized registration form\n" + www.text);

        AS_AccountInfo temp = www.text.ToAccountInfo();
        registrationForm.fields = temp.fields;
        resultCallback("Registration Form downloaded Successfully!");

        yield break;

    }

    /// <summary>
    /// Retrieves the password.
    /// </summary>
    /// <returns>The password.</returns>
    /// <param name="credentials">Credentials.</param>
    /// <param name="resultCallback">Result callback.</param>
    public static IEnumerator TryToRecoverPassword(string email, Action<string> resultCallback, string hostUrl = null)
    {
        if (hostUrl == null)
            hostUrl = AS_Credentials.phpScriptsLocation;

        if (hostUrl == "")
        {
            Log( LogType.Error, "Host URL not set..! Please load the Account System Setup window.");
            yield break;
        }
        // Location of the registration script
        string url = hostUrl + passResetPhp;
		
						
		url += "&requiredForMobile=" + UnityEngine.Random.Range(0, int.MaxValue).ToString();
		
		
        // Create a new form
        WWWForm form = new WWWForm();

        // Add The required fields
        form.AddField("email", WWW.EscapeURL(email).Replace("@", "%40").Replace("+", "%2b"));
        // Connect to the script
        WWW www = new WWW(url, form);

        // Wait for it to respond
        yield return www;

        if (www.error != null && www.error != "")
        {
            Log( LogType.Error, "WWW Error:\n" + www.error);
            resultCallback(www.error);
            yield break;
        }
        if (www.text.ToLower().Contains("error"))
        {
            Log( LogType.Error, "PHP/MySQL Error:\n" + www.text);
            resultCallback(www.text.HandleError());
            yield break;
        }

        if (www.text.ToLower().Contains("success"))
        {
            Log(LogType.Log, "Emailed password reset link!\n" + www.text);
            resultCallback("Emailed password reset link!");
        }
        else
        {
            Log(LogType.Warning, "Could not email password reset link - Check Message:\n" + www.text);
            resultCallback("Error: Could not email password reset link");
        }


        yield break;

    }


    /// <summary>
    /// Checks the registration fields.
    /// </summary>
    /// <returns><c>true</c>, if all fields cleared the check, <c>false</c> otherwise.</returns>
    /// <param name="regInfo">Reg info.</param>
    /// <param name="passwordConfirm">password confirm.</param>
    /// <param name="emailConfirm">Email confirm.</param>
    /// <param name="registrationMessage"> Success / Error message message.</param>
	public static bool CheckFields(AS_AccountInfo regInfo, string passwordConfirm, string emailConfirm, ref string errorMessage)
    {

        errorMessage = "";

		// Validate the data in the fields (make sure it matches their type
		if ( !regInfo.fields.CheckMySQLFields (ref errorMessage) )
			return false;

        if (regInfo.GetFieldValue("password") == null || 
		    (AS_Preferences.askUserForEmail && regInfo.GetFieldValue("email") == null))
        {
            errorMessage = "Account info not set up correctly..! Missing fields..!";
            return false;
        }

        // Password must match
        if (regInfo.GetFieldValue ("password") != passwordConfirm) {
			errorMessage = "Passwords must match..!";
			return false;
		}
		// If an email has been entered
		else if (AS_Preferences.askUserForEmail && regInfo.GetFieldValue ("email") != "") {
			// It must be valid
			if (!new Regex (@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
			                     + "@"
			                     + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$").Match (regInfo.GetFieldValue ("email")).Success) {
				errorMessage = "Invalid email..!";
				return false;
			} 
			// And match its confirm
			if ( regInfo.GetFieldValue ("email") != emailConfirm)
			{ 
				errorMessage = "Emails must match..!";
				return false;
			} 
		}

		// All good..!
		return true;
	}
	
	
    /// <summary>
    /// Attempt to register to our database. When we are done with that attempt, return something meaningful or an error message.
    /// </summary>
    public static IEnumerator TryToRegister(AS_AccountInfo newAccountInfo, Action<string> resultCallback, string hostUrl = null)
    {
        if (hostUrl == null)
            hostUrl = AS_Credentials.phpScriptsLocation;

        if (hostUrl == "")
        {
            Log( LogType.Error, "Host URL not set..! Please load the Account System Setup window.");
            yield break;
        }

        // Location of the registration script
        string url = hostUrl + registerPhp;
		
						
		url += "&requiredForMobile=" + UnityEngine.Random.Range(0, int.MaxValue).ToString();
		
        // Create a new form
        WWWForm form = new WWWForm();

        // Hash the password
        string hashedPassword = newAccountInfo.fields.GetFieldValue("password").Hash();
        newAccountInfo.fields.SetFieldValue("password", hashedPassword);

        // If there should be an account activation, make sure we require it
        bool requireEmailActivation = (newAccountInfo.fields.GetIndex("isactive") >= 0);
        if (requireEmailActivation)
            newAccountInfo.fields.SetFieldValue("isactive", "FALSE");

		// Serialize the custom info field
		newAccountInfo.SerializeCustomInfo ();

        // Serialize the account info
        string serializedAccountInfo = newAccountInfo.AccInfoToString(false);

        if (serializedAccountInfo == null)
        {
            Log( LogType.Error, "Could not serialize account info - check previous Log for errors");
            yield break;
        }
		
		form.AddField("newAccountInfo", serializedAccountInfo);

        form.AddField("requireEmailActivation", requireEmailActivation ? "true" : "false");

        // Connect to the script
        WWW www = new WWW(url, form);

        // Wait for it to respond
        yield return www;

        if (www.error != null && www.error != "")
        {
            Log( LogType.Error, "WWW Error:\n" + www.error);
            resultCallback("Error: Could not connect. Please try again later!");
            yield break;
        }
        if (www.text.ToLower().Contains("error"))
        {
            Log( LogType.Error, "PHP/MySQL Error:\n" + www.text);
            resultCallback(www.text.HandleError());
            yield break;
        }

        if (www.text.ToLower().Contains("success"))
        {
            Log(LogType.Log, "New account registered successfully!\n" + www.text);
            resultCallback("New account registered successfully!");
        }
        else
        {
            Log( LogType.Error, "Could not register new account - Check Message:\n" + www.text);
            resultCallback("Error: Could not register new account");
        }


        yield break;


    }


    /// <summary>
    /// Attempt to login to our database. When we are done with that attempt, return something meaningful or an error message.
    /// </summary>
    public static IEnumerator TryToLogin(string username, string password, Action<string> resultCallback, string hostUrl = null)
    {
        if (hostUrl == null)
            hostUrl = AS_Credentials.phpScriptsLocation;

        if (hostUrl == "")
        {
            Log( LogType.Error, "Host URL not set..! Please load the Account System Setup window.");
            yield break;
        }

        // Location of our login script
        string url = hostUrl + loginPhp;
				
		url += "&requiredForMobile=" + UnityEngine.Random.Range(0, int.MaxValue).ToString();		
		
        // Create a new form
        WWWForm form = new WWWForm();

		// Add The required fields
        form.AddField("username", username);
        // Hash the password
        string hashedPassword = password.Hash();
        form.AddField("password", hashedPassword);

        // Connect to the script
        WWW www = new WWW(url, form);

        // Wait for it to respond
        yield return www;

        if (www.error != null && www.error != "")
        {
            Log( LogType.Error, "WWW Error:\n" + www.error);
            resultCallback("Error: Could not connect. Please try again later!");
            yield break;
        }
        if (www.text.ToLower().Contains("error"))
        {
            Log( LogType.Error, "PHP/MySQL Error:\n" + www.text);
            resultCallback(www.text.HandleError());
            yield break;
        }

        if (www.text == "-1")
        {
            Log(LogType.Warning, "Failed Login Attempt (Account Inactive)\n" + www.text);
            resultCallback("Error: Account is Inactive - Please check your emails.");
        }
        else if (www.text == "-2")
        {
            Log(LogType.Warning, "Failed Login Attempt (Invalid Username / Password)\n" + www.text);
            resultCallback("Error: Invalid Username / Password");
        }
        else
        {
			int id = -1;
			if (!int.TryParse(www.text, out id))
			{				
				resultCallback("Error: Could not connect. Please try again later!");
				Log( LogType.Error, "Failed Login Attempt (Unknown Error / Warning)\n" + www.text);
				yield break;
			}
			Log(LogType.Log, "Successful Login for user with ID: " + www.text);
            resultCallback(www.text);

        }

    }

    static void Log(LogType logType, string msg) { AS_Methods.Log(MethodBase.GetCurrentMethod().DeclaringType.Name, logType, msg); }
}