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
    public int[] campaignLevel;

    [HideInInspector]
    public static int loginID;
    public static string userName;
    public static int playerScore;
    private Text[] scoreText;

    [HideInInspector]
    public Vector2 screenMin, screenMax;

    public static int highestLevel;
    public static int currentLevel;

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
    public void IncreaseScore(int amount)
    {
        playerScore += amount;

        accountInfo.customInfo.totalScore = playerScore;

        accountInfo.TryToDownload(loginID, UploadScore);

        

        /*
        foreach (var score in scoreText)
        {
            score.text = playerScore.ToString();
        }
        */
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

        accountInfo.TryToUpload(loginID, OnUpload);
    }
}
