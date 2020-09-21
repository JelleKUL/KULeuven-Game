using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//**************** The UI manager for the campaign select menu ******************//

public class CampaignSelectUI : MonoBehaviour
{
    [SerializeField]
    [Tooltip("the textfield which should be changed")]
    private Text levelCamp1, scoreCamp1, levelCamp2, scoreCamp2, scoreFreeTotal;

    // Start is called before the first frame update
    void Start()
    {
        SetValues();
    }

    // sets the text fields to the correct values from the gamemanager's static variables from the login
    void SetValues()
    {
        if (levelCamp1 && scoreCamp1) //if added
        {
            levelCamp1.text = GameManager.levelCamp1.ToString() + " / " + GameManager.scoreCamp1.Length;

            int scoreTotal = 0;
            foreach (var score in GameManager.scoreCamp1)
            {
                scoreTotal += score;
            }
            scoreCamp1.text = scoreTotal.ToString();
        }

        if (levelCamp2 && scoreCamp2) //if added
        {
            levelCamp2.text = GameManager.levelCamp2.ToString() + " / " + GameManager.scoreCamp2.Length;

            int scoreTotal = 0;
            foreach (var score in GameManager.scoreCamp2)
            {
                scoreTotal += score;
            }
            scoreCamp2.text = scoreTotal.ToString();
        }
        if (scoreFreeTotal) //if added
        {
            scoreFreeTotal.text = GameManager.scoreFreeTotal.ToString();
        }
    }
}
