using UnityEngine;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace AS
{

    [Serializable]
    public class AS_AccountInfo
    {
        public List<AS_MySQLField> fields = new List<AS_MySQLField>();
        public AS_CustomInfo customInfo = new AS_CustomInfo();
    }

    public static class AS_MySQLFieldMethods
    {

        public static string ToReadableString(this AS_AccountInfo accountInfo)
        {

            string readableString = "(FieldName, Value, Type, Unique?, Required?)\n";
            foreach (AS_MySQLField field in accountInfo.fields)
            {
                readableString += "(" + field.name;
                readableString += ", " + field.stringValue;
                readableString += ", " + field.type.ToString();
                readableString += (field.mustBeUnique ? ", MUST BE UNIQUE" : "");
                readableString += (field.isRequired ? ", IS REQUIRED" : "");
                readableString += ")\n";
            }
            return readableString;
        }

        public static bool DeSerializeCustomInfo(this AS_AccountInfo accountInfo)
        {

            if (accountInfo.fields.GetIndex("custominfo") < 0)
            {
                Log(LogType.Error, "There is no custom info field in account info. Is there one in your database? If not try setting up your database again or contact customer support.");
                return false;
            }
            else
            {
                string temp = accountInfo.fields.GetFieldValue("custominfo");

                if (temp == "")
                {
                    accountInfo.customInfo = new AS_CustomInfo();
                    Log(LogType.Log, "CustomInfo field was empty - instantiating new CustomInfo!");
                }
                else
                {
                    accountInfo.customInfo = temp.XmlDeserializeFromString<AS_CustomInfo>();
                    Log(LogType.Log, "Successfully deserialized the downloaded custom info!");
                }

                return true;

            }
        }

        public static bool SerializeCustomInfo(this AS_AccountInfo accountInfo)
        {
            if (accountInfo.customInfo == null)
            {
                Log(LogType.Error, "The custom info field of Account Info has not been set. Uploading a null field will erase the previously stored value of custom info from the database!\nAborting. Feel free to change this.");
                return false;
            }

            if (accountInfo.fields.GetIndex("custominfo") >= 0)
            {
                accountInfo.fields.SetFieldValue("custominfo", accountInfo.customInfo.XmlSerializeToString());
                Log(LogType.Log, "Successfully serialized custom info - ready for upload");
                return true;
            }
            else
            {

                Log(LogType.Error, "There is no custom info field in account info. Is there one in your database? If not try setting up your database again or contact customer support.");
                return false;

            }
        }

        public static bool SetFieldValue(this AS_AccountInfo accountInfo, string fieldKey, string fieldVal)
        {

            if (accountInfo.fields.GetIndex(fieldKey) < 0)
                return false;

            accountInfo.fields.SetFieldValue(fieldKey, fieldVal);
            Log(LogType.Log, "Successfully set thefield value to: " + fieldVal);
            return true;

        }

        public static string GetFieldValue(this AS_AccountInfo accountInfo, string fieldKey)
        {

            if (accountInfo.fields.GetIndex(fieldKey) < 0)
                return null;

            return accountInfo.fields.GetFieldValue(fieldKey);
        }


        /// <summary>
        /// Tries to login.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <param name="callback">What to call when we are done.</param>
        /// <param name="phpScriptsLocation">Where the PHP scripts are located.</param>
        public static void TryGetId(this string username, Action<string> callback, string phpScriptsLocation = null)
        {

            AS_CoroutineCaller caller = AS_CoroutineCaller.Create();
            caller.StartCoroutine(AS_Login.TryGetId
                                   (username,
             value =>
             {
                 caller.Destroy();
                 callback(value);
             },
            phpScriptsLocation));
        }

        /// <summary>
        /// Tries to download.
        /// </summary>
        /// <param name="accountInfo">Where to store the downloaded info.</param>
        /// <param name="accountId">Account identifier.</param>
        /// <param name="callback">What to call when we are done.</param>
        /// <param name="phpScriptsLocation">Where the PHP scripts are located.</param>
        public static void TryToDownload(this AS_AccountInfo accountInfo, int accountId, Action<string> callback, string phpScriptsLocation = null)
        {

            AS_CoroutineCaller caller = AS_CoroutineCaller.Create();
            caller.StartCoroutine(AS_AccountManagement.TryToDownloadAccountInfoFromDb
                                   (accountId,
                                    accountInfo,
                                     value =>
                                     {
                                         caller.Destroy();
                                         callback(value);
                                     },
                                    phpScriptsLocation));
        }

        public static void TryToDownloadLeaderBoard(this string[,] leaderBoardData, Action<string> callback, string phpScriptsLocation = null)
        {

            AS_CoroutineCaller caller = AS_CoroutineCaller.Create();
            caller.StartCoroutine(AS_AccountManagement.TryToDownloadAllAccountInfoFromDb
                                   (
                                    leaderBoardData,
                                     value =>
                                     {
                                         caller.Destroy();
                                         callback(value);
                                     },
                                    phpScriptsLocation));
        }


        /// <summary>
        /// Tries to upload.
        /// </summary>
        /// <param name="accountInfo">What we're trying to upload.</param>
        /// <param name="accountId">Account identifier.</param>
        /// <param name="callback">What to call when we are done.</param>
        /// <param name="phpScriptsLocation">Where the PHP scripts are located.</param>
        public static void TryToUpload(this AS_AccountInfo accountInfo, int accountId, Action<string> callback, string phpScriptsLocation = null)
        {

            AS_CoroutineCaller caller = AS_CoroutineCaller.Create();
            caller.StartCoroutine(AS_AccountManagement.TryToUploadAccountInfoToDb
                                  (accountId,
                                    accountInfo,
                                    value =>
                                    {
                                        caller.Destroy();
                                        callback(value);
                                    },
                                    phpScriptsLocation));
        }



        public static int GetIndex(this List<AS_MySQLField> fields, string name)
        {

            for (int i = 0; i < fields.Count; i++)
                if (fields[i].name.ToLower() == name.ToLower())
                    return i;

            return -1;
        }

        public static string GetFieldValue(this List<AS_MySQLField> fields, string name)
        {

            AS_MySQLField field = fields.GetField(name);
            return (field == null) ? "" : field.stringValue;
        }

        public static AS_MySQLField GetField(this List<AS_MySQLField> fields, string name)
        {

            int idx = fields.GetIndex(name);
            if (idx < 0)
                return null;

            return fields[idx];
        }

        public static bool SetFieldValue(this List<AS_MySQLField> fields, string name, string value)
        {

            for (int i = 0; i < fields.Count; i++)
                if (fields[i].name.ToLower() == name.ToLower())
                {
                    fields[i].stringValue = value;
                    return true;
                }

            return false;
        }

        static void Log(LogType logType, string msg) { AS_Methods.Log(MethodBase.GetCurrentMethod().DeclaringType.Name, logType, msg); }
    }


    [System.Serializable]
    public class AS_SerializableDictionary : System.Object
    {
        [SerializeField]
        public List<string> keys;
        [SerializeField]
        public List<string> vals;

        public AS_SerializableDictionary()
        {
            keys = new List<string>();
            vals = new List<string>();
        }

        public string GetValue(string key)
        {
            if (GetIndex(key) == -1)
                return "";
            return vals[GetIndex(key)];
        }
        public bool SetValue(string key, string val)
        {
            if (GetIndex(key) == -1)
                return false;
            vals[GetIndex(key)] = val;
            return true;
        }

        public int Count
        {
            get { return keys.Count; }
        }

        public int GetIndex(string key)
        {
            if (keys.Contains(key) == false)
            {
                return -1;
            }
            return keys.IndexOf(key);
        }
    }


    public static class ObjectDictionaryMethods
    {

        public static bool Add(this AS_SerializableDictionary dictionary, string key, string val)
        {

            if (dictionary.keys.Contains(key))
            {
                return false;
            }
            dictionary.keys.Add(key);
            dictionary.vals.Add(val);
            return true;
        }

    }

}