#if UNITY_EDITOR
using UnityEditor;
using UnityEngine; 
using System.Collections.Generic;


public enum AS_SetupState {CredentialsAndPreferences, Fields, Done}
public class AS_SetupWindow : EditorWindow
{ 
	// Add menu item named "My Window" to the Window menu
	[MenuItem("Window/Online Account System")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		AS_SetupWindow window = (AS_SetupWindow)GetWindow(typeof(AS_SetupWindow));
		
		window.minSize = new Vector2 (620, 500);
		window.name = "Setup";
#if UNITY_4 || UNITY_5
        window.titleContent.text = "Setup";
#else
		window.title = "Setup";
#endif

		window.position = new Rect (400,	200,  	window.minSize.x, window.minSize.y);           
			
	}

	private List<AS_MySQLField> additionalFields = new List<AS_MySQLField>();

	void Awake() 
	{ 
		additionalFields.Add (new AS_MySQLField ("Age", AS_MySQLFieldType.Int, false, true, "The user's Age."));
		additionalFields.Add (new AS_MySQLField ("Clan", AS_MySQLFieldType.Varchar, false, false, "The user's Clan."));
	}

    private void OnEnable()
    {
        _phpScriptsLocation = AS_Credentials.phpScriptsLocation;
    }

    private string 	initializeDbMessage = "";
	private Vector2 lastPos = Vector2.zero; 
	private bool 	fieldsOk = true;
	private bool 	credentialsOk = true;


	public AS_SetupState state = AS_SetupState.CredentialsAndPreferences;

	void OnGUI()
	{
		
		GUILayout.BeginArea (new Rect (10, 10, 600, 500));

#if !UNITY_WEBPLAYER
		
		if (state == AS_SetupState.CredentialsAndPreferences)
			CredentialsGUI ();
		else if (state == AS_SetupState.Fields)
			FieldsGUI ();

#else
			AS_WrongBuildTargetWindow.ShowWindow(this);
#endif

		GUILayout.EndArea ();
	}

    string _phpScriptsLocation;

	void CredentialsGUI () {

		credentialsOk = true;
		
		EditorGUILayout.BeginVertical ();
		
		GUILayout.Label ("Credentials", EditorStyles.boldLabel);
		
		_phpScriptsLocation = EditorGUILayout.TextField ("PHP Scripts Location:", 	_phpScriptsLocation, GUILayout.Width(400) );	
		
		AS_Credentials.databaseHostname 	= EditorGUILayout.TextField ("Database Hostname:", 		AS_Credentials.databaseHostname, GUILayout.Width(400));

		AS_Credentials.databasePort 		= EditorGUILayout.TextField ("Database Port:", 			AS_Credentials.databasePort, GUILayout.Width(400));
		
		AS_Credentials.databaseUsername 	= EditorGUILayout.TextField ("Database Username:", 		AS_Credentials.databaseUsername, GUILayout.Width(400));
		
		AS_Credentials.databasePassword 	= EditorGUILayout.TextField ("Database Password:", 		AS_Credentials.databasePassword, GUILayout.Width(400));
		
		AS_Credentials.databaseDbName 		= EditorGUILayout.TextField ("Database Name:", 			AS_Credentials.databaseDbName, GUILayout.Width(400));	

		
		PreferencesGUI ();		
		EditorGUILayout.EndVertical ();
		
		if (GUI.Button (new Rect (420, 330, 120, 30), "Next")){

            // Make sure there is a "http://" part in front
            if (!_phpScriptsLocation.Contains("http://") && 
                !_phpScriptsLocation.Contains("https://"))
            {
                this.Log(LogType.Error, "Make sure there's a 'http://' or 'https://' in front of the php script location..!");
            }
            else
            {
                AS_Credentials.phpScriptsLocation = _phpScriptsLocation;
                SetupPHPScripts(_phpScriptsLocation);
            }
		}


	}
	
	void PreferencesGUI () {

		
		GUILayout.Label ("Preferences", EditorStyles.boldLabel);
		
		// Mandatory Prefs
		
		AS_Preferences.askUserForEmail 			= EditorGUILayout.Toggle ("Ask User's Email Address", 	AS_Preferences.askUserForEmail);
		AS_Preferences.requireEmailActivation 	= EditorGUILayout.Toggle ("Require Email Activation", 	AS_Preferences.requireEmailActivation);
		AS_Preferences.enablePasswordRecovery 	= EditorGUILayout.Toggle ("Allow Password Recovery", 	AS_Preferences.enablePasswordRecovery	);
		
		// Extra Prefs
		GUILayout.Label ("", GUILayout.Height(8));
		EditorGUILayout.BeginToggleGroup ("Outgoing Email Account:", AS_Preferences.requireEmailActivation | AS_Preferences.enablePasswordRecovery);	
		EditorGUILayout.BeginHorizontal();
		AS_Credentials.emailAccount = EditorGUILayout.TextField ("", AS_Credentials.emailAccount, GUILayout.Width(300));
		if (AS_Preferences.requireEmailActivation | AS_Preferences.enablePasswordRecovery)
			credentialsOk &= AS_Setup.ValueCheck ("Email Account", AS_Credentials.emailAccount, 250);
		
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndToggleGroup ();
		

		
	}

