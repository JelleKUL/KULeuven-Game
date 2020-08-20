using UnityEngine;
using System.Collections;
using System; 
using UnityEngine.UI; 

public class AS_CanvasUI : MonoBehaviour
{
    /*
     * 		--- TODO: CUSTOMIZE ---
     * 
     * Called upon Successful Login - Add any custom logic here
     * You could load a level, initialize some characters or
     * download additional info. The latter can be done by an
     * AS_AccountManagementGUI instance.
    */
    public void OnSuccessfulLogin(int id)
	{
		this.Log(LogType.Log, "Successfully Logged In User with id: " + id + " - Add any custom Logic Here!");
		loginState = AS_LoginState.LoginSuccessful;  
		
		if (accountManagementGUI)
		{
			accountManagementGUI.enabled = true;
			accountManagementGUI.Init(id);
		}
	} 
	
	// Messages to the user
	/// <summary>
	/// Make sure there's a child with that name in every canvas group
	/// </summary>
	public string guiMessageTextName = "GUI Message";
	/// <summary>
	/// Gui messages disappear after this time. Set negative or 0 to keep indefinitely.
	/// </summary>
	public float guiMessageTime = 5;

	// The parent groups of the different login states
	public CanvasGroup loginParent, registrationParent, recoveryParent;

	public InputField usernameField, passwordField, recoveryField;
	/// <summary>
	/// Container to place all the registration fields. The list is populated online by quering the server.
	/// </summary>
	public GridLayoutGroup registrationFieldsContainer;

	/// <summary>
	/// A prefab to serve as a registration input field
	/// </summary>
	public GameObject inputFieldPrefab; 
	
	/// <summary>
	/// Only need to set this if using Password Recovery (from Setup)
	/// </summary>
	public Button recoveryButton;

	Text guiMessageText;
	AS_AccountInfo accountInfo = new AS_AccountInfo();
	AS_MySQLField passwordConfirm, emailConfirm;
	AS_AccountManagementGUI accountManagementGUI;

	// Shut everything off
	void Awake()
	{  
		loginState = AS_LoginState.Idle;  
	}

	// Check if we're good to go, and load up the first screen
	void Start()
	{ 
		accountManagementGUI = GetComponentInChildren<AS_AccountManagementGUI> ();

		if (!loginParent || !registrationParent || !recoveryParent 
		    || !registrationFieldsContainer || !inputFieldPrefab) {
			this.Log (LogType.Exception, "Unassigned variables - make sure you have assigned all the variables in this object");
			return;
		}
		if ((!recoveryButton||!recoveryField) && AS_Preferences.enablePasswordRecovery) {
			this.Log (LogType.Exception, "You haven't assigned the Recovery button and/or field. Either disable password recovery from Setup, or assign a button and a field.");
			return;
		} else if ((recoveryButton || recoveryField) && !AS_Preferences.enablePasswordRecovery) {
			this.Log(LogType.Warning, "You have assigned a Recovery button and/or field, but haven't enabled password recovery in the setup. Disabling the button & field");
			if (recoveryButton)
				recoveryButton.gameObject.SetActive(false);
			if (recoveryField)
				recoveryField.gameObject.SetActive(false);
		} 
		loginState = AS_LoginState.LoginPrompt; 
	} 
	
	public string guiMessage { set { 
			if (!guiMessageText)
				return;
			guiMessageText.text = value; 
			if(guiMessageTime > 0)
			{
				StopCoroutine("ClearGUIText"); 
				StartCoroutine ("ClearGUIText", guiMessageText);
			} } } 
	
	// If the state changes update messages / load level
	AS_LoginState _loginState = AS_LoginState.Idle;
	AS_LoginState loginState
	{
		get { return _loginState; }
		set
		{
			if (value == loginState)
				return;
			switch (value)
			{
			case AS_LoginState.Idle:
				ToggleCanvasGroup(null);
				break;

			case AS_LoginState.LoginPrompt: 
				if (accountManagementGUI)
					accountManagementGUI.enabled = false;
				ToggleCanvasGroup(loginParent); 
				break;
				
			case AS_LoginState.Registering:
				ToggleCanvasGroup(registrationParent);
				break;
				
			case AS_LoginState.RecoverPassword:
				ToggleCanvasGroup(recoveryParent);
				break;
				
			case AS_LoginState.LoginSuccessful:
				ToggleCanvasGroup(null);  
				// ToggleCanvasGroup(accountManagementParent);
				break;  
			}
			guiMessage = "";
			_loginState = value;
		}
	}


