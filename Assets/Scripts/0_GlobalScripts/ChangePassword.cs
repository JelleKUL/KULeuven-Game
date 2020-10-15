using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangePassword : MonoBehaviour
{
	public Text usernameText;
	public InputField newPasswordField, confirmNewPasswordField;
	public Text debugOutput;

	public GameObject passwordCanvas;

	public AS_AccountInfo accountInfo = new AS_AccountInfo();

    private void Start()
    {
		if (!GameManager.isLoggedIn) passwordCanvas.SetActive(false);

		if(usernameText && GameManager.isLoggedIn)
        {
			usernameText.text = GameManager.userName;
        }
    }

    public void UploadNewPassword()
    {
		if(GameManager.isLoggedIn)
		{
			accountInfo.TryToDownload(GameManager.loginID, tryToUpload);

		}
	}

	void tryToUpload(string message)
    {

		// If the user set a new password
		if (newPasswordField.text != "")
		{
			if (newPasswordField.text != confirmNewPasswordField.text)
			{
				debugOutput.text = "New passwords do not match.";
				return;
			}

			this.Log(LogType.Log, "Updated Password!");
			accountInfo.fields.SetFieldValue("password", newPasswordField.text.Hash());
		}
		debugOutput.text = "Uploading your info..";

		accountInfo.TryToUpload(GameManager.loginID, AccountInfoUploaded);
	}

	// This is called when new info is uploaded
	// You can use this as a trigger - perhaps you want 
	// a message to be displayed once the account info is uploaded!
	void AccountInfoUploaded(string message)
	{
		if (message.ToLower().Contains("error"))
		{
			this.Log(LogType.Error, "Account System: " + message);
			debugOutput.text = "An error occured while Uploading your info. Try again later!";
		}
		else
		{
			this.Log(LogType.Log, "Account System: " + message + " - Add any custom Logic here!");
			debugOutput.text = "Your pasword was successfully changed!";

			GameManager.LoadScene(1, false);
		}

	}
}
