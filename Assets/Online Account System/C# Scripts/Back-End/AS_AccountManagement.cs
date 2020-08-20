using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

public static class AS_AccountManagement
{


    public static void SetCustomInfo<T>(this AS_AccountInfo accountInfo, T customInfo, string customInfoFieldName = "custominfo")
    {
        // Serialize the custom info
        string serializedCustomInfo = customInfo.XmlSerializeToString();

        // Find the custom info field and update its value
        if (!accountInfo.fields.SetFieldValue(customInfoFieldName, serializedCustomInfo))
        {
            Log( LogType.Error, "Could not find custom info field " + customInfoFieldName);
            return;
        }

        // Log ("Custom info field " + customInfoFieldName + " updated with new info (of type " + typeof(T).FullName + ")");

    }

    public static T GetCustomInfo<T>(this AS_AccountInfo accountInfo, string customInfoFieldName = "custominfo") where T : class, new()
    {
        // Find the custom info field
        int customInfoFieldIndex = accountInfo.fields.GetIndex(customInfoFieldName);

        if (customInfoFieldIndex < 0)
        {
            Log( LogType.Error, "Could not find custom info field " + customInfoFieldName);
            return new T();
        }

        // Get its value
        string customInfoXMLValue = accountInfo.fields[customInfoFieldIndex].stringValue;

        // If it's never set
        if (customInfoXMLValue == "")
        {
            Log(LogType.Warning, "Custom info field " + customInfoFieldName + " hasn't been set.");
            return new T();
        }

        // Attempt to deserialize the text we got
        T customInfo = customInfoXMLValue.XmlDeserializeFromString<T>();
        // Log ("Read info (of type " + typeof(T).FullName + ") from custom info field " + customInfoFieldName );
        return customInfo;

    }

    const string setAccountInfoPhp = "/setaccountinfo.php?";
    /// <summary>
    /// Attempt to login to our database. When we are done with that attempt, return something meaningful or an error message.
    /// </summary>
    public static IEnumerator TryToUploadAccountInfoToDb(int id, AS_AccountInfo accountInfo, System.Action<string> callback, string phpScriptsLocation = null)
    {

        if (phpScriptsLocation == null)
            phpScriptsLocation = AS_Credentials.phpScriptsLocation;

        if (phpScriptsLocation == "")
        {
            Log( LogType.Error, "PHP Scripts Location not set..! Please load the Setup scene located in ../AccountSystem/Setup/");
            yield break;
        }

        Log(LogType.Log, "Uploading Account Info for user with id " + id);


        // Location of our download info script
        string url = phpScriptsLocation + setAccountInfoPhp;
						
		
		url += "&requiredForMobile=" + UnityEngine.Random.Range(0, int.MaxValue).ToString();
		
		
        // Create a new form
        WWWForm form = new WWWForm();

        // Add The required fields
        form.AddField("id", WWW.EscapeURL(id.ToString()));

		// Serialize the customInfo field
		if(!accountInfo.SerializeCustomInfo())
		{
			Log( LogType.Error, "Could not serialize custom info - check previous Log for errors");
			yield break;

		}
        string serializedAccountInfo = accountInfo.AccInfoToString(true);

        if (serializedAccountInfo == null)
        {
            Log( LogType.Error, "Could not serialize account info - check previous Log for errors");
            yield break;
        }

        form.AddField("info", serializedAccountInfo);

		
        // Connect to the script
        WWW www = new WWW(url, form);

        // Wait for it to respond
        Log(LogType.Log, "Awaiting response from: " + url);
        yield return www;

        if (www.error != null && www.error != "")
        {
            Log( LogType.Error, "WWW Error:\n" + www.error);
            if (callback != null)
                callback("error: " + www.error);
            yield break;
        }
        if (www.text.ToLower().Contains("error"))
        {
            Log( LogType.Error, "PHP/MySQL Error:\n" + www.text);
            if (callback != null)
                callback("error: " + www.text);
            yield break;
        }

        if (www.text.ToLower().Contains("success"))
        {
            Log(LogType.Log, "Account Info uploaded successfully for user with id " + id);
            if (callback != null)
                callback("Account Info uploaded successfully for user with id " + id);
            yield break;
        }
    }


