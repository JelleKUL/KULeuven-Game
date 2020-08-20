using UnityEngine; 
using System;
using System.IO;
using System.Text; 
using System.Xml.Serialization;
using System.Security.Cryptography; 
using System.Globalization;
using System.Reflection;


public class AS_CoroutineCaller : MonoBehaviour
{

    public static AS_CoroutineCaller Create()
    {
        GameObject temp = new GameObject("AS_CoroutineCaller");
        return temp.AddComponent<AS_CoroutineCaller>();
    }

}

/// <summary>
/// Account system helper methods.
/// </summary>
public static class AS_Methods
{

    public static void Destroy(this AS_CoroutineCaller caller)
    {
        UnityEngine.Object.Destroy(caller.gameObject);
    }

    public static string XmlSerializeToString(this object objectInstance)
    {
        var serializer = new XmlSerializer(objectInstance.GetType());
        var sb = new StringBuilder();

        using (TextWriter writer = new StringWriter(sb))
        {
            serializer.Serialize(writer, objectInstance);
        }

        return sb.ToString();
    }

    public static T XmlDeserializeFromString<T>(this string objectData)
    {
        return (T)XmlDeserializeFromString(objectData, typeof(T));
    }

    public static object XmlDeserializeFromString(this string objectData, Type type)
    {
        var serializer = new XmlSerializer(type);
        object result;

        using (TextReader reader = new StringReader(objectData))
        {
            result = serializer.Deserialize(reader);
        }

        return result;
    }

    public const string fieldsSeparator = "$#@(_fields_separator_*&%^";
    public const string fieldNameValueSeparator = "$#@(_field_name_value_separator_*&%^";

    // Name ~ Value ~ Type ~ Must Be Unique ~ Can Be Ommited
    public static AS_AccountInfo ToAccountInfo(this string _string)
    {
        // this.Log (_string);
        AS_AccountInfo accInfo = new AS_AccountInfo();
        string[] _fieldsSeparator = new string[] { fieldsSeparator };
        string[] _fieldNameValueSeparator = new string[] { fieldNameValueSeparator };
        string[] fields = _string.Split(_fieldsSeparator, StringSplitOptions.RemoveEmptyEntries);
        foreach (string field in fields)
        {

            string[] words = field.Split(_fieldNameValueSeparator, StringSplitOptions.None);
            string fieldName = words[0];
            string fieldValue = words[1];
            AS_MySQLFieldType fieldType = words[2].GetEnumType();
            bool mustBeUnique = (words[3].ToLower() == "true");
            bool isRequired = (words[4].ToLower() == "true");

            // this.Log(LogType.Log, "Name: " + fieldName + " ~ Value: " + fieldValue + " ~ Type: " + words[2] + " (" + fieldType + ") ~ Must Be Unique: " + mustBeUnique.ToString() + " ~ Can Be Ommited: " + canBeOmmited.ToString());
            accInfo.fields.Add(new AS_MySQLField(fieldName, fieldValue, fieldType, mustBeUnique, isRequired, ""));
        }

        return accInfo;
    }

    public static string AccInfoToString(this AS_AccountInfo accInfo, bool checkForOmmit)
    {
        string _string = "";
        for (int i = 0; i < accInfo.fields.Count; i++)
        {
            if (checkForOmmit && accInfo.fields[i].isRequired && accInfo.fields[i].stringValue == "")
            {
                Log(LogType.Error, "Failed to serialize AccountInfo - " + accInfo.fields[i].name + " field can not be empty!");
                return null;
            }
            _string += accInfo.fields[i].name + fieldNameValueSeparator + accInfo.fields[i].stringValue + fieldsSeparator;

        }
        if (accInfo.fields.Count > 0)
            _string.Remove(_string.Length - fieldsSeparator.Length);

        // this.Log (_string);
        return _string;
    }
    
