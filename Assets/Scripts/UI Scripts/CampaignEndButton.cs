using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(RectTransform))]
public class CampaignEndButton : MonoBehaviour
{

    [SerializeField]
    private Text levelNrText, scoreText, levelNameText, buttonText;
    [SerializeField]
    private Color goodColor, badColor;

    private RectTransform rect;
    private int targetSceneLevel;

    public void SetValues(float rectMin, float rectMax, int levelNr, int levelScore, bool Complete, int sceneNr, string sceneName)
    {
        //set the button dimentions

        rect = GetComponent<RectTransform>();

        rect.anchorMin = new Vector2(rectMin, rect.anchorMin.y);
        rect.anchorMax = new Vector2(rectMax, rect.anchorMax.y);

        //set the textcolor and values

        if (levelNrText)
        {
            levelNrText.text = (1 + levelNr).ToString();
            levelNrText.color = Complete? goodColor: badColor;
        }
        if(scoreText) scoreText.text = levelScore.ToString();
        if(levelNameText) levelNameText.text = sceneName;
        if (buttonText) buttonText.text = Complete ? "Replay" : "Play";

        targetSceneLevel = sceneNr;

        Debug.Log("buttonnr: " + levelNr + ", with Score: " + levelScore + ", SceneName: " + sceneName + " SceneNr: " + targetSceneLevel);
    }

    public void LoadThisLevel()
    {
        GameManager.LoadScene(targetSceneLevel, true);
    }
}
