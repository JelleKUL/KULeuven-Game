using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//*********** The PolygonationQuestions sets the required parameters for a specific question ******************//


public class PolygonationQuestions : MonoBehaviour
{
    [Header("Predefined TextFields")]
    public Text titleQuestionText;
    public Text questionText;
    public Text answerInputX;
    public Text answerInputY;
    public Text answerInputH;
    public Text answerOutput;

    public GameObject winMenu;
    public Color falseColor, CorrectColor;


    public enum QuestionType { Geen, Coordinaat1Punt, Afstand2PuntenPolygoon, VoorwaardseInsnijding, AchterwaardseInsnijding, Tabel, Bilateratie }
    [Tooltip("Kies het soort vraag voor de oefening")]
    public QuestionType SoortVraag;

    public int scoreIncrease;
  

    private PolygonLineController lineController;
    private ObjectPlacer placer;
    public PolygonatieLoopTabel tabel;
    private GameManager gm;

    private float[] correctAnswerArray;
    private float[] obsructedPointsArray;
    private float correctAnswerX;
    private float correctAnswerY;
    private float correctAnswerH;
    private string correctAnswer;

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
                correctAnswerX = correctAnswerArray[0] * GameManager.worldScale;
                correctAnswerY = correctAnswerArray[1] * GameManager.worldScale;
                correctAnswer = "X: " + correctAnswerX + ", Y: " + correctAnswerY;

                titleQuestionText.text = "Bepaal het coördinaat van punt P";
                questionText.text = "Met behulp van de afstand en map angle vanaf het meetpunt.";
                break;

            case QuestionType.Afstand2PuntenPolygoon:
                //start oefening DragEnDropEllips
                lineController.SetVisibles(true, false, false, false, true, true, 2);
                correctAnswerArray = placer.PlaceCalculatePoints(2);
                correctAnswerH = GameManager.worldScale * Mathf.Sqrt(Mathf.Pow(correctAnswerArray[2] - correctAnswerArray[0], 2) + Mathf.Pow(correctAnswerArray[3] - correctAnswerArray[1], 2));
                correctAnswer = correctAnswerH.ToString();

                titleQuestionText.text = "Bepaal de afstand tussen A en B";
                questionText.text = "Met behulp van de afstand en map angle vanaf het meetpunt.";
                break;

            case QuestionType.VoorwaardseInsnijding:
                //start oefening voorwaardse insnijding
                lineController.SetVisibles(false, true, true, false, false, false, 3);
                obsructedPointsArray = placer.PlaceObstructedCalculatePoints(1);
                correctAnswerArray = placer.PlaceCalculatePoints(2);
                
                //placer.PlaceObstacles(2);
                correctAnswerX = obsructedPointsArray[0] * GameManager.worldScale;
                correctAnswerY = obsructedPointsArray[1] * GameManager.worldScale;
                correctAnswer = "X: " + correctAnswerX + ", Y: " + correctAnswerY;

                titleQuestionText.text = "Voorwaardse Insnijding bepaal P";
                questionText.text = "A = x:" + correctAnswerArray[0] * GameManager.worldScale + ", y:" + correctAnswerArray[1] * GameManager.worldScale + "\n\u2022 B = x:" + correctAnswerArray[2] * GameManager.worldScale + ", y:" + correctAnswerArray[3] * GameManager.worldScale;
                break;

            case QuestionType.AchterwaardseInsnijding:
                //start oefening achterwaardse insnijding
                lineController.SetVisibles(false, true, true, false, false, false, 3);
                correctAnswerArray = placer.PlaceCalculatePoints(1);
                obsructedPointsArray = placer.PlaceObstructedCalculatePoints(3);
                correctAnswerX = correctAnswerArray[0] * GameManager.worldScale;
                correctAnswerY = correctAnswerArray[1] * GameManager.worldScale;
                correctAnswer = "X: " + correctAnswerX + ", Y: " + correctAnswerY;

