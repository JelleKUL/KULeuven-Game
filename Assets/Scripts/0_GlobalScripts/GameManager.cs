﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//*********** The GameManager manages all the global settings and methods ******************//

public class GameManager : MonoBehaviour
{


    [Header("GameObjects")]
    [SerializeField]
    private Text usernameText;

    public static float worldScale = 10f;
    [SerializeField]
    private List<string> adminNames;

    [SerializeField]
    private ChapterListScriptableObject chapterList;

    [Tooltip("The scenes of the first Chapter (WaterPassing)")]
    [Scene] public string[] WaterpassingScenes;
    [Tooltip("The scenes of the second Chapter (map angle)")]
    [Scene] public string[] MapAngleScenes;
    [Tooltip("The scenes of the third Chapter (foutenpropagatie)")]
    [Scene] public string[] FoutenPropagatieScenes;
    [Tooltip("The scenes of the fourth Chapter (polygonatie)")]
    [Scene] public string[] PolygonatieScenes;



    // the static variables that are used all over the place
        public static bool showDebugAnswer = true;
        public static int loginID;
        public static string userName;
        public static int playerScore;
    
        public static int levelCamp1;
        public static int[] scoreCamp1 = new int[10];
        public static bool[] compLevelCamp1 = new bool[10];

        public static int levelCamp2;
        public static int[] scoreCamp2 = new int[13];
        public static bool[] compLevelCamp2 = new bool[13];

        public static int scoreFreeTotal = 0;
    
        public static int highestLevel;
        public static int currentLevel;
        public static bool isLoggedIn;
        public static bool campaignMode;

    public static List<ChapterInfo> chaptersInfos = new List<ChapterInfo>();

    [HideInInspector]
    public Vector2 screenMin, screenMax; // calculated based on the x&y scale of the gamemanager


    [HideInInspector]public int firstCamp1Level;
    [HideInInspector]public int nrOfCamp1Levels;
    [HideInInspector]public int firstCamp2Level;
    [HideInInspector]public int[] NrOfCamp2ChapterScenes;

    // a new instance for the account system
    private AS_AccountInfo accountInfo = new AS_AccountInfo();

    // activates before the start functions
    private void Awake()
    {
        if (chaptersInfos.Count == 0) ResetChapterInfos();

        SetSceneNrs();
        SetPlayArea();
        if (usernameText) ShowUsername();

        if (adminNames.Contains(userName) || !isLoggedIn) showDebugAnswer = true;
        else showDebugAnswer = false;

    }