    public static string Hash(this string password)
    {
        string hashedPassword = "";
#if NETFX_CORE || WINDOWS_APP
        ///Hash Algorithm Provider is Created 
        HashAlgorithmProvider Algorithm = HashAlgorithmProvider.OpenAlgorithm("SHA512");
        ///Creating a Buffer Stream using the Cryptographic Buffer class and UTF8 encoding 
        IBuffer vector = CryptographicBuffer.ConvertStringToBinary(password, BinaryStringEncoding.Utf8);
        IBuffer digest = Algorithm.HashData(vector);////Hashing The Data 
        hashedPassword = CryptographicBuffer.EncodeToHexString(digest);//Encoding it to a Hex String 
#else
        byte[] data = Encoding.ASCII.GetBytes(password);
        SHA512 sha512 = new SHA512Managed();
        byte[] hashValue = sha512.ComputeHash(data);
        foreach (byte hexdigit in hashValue)
        {
            hashedPassword += hexdigit.ToString("X2", CultureInfo.InvariantCulture.NumberFormat);
        }
#endif
        return hashedPassword;
    }

    public static WWWForm AddCredentials(this WWWForm form)
    {
        form.AddField("databaseHostname", WWW.EscapeURL(AS_Credentials.databaseHostname));
        form.AddField("databasePort", WWW.EscapeURL(AS_Credentials.databasePort));
        form.AddField("databaseUsername", WWW.EscapeURL(AS_Credentials.databaseUsername));
        form.AddField("databasePassword", WWW.EscapeURL(AS_Credentials.databasePassword));
        form.AddField("databaseDbName", WWW.EscapeURL(AS_Credentials.databaseDbName));
        form.AddField("phpScriptsLocation", WWW.EscapeURL(AS_Credentials.phpScriptsLocation));
        form.AddField("emailAccount", WWW.EscapeURL(AS_Credentials.emailAccount));
        return form;
    }
    public static WWWForm AddPreferences(this WWWForm form)
    {
        form.AddField("overrideExistingTables", WWW.EscapeURL(AS_Preferences.overrideExistingTables.ToString().ToLower()));
        form.AddField("askUserForEmail", WWW.EscapeURL(AS_Preferences.askUserForEmail.ToString().ToLower()));
        form.AddField("requireEmailActivation", WWW.EscapeURL(AS_Preferences.requireEmailActivation.ToString().ToLower()));
        form.AddField("enablePasswordRecovery", WWW.EscapeURL(AS_Preferences.enablePasswordRecovery.ToString().ToLower()));
        return form;
    }

    public static string UppercaseFirst(this string s)
    {
        // Check for empty string.
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }
        // Return char and concat substring.
        return char.ToUpper(s[0]) + s.Substring(1);
    } 

    static void Log(LogType logType, string msg) { AS_Methods.Log( MethodBase.GetCurrentMethod().DeclaringType.Name, logType, msg); }

    public static void Log(this UnityEngine.Object caller, LogType logType, string msg)
    { 
        Log(caller.GetType().ToString(), logType, msg);
    }

    public static void Log(string caller, LogType logType, string msg)
    {
#if UNITY_EDITOR
        if (logType == LogType.Log)
            Debug.Log(caller + ": " + msg);
        else if (logType == LogType.Warning)
            Debug.Log(caller + ": " + msg);
        else if (logType == LogType.Error || logType == LogType.Exception || logType == LogType.Assert)
            Debug.LogError(caller + ": " + msg);
#endif
    }
} 

public enum AS_LoginState
{
	Idle = 0, LoginPrompt, Registering, RecoverPassword, LoginSuccessful,
	NUM_STATES
};


/// <summary>
/// Link between PlayerPrefs and Front End
/// </summary>
[Serializable]
public class AS_Credentials
{

    public static void Save()
    {
        PlayerPrefs.Save();
    }