    #region Button Accessors
    public void OnLoginRequested()
    {
        string username = usernameField.text;
        string password = passwordField.text;
        username.TryToLogin(password, LoginAttempted);
    }

    public void OnMessageClicked()
    {

    }

    public void OnLogoutRequested()
	{
		loginState = AS_LoginState.LoginPrompt;
	}

	public void OnRegistrationRequested()
	{ 
		// When the form is downloaded, RegistrationFormDownloaded is called
		accountInfo.TryToDownloadRegistrationForm(RegistrationFormDownloaded);
		// Equivalent to: 
		// StartCoroutine ( AS_Login.TryToDownloadRegistrationForm (accountInfo, RegistrationFormDownloaded) );
		
		guiMessage = "Loading..";
	}
	
	public void OnRegistrationCancelled()
	{ 
		loginState = AS_LoginState.LoginPrompt;
	}
	public void OnRegistrationSubmitted()
	{ 
		// Offline field check
		string errorMessage = "";
		string emailValue = "";
		if (AS_Preferences.askUserForEmail && emailConfirm != null)
			emailValue = emailConfirm.stringValue;

		if (!AS_Login.CheckFields (accountInfo, passwordConfirm.stringValue, emailValue, ref errorMessage)) {
			guiMessage = errorMessage;
			return; 
		}
		
		// Online check with the given database
		guiMessage = "Attempting to Register..";
		accountInfo.TryToRegister(RegistrationAttempted);
		// Equivalent to: 
		// StartCoroutine ( AS_Login.TryToRegister( accountInfo, RegistrationAttempted ) ) ;

	}
	
	public void OnRecoveryRequested()
	{ 
		loginState = AS_LoginState.RecoverPassword;
	}
	public void OnRecoveryCancelled()
	{ 
		loginState = AS_LoginState.LoginPrompt;
	}
	public void OnRecoverySubmitted()
	{ 
		recoveryField.text.TryToRecoverPassword(PasswordRecoveryAttempted);
		// Equivalent to: 
		// StartCoroutine(AS_Login.TryToRecoverPassword ( emailPasswordRecovery, PasswordRecoveryAttempted ) );
		
		guiMessage = "Processing your request..";
	}
	#endregion
	
	#region Callbacks
	// Called by the AttemptLogin coroutine when it's finished executing
	public void LoginAttempted(string callbackMessage)
	{
		
		
		// If our log in failed,
		if (callbackMessage.IsAnError())
        {
            string [] s = callbackMessage.Split( new string[] { "Error: " }, StringSplitOptions.RemoveEmptyEntries);
            guiMessage = s.Length >= 1 ? s[0] : callbackMessage;
            this.Log( LogType.Error, callbackMessage);
			return;
		}
		
		// Otherwise,
		int accountId = Convert.ToInt32(callbackMessage);
		OnSuccessfulLogin(accountId);
		
	} 
	// Used by the AttemptDownloadRegistrationForm when it's finished executing
	void RegistrationFormDownloaded(string callbackMessage)
	{ 
		
		if (callbackMessage.IsAnError())
		{
			this.Log( LogType.Error, callbackMessage);
			guiMessage = callbackMessage;
			return;
		}

		PopulateRegistrationGroup ();

		loginState = AS_LoginState.Registering;
		
		// What you want to appear in the registration GUI
		guiMessage = "Please fill in the required fields."; 
		
	}
	
	// Called by the AttemptRegistration coroutine when it's finished executing
	public void RegistrationAttempted(string callbackMessage)
	{
		
		guiMessage = callbackMessage;
		
		// If our registration failed,
		if (callbackMessage.IsAnError())
		{
			this.Log( LogType.Error, callbackMessage);
			return;
		}
		
		// Otherwise, success 		
		loginState = AS_LoginState.LoginPrompt;
		
		guiMessage = callbackMessage;
		
	}
	