    void SetSceneNrs()
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            if (SceneManager.GetSceneByBuildIndex(i).name == WaterpassingScenes[0]) firstCamp1Level = i;
            if (SceneManager.GetSceneByBuildIndex(i).name == MapAngleScenes[0]) firstCamp2Level = i;
        }

        //firstCamp1Level = SceneManager.GetSceneByName(WaterpassingScenes[0]).buildIndex;
        nrOfCamp1Levels = WaterpassingScenes.Length;
        //firstCamp2Level = SceneManager.GetSceneByName(MapAngleScenes[0]).buildIndex;
        NrOfCamp2ChapterScenes = new int[] { MapAngleScenes.Length, FoutenPropagatieScenes.Length, PolygonatieScenes.Length };
    }


    //score Control

    // increases the score by a set amount
    public void IncreaseScore(int amount)
    {
        string sceneName = SceneManager.GetActiveScene().name;

        playerScore += amount;
        if (campaignMode)
        {
            Vector2Int levelIndex = GetCurrentLevel();

            if(levelIndex.x <= 0)
            {
                if (chaptersInfos[levelIndex.x].scores[levelIndex.y] < amount) chaptersInfos[levelIndex.x].scores[levelIndex.y] = amount; //only update if score is higher
            }
        }
        /*
        if (campaignNr == 1 && campaignMode)
        {
            
            
            compLevelCamp1[sceneNr - firstCamp1Level] = true;

            levelCamp1 = CountBoolTrue(compLevelCamp1);
            

            if (scoreCamp1[sceneNr - firstCamp1Level] < amount)
            {
                scoreCamp1[sceneNr - firstCamp1Level] = amount;
            }
            
        }
        else if(campaignNr == 2 && campaignMode)
        {
           
            compLevelCamp2[sceneNr - firstCamp2Level] = true;

            levelCamp2 = CountBoolTrue(compLevelCamp2);

            if (scoreCamp2[sceneNr - firstCamp2Level] < amount)
            {
                scoreCamp2[sceneNr - firstCamp2Level] = amount;
            }
        }
        
        else
        {
            scoreFreeTotal += amount;
        }
        */

        if (isLoggedIn)
        {
            accountInfo.TryToDownload(loginID, UploadScore); //downloads the player information
        }

        /*
        foreach (var score in scoreText)
        {
            score.text = playerScore.ToString();
        }
        */
    }

    
    // sets the latest level the player has reached in the account system
    public void SetMaxLevel(int campaignNr)
    {
        if( campaignNr == 1)
        {
            int sceneNr = SceneManager.GetActiveScene().buildIndex;

            if(sceneNr - firstCamp1Level >= levelCamp1)
            {
                levelCamp1 = sceneNr - firstCamp1Level + 1;

            }
        }
        if (campaignNr == 2)
        {
            int sceneNr = SceneManager.GetActiveScene().buildIndex;

            if (sceneNr - firstCamp2Level >= levelCamp2)
            {
                levelCamp2 = sceneNr - firstCamp2Level + 1;
            }
        }
    }

    


