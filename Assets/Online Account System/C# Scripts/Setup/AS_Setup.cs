#if UNITY_EDITOR
using UnityEditor;
using UnityEngine; 
using System.Collections.Generic;
using System.Reflection;

public static class AS_Setup {

	public static bool ValueCheck ( string key, string val, float width )
	{
		if (val == ""){
			//Log(LogType.Log, key + " can not be empty!");
			GUILayout.Label (key + " can not be empty!", EditorStyles.boldLabel, GUILayout.Width(width));
			return false;
		}
		
		return true;
	}
	public static bool ValueCheck ( string key, int idx, AS_MySQLField[] fields, float width )
	{
		if (!ValueCheck ( key, fields[idx].name, width))
			return false;
		
		if (fields [idx].name.ToLower () == "id" ||
		    fields [idx].name.ToLower () == "username" ||
		    fields [idx].name.ToLower () == "password" ||
		    fields [idx].name.ToLower () == "email" ||
		    fields [idx].name.ToLower () == "isactive" ||
		    fields [idx].name.ToLower () == "custominfo") {
			GUILayout.Label ("Reserved name..!", EditorStyles.boldLabel, GUILayout.Width (width));
			return false;
		}
		
		
		foreach (AS_MySQLField field in fields) {
			if (fields [idx] == field)
				continue;
			if (fields [idx].name.ToLower () == field.name.ToLower ()) {
				GUILayout.Label ("Name is duplicate..!", EditorStyles.boldLabel, GUILayout.Width (width));
				return false;
			}
		}
		
		return true;
	}

	const float timeOut = 30.0f;
	
	const string createDBPhp = "/createdatabase.php?";
	public static string InitializeDatabase (AS_MySQLField[] additionalFields = null ) {


		float timeLeft = timeOut;
		
		string msg = "";
		
		Log(LogType.Log, "Initializing Database..");
				
		if (AS_Credentials.phpScriptsLocation == ""){
			
			Log( LogType.Error, "Host URL not set..! Please load the Account System Setup window!");
			return "Host URL not set..! Please load the Setup scene located in ../AccountSystem/Setup/";
		}
		
		// Location of the initialization script
		string url = AS_Credentials.phpScriptsLocation + createDBPhp;
		
		// Create a new form
		WWWForm form = new WWWForm();

		// Pass the AS_Preferences
		form.AddPreferences();

		List<AS_MySQLField> fields = new List<AS_MySQLField> ();

		// Add the username and password fields
		fields.Add (new AS_MySQLField ("username", AS_MySQLFieldType.Varchar, true, true, "Unique - Used for loging in"));
		fields.Add (new AS_MySQLField ("password", AS_MySQLFieldType.Varchar, false, true, "This is a hashed password, should be case invariant"));

		// Check if we should add an email
		if (AS_Preferences.askUserForEmail) {

			bool emailMustBeUnique = false;
			bool emailIsRequired = false;

			if (AS_Preferences.enablePasswordRecovery | AS_Preferences.requireEmailActivation){
				emailMustBeUnique = true;
				emailIsRequired = true;
			}

			fields.Add( new AS_MySQLField("email", AS_MySQLFieldType.Varchar, emailMustBeUnique, emailIsRequired) );

			// Add the field required for account activation
			if (AS_Preferences.requireEmailActivation)
				fields.Add( new AS_MySQLField("isactive", AS_MySQLFieldType.Bool, false, true) );

		}

		// Add any extra fields
		if (additionalFields != null)
			fields.AddRange(additionalFields);

		
		// Stores XML data (potentially needs a lot of space, TEXT will do)
		fields.Add( new AS_MySQLField("custominfo", AS_MySQLFieldType.LongText, false, false, 
		                              "This is used by [Upload|Download]InfoToDb (C#) to store the XML representation of custom serializable classes.") );

		// Add the fields to the form
		msg = form.AddMySQLFields ("fields", fields.ToArray());

		if (msg.ToLower().Contains("error"))
			return "Make sure all field names have values";

		// Connect to the script
		WWW www = new WWW(url, form);
		
		// Wait for it to respond
		Log(LogType.Log, "Connecting to " + url);
		
		while (!www.isDone && timeLeft > 0) {
			Log(LogType.Log, "Time Left " + timeLeft.ToString("#.00"));
			timeLeft -= Time.fixedDeltaTime; 
			
			if(timeLeft / timeOut > 0)
				EditorUtility.DisplayProgressBar(
					"Connecting to " + url + "..",
					"Timing out in " + timeLeft.ToString("#.") + " seconds",
					timeLeft / timeOut);
			else
				EditorUtility.ClearProgressBar();
		}
		
		if (timeLeft <= 0 )			
			return "Connection timed out!\nCheck your credentials, internet connection and firewall and try again.";
		
		
		EditorUtility.ClearProgressBar();
		
		// Check for errors
		if (www.error != null && www.error != ""){
			Log( LogType.Error, "WWW Error:\n" + www.error);
			if (www.error.ToLower().Contains("404"))
				return "Could not find CREATEDATABASE.PHP at " + AS_Credentials.phpScriptsLocation.ToUpper() +"\nHave you uploaded all the scripts from the folder 'Server-Side Scripts'?"+ "\nCheck Log for more info.";
			
			return "WWW Error\nCheck Log for more info";
		}
		if (www.text.ToLower().Contains("error")){
			Log( LogType.Error, "PHP/MySQL Error:\n" + www.text);
			if (www.text.ToLower().Contains("cannot connect"))
				return "Could not connect to the database.\nPlease check your credentials.\nCheck Log for more info.";

			if (www.text.ToLower().Contains("404"))
				return "Could not find CREATEDATABASE.PHP at " + AS_Credentials.phpScriptsLocation.ToUpper() +"\nHave you uploaded all the scripts from the folder 'Server-Side Scripts'?"+ "\nCheck Log for more info.";

			return "PHP/MySQL Error\nCheck Log for more info";
		}
		
		if (www.text.ToLower().Contains("success") ){
			Log(LogType.Log, "Database Initialized successfully! You can now load the Demo scene!\n" + www.text);
			return "";
		}
		else{
			Log(LogType.Warning, "Database could not be Initialized - Check Message:\n" + www.text);
			return "Database could not be Initialized- check Log for more info";;
		}
		
	}




