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

    [Header("PlayField")]
    [Header ("Global Parameters")]
    [Tooltip ("the errormargin for answers")]
    public float errorMargin = 0.001f;
    public static float worldScale = 10f;


    [Header("CampaignOrder")]
    [Tooltip("the number of the first scene of campaign 1 in the build settings")]
    public int firstCamp1Level;
    [Tooltip("the number of scenes/levels of campaign 1 in the build settings")]
    public int nrOfCamp1Levels;
    [Tooltip("the number of the first scene of campaign 2 in the build settings")]
    public int firstCamp2Level;
    [Tooltip("the number of scenes/levels of campaign 1 in the build settings")]
    public int nrOfCamp2Levels;

    [Header("ShowAnswersInDebug")]
    public static bool showDebugAnswer = true; 

    // the static variables that are used all over the place
        public static int loginID;
        public static string userName;
        public static int playerScore;

        public static int levelCamp1;
        public static int[] scoreCamp1 = new int[10];
        public static bool[] compLevelCamp1 = new bool[10];

        public static int levelCamp2;
        public static int[] scoreCamp2 = new int[14];
        public static bool[] compLevelCamp2 = new bool[14];

        public static int scoreFreeTotal = 0;
        //public static int[] scoreFree = new int[13];

        public static int highestLevel;
        public static int currentLevel;
        public static bool isLoggedIn;
        public static bool campaignMode;


    [HideInInspector]
    public Vector2 screenMin, screenMax; // calculated based on the x&y scale of the gamamanager

    // a new instance for the account system
    private AS_AccountInfo accountInfo = new AS_AccountInfo();

    // activates before the start functions
    private void Awake()
    {
        SetPlayArea();

        if (usernameText) ShowUsername();

    }


//score Control
    
    // increases the score by a set amount
    public void IncreaseScore(int amount, int campaignNr)
    {
        int sceneNr = SceneManager.GetActiveScene().buildIndex;

        playerScore += amount;

        
        if (campaignNr == 1 && campaignMode)
        {
            if (sceneNr - firstCamp1Level >= levelCamp1)
            {
                levelCamp1 = sceneNr - firstCamp1Level + 1;
                compLevelCamp1[levelCamp1 - 1] = true;
            }

            if (scoreCamp1[sceneNr - firstCamp1Level] < amount)
            {
                scoreCamp1[sceneNr - firstCamp1Level] = amount;
            }
            
        }
        else if(campaignNr == 2 && campaignMode)
        {
            if (sceneNr - firstCamp2Level >= levelCamp2)
            {
                levelCamp2 = sceneNr - firstCamp2Level + 1;
                compLevelCamp2[levelCamp2 - 1] = true;
            }

            if (scoreCamp2[sceneNr - firstCamp2Level] < amount)
            {
                scoreCamp2[sceneNr - firstCamp2Level] = amount;
            }
        }
        else
        {
            scoreFreeTotal += amount;
        }

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
    public void LoadScene (int sceneNr)
    {
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

    public void LoadCampaign1()
    {
        campaignMode = true; //set the campaignmode so the game can count the points to the campaign instead of free mode
        Debug.Log(firstCamp1Level + " & " + levelCamp1);
        //SceneManager.LoadScene(firstCamp1Level + levelCamp1);

        for (int i = 0; i < compLevelCamp1.Length; i++) // checks the first uncompleted level
        {
            if (!compLevelCamp1[i])
            {
                SceneManager.LoadScene(firstCamp1Level + i);

                break;
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

                break;
            }
        }

        SceneManager.LoadScene(firstCamp2Level); // loads the first level if all levels are completed
    }
    public void LoadFreeMode()
    {
        campaignMode = false;
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LogOut()
    {
        isLoggedIn = false;
        SceneManager.LoadScene("LoginMenu");
    }

//CampaignManagement
    


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


//answer Control
    // checks if a given string equals the float value minus the error margin
    public bool CheckCorrectAnswer(string answer, float correct)
    {
        answer = answer.Replace(",",".");

        float answerNr;
        if(float.TryParse(answer,out answerNr))
        {
            Debug.Log(correct + " = " + answerNr + " ? -> " + (Mathf.Abs(correct - answerNr) < errorMargin));
            if (Mathf.Abs(correct - answerNr) < errorMargin)
            {
                return true;
            }
            return false;
        }
        return false;
       
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

    void ShowUsername()
    {
        usernameText.text = userName;
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

        accountInfo.customInfo.levelCamp1 = levelCamp1;
        accountInfo.customInfo.scoreCamp1 = scoreCamp1;
        accountInfo.customInfo.compLevelCamp1 = compLevelCamp1;

        accountInfo.customInfo.levelCamp2 = levelCamp2;
        accountInfo.customInfo.scoreCamp2 = scoreCamp2;
        accountInfo.customInfo.compLevelCamp2 = compLevelCamp2;

        accountInfo.customInfo.scoreFreeTotal = scoreFreeTotal;

        accountInfo.TryToUpload(loginID, OnUpload);
    }

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
}