//scenemanagement
    public static void LoadScene (int sceneNr, bool campaign)
    {
        campaignMode = campaign;

        SceneManager.LoadScene(sceneNr);
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadSceneName(string scene)
    {
        SceneManager.LoadScene(scene);
    }
    public void LoadSceneObject(Object scene)
    {
        SceneManager.LoadScene(scene.name);
    }

    public void LoadChapter(int nr)
    {

    }

    public void LoadCampaign1()
    {
        campaignMode = true; //set the campaignmode so the game can count the points to the campaign instead of free mode
        Debug.Log(firstCamp1Level + " & " + levelCamp1);
        //SceneManager.LoadScene(firstCamp1Level + levelCamp1);

       

        for (int i = 0; i < compLevelCamp1.Length; i++) // checks the first uncompleted level
        {
            if (!compLevelCamp1[i])
            {
                Debug.Log(i + " Completed");

                SceneManager.LoadScene(firstCamp1Level + i);

                return;
            }
        }

        SceneManager.LoadScene(firstCamp1Level); // loads the first level if all levels are completed

    }
    public void LoadCampaign2()
    {
        campaignMode = true;
        Debug.Log(firstCamp2Level + " & " + levelCamp2);
        //SceneManager.LoadScene(firstCamp2Level + levelCamp2);

        for (int i = 0; i < compLevelCamp2.Length; i++) // checks the first uncompleted level
        {
            if (!compLevelCamp2[i])
            {
                SceneManager.LoadScene(firstCamp2Level + i);

                return;
            }
        }

        SceneManager.LoadScene(firstCamp2Level); // loads the first level if all levels are completed
    }
    public void LoadFreeMode()
    {
        campaignMode = false;
    }

    public void SkipLevel(int chapter)
    {
        if (campaignMode)
        {
            LoadNextScene();
        }
        else
        {
            LoadRandomCamp();
        }
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // loading random available scenes

    public void LoadRandomCamp() // load a random level that is unlocked if logged in, or all if not
    {

        campaignMode = false;

        int currentChapter = GetCurrentLevel().x;

        if (currentChapter >= 0)
        {
            List<string> availableScenes = new List<string>();
            availableScenes.Add(chapterList.chapters[currentChapter].levels[0]);

            for (int i = 1; i < chapterList.chapters[currentChapter].levels.Count; i++)
            {
                if (!isLoggedIn || chaptersInfos[currentChapter].scores[i] != 0)
                {
                    availableScenes.Add(chapterList.chapters[currentChapter].levels[i]);
                }
            }

            SceneManager.LoadScene(availableScenes[Random.Range(0, availableScenes.Count)]);
        }

        

        /*
        int firstLevel = firstCamp1Level;

        if (chapter == 0)
        {
            if (isLoggedIn)
            {
                List<int> availableLevels = new List<int>();

                for (int i = 0; i < compLevelCamp1.Length; i++)
                {
                    if (compLevelCamp1[i])
                    {
                        availableLevels.Add(i);
                    }
                }
                Debug.Log("Loading scene: " + (firstLevel + availableLevels[Random.Range(0, availableLevels.Count)]));
                SceneManager.LoadScene(firstLevel + availableLevels[Random.Range(0, availableLevels.Count)]);
            }
            else
            {
                SceneManager.LoadScene(firstLevel + Random.Range(0, nrOfCamp1Levels));
            }
        }

        else
        {
            firstLevel = firstCamp2Level;

            for (int i = 1; i < chapter; i++)
            {
                firstLevel += NrOfCamp2ChapterScenes[i-1];

            }

            if (isLoggedIn)
            {
                List<int> availableLevels = new List<int>();

                for (int i = 0; i < NrOfCamp2ChapterScenes[chapter - 1]; i++)
                {
                    if (compLevelCamp2[firstLevel - firstCamp2Level + i])
                    {
                        availableLevels.Add(i);
                    }
                }
                Debug.Log("Loading scene: " + (firstLevel + availableLevels[Random.Range(0, availableLevels.Count)]) + ", from chapter: " + chapter);
                SceneManager.LoadScene(firstLevel + availableLevels[Random.Range(0, availableLevels.Count)]);
            }
            else
            {
                Debug.Log("Loading scene: " + firstLevel +  Random.Range(0, NrOfCamp2ChapterScenes[chapter - 1]));
                SceneManager.LoadScene(firstLevel + Random.Range(0, NrOfCamp2ChapterScenes[chapter - 1]));
            }
        }
        */

    }

    public void LogOut()
    {
        isLoggedIn = false;
        SceneManager.LoadScene("LoginMenu");

         // the static variables that are used all over the place
        loginID = -1;
        userName = "";
        playerScore = 0;

        //resets the chapter info list
        ResetChapterInfos();

        /*
        levelCamp1 = 0;
        scoreCamp1 = new int[10];
        compLevelCamp1 = new bool[10];

        levelCamp2 = 0;
        scoreCamp2 = new int[13];
        compLevelCamp2 = new bool[13];

        scoreFreeTotal = 0;
        //scoreFree = new int[13];
        */
        highestLevel = 0;
        currentLevel = 0;
    }

    /// <summary>
    /// get the current idexes of the level
    /// </summary>
    /// <returns>x: the current chapter, y: the current level</returns>
    public Vector2Int GetCurrentLevel()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        for (int i = 0; i < chapterList.chapters.Count; i++) //go over every chapter
        {
            if (chapterList.chapters[i].levels.Contains(sceneName))
            {
                int index = chapterList.chapters[i].levels.IndexOf(sceneName);
                return new Vector2Int(i, index);
            }
        }
        return new Vector2Int(-1, -1);

    }



    //mouse Navigation
    // checks if position is in the boundary
    public bool IsBetweenValues(Vector2 check)
    {
        if (check.x < screenMax.x && check.y < screenMax.y && check.x > screenMin.x && check.y > screenMin.y) return true;
        else return false;
    }
    // translates a mousposition into a worldposition with offset z
    public Vector3 SetObjectToMouse(Vector2 mousePos, float z)
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, z-Camera.main.transform.position.z));
    }

    

    public static float RoundFloat(float input, int digits)
    {
        return Mathf.Round(input * Mathf.Pow(10, digits)) / (float)Mathf.Pow(10.0f, digits);
    }

