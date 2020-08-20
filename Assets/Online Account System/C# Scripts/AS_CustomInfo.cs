/*
 * 		Account System - Custom Info
 * 
 * This class is used to store custom information for an account in a MySQL database.
 * 
 * You can use it to store anything from player preferences and progress,
 * to inventories, chat dialogues and any other custom classes, as long as
 * they are [Serializable].
 * 
 * The CustomInfo class is serialized and stored in XML format in the 'accounts'
 * table of your database, under the 'custominfo' field by AS_AccountManagement.UploadAccountInfoToDb
 * It is retrieved as a string and de-serialized by AS_AccountManagement.DownloadAccountInfoFromDb
 * 
 * All these are showcased in the demo scene. 
 * 
 */

using UnityEngine;
using System;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;


/// <summary>
/// Use this class to easily store custom class instances to the database.
/// </summary>
[Serializable]
public class AS_CustomInfo
{
    // Required to be serializable
    public AS_CustomInfo() { }


    // -------- ADD YOUR CUSTOM VARIABLES HERE --------
    // !! MAKE SURE THEY ARE [Serializable] !!
	// !! IF THEY ARE NOT, Add the [XmlIgnore] property on them
	// !! NOTE THAT ANY VARIABLE WHICH HAS A TRANSFORM PROPERTY CAN NOT BE SERIALIZED!

	[XmlIgnore]
	public GameObject someObject;

    // Altering these variables will cause the Demo script
    // named AS_AccountManagementGUI to stop working
    // (although you can probably edit it to suit your needs,
    // if you ever need an ingame editor for the CustomInfo class)

    public AS_PlayerClass playerClass = AS_PlayerClass.Mage;
    
    public AS_LevelInfo[] levels = new AS_LevelInfo[]
	{ new AS_LevelInfo ("Funky Town", 5),
		new AS_LevelInfo ("Spooky Town", 2) };

    public AS_StatInfo[] stats = new AS_StatInfo[]
	{ new AS_StatInfo ("Stamina", 40),
		new AS_StatInfo ("Intelligence", 75),
		new AS_StatInfo ("Agility", 20)};

}


/*	
 * -------Alter any class you wish below here although note that-----------
 * 		altering these classes will cause the Demo script
 * 		named AS_AccountManagementGUI to stop working
 * 		(although you can probably edit it to suit your needs,
 *     	if you need an ingame editor for the Custom Info class)
 * 
 */

public enum AS_PlayerClass { Warrior, Rogue, Mage, Priest }

[Serializable]
public class AS_LevelInfo
{

    // Required to be serializable
    public AS_LevelInfo() { }

    public bool show = false;

    public AS_LevelInfo(string _name, int _score)
    { name = _name; score = _score; }

    public string name = "";
    public int score = 0;


}

[Serializable]
public class AS_StatInfo
{

    // Required to be serializable
    public AS_StatInfo() { }

    public bool show = false;

    public AS_StatInfo(string _name, int _value)
    { name = _name; value = _value; }

    public string name = "";
    public int value = 0;

}

public static class AS_CustomInfoMethods
{

    public static AS_CustomInfo CustomInfoOnGUI(this AS_CustomInfo customInfo)
    {

        if (customInfo == null)
            return customInfo;

        GUILayout.BeginVertical();

        // Title

        // LEVELS
        List<AS_LevelInfo> levels = customInfo.levels.ToList();

        GUILayout.Label("");
        GUILayout.BeginHorizontal();
        GUILayout.Label("Level Info", GUILayout.Width(100));


        if (GUILayout.Button("Add Level", GUILayout.Width(100)))
        {
            levels.Add(new AS_LevelInfo());
        }
        GUILayout.EndHorizontal();
        foreach (AS_LevelInfo level in levels)
        {

            GUILayout.BeginHorizontal();

            GUILayout.Label("Name: ", GUILayout.Width(50));
            level.name = GUILayout.TextField(level.name, GUILayout.MinWidth(100), GUILayout.MaxWidth(150));
            GUILayout.Label("", GUILayout.Width(25));
            GUILayout.Label("Score: ", GUILayout.Width(50));
            level.score = (int)GUILayout.HorizontalSlider(level.score, 0, 10, GUILayout.MinWidth(50), GUILayout.MaxWidth(75));
            try
            {
                level.score = Mathf.Clamp(
                                           Convert.ToInt32(
                                GUILayout.TextField(level.score.ToString(), GUILayout.MinWidth(50), GUILayout.MaxWidth(75))),
                                           0, 10);
            }
            catch { }

            if (GUILayout.Button("Remove", GUILayout.Width(75)))
            {

                levels.Remove(level);
                break;
            }

            GUILayout.EndHorizontal();

		}

        customInfo.levels = levels.ToArray();

        // ITEMS
        List<AS_StatInfo> stats = customInfo.stats.ToList();
        GUILayout.Label("");

        GUILayout.BeginHorizontal();
        GUILayout.Label("Stat Info", GUILayout.Width(100));
        if (GUILayout.Button("Add Stat", GUILayout.Width(100)))
        {
            stats.Add(new AS_StatInfo());
        }
        GUILayout.EndHorizontal();
        foreach (AS_StatInfo stat in stats)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Name: ", GUILayout.Width(50));
            stat.name = GUILayout.TextField(stat.name, GUILayout.MinWidth(100), GUILayout.MaxWidth(150));
            GUILayout.Label("", GUILayout.Width(25));
            GUILayout.Label("Value: ", GUILayout.Width(50));
            stat.value = (int)GUILayout.HorizontalSlider(stat.value, 0, 100, GUILayout.MinWidth(50), GUILayout.MaxWidth(75));
            try
            {
                stat.value = Mathf.Clamp(
                                          Convert.ToInt32(GUILayout.TextField(stat.value.ToString(), GUILayout.MinWidth(50), GUILayout.MaxWidth(75))), 0, 100);
            }
            catch { }


            if (GUILayout.Button("Remove", GUILayout.Width(75)))
            {
                stats.Remove(stat);
                break;
            }

            GUILayout.EndHorizontal();


        }
        customInfo.stats = stats.ToArray();
        GUILayout.EndVertical();
        return customInfo;
    }

}