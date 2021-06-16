using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalisationManager : MonoBehaviour
{
    public enum Language { NL, EN }

    public static Language language = Language.NL;

    private static Dictionary<string, string> localisedNL;
    private static Dictionary<string, string> localisedEN;
    public static CSVLoader csvLoader;

    public static bool isInit;

    public static void Init()
    {
        csvLoader = new CSVLoader();
        csvLoader.LoadCSV();
        UpdateDictionaries();


        isInit = true;
    }

    public static void ChangeLanguage()
    {
        language = (Language)((int)(language + 1) % System.Enum.GetValues(typeof(Language)).Length);
    }

    public static void UpdateDictionaries()
    {
        localisedNL = csvLoader.GetDictionaryValues("nl");
        localisedEN = csvLoader.GetDictionaryValues("en");
    }

    /// <summary>
    /// returns a localised value of a given key, throws warning if not found
    /// </summary>
    /// <param name="key"></param>
    /// <returns>the correct value of the current language, returns empty if key is empty</returns>
    public static string GetLocalisedValue(string key)
    {
        if (!isInit) Init();

        if (key == "") return key;

        string value = key;

        switch (language)
        {
            case Language.NL:
                localisedNL.TryGetValue(key, out value);
                break;
            case Language.EN:
                localisedEN.TryGetValue(key, out value);
                break;

        }
        if (value == null || value == "")
        {
            value = "NO VAL IN: " + language;
            Debug.LogWarning("The key: <color=white><i>" + key + "</i></color>, has no corresponding value in " + language + ". Please add a value or dubblecheck the key");
        }
        return value;
    }
#if UNITY_EDITOR
    public static void Add(string key, string valueNL, string valueEN)
    {
        if (valueNL.Contains("\""))
        {
            valueNL.Replace('"', '\"');
        }
        if (valueEN.Contains("\""))
        {
            valueEN.Replace('"', '\"');
        }

        if (csvLoader == null) csvLoader = new CSVLoader();

        csvLoader.LoadCSV();
        csvLoader.Add(key, valueNL, valueEN);
        csvLoader.LoadCSV();

        UpdateDictionaries();
    }

    public static void Edit(string key, string valueNL, string valueEN)
    {
        if (valueNL.Contains("\""))
        {
            valueNL.Replace('"', '\"');
        }
        if (valueEN.Contains("\""))
        {
            valueEN.Replace('"', '\"');
        }

        if (csvLoader == null) csvLoader = new CSVLoader();

        csvLoader.LoadCSV();
        csvLoader.Edit(key, valueNL, valueEN);
        csvLoader.LoadCSV();

        UpdateDictionaries();
    }
    public static void Remove(string key)
    {
        if (csvLoader == null) csvLoader = new CSVLoader();

        csvLoader.LoadCSV();
        csvLoader.Remove(key);
        csvLoader.LoadCSV();

        UpdateDictionaries();
    }
#endif

}