// playAreaSetting
    void SetPlayArea()
    {
        screenMin.x = transform.position.x - transform.localScale.x / 2f;
        screenMax.x = transform.position.x + transform.localScale.x / 2f;

        screenMin.y = transform.position.y - transform.localScale.y / 2f;
        screenMax.y = transform.position.y + transform.localScale.y / 2f;
    }

// accountSystemControl

    // create a new info list to store all the player data in the database
    void ResetChapterInfos()
    {
        chaptersInfos = new List<ChapterInfo>();

        for (int i = 0; i < chapterList.chapters.Count; i++)
        {
            chaptersInfos.Add(new ChapterInfo(chapterList.chapters[i]));
        }
    }

    // updates the info list to match the games new chapters if there are any
    void UpdateChapterInfos()
    {
        for (int i = 0; i < chapterList.chapters.Count; i++)
        {
            bool chapterInData = false;
            for (int j = 0; j < chaptersInfos.Count; j++)
            {
                if (chapterList.chapters[i].UID == chaptersInfos[j].UID)
                {
                    chapterInData = true;
                    break;
                }
            }
            if(chapterInData == false) chaptersInfos.Add(new ChapterInfo(chapterList.chapters[i]));
        }
    }

    void ShowUsername()
    {
        if (isLoggedIn) usernameText.text = userName;
        else usernameText.text = "Gast";
    }

    // This is called when the upload has finished
    void OnUpload(string message)
    {
        this.Log(LogType.Log, "Uploaded Successfully the following account info:\n" + accountInfo.ToReadableString());
    }

    void UploadScore(string message)
    {
        Debug.Log("gamemanager: id= " + accountInfo.GetFieldValue("id") + " name= " + accountInfo.GetFieldValue("username") + " score: " + accountInfo.customInfo.totalScore);

        accountInfo.customInfo.totalScore = playerScore;
        accountInfo.customInfo.chapters = chaptersInfos;

        /*
        accountInfo.customInfo.levelCamp1 = levelCamp1;
        accountInfo.customInfo.scoreCamp1 = scoreCamp1;
        accountInfo.customInfo.compLevelCamp1 = compLevelCamp1;

        accountInfo.customInfo.levelCamp2 = levelCamp2;
        accountInfo.customInfo.scoreCamp2 = scoreCamp2;
        accountInfo.customInfo.compLevelCamp2 = compLevelCamp2;

        accountInfo.customInfo.scoreFreeTotal = scoreFreeTotal;
        */

        accountInfo.TryToUpload(loginID, OnUpload);
    }
    /*
    private void UpdateArrayLengths()
    {
        Debug.Log("gamemanager: id= " + accountInfo.GetFieldValue("id") + " name= " + accountInfo.GetFieldValue("username") + " score: " + accountInfo.customInfo.totalScore);

        accountInfo.customInfo.totalScore = playerScore;

        accountInfo.customInfo.levelCamp1 = levelCamp1;
        accountInfo.customInfo.scoreCamp1 = scoreCamp1;
        accountInfo.customInfo.compLevelCamp1 = compLevelCamp1;

        accountInfo.customInfo.levelCamp2 = levelCamp2;
        accountInfo.customInfo.scoreCamp2 = scoreCamp2;
        accountInfo.customInfo.compLevelCamp2 = compLevelCamp2;

        accountInfo.customInfo.scoreFreeTotal = scoreFreeTotal;

        accountInfo.TryToUpload(loginID, OnUpload);
    }
     */
    public int CountBoolTrue(bool[] boolArray)
    {
        int counter = 0;

        for (int i = 0; i < boolArray.Length; i++)
        {
            if (boolArray[i]) counter++;
        }

        return counter;
    }
   
}
