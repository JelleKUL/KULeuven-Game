using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PolygonationQuestions : MonoBehaviour
{
    [Header("Predefined TextFields")]
    public Text questionText;
    public Text answerInputX;
    public Text answerInputY;
    public Text answerInputH;
    public Text answerOutput;

    public enum QuestionType { Geen, Coordinaat1Punt, Afstand2PuntenPolygoon, VoorwaardseInsnijding }
    [Tooltip("Kies het soort vraag voor de oefening")]
    public QuestionType SoortVraag;

    public int scoreIncrease;

    private PolygonLineController lineController;
    private ObjectPlacer placer;
    private GameManager gm;

    private float[] correctAnswerArray;
    private float correctAnswerX;
    private float correctAnswerY;
    private float correctAnswerH;

    // Start is called before the first frame update
    void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        lineController = GameObject.FindGameObjectWithTag("PolygonLine").GetComponent<PolygonLineController>();
        placer = GetComponent<ObjectPlacer>();

        SetQuestionType(SoortVraag);



    }

    // Update is called once per frame
    void Update()
    {

    }


    //sets the type of question, can be altered by another script
    public void SetQuestionType(QuestionType vraag)
    {
        switch (vraag)
        {
            case QuestionType.Geen:
                
                break;
            case QuestionType.Coordinaat1Punt:
                //start oefening Coordinate1Point
                lineController.SetVisibles(true, false, false, false, true, true, 2);
                correctAnswerArray = placer.PlaceCalculatePoints(1);
                correctAnswerX = correctAnswerArray[0];
                correctAnswerY = correctAnswerArray[1];
                questionText.text = "Bepaal het coördinaat van punt B";
                break;

            case QuestionType.Afstand2PuntenPolygoon:
                //start oefening DragEnDropEllips
                lineController.SetVisibles(true, false, false, false, true, true, 2);
                correctAnswerArray = placer.PlaceCalculatePoints(2);
                correctAnswerH = Mathf.Sqrt(Mathf.Pow(correctAnswerArray[2] - correctAnswerArray[0], 2) + Mathf.Pow(correctAnswerArray[3] - correctAnswerArray[1], 2));
                questionText.text = "Bepaal de afstand tussen A en B";
                break;

            case QuestionType.VoorwaardseInsnijding:
                //start oefening MinimaleGrote
                lineController.SetVisibles(false, true, true, false, false, false, 3);
                correctAnswerArray = placer.PlaceCalculatePoints(3);
                placer.PlaceObstacles(2);
                questionText.text = "Voorwaardse Insnijding bepaal C, A = x:" + correctAnswerArray[0]+", y:" + correctAnswerArray[1] + " B = x:" + correctAnswerArray[2] + ", y:" + correctAnswerArray[3];
                break;
        }
    }

    //checks if the given anwser is correct
    public void CheckAnswerH()
    {
        if (gm.CheckCorrectAnswer(answerInputH.text, correctAnswerH))
        {
            gm.IncreaseScore(scoreIncrease);
            Debug.Log("true");

        }
        else Debug.Log("false");

    }
    public void CheckAnswerXY()
    {
        if (gm.CheckCorrectAnswer(answerInputX.text, correctAnswerX) && gm.CheckCorrectAnswer(answerInputY.text, correctAnswerY))
        {
            gm.IncreaseScore(scoreIncrease);
            Debug.Log("true");

        }
        else Debug.Log("false");

    }
}