	void FieldsGUI () {
		AS_Preferences.overrideExistingTables = EditorGUILayout.BeginToggleGroup ("Override Existing Tables", AS_Preferences.overrideExistingTables);	
		string dropMsg = "This will DROP (if they exist) the following database tables:\n'Accounts'";
		dropMsg += AS_Preferences.requireEmailActivation ? ", 'Confirm'" : "";
		dropMsg += AS_Preferences.enablePasswordRecovery ? ", 'PasswordReset'" : "";
		GUILayout.Label (  dropMsg, EditorStyles.boldLabel, GUILayout.Height(30));		
		EditorGUILayout.EndToggleGroup ();
		GUILayout.Label ("", GUILayout.Height(8));

		GUILayout.Label ("Accounts Table:", EditorStyles.boldLabel);
		string tableMsg = "By default the 'Accounts' table contains the following fields:\n{id, username, password";
		tableMsg += AS_Preferences.askUserForEmail ? ", email" : "";
		tableMsg += AS_Preferences.requireEmailActivation ? ", isactive" : "";
		tableMsg += ", custominfo}";
		GUILayout.Label (tableMsg);
		GUILayout.Label ("", GUILayout.Height(8));
		
		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Add Field", new GUILayoutOption[]{GUILayout.Width(150), GUILayout.Height(20)})) {
			additionalFields.Add( new AS_MySQLField () );
		}		
		EditorGUILayout.EndHorizontal ();
		
		GUILayout.Label ("", GUILayout.Height(12));
		
		
		int count = additionalFields.Count;
		
		if (count > 0) {
			
			EditorGUILayout.BeginVertical ();
			
			lastPos = EditorGUILayout.BeginScrollView (lastPos,  new GUILayoutOption[]{GUILayout.Width (600)});
			
			EditorGUILayout.BeginHorizontal ();
			
			fieldsOk = true;
			for (int i = 0; i < count; i ++)
				if ( FieldGUI(i) )
					break;
			
			
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndScrollView ();
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndVertical ();
		}

		if (GUI.Button (new Rect (10, 430, 120, 30), "Back"))
			state = AS_SetupState.CredentialsAndPreferences;

		GUI.Label (new Rect (10, 380, 400, 40), initializeDbMessage);

		if (GUI.Button (new Rect (200, 420, 200, 50), "Initialize Database"))
			InitializeDatabase ();

		
	}


	bool FieldGUI (int i) {
		
		GUILayout.Label ("", GUILayout.Width(10));
		EditorGUILayout.BeginVertical (new GUILayoutOption[]{GUILayout.ExpandWidth (false), GUILayout.Width (200)});
		
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label ("Additional Field " + i, EditorStyles.boldLabel);
		
		if (GUILayout.Button ("Remove")) {
			additionalFields.Remove (additionalFields [i]);
			return true;
		}				
		
		EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label ("Field Name:", GUILayout.Width(100) );
		additionalFields [i].name = EditorGUILayout.TextField ("", additionalFields[i].name, GUILayout.Width(100) );
		EditorGUILayout.EndHorizontal ();
				
		additionalFields [i].type = (AS_MySQLFieldType)EditorGUILayout.EnumPopup ("Field Type: ", additionalFields [i].type);


		additionalFields [i].mustBeUnique = EditorGUILayout.Toggle ("Must be Unique: ", additionalFields [i].mustBeUnique);
		additionalFields [i].isRequired = EditorGUILayout.Toggle ("Is Required: ", additionalFields [i].isRequired);
		
		
		GUILayout.Label ("Comment:");
		additionalFields [i].comment = EditorGUILayout.TextField ("", additionalFields [i].comment);

		if (additionalFields [i].type == AS_MySQLFieldType.UNSPECIFIED){
			GUILayout.Label ("Type can not be UNSPECIFIED!", EditorStyles.boldLabel, GUILayout.Width(200));
			fieldsOk = false;
		}
		else 
			fieldsOk &= AS_Setup.ValueCheck("Name", i, additionalFields.ToArray(), 200);
		
		EditorGUILayout.EndVertical ();
		GUILayout.Label ("", GUILayout.Width(10));
		
		return false;
	}

	void SetupPHPScripts(string _phpScriptsLocation){

		if ( !credentialsOk){
			initializeDbMessage = "Please check your credentials for errors";
			AS_PopUpWindow.ShowWindow("Error", initializeDbMessage);
			return;
		}
		
		AS_Credentials.Save();
		AS_Preferences.Save();

		string errorMessage = AS_Setup.Setup ( _phpScriptsLocation );
		
		if (errorMessage == "")
			AS_PopUpWindow.ShowWindow("Success!", "The PHP Scripts were successfully set up.\nYou can now Initialize the Database!", this, AS_SetupState.Fields);
		else 
			AS_PopUpWindow.ShowWindow("Error", errorMessage);
	}
	void InitializeDatabase (){
		
		if ( !fieldsOk){
			initializeDbMessage = "Please check your fields for errors";
			AS_PopUpWindow.ShowWindow("Error", initializeDbMessage);
			return;
		}
		
		string errorMessage = AS_Setup.InitializeDatabase ( additionalFields.ToArray() );

		if (errorMessage == "")
			AS_PopUpWindow.ShowWindow("Success!", "The Database was Initialized successfully.\nYou can now load the Demo Scene!", this, AS_SetupState.Done);
		else if (errorMessage.ToLower().Contains("credentials"))
			AS_PopUpWindow.ShowWindow("Error", errorMessage, this, AS_SetupState.CredentialsAndPreferences);
	}

}
#endif