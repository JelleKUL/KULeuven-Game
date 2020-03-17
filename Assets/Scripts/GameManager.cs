using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float errorMargin = 0.1f;
    public static int playerScore;

    public Vector2 screenMin;
    public Vector2 screenMax;

    private Text scoreText;

    // Start is called before the first frame update
    void Start()
    {
        scoreText = GameObject.FindGameObjectWithTag("scoreText").GetComponent<Text>();
        scoreText.text = playerScore.ToString();
    }


//score Control
    // increases th score by a set amount
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

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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


//aswer Control
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