                //placer.PlaceObstacles(1);
                titleQuestionText.text = "Achterwaardse Insnijding bepaal P";
                questionText.text = "\n\u2022 A = x: " + obsructedPointsArray[0] * GameManager.worldScale + ", y: " + obsructedPointsArray[1] * GameManager.worldScale + "\n\u2022 B = x: " + obsructedPointsArray[2] * GameManager.worldScale + ", y: " + obsructedPointsArray[3] * GameManager.worldScale + "\n\u2022 C = x: " + obsructedPointsArray[4] * GameManager.worldScale + ", y: " + obsructedPointsArray[5] * GameManager.worldScale;
                break;

            case QuestionType.Tabel:
                //start oefening tabel
                lineController.SetVisibles(false, true, true, true, false, false, 3);
                correctAnswerArray = placer.placeLoopedPoints(2);
                
                correctAnswerX = correctAnswerArray[12] * GameManager.worldScale;
                correctAnswerY = correctAnswerArray[13] * GameManager.worldScale;
                correctAnswer = "X: " + correctAnswerX + ", Y: " + correctAnswerY;

                lineController.SetPoints(correctAnswerArray);
                //placer.PlaceObstacles(2);
                titleQuestionText.text = "Vervolledig onderstaande tabel";
                break;

            case QuestionType.Bilateratie:
                //start oefening Bilateratie
                lineController.SetVisibles(false, false, true, true, false, false, 3);
                correctAnswerArray = placer.PlaceCalculatePoints(1);
                obsructedPointsArray = placer.PlaceObstructedCalculatePoints(2);
                correctAnswerX = correctAnswerArray[0] * GameManager.worldScale;
                correctAnswerY = correctAnswerArray[1] * GameManager.worldScale;
                correctAnswer = "X: " + correctAnswerX + ", Y: " + correctAnswerY;

                //placer.PlaceObstacles(2);
                titleQuestionText.text = "Bilateratie bepaal P";
                questionText.text = "A = x:" + obsructedPointsArray[0] * GameManager.worldScale + ", y:" + obsructedPointsArray[1] * GameManager.worldScale + " B = x:" + obsructedPointsArray[2] * GameManager.worldScale + ", y:" + obsructedPointsArray[3] * GameManager.worldScale;
                break;
        }
    }

    //checks if the given anwser is correct
    public void CheckAnswerH()
    {
        if (gm.CheckCorrectAnswer(answerInputH.text, correctAnswerH))
        {
            gm.IncreaseScore(scoreIncrease, 2);
            Debug.Log("true");
            winMenu.SetActive(true);

        }
        else
        {
            answerInputH.color = falseColor;

            Debug.Log("false");
        }
    }

    // checks if a given coordinate is correct
    public void CheckAnswerXY()
    {
        if (gm.CheckCorrectAnswer(answerInputX.text, correctAnswerX) && gm.CheckCorrectAnswer(answerInputY.text, correctAnswerY))
        {
            gm.IncreaseScore(scoreIncrease, 2);
            Debug.Log("true");
            winMenu.SetActive(true);

        }
        else
        {
            answerInputX.color = falseColor;
            answerInputY.color = falseColor;
            Debug.Log("false");
        }
    }
    public void CheckAnswerArray()
    {
        if (tabel.checkAnswers())
        {
            gm.IncreaseScore(scoreIncrease, 2);
            Debug.Log("true");
            winMenu.SetActive(true);

        }
        else Debug.Log("false");

    }

    //displays the correct answer
    public void ShowAnswer()
    {
        answerOutput.text = "Het antwoord is: " + correctAnswer;
        answerInputH.color = falseColor;
        answerInputX.color = falseColor;
        answerInputY.color = falseColor;
        //answerInputH.text = "Het antwoord is: " + CorrectAnswer().ToString();
        //waterpassing.ShowAnswer();
        Debug.Log("showing answer");

    }
}
