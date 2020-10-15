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
    [Tooltip("the amount of scenes of the 3 Campaign2 chapters, the total must be equal to nrOfCamp2Levels")]
    public int[] NrOfCamp2ChapterScenes;



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
            LoadRandomCamp(chapter);
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

    public void LoadRandomCamp(int chapter) // load a random level that is unlocked if logged in, or all if not
    {
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
            Debug.Log(correct + " = " + answerNr + " ? -> " + (Mathf.Abs(correct - answerNr) < errorMargin) + ", with errormargin: " + errorMargin);
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
