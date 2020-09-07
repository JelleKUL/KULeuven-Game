using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CampaignSelectUI : MonoBehaviour
{
    [SerializeField]
    private Text levelCamp1, scoreCamp1, levelCamp2, scoreCamp2, scoreFreeTotal;

    // Start is called before the first frame update
    void Start()
    {
        SetValues();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetValues()
    {
        if (levelCamp1 && scoreCamp1)
        {
            levelCamp1.text = GameManager.levelCamp1.ToString() + " / " + GameManager.scoreCamp1.Length;

            int scoreTotal = 0;
            foreach (var score in GameManager.scoreCamp1)
            {
                scoreTotal += score;
            }
            scoreCamp1.text = scoreTotal.ToString();
        }

        if (levelCamp2 && scoreCamp2)
        {
            levelCamp2.text = GameManager.levelCamp2.ToString() + " / " + GameManager.scoreCamp2.Length;

            int scoreTotal = 0;
            foreach (var score in GameManager.scoreCamp2)
            {
                scoreTotal += score;
            }
            scoreCamp2.text = scoreTotal.ToString();
        }
        if (scoreFreeTotal)
        {
            scoreFreeTotal.text = GameManager.scoreFreeTotal.ToString();
        }
    }
}
