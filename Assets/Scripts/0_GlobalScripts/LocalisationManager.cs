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

    public static string GetLocalisedValue(string key)
    {
        if (!isInit) Init();

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
        return value;
    }

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


}
