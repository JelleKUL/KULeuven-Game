using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//*********** The WaterpassingQuestions sets the required parameters for a specific question ******************//


public class WaterpassingQuestions : MonoBehaviour
{
    [Header("Predefined TextFields")]
    public Text questionText;
    public Text answerInputH;   
    public Text answerOutput;

    public enum QuestionType { Geen, Hoogteverschil2Punten, Afstand2Punten, Hoekfout, HoogteVerschilMeerPunten }
    [Tooltip("Kies het soort vraag voor de oefening")]
    public QuestionType SoortVraag;

    public int scoreIncrease = 1;

    private WaterPassingController waterpassing;
    private GameManager gm;
    private float correctAnswer;

    // awake is called before start
    void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        waterpassing = GetComponent<WaterPassingController>();

        SetQuestionType(SoortVraag);
    }


    //sets the parameters for the type of question
    public void SetQuestionType(QuestionType vraag)
    {
        switch (vraag)
        {
            case QuestionType.Geen:
                break;

            case QuestionType.Hoogteverschil2Punten:
                waterpassing.SetParameters(2, 2, 1, false, false, Vector2.zero, false, Vector2.zero);
                correctAnswer = waterpassing.correctHeight;

                break;

            case QuestionType.Afstand2Punten:
                waterpassing.SetParameters(2, 2, 1, true, false, Vector2.zero, false, Vector2.zero);
                correctAnswer = waterpassing.correctDistance;

                break;

            case QuestionType.Hoekfout:
                waterpassing.SetParameters(0, 1, 1, true, true, new Vector2(3,1), true, new Vector2(6,1));
                correctAnswer = waterpassing.correctErrorAngle;

                break;

            case QuestionType.HoogteVerschilMeerPunten:
                waterpassing.SetParameters(3, 3, 2, false, false, Vector2.zero, false, Vector2.zero);
                correctAnswer = waterpassing.correctHeight;

                break;
        }
    }



    //checks if the given anwser is correct
    public void CheckAnswer()
    {
        if (gm.CheckCorrectAnswer(answerInputH.text, correctAnswer))
        {
            gm.IncreaseScore(scoreIncrease);
            Debug.Log("true");
            waterpassing.ChangePoints();
        }
        else Debug.Log("false");

    }

    //displays the correct answer
    public void ShowAnswer()
    {
        answerOutput.text = "Het antwoord is: " + correctAnswer.ToString();

        waterpassing.ShowAnswer();



    }
}