using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


/*
 * 		A GUI demonstration of managing 
 * the AccountInfo and CustomInfo classes
 * 
 * OnGUI provides three things: 
 * 
 * 1) Upload/Download Info buttons
 * 
 * 		* Upload Info: Serializes the customInfo variable,
 * stores it in the 'custominfo' field of the accountInfo variable
 * and then uploads accountInfo to the database
 * 
 * 		* Download Info: Downloads the accountInfo variable
 * from the database, extracts the 'custominfo' field,
 * de-serializes it and stores it as customInfo
 * 
 * 2) An Account Management GUI
 * 
 *		* Alter any fields that are found in the accountInfo variable
 * after downloading it using Download Info (check above). You can then
 * upload the new info to the database using Upload Info (check above).
 * 
 * 3) A Custom Info GUI
 * 
 * 		* This works as a custom in-game editor for your customInfo variable.
 * Because this is stored in XML format in the database, it's impractical to
 * edit the XML string - so we provide a GUI interface to do so. Any changes
 * you make are then passed into the 'custominfo' field of the accountInfo
 * and uploaded with the rest to the database.
 * 
 */
[ExecuteInEditMode]
public class AS_AccountManagementGUI : MonoBehaviour
{

    ManagementPage currentPage = ManagementPage.AccountInfo;

    // This is called when we know the account's ID so we 
    // can proceed with downloading (that's all it needs to know)
    public void Init(int _accountId)
    {

        accountId = _accountId;

        tempPasswordVal = "";

        accountInfo.TryToDownload(accountId, AccountInfoDownloaded);

    }

    // This is called when new info is downloaded
    // You can use this as a trigger - perhaps you want 
    // a level to be loaded once the account info is downloaded!
    void AccountInfoDownloaded(string message)
    {
		if (message.ToLower().Contains("error"))
		{
			this.Log( LogType.Error, "Account System: " + message);
			guiMessage = "An error occured while Uploading your info. Try again later!";
		}
		else
		{
			this.Log(LogType.Log, "Account System: " + message + " - Add any custom Logic here!");
			guiMessage = "Your info was successfully downloaded!";
		}

	}

    // This is called when new info is uploaded
    // You can use this as a trigger - perhaps you want 
    // a message to be displayed once the account info is uploaded!
    void AccountInfoUploaded(string message)
    {
        if (message.ToLower().Contains("error"))
		{
			this.Log( LogType.Error, "Account System: " + message);
			guiMessage = "An error occured while Uploading your info. Try again later!";
		}
        else
		{
			this.Log(LogType.Log, "Account System: " + message + " - Add any custom Logic here!");
			guiMessage = "Your info was successfully uploaded!";
		}

    }

    // This is called when the similar accounts info are downloaded
    // You can use this as a trigger - perhaps you want 
    // a message to be displayed once the info has been downloaded!
    private void OnSimilarAccountsFound(string message)
    {
        if (message.ToLower().Contains("error"))
        {
            this.Log(LogType.Error, "Account System: " + message);
            guiMessage = "An error occured while Downloading the similar accounts' info. Try again later!";
        }
        else
        {
            this.Log(LogType.Log, "Account System: " + message + " - Add any custom Logic here!");

            if (similarAccountsInfo.Count == 0)
                guiMessage = "No similar accounts were found!";
            else
            {
                guiMessage = similarAccountsInfo.Count.ToString() + " Similar accounts" + (similarAccountsInfo.Count >= 2 ? "s were" : " was") + " successfully downloaded!\n";
                string output = "";
                foreach (string info in similarAccountsInfo)
                    output += info + " | ";
                output = output.Remove(output.Length - 3, 3);
                guiMessage += output;
                this.Log(LogType.Log, "Account System:\nSimilar account Info:\n" + output);
            }
        }
    }


    // Public variables
    public AS_AccountInfo accountInfo = new AS_AccountInfo();
	 
    private int accountId = 1;


	private string guiMessage ="";