    const string getAccountInfoPhp = "/getaccountinfo.php?";
    /// <summary>
    /// Attempt to login to our database. When we are done with that attempt, return something meaningful or an error message.
    /// </summary>
    public static IEnumerator TryToDownloadAccountInfoFromDb(int id, AS_AccountInfo accountInfo, System.Action<string> callback, string phpScriptsLocation = null)
    {

        if (phpScriptsLocation == null)
            phpScriptsLocation = AS_Credentials.phpScriptsLocation;

        if (phpScriptsLocation == "")
        {
            Log( LogType.Error, "PHP Scripts Location not set..! Please load the Setup scene located in ../AccountSystem/Setup/");

            if (callback != null)
                callback("error: PHP Scripts Location unknown");
            yield break;
        }

        Log(LogType.Log, "Downloading Account Info for user with id " + id);


        // Location of our download info script
        string url = phpScriptsLocation + getAccountInfoPhp;
				
		url += "&requiredForMobile=" + UnityEngine.Random.Range(0, int.MaxValue).ToString();
		
        // Create a new form
        WWWForm form = new WWWForm();

        // Add The required fields
        form.AddField("id", WWW.EscapeURL(id.ToString()));

        // Connect to the script
        WWW www = new WWW(url, form);

        // Wait for it to respond
        Log(LogType.Log, "Awaiting response from: " + url);
        yield return www;

        if (www.error != null && www.error != "")
        {
            Log( LogType.Error, "WWW Error:\n" + www.error);
            if (callback != null)
                callback("error: " + www.text);
            yield break;
        }
        if (www.text.ToLower().Contains("error"))
        {
            Log( LogType.Error, "PHP/MySQL Error:\n" + www.text);
            if (callback != null)
                callback("error: " + www.text);
            yield break;
        }

        //Log(LogType.Log, www.text);

        // Attempt to deserialize the text we got
        AS_AccountInfo temp = www.text.ToAccountInfo();
        accountInfo.fields = temp.fields;
		if (!accountInfo.DeSerializeCustomInfo ())
			Log(LogType.Warning, "Could not deserialize Custom Info");

        if (callback != null)
            callback("Account Info downloaded successfully for user with id " + id);

    }

    const string getSimilarAccountsPhp = "/getsimilaraccounts.php?";
    /// <summary>
    /// Attempt to login to our database. When we are done with that attempt, return something meaningful or an error message.
    /// </summary>
    public static IEnumerator TryToGetSimilarAccounts (AS_AccountInfo accountInfo, string fieldNameToCheck,
        string fieldNameToReturn, bool includeSelf, List<string> similarAccountsInfo, 
        System.Action<string> callback, string phpScriptsLocation = null)
    {

        if (phpScriptsLocation == null)
            phpScriptsLocation = AS_Credentials.phpScriptsLocation;

        if (phpScriptsLocation == "")
        {
            Log(LogType.Error, "PHP Scripts Location not set..! Please load the Setup scene located in ../AccountSystem/Setup/");
            yield break;
        }

        if (accountInfo.GetFieldValue(fieldNameToCheck) == null || 
            accountInfo.GetFieldValue(fieldNameToReturn) == null)
        {
            Log(LogType.Error, "Invalid fields!! Make sure you enter valid field names!!");
            yield break;
        }

        string fieldValueToMatch = accountInfo.GetFieldValue(fieldNameToCheck);

        if (fieldValueToMatch == "")
        {
            Log(LogType.Error, "Empty Field '" + fieldNameToCheck + "' of account " + accountInfo.ToReadableString());
            yield break;
        }

        Log(LogType.Log, "Downloading Accounts with same value in " + fieldNameToCheck + " as user with id " + accountInfo.GetFieldValue("id"));


        // Location of our download info script
        string url = phpScriptsLocation + getSimilarAccountsPhp;

        url += "&requiredForMobile=" + UnityEngine.Random.Range(0, int.MaxValue).ToString();

        // Create a new form
        WWWForm form = new WWWForm();

        // Add The required fields
        form.AddField("fieldNameToCheck", WWW.EscapeURL(fieldNameToCheck));
        form.AddField("fieldValueToMatch", WWW.EscapeURL(fieldValueToMatch));
        form.AddField("fieldNameToReturn", WWW.EscapeURL(fieldNameToReturn));

        // Connect to the script
        WWW www = new WWW(url, form);

        // Wait for it to respond
        Log(LogType.Log, "Awaiting response from: " + url);
        yield return www;

        if (www.error != null && www.error != "")
        {
            Log(LogType.Error, "WWW Error:\n" + www.error);
            if(callback != null)
                callback("error: " + www.text);
            yield break;
        }
        if (www.text.ToLower().Contains("error"))
        {
            Log(LogType.Error, "PHP/MySQL Error:\n" + www.text);
            if (callback != null)
                callback("error: " + www.text);
            yield break;
        }

        //Log(LogType.Log, www.text);

        // Attempt to deserialize the text we got
        string[] similarAccountsInfo_Temp = www.text.Split(new string[] { AS_Methods.fieldsSeparator }, StringSplitOptions.RemoveEmptyEntries);

        similarAccountsInfo.Clear();

        for (int i = 0; i < similarAccountsInfo_Temp.Length; i++ )
        {
            // Is it us?
            // To check, we must call this function with the "id" argument as fieldNameToReturn
            // if (!includeSelf && similarAccountIDs_Temp[i].CompareTo(accountInfo.GetFieldValue("id")) == 0)
            //     continue;
            
            similarAccountsInfo.Add(similarAccountsInfo_Temp[i]);
        }

        if (callback != null)
            callback("Successfully downloaded Accounts with same value in " + fieldNameToCheck + " as user with id " + accountInfo.GetFieldValue("id"));
        
    }



    static void Log(LogType logType, string msg) { AS_Methods.Log(MethodBase.GetCurrentMethod().DeclaringType.Name, logType, msg); }
}