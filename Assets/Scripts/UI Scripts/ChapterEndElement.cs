using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChapterEndElement : MonoBehaviour
{
    [SerializeField]
    private TextLocaliser levelNameText;
    [SerializeField]
    private Text scoreText, buttonText;
    [SerializeField]
    private Color goodColor, badColor;

    string levelName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUI(int levelScore, bool Complete, string sceneName)
    {
        //set the textcolor and values

        levelName = sceneName;
        if (levelNameText)
        {
            levelNameText.UpdateText(sceneName);
            
        }
        if (scoreText)
        {
            scoreText.text = levelScore.ToString();
            scoreText.color = Complete ? goodColor : badColor;
        }
        
        if (buttonText) buttonText.text = Complete ? "Replay" : "Play";
    }

    public void LoadLevel()
    {
        if (levelName == "") return;
        SceneManager.LoadScene(levelName);
    }
}
