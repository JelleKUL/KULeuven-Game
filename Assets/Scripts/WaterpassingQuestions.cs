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

    public enum QuestionType { Geen, Hoogteverschil2Punten, Afstand2Punten, Hoekfout, HoogteVerschilMeerPunten, Scheefstand, OmgekeerdeBaak }
    [Tooltip("Kies het soort vraag voor de oefening")]
    public QuestionType SoortVraag;

    public int scoreIncrease = 1;

    private WaterPassingController waterpassing;
    private GameManager gm;
    private float correctAnswer;
    private float[] correctPoints;

    // awake is called before start
    void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        waterpassing = GetComponent<WaterPassingController>();

        SetQuestionType(SoortVraag);
    }
    public void ResetCurrentQuestion()
    {
        switch (SoortVraag)
        {
            case QuestionType.Geen:
                break;

            case QuestionType.Hoogteverschil2Punten:
                waterpassing.ChangePoints();
                correctAnswer = waterpassing.correctHeight;

                break;

            case QuestionType.Afstand2Punten:
                waterpassing.ChangePoints();
                correctAnswer = waterpassing.correctDistance;

                break;

            case QuestionType.Hoekfout:
                waterpassing.ChangePoints();
                correctAnswer = waterpassing.correctErrorAngle;

                break;

            case QuestionType.HoogteVerschilMeerPunten:
                waterpassing.ChangePoints();
                correctAnswer = waterpassing.correctHeight;

                break;

            case QuestionType.Scheefstand:
                waterpassing.ChangePoints();
                correctAnswer = waterpassing.correctHeight;

                break;
        }
    }

    //sets the parameters for the type of question
    public void SetQuestionType(QuestionType vraag)
    {
        switch (vraag)
        {
            case QuestionType.Geen:
                break;

            case QuestionType.Hoogteverschil2Punten:
                waterpassing.SetParameters(2, 2, 1, false, false, Vector2.zero, false, Vector2.zero, false);
                correctAnswer = waterpassing.correctHeight;

                break;

            case QuestionType.Afstand2Punten:
                waterpassing.SetParameters(2, 2, 1, true, false, Vector2.zero, false, Vector2.zero, false);
                correctAnswer = waterpassing.correctDistance;

                break;

            case QuestionType.Hoekfout:
                waterpassing.SetParameters(0, 1, 1, true, true, new Vector2(4,1), true, new Vector2(7,1), false);
                correctAnswer = waterpassing.correctErrorAngle;

                break;

            case QuestionType.HoogteVerschilMeerPunten:
                waterpassing.SetParameters(3, 5, 3, false, false, Vector2.zero, false, Vector2.zero, true);
                correctPoints = waterpassing.correctHeightDifferences;

                break;

            case QuestionType.Scheefstand:
                waterpassing.SetParameters(2, 2, 1, false, false, Vector2.zero, false, Vector2.zero, false);
                correctAnswer = waterpassing.correctHeight;

                break;

            case QuestionType.OmgekeerdeBaak:
                waterpassing.SetParameters(1, 2, 1, false, false, Vector2.zero, false, Vector2.zero, false);
                correctAnswer = waterpassing.correctHeight;

                break;
        }
        Debug.Log("correctAnswer");
    }



    //checks if the given anwser is correct
    public void CheckAnswer()
    {
        
        if (gm.CheckCorrectAnswer(answerInputH.text, CorrectAnswer()))
        {
            gm.IncreaseScore(scoreIncrease);
            Debug.Log("true");
            gm.ReloadScene();
        }
        else Debug.Log("false");

    }

    public void CheckAnswerArray()
    {
        if (waterpassing.CheckTabelAnswer())
        {
            gm.IncreaseScore(scoreIncrease);
            Debug.Log("true");
            //gm.ReloadScene();
            answerOutput.text = "De Waarden die zijn ingevoerd zijn correct";
        }
        else
        {
            answerOutput.text = "De Waarden die zijn ingevoerd zijn niet correct";
            Debug.Log("false");
        }
    }

    //displays the correct answer
    public void ShowAnswer()
    {
        answerOutput.text = "Het antwoord is: " + CorrectAnswer().ToString();

        waterpassing.ShowAnswer();

    }

    public float CorrectAnswer()
    {
        switch (SoortVraag)
        {
            case QuestionType.Hoogteverschil2Punten:

                return waterpassing.correctHeight;


            case QuestionType.Afstand2Punten:

                return waterpassing.correctDistance;


            case QuestionType.Hoekfout:

                return waterpassing.correctErrorAngle;


            case QuestionType.HoogteVerschilMeerPunten:

                return waterpassing.correctHeight;

            case QuestionType.Scheefstand:

                return waterpassing.correctHeight;

        }
        return 0;
    }
}