	// Called by the AttemptPasswordRecovery coroutine when it's finished executing
	public void PasswordRecoveryAttempted(string callbackMessage)
	{
		
		guiMessage = callbackMessage;
			// If our registration failed,
		if (callbackMessage.IsAnError())
		{
			this.Log( LogType.Error, callbackMessage);
			return;
		}
		
		// Otherwise,
		loginState = AS_LoginState.LoginPrompt;
		
		guiMessage = callbackMessage;
		
	}
	#endregion

	#region Helpers
	/// <summary>
	/// Toggles on a specific Canvas Group - and the rest off
	/// </summary> 
	private void ToggleCanvasGroup(CanvasGroup groupToToggle)
	{
		if (groupToToggle) {
			groupToToggle.gameObject.SetActive(true);
			guiMessageText = FindGUIMessage (groupToToggle.transform);
		}
		else
			guiMessageText = null;

		foreach (CanvasGroup cG in gameObject.GetComponentsInChildren<CanvasGroup>())
		{
			cG.alpha = cG == groupToToggle ? 1 : 0;
			cG.blocksRaycasts = cG == groupToToggle; 
		}
	}

	/// <summary>
	/// Finds the GUI message in the hierarchy of the given transform
	/// </summary> 
	private Text FindGUIMessage(Transform parent)
	{
		foreach (Text text in parent.GetComponentsInChildren<Text>()) {

			if (text.name.CompareTo(guiMessageTextName) != 0) 
				continue;  
			return text; 
		} 

		this.Log(LogType.Exception, "Couldn't find a Text component named '" +guiMessageTextName+"' as a child of '" + parent.name +"'. Make sure you add one.");

		return null; 
	} 

	IEnumerator ClearGUIText(Text text)
	{
		if (!text)
			yield break; 
		yield return new WaitForSeconds(guiMessageTime);
		if (text)
			text.text = ""; 
	}

	AS_InputField CreateInputField(AS_MySQLField field)
	{
		GameObject temp = GameObject.Instantiate (inputFieldPrefab) as GameObject;
		
		// Place it in the container
		temp.transform.SetParent (registrationFieldsContainer.transform, false);
		
		// Grab its AS_InputField component (or add it)
		AS_InputField inputField = temp.GetComponent<AS_InputField> ();
		if (!inputField)
			inputField = temp.AddComponent<AS_InputField> ();

		inputField.Initialize (field);
		return inputField;
	}
     
	void PopulateRegistrationGroup()
	{  
		// Clean up first
		for (int c = 0; c < registrationFieldsContainer.transform.childCount; c++)
			Destroy(registrationFieldsContainer.transform.GetChild (c).gameObject); 

		// Registration Info has the fields the user should fill in
		foreach (AS_MySQLField field in accountInfo.fields)
		{
			
			// Id is an auto-increment unique identifier
			// and custom info is not specified during registration
			if (field.name.ToLower() == "id" | field.name.ToLower() == "custominfo" | field.name.ToLower() == "isactive")
				continue;
			
			// For any other field, create an InputField prefab

			// Initialize it
			CreateInputField(field); 

			// User requires one more space for PWD & Confirm to allign
			if (field.name.ToLower().Contains("username"))
			{
                AS_MySQLField dummyField = new AS_MySQLField();
                dummyField.name = "<b>Bold *</b>: Required Field";
                AS_InputField dummy = CreateInputField(dummyField);
                dummy.background.gameObject.SetActive(false);
			}

			/// Password / Email -> Require confirmation
			if (field.name.ToLower().Contains("password"))
			{ 
				passwordConfirm = new AS_MySQLField(field);
				passwordConfirm.name = "Confirm Password"; 
				CreateInputField(passwordConfirm); 
			}
			else if (field.name.ToLower().Contains("email"))
			{ 
				emailConfirm = new AS_MySQLField(field);
				emailConfirm.name = "Confirm Email"; 
				CreateInputField(emailConfirm);  
			}
		}
	}
	#endregion 
}