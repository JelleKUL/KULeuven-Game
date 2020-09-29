using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardElement : MonoBehaviour
{
    [SerializeField]
    private Text placeText, nameText, scoreText;


    public void SetValues(int nr, string name, int score)
    {
        if(placeText) placeText.text = nr.ToString();
        if(nameText) nameText.text = name;
        if(scoreText) scoreText.text = score.ToString();

    }

    public void SetLayout(Color textColor, Font textFont, float fieldHeight, float offset)
    {
        placeText.color = textColor;
        placeText.font = textFont;

        nameText.color = textColor;
        nameText.font = textFont;

        scoreText.color = textColor;
        scoreText.font = textFont;

        GetComponent<RectTransform>().anchoredPosition = new Vector2(0,-offset);
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, fieldHeight * 0.95f);
    }

    public void SetPlayerEleement()
    {

    }
}
