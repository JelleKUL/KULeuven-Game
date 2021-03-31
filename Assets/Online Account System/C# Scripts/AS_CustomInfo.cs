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
    
    public int totalScore = 0;

    //old data
    public int levelCamp1 = 0;
    public int [] scoreCamp1 = new int[10];
    public bool[] compLevelCamp1 = new bool[10];

    public int levelCamp2 = 0;
    public int[] scoreCamp2 = new int[14];
    public bool[] compLevelCamp2 = new bool[14];

    public int scoreFreeTotal = 0;
    public int[] scoreFree = new int[24];
    

    //new DataStorageSystem
    public List<ChapterInfo> chapters = new List<ChapterInfo>();



}

[Serializable]
public class ChapterInfo
{
    // Required to be serializable
    public ChapterInfo() { }

    public ChapterInfo(ChapterScriptableObject chapter)
    {
        UID = chapter.UID;
        for (int j = 0; j < chapter.levels.Count; j++)
        {
            scores.Add(0);
        }
    }
    // the unique ID to link the scores to the correct chapter, even if the order changes
    public string UID = "";
    // the scores for each level in the chapter
    public List<int> scores = new List<int>();


}

