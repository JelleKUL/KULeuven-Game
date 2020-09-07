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
    public float errorMargin = 0.1f;
    public static float worldScale = 10f;


    [Header("CampaignOrder")]
    public int firstCamp1Level;
    public int nrOfCamp1Levels;
    public int firstCamp2Level;
    public int nrOfCamp2Levels;

    [HideInInspector]
    public static int loginID;
    public static string userName;
    public static int playerScore;

    public static int levelCamp1;
    public static int[] scoreCamp1 = new int[11];
    public static int levelCamp2;
    public static int[] scoreCamp2 = new int[13];
    public static int scoreFreeTotal = 0;
    //public static int[] scoreFree = new int[13];
    private Text[] scoreText;

    [HideInInspector]
    public Vector2 screenMin, screenMax;

    public static int highestLevel;
    public static int currentLevel;
    public static bool isLoggedIn;
    public static bool campaignMode;

    private AS_AccountInfo accountInfo = new AS_AccountInfo();

    private void Awake()
    {
        SetPlayArea();

        if (usernameText) ShowUsername();
    }

    // Start is called before the first frame update
    void Start()
    {
        

        
        
    }


//score Control
    // searches all the active score displays at the start of the scene
    private void SearchScoreTexts()
    {
        GameObject[] scoreObjects = GameObject.FindGameObjectsWithTag("scoreText");
        scoreText = new Text[scoreObjects.Length];

        for (int i = 0; i < scoreText.Length; i++)
        {
            scoreText[i] = scoreObjects[i].GetComponent<Text>();
        }

        if (scoreText.Length > 0)
        {
            foreach (var score in scoreText)
            {
                score.text = playerScore.ToString();
            }


        }
    }
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
        campaignMode = true;
        Debug.Log(firstCamp1Level + " & " + levelCamp1);
        SceneManager.LoadScene(firstCamp1Level + levelCamp1);
    }
    public void LoadCampaign2()
    {
        campaignMode = true;
        Debug.Log(firstCamp2Level + " & " + levelCamp2);
        SceneManager.LoadScene(firstCamp2Level + levelCamp2);
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
        accountInfo.customInfo.levelCamp2 = levelCamp2;
        accountInfo.customInfo.scoreCamp2 = scoreCamp2;
        accountInfo.customInfo.scoreFreeTotal = scoreFreeTotal;

        accountInfo.TryToUpload(loginID, OnUpload);
    }
}