    public static string databaseHostname
    {
        get
        {
            if (!PlayerPrefs.HasKey("AS_DatabaseHostname"))
                PlayerPrefs.SetString("AS_DatabaseHostname", "");
            return PlayerPrefs.GetString("AS_DatabaseHostname");
        }
        set
        {
            PlayerPrefs.SetString("AS_DatabaseHostname", value);
        }
    }
    public static string databasePort
    {
        get
        {
            if (!PlayerPrefs.HasKey("AS_DatabasePort"))
                PlayerPrefs.SetString("AS_DatabasePort", "");
            return PlayerPrefs.GetString("AS_DatabasePort");
        }
        set
        {
            PlayerPrefs.SetString("AS_DatabasePort", value);
        }
    }
    public static string databaseUsername
    {
        get
        {
            if (!PlayerPrefs.HasKey("AS_DatabaseUsername"))
                PlayerPrefs.SetString("AS_DatabaseUsername", "");
            return PlayerPrefs.GetString("AS_DatabaseUsername");
        }
        set
        {
            PlayerPrefs.SetString("AS_DatabaseUsername", value);
        }
    }
    public static string databasePassword
    {
        get
        {
            if (!PlayerPrefs.HasKey("AS_DatabasePassword"))
                PlayerPrefs.SetString("AS_DatabasePassword", "");
            return PlayerPrefs.GetString("AS_DatabasePassword");
        }
        set
        {
            PlayerPrefs.SetString("AS_DatabasePassword", value);
        }
    }
    public static string databaseDbName
    {
        get
        {
            if (!PlayerPrefs.HasKey("AS_DatabaseDbName"))
                PlayerPrefs.SetString("AS_DatabaseDbName", "");
            return PlayerPrefs.GetString("AS_DatabaseDbName");
        }
        set
        {
            PlayerPrefs.SetString("AS_DatabaseDbName", value);
        }
    }

    public static string emailAccount
    {
        get
        {
            if (!PlayerPrefs.HasKey("AS_EmailAccount"))
                PlayerPrefs.SetString("AS_EmailAccount", "");
            return PlayerPrefs.GetString("AS_EmailAccount");
        }
        set
        {
            PlayerPrefs.SetString("AS_EmailAccount", value);
        }
    }
	const string _phpScriptsLocation = "http://";
    public static string phpScriptsLocation
    {
        get
        {
#if UNITY_EDITOR
				if (! PlayerPrefs.HasKey("AS_PhpScriptsLocation") )
					PlayerPrefs.SetString("AS_PhpScriptsLocation", 	"");
				return PlayerPrefs.GetString("AS_PhpScriptsLocation");
#else
            return _phpScriptsLocation;
#endif
        }
        set
        {
#if UNITY_EDITOR
				PlayerPrefs.SetString("AS_PhpScriptsLocation", 	value);
				if (value != phpScriptsLocation || value != _phpScriptsLocation)
					EditPhpScriptsLocation(value);
#endif
        }
    }

    // Use this for initialization
    static void EditPhpScriptsLocation(string newLoc)
    {

        Log(LogType.Log, "Replacing php script location in C# files..!");

        string fileName = Application.dataPath + "/Online Account System/C# Scripts/Helper/AS_Helper.cs";
        string[] lines = System.IO.File.ReadAllLines(fileName);
        for (int i = 0; i < lines.Length; i++)
        {
            // Make sure we don't target ourselves
            if (!lines[i].Contains("const string " + "_phpScriptsLocation"))
                continue;
            lines[i] = "\tconst string " + "_phpScriptsLocation = \"" + newLoc + "\";";

            Log(LogType.Log, "Replaced at line " + i.ToString() + " of file " + fileName + "\n" + lines[i]); 
        }
#if !UNITY_WEBPLAYER
        System.IO.File.WriteAllLines(fileName, lines);
#else
		Log( LogType.Error, "Can Not Properly setup from WebPlayer settings!!");
		Log( LogType.Error, "Please select Standalone Build Options!!");
#endif
    }