	const string setupPhp = "/setup.php?";
	// "" for success, otherwise error message
	public static string Setup ( string phpScriptsLocation ){

		Log(LogType.Log, "Setting up PHP scripts..");

		if (phpScriptsLocation == ""){
			Log( LogType.Error, "Php Script Location not set..!");
			return "Php Script Location not set..!";
		}
		
		// Location of the setup script
		string url = phpScriptsLocation + setupPhp;
		
		// Create a new form
		WWWForm form = new WWWForm();

		// Add the credentials
		form.AddCredentials ();
				
		// Connect to the script
		WWW www = new WWW(url, form);
		
		// Wait for it to respond
		Log(LogType.Log, "Connecting to " + url);
		
		float timeLeft = timeOut;

		while (!www.isDone && timeLeft > 0) {
			Log(LogType.Log, "Time Left " + timeLeft.ToString("#.00"));
			timeLeft -= Time.fixedDeltaTime; 
			
			if(timeLeft / timeOut > 0)
				EditorUtility.DisplayProgressBar(
					"Connecting to " + url + "..",
					"Timing out in " + timeLeft.ToString("#.") + " seconds",
					timeLeft / timeOut);
			else
				EditorUtility.ClearProgressBar();

		}
		
		if (timeLeft <= 0 ){
			
			return "Connection timed out!\nCheck your credentials, internet connection and firewall and try again.";
		}
		
		EditorUtility.ClearProgressBar();		

		// Check for errors		
		if (www.error != null && www.error != ""){
			Log( LogType.Error, "WWW Error:\n" + www.error);
			if (www.error.ToLower().Contains("could not resolve"))
				return "Could not access the provided PHP Script Location.\nPlease check your credentials."+ "\nCheck Log for more info.";
			if (www.error.ToLower().Contains("404"))
				return "Could not find SETUP.PHP at " + AS_Credentials.phpScriptsLocation.ToUpper() +"\nHave you uploaded all the scripts from the folder 'Server-Side Scripts'?"+ "\nCheck Log for more info.";

			return "WWW Error: \n" + www.error + "\nCheck Log for more info.";
		}
		if (www.text.ToLower().Contains("error")){
			Log( LogType.Error, "PHP/MySQL Error:\n" + www.text);
			if (www.text.ToLower().Contains("could not locate file"))
				return "Could not find one of the PHP files at " + AS_Credentials.phpScriptsLocation.ToUpper() +"\nHave you uploaded all the scripts from the folder 'Server-Side Scripts'?"+ "\nCheck Log for more info.";

			return "PHP/MySQL Error - Check Log for more info";
		}
		
		if (www.text.ToLower().Contains("success") ){
			Log(LogType.Log, "PHP Scripts were setup successfully - You can now Initialize the Database!!\n" + www.text);
			return "";
		}
		else{
			Log(LogType.Warning, "PHP Scripts could not be set up - Check Message:\n" + www.text);
			return "PHP Scripts could not be set up - Check Log for more info";
		}		
		
	} 
    static void Log(LogType logType, string msg) { AS_Methods.Log(MethodBase.GetCurrentMethod().DeclaringType.Name, logType, msg); }
}
#endif