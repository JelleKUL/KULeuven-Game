﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Globalization;

public class CSVLoader
{
    private TextAsset csvFile;
    private char lineSeperator = '\n';
    private char surround = '"';
    private string[] fieldSeperator = { "\",\"" };

    public void LoadCSV()
    {
        csvFile = Resources.Load<TextAsset>("localisation");
    }

    public Dictionary<string, string> GetDictionaryValues(string attrbuteId)
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

        Dictionary<string, string> dictionary = new Dictionary<string, string>();

        string[] lines = csvFile.text.Split(lineSeperator);
        int attributeIndex = -1;
        string[] headers = lines[0].Split(fieldSeperator, System.StringSplitOptions.None);

        for (int i = 0; i < headers.Length; i++)
        {
            if (headers[i].Contains(attrbuteId))
            {
                attributeIndex = i;
                break;
            }
        }

        Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] fields = CSVParser.Split(line);

            for (int j = 0; j < fields.Length; j++)
            {
                fields[j] = fields[j].TrimStart(' ', surround);
                fields[j] = fields[j].TrimEnd('\n','\r',surround);
            }

            if (fields.Length > attributeIndex)
            {
                var key = fields[0];

                if (dictionary.ContainsKey(key)) continue;

                var value = fields[attributeIndex];
                dictionary.Add(key, value);
            }
        }
        return dictionary;
    }

    public Dictionary<string, string>[] GetFullDictionaryValues()
    {
        // set the language
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
        Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

        

        //split all the lines
        string[] lines = csvFile.text.Split(lineSeperator);

        //get the number of columns based on the first row
        string[] headers = lines[0].Split(fieldSeperator, System.StringSplitOptions.None);
        int nrOfLanguages = headers.Length-1;
        //define the new list to put all the values in
        Dictionary<string, string>[] dictionaries = new Dictionary<string, string>[nrOfLanguages];
        for (int i = 0; i < dictionaries.Length; i++)
        {
            dictionaries[i] = new Dictionary<string, string>();
        }

        // i = current row, j = current column
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] fields = CSVParser.Split(line);
            var key = fields[0];
            // remove spaces and newlines
            key = key.TrimStart(' ', surround).TrimEnd('\n', '\r', surround);
            if (key == "") continue;

            for (int j = 1; j < Mathf.Min(fields.Length, nrOfLanguages+1); j++)
            {
                // remove spaces and newlines
                fields[j] = fields[j].TrimStart(' ', surround);
                fields[j] = fields[j].TrimEnd('\n', '\r', surround);

                //check if the key is not yet present and add to the dictionary
                if (dictionaries[j-1].ContainsKey(key)) continue;
                var value = fields[j];
                //Debug.Log(key+ ": "+ value);
                dictionaries[j-1].Add(key, value);
            }
        }
        return dictionaries;
    }

#if UNITY_EDITOR
    public void Add(string key, string valueNL, string valueEN)
    {
        string append = string.Format("\n\"" + key + "\",\"" + valueNL + "\",\"" + valueEN + "\"");

        File.AppendAllText("Assets/Resources/localisation.csv", append);
        UnityEditor.AssetDatabase.Refresh();
    }

    public void Remove(string key)
    {
        string[] lines = csvFile.text.Split(lineSeperator);
        string[] keys = new string[lines.Length];

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            keys[i] = line.Split(fieldSeperator, System.StringSplitOptions.None)[0];
        }
        int index = -1;

        for (int i = 0; i < keys.Length; i++)
        {
            if (keys[i].Contains(key))
            {
                index = i;
                break;
            }
        }

        if(index > -1)
        {
            string[] newLines;
            newLines = lines.Where(w => w != lines[index]).ToArray();

            string replaced = string.Join(lineSeperator.ToString(), newLines);
            File.WriteAllText("Assets/Resources/localisation.csv", replaced);
        }
    }

    public void Edit(string key, string newValueNL = "", string newValueEN = "")
    {
        Remove(key);
        Add(key, newValueNL, newValueEN);
    }
#endif

}
