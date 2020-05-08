using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//*********** The PolygonationQuestions sets the required parameters for a specific question ******************//


public class PolygonationQuestions : MonoBehaviour
{
    [Header("Predefined TextFields")]
    public Text questionText;
    public Text answerInputX;
    public Text answerInputY;
    public Text answerInputH;
    public Text answerOutput;

    public enum QuestionType { Geen, Coordinaat1Punt, Afstand2PuntenPolygoon, VoorwaardseInsnijding, AchterwaardseInsnijding }
    [Tooltip("Kies het soort vraag voor de oefening")]
    public QuestionType SoortVraag;

    public int scoreIncrease;

    private PolygonLineController lineController;
    private ObjectPlacer placer;
    private GameManager gm;

    private float[] correctAnswerArray;
    private float[] obsructedPointsArray;
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
    public void ResetCurrentQuestion()
    {
        SetQuestionType(SoortVraag);
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
                //start oefening voorwaardse insnijding
                lineController.SetVisibles(false, true, true, false, false, false, 3);
                obsructedPointsArray = placer.PlaceObstructedCalculatePoints(1);
                correctAnswerArray = placer.PlaceCalculatePoints(2);
                
                placer.PlaceObstacles(2);
                correctAnswerX = obsructedPointsArray[0];
                correctAnswerY = obsructedPointsArray[1];
                questionText.text = "Voorwaardse Insnijding bepaal P, A = x:" + correctAnswerArray[0]+", y:" + correctAnswerArray[1] + " B = x:" + correctAnswerArray[2] + ", y:" + correctAnswerArray[3];
                break;

            case QuestionType.AchterwaardseInsnijding:
                //start oefening achterwaardse insnijding
                lineController.SetVisibles(false, true, true, false, false, false, 3);
                correctAnswerArray = placer.PlaceCalculatePoints(1);
                obsructedPointsArray = placer.PlaceObstructedCalculatePoints(3);
                correctAnswerX = correctAnswerArray[0];
                correctAnswerY = correctAnswerArray[1];
                //placer.PlaceObstacles(2);
                questionText.text = "Achterwaardse Insnijding bepaal P, A = x:" + obsructedPointsArray[0] + ", y:" + obsructedPointsArray[1] + " B = x:" + obsructedPointsArray[2] + ", y:" + obsructedPointsArray[3] + " C = x:" + obsructedPointsArray[4] + ", y:" + obsructedPointsArray[5];
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

    // checks if a given coordinate is correct
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
