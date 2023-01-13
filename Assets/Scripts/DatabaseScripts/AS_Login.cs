using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AS
{


    public static class AS_Login
    {


        const string loginPhp = "/checklogin.php?";
        const string getidPhp = "/getid.php?";
        const string getRegFormPhP = "/getregistrationform.php?";
        const string registerPhp = "/register.php?";
        const string passResetPhp = "/requestpasswordreset.php?";


        /// <summary>
        /// Attempt to register to our database. When we are done with that attempt, return something meaningful or an error message.
        /// </summary>
        public static IEnumerator TryToRegister(AS_AccountInfo newAccountInfo, Action<string> resultCallback, string hostUrl = null)
        {
            if (hostUrl == null)
                hostUrl = AS_Credentials.phpScriptsLocation;

            if (hostUrl == "")
            {
                Log(LogType.Error, "Host URL not set..! Please load the Account System Setup window.");
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
            newAccountInfo.SerializeCustomInfo();

            // Serialize the account info
            string serializedAccountInfo = newAccountInfo.AccInfoToString(false);

            if (serializedAccountInfo == null)
            {
                Log(LogType.Error, "Could not serialize account info - check previous Log for errors");
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
                Log(LogType.Error, "WWW Error:\n" + www.error);
                resultCallback("Error: Could not connect. Please try again later!");
                yield break;
            }
            if (www.text.ToLower().Contains("error"))
            {
                Log(LogType.Error, "PHP/MySQL Error:\n" + www.text);
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
                Log(LogType.Error, "Could not register new account - Check Message:\n" + www.text);
                resultCallback("Error: Could not register new account");
            }


            yield break;


        }


        /// <summary>
        /// Attempt to login to our database. When we are done with that attempt, return something meaningful or an error message.
        /// </summary>
        public static IEnumerator TryGetId(string username, Action<string> resultCallback, string hostUrl = null)
        {
            if (hostUrl == null)
                hostUrl = AS_Credentials.phpScriptsLocation;

            if (hostUrl == "")
            {
                Log(LogType.Error, "Host URL not set..! Please load the Account System Setup window.");
                yield break;
            }

            // Location of our login script
            string url = hostUrl + getidPhp;

            url += "&requiredForMobile=" + UnityEngine.Random.Range(0, int.MaxValue).ToString();

            // Create a new form
            WWWForm form = new WWWForm();

            // Add The required fields
            form.AddField("username", username);

            // Connect to the script
            WWW www = new WWW(url, form);

            // Wait for it to respond
            yield return www;

            Debug.Log("Got message: " + www.text);

            if (www.error != null && www.error != "")
            {
                Log(LogType.Error, "WWW Error:\n" + www.error);
                resultCallback("Error: Could not connect. Please try again later!");
                yield break;
            }
            if (www.text.ToLower().Contains("error"))
            {
                Log(LogType.Error, "PHP/MySQL Error:\n" + www.text);
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
                    resultCallback("Couldn't parse the recieved message:" + www.text);
                    Log(LogType.Error, "Failed Login Attempt (Unknown Error / Warning)\n" + www.text);
                    yield break;
                }
                Log(LogType.Log, "Successful Login for user with ID: " + www.text);
                resultCallback(www.text);

            }

        }



        static void Log(LogType logType, string msg) { AS_Methods.Log(MethodBase.GetCurrentMethod().DeclaringType.Name, logType, msg); }
    }

}