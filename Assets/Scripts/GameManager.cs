using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//*********** The GameManager manages all the global settings and methods ******************//

public class GameManager : MonoBehaviour
{
    [Header ("Global Parameters")]
    [Tooltip ("the errormargin for answers")]
    public float errorMargin = 0.1f;
    public static float worldScale = 10f;
    [Tooltip ("the size of the interactable screen in world dimentions")]
    public Vector2 screenMin;
    public Vector2 screenMax;
    [Header("CampaignOrder")]
    public int[] campaignLevel;

    [HideInInspector]
    public static int playerScore;
    private Text scoreText;

    public static int highestLevel;
    public static int currentLevel;


    // Start is called before the first frame update
    void Start()
    {
        scoreText = GameObject.FindGameObjectWithTag("scoreText").GetComponent<Text>();
        if (scoreText)
        {
            scoreText.text = playerScore.ToString();
        }
        
        
    }


//score Control
    // increases the score by a set amount
    public void IncreaseScore(int amount)
    {
        playerScore += amount;
        scoreText.text = playerScore.ToString();
    }


//scenemanagement
    public void LoadScene (int sceneNr)
    {
        SceneManager.LoadScene(sceneNr);
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
        float answerNr = float.Parse(answer);
        Debug.Log(Mathf.Abs(correct - answerNr) < errorMargin);
        if (Mathf.Abs(correct - answerNr) < errorMargin)
        {
            return true;
        }
        return false;
    }
}
