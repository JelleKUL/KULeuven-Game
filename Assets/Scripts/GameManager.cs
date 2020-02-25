using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float errorMargin = 0.1f;
    public static int playerScore;

    public Vector2 min;
    public Vector2 max;

    private Text scoreText;

    // Start is called before the first frame update
    void Start()
    {
        scoreText = GameObject.FindGameObjectWithTag("scoreText").GetComponent<Text>();
        scoreText.text = playerScore.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreaseScore(int amount)
    {
        playerScore += amount;
        scoreText.text = playerScore.ToString();
    }

    public void LoadScene (int sceneNr)
    {
        SceneManager.LoadScene(sceneNr);
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public bool IsBetweenValues(Vector2 check)
    {
        if (check.x < max.x && check.y < max.y && check.x > min.x && check.y > min.y) return true;
        else return false;
    }

    public Vector3 SetObjectToMouse(Vector2 mousePos, float z)
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, z-Camera.main.transform.position.z));
    }
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