    void OnGUI()
    {

        GUILayout.BeginArea(new Rect(10, 10, Screen.width - 10, Screen.height - 10));

        // ------------ BUTTONS ------------------

        // Prompt to Download Info from the Database
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Download Account Info", GUILayout.Width(200)))
        {
			guiMessage = "Downloading your info..";
            accountInfo.TryToDownload(accountId, AccountInfoDownloaded);
        }
		
		// Prompt to Upload Info to the Database
		if (GUILayout.Button("Upload Account Info", GUILayout.Width(200)))
		{
			string errorMessage = "";
			if(!accountInfo.fields.CheckMySQLFields(ref errorMessage))
			{
				this.Log( LogType.Error, "Invalid MySQL Field Value:\n" + errorMessage);
				guiMessage = errorMessage;
				
			}
			else 
			{
				// If the user set a new password
				if (tempPasswordVal != "")
				{
					this.Log(LogType.Log, "Updated Password!");
					accountInfo.fields.SetFieldValue("password", tempPasswordVal.Hash());
				}
				guiMessage = "Uploading your info..";
				accountInfo.TryToUpload(accountId, AccountInfoUploaded);
			}
		}

        // Prompt to Upload Info to the Database
        if (GUILayout.Button("Logout", GUILayout.Width(200)))
		{
			#if !UNITY_1 && !UNITY_2 && !UNITY_3 && (!UNITY_4 || UNITY_4_6)
			 if (GetComponent<AS_CanvasUI>())
				GetComponent<AS_CanvasUI>().OnLogoutRequested();
			#endif
		}

        GUILayout.EndHorizontal();


        GUILayout.Label(" ", GUILayout.Width(100));

        if (currentPage == ManagementPage.AccountInfo)
        {

            GUILayout.Label("~~~==== Account Management ====~~~", GUILayout.Width(300));
            if (GUILayout.Button("Go to Custom Info", GUILayout.Width(200)))
                currentPage = ManagementPage.CustomInfo;

            // ------------ ACCOUNT INFO ------------------		
            accountInfo = AccountInfoOnGUI(accountInfo);


        }
        else if (currentPage == ManagementPage.CustomInfo)
        {

            // ------------ CUSTOM INFO ------------------
            // Note that upon altering the CustomInfo class,
            // this part will seize to work - 
            // Although don't worry, you will still
            // be able to upload / download and see the custom
            // Info class in the default inspector

            GUILayout.Label("~~~==== Custom Info ====~~~", GUILayout.Width(300));
            if (GUILayout.Button("Go to Account Management", GUILayout.Width(200)))
                currentPage = ManagementPage.AccountInfo;

            accountInfo.customInfo = accountInfo.customInfo.CustomInfoOnGUI();
        }

		
		GUILayout.Label ("", GUILayout.Height(10));
		
		GUILayout.Label (guiMessage);

        // Tutorial
#if UNITY_EDITOR
		GUILayout.Label ("", GUILayout.Height(10));
		GUILayout.Label ("\bHow To Manage your Account:" +
			"\n1) Alter your Account Info" +
			"\n2) Alter your Custom Info" +
			"\n3) Hit Upload" +
			"\n\nThis message was printed from AS_AccountManagementGUI.cs", GUILayout.Width(500));
#endif

        GUILayout.EndArea();
    }


    string tempPasswordVal = "";
    List<string> similarAccountsInfo = new List<string>();
    // Called by OnGUI and provides a basic account information management GUI
    public AS_AccountInfo AccountInfoOnGUI(AS_AccountInfo accountInfo)
    {

        GUILayout.BeginVertical();

        // Title


        GUILayout.Label("Account Id: " + accountId, GUILayout.Width(300));
        if (accountInfo.fields.GetIndex("isactive") > 0)
            GUILayout.Label("Status: " + accountInfo.fields.GetFieldValue("isactive"), GUILayout.Width(300));


        // For each field in Account Info
        foreach (AS_MySQLField field in accountInfo.fields)
        {

            // Id is an auto-increment unique identifier
            // and custom info is not specified during registration
            if (field.name.ToLower() == "id" | field.name.ToLower() == "custominfo" | field.name.ToLower() == "isactive")
                continue;



            // For any given field
            GUILayout.BeginHorizontal();

            // Prompt the user to input the value
            string tempVal = "";

            // Print the name


            // If it's the password,
            if (field.name.ToLower() == "password")
            {
                GUILayout.Label("New Password", GUILayout.Width(200));
                tempPasswordVal = GUILayout.TextField(tempPasswordVal,
                                                     new GUILayoutOption[] { GUILayout.Width(200), GUILayout.Height(20) });
                // Don't store it on the account Info.
                // If on Upload the tempPasswordVal is not empty, we hash it and upload the hashed pass
            }
            else
            {
                GUILayout.Label(field.name.UppercaseFirst(), GUILayout.Width(200));
                tempVal = GUILayout.TextField(accountInfo.fields.GetFieldValue(field.name),
                                    new GUILayoutOption[] { GUILayout.Width(200), GUILayout.Height(20) });

                // Store the value
                accountInfo.fields.SetFieldValue(field.name, tempVal);



                // Prompt to Upload Info to the Database
                if (!field.mustBeUnique && GUILayout.Button("Get similar", GUILayout.Width(200)))
                {
                    accountInfo.TryToGetSimilar(field.name, "username", false, similarAccountsInfo, OnSimilarAccountsFound);
                }
            }

            GUILayout.EndHorizontal();


        }

        GUILayout.EndVertical();

        return accountInfo;
    }

}

public enum ManagementPage { AccountInfo, CustomInfo };
