using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private string pointsSuffix = "pts";

    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = GameManager.playerScore.ToString() + " " + pointsSuffix;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