    // Put in all classes
    static void Log(LogType logType, string msg) { AS_Methods.Log(MethodBase.GetCurrentMethod().DeclaringType.Name, logType, msg); } 

}

[Serializable]
public class AS_Preferences
{

    public static bool overrideExistingTables
    {
        get
        {
            if (!PlayerPrefs.HasKey("AS_OverrideExistingTables"))
                PlayerPrefs.SetInt("AS_OverrideExistingTables", 0);
            return (PlayerPrefs.GetInt("AS_OverrideExistingTables") != 0);
        }
        set
        {
            PlayerPrefs.SetInt("AS_OverrideExistingTables", value ? 1 : 0);
        }
    }

    public static bool askUserForEmail
    {
        get
        {
            if (!PlayerPrefs.HasKey("AS_AskUserForEmail"))
                PlayerPrefs.SetInt("AS_AskUserForEmail", 0);
            return (PlayerPrefs.GetInt("AS_AskUserForEmail") != 0);
        }
        set
        {
            PlayerPrefs.SetInt("AS_AskUserForEmail", value ? 1 : 0);
            if (value == false)
            {
                enablePasswordRecovery = false;
                requireEmailActivation = false;
            }
        }
    }

    public static bool requireEmailActivation
    {
        get
        {
            if (!PlayerPrefs.HasKey("AS_RequireEmailActivation"))
                PlayerPrefs.SetInt("AS_RequireEmailActivation", 0);
            return (PlayerPrefs.GetInt("AS_RequireEmailActivation") != 0);
        }
        set
        {
            PlayerPrefs.SetInt("AS_RequireEmailActivation", value ? 1 : 0);
            if (value == true)
                askUserForEmail = true;
        }
    }

	const bool _enablePasswordRecovery = false;
    public static bool enablePasswordRecovery
    {
        get
        {
            // If we are in edit mode
#if UNITY_EDITOR
				if (! PlayerPrefs.HasKey("AS_EnablePasswordRecovery") )
					PlayerPrefs.SetInt("AS_EnablePasswordRecovery", 	0);
				return (PlayerPrefs.GetInt("AS_EnablePasswordRecovery"	) != 0);	
#else
            // If we are in play mode						
            return _enablePasswordRecovery;
#endif

        }
        set
        {
            // If we are in edit mode
#if UNITY_EDITOR
				if ( value == true)
					askUserForEmail = true;
				if (value != enablePasswordRecovery)
					EditEnablePasswordRecovery(value);
				PlayerPrefs.SetInt("AS_EnablePasswordRecovery", 	value?1:0);
#endif

        }
    }

    // Use this for initialization
    static void EditEnablePasswordRecovery(bool newVal)
    {

        string stringVal = newVal ? "true" : "false";

        Log(LogType.Log, "Replacing EnablePasswordRecovery in C# files..!");

        string fileName = Application.dataPath + "/Online Account System/C# Scripts/Helper/AS_Helper.cs";
        string[] lines = System.IO.File.ReadAllLines(fileName);
        for (int i = 0; i < lines.Length; i++)
        {
            // Make sure we don't target ourselves
            if (!lines[i].Contains("const bool " + "_enablePasswordRecovery"))
                continue;
            lines[i] = "\tconst bool " + "_enablePasswordRecovery = " + stringVal + ";";

            Log(LogType.Log, "Replaced at line " + i.ToString() + " of file " + fileName+"\n"+lines[i]); 
        }
#if !UNITY_WEBPLAYER
        System.IO.File.WriteAllLines(fileName, lines);
#else
		Log( LogType.Error, "Can Not Properly setup from WebPlayer settings!!");
		Log( LogType.Error, "Please select Standalone Build Options!!");
#endif
    }
    public static void Save()
    {

        PlayerPrefs.Save();

        Log(LogType.Log, "Preferences saved successfully!");
    }
    static void Log(LogType logType, string msg) { AS_Methods.Log(MethodBase.GetCurrentMethod().DeclaringType.Name, logType, msg); }
}
