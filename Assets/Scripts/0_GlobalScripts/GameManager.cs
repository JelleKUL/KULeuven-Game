using System.Collections;
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

    // the static variables that are used all over the place
        public static bool showDebugAnswer = true;
        public static int loginID;
        public static string userName;
        public static int playerScore;
    
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

        SetPlayArea();
        if (usernameText) ShowUsername();

        if (adminNames.Contains(userName) || !isLoggedIn) showDebugAnswer = true;
        else showDebugAnswer = false;

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

        if (isLoggedIn)
        {
            accountInfo.TryToDownload(loginID, UploadScore); //downloads the player information
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
            for (int j = 0; j < chapterList.chapters[i].levels.Count; j++)
            {
                if(chapterList.chapters[i].levels[j] == sceneName)
                {
                    return new Vector2Int(i, j);
                }
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

        accountInfo.TryToUpload(loginID, OnUpload);
    }

    public int CountBoolTrue(bool[] boolArray)
    {
        int counter = 0;

        for (int i = 0; i < boolArray.Length; i++)
        {
            if (boolArray[i]) counter++;
        }

        return counter;
    }
    public static float RoundFloat(float input, int digits)
    {
        return Mathf.Round(input * Mathf.Pow(10, digits)) / (float)Mathf.Pow(10.0f, digits);
    }

}
