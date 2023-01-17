using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class LocalisationManager : MonoBehaviour
{

    public static int language = 0;

    private static Dictionary<string, string>[] localisedValues;
    public static CSVLoader csvLoader;

    public static bool isInit;

    public static void Init()
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

        csvLoader = new CSVLoader();
        csvLoader.LoadCSV();
        UpdateDictionaries();


        isInit = true;
    }

    public static string ChangeLanguage()
    {
        if (!isInit) Init();

        language = (language + 1) % localisedValues.Length;
        localisedValues[language].TryGetValue("key", out string currentLang);
        return currentLang;
    }

    public static string GetLanguage()
    {
        if (!isInit) Init();

        localisedValues[language].TryGetValue("key", out string currentLang);
        return currentLang;
    }


    public static void UpdateDictionaries()
    {
        localisedValues = csvLoader.GetFullDictionaryValues();
        /*
        foreach (var val in localisedValues)
        {
            val.Select(i => $"{i.Key}: {i.Value}").ToList().ForEach(Debug.Log);
        }
        */
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
        localisedValues[language].TryGetValue(key, out string value);

        if (value == null || value == "")
        {
            value = "NO VAL IN: " + GetLanguage();
            Debug.LogWarning("The key: <color=white><i>" + key + "</i></color>, has no corresponding value in " + GetLanguage() + ". Please add a value or dubblecheck the key");
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
