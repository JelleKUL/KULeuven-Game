using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//*********** The MapAngleQuestions sets the required parameters for a specific question ******************//


public class MapAngleQuestions : MonoBehaviour
{
    [Header("Predefined TextFields")]
    public Text titleQuestionText;
    public Text questionText;

    public Text answerInputX;
    public Text answerInputY;
    public Text answerInputH;
    public Text answerOutput;
    public GameObject assenkruis;

    public GameObject winMenu;
    public Color falseColor, CorrectColor;


    public enum QuestionType { Geen, BepaalMapAngle, BepaalCoordinaat, BepaalVorigPunt, AnderAssenStelsel, Afstand2Punten }
    [Tooltip("Kies het soort vraag voor de oefening")]
    public QuestionType SoortVraag;

    [Header ("Axis transform controls")]
    public bool rotateAxis;
    [Tooltip("X & Y value: position offset, Z value rotation offset")]
    public Vector3 axisTransform; // X & Y value: position offset, Z value rotation offset

    public int numberOfPoints;
    public int scoreIncrease;


    
    private float[] correctAnswerArray;
    private float correctAnswerX;
    private float correctAnswerY;
    private float correctAnswerH;

    private string correctAnswer;

    private PolygonLineController lineController;
    private GameManager gm;
    private ObjectPlacer placer;

    // Start is called before the first frame update
    void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        lineController = GameObject.FindGameObjectWithTag("PolygonLine").GetComponent<PolygonLineController>();
        placer = GetComponent<ObjectPlacer>();


        SetQuestionType(SoortVraag);

        answerOutput.text = "";

    }


    //sets the type of question, can be altered by another script
    public void SetQuestionType(QuestionType vraag)
    {
        switch (vraag)
        {
            case QuestionType.Geen:

                break;

            case QuestionType.BepaalMapAngle:
                //start oefening BepaalMapAngle
                lineController.SetVisibles(true, false, false, false, true, true, 2);
                correctAnswerArray = placer.PlaceCalculatePoints(1);
                
                titleQuestionText.text = "Bepaal de Kaarthoek";
                questionText.text = "Van het punt P naar het opstelpunt.";
                correctAnswerH = lineController.GetMapAngle(Vector2.up, new Vector2(correctAnswerArray[0], correctAnswerArray[1]));
                correctAnswer = correctAnswerH.ToString();
                Debug.Log(correctAnswerArray[0]+ "," + correctAnswerArray[1] + ",  " + correctAnswerH);
                

                break;
            case QuestionType.BepaalCoordinaat:
                //start oefening BepaalCoordinaat
                lineController.SetVisibles(true, false, false, false, true, true, 2);
                correctAnswerArray = placer.PlaceCalculatePoints(1);
                correctAnswerX = correctAnswerArray[0] * GameManager.worldScale;
                correctAnswerY = correctAnswerArray[1] * GameManager.worldScale;
                correctAnswer = "X: " + correctAnswerX + ", Y: " + correctAnswerY;
                titleQuestionText.text = "Bepaal het coördinaat van punt P";
                questionText.text = "Het meettoestel staat op het nulpunt.";
                break;

            case QuestionType.BepaalVorigPunt:
                //start oefening BepaalVorigPunt
                lineController.SetVisibles(false, false, false, false, true, true, 2);
                correctAnswerArray = placer.PlaceCalculatePoints(2);
                correctAnswerX = correctAnswerArray[0] * GameManager.worldScale;
                correctAnswerY = correctAnswerArray[1] * GameManager.worldScale;
                correctAnswer = "X: " + correctAnswerX + ", Y: " + correctAnswerY;
                titleQuestionText.text = "Bepaal het coördinaat van punt P";
                questionText.text = " Via de verkregen meting van A: \n\u2022 x: " + (Mathf.Round(correctAnswerArray[2] * 1000)/1000f) * GameManager.worldScale + "m \n\u2022 y: " + (Mathf.Round(correctAnswerArray[3] * 1000) / 1000f) * GameManager.worldScale + "m";
                Debug.Log(correctAnswerArray[0] * GameManager.worldScale + "," + correctAnswerArray[1] * GameManager.worldScale);

                break;

            case QuestionType.AnderAssenStelsel:
                lineController.SetVisibles(true, false, false, false, true, true, 2);
                correctAnswerArray = placer.PlaceCalculatePoints(1);
                
                placer.calculatePoints[0].transform.SetParent(assenkruis.transform);
                assenkruis.transform.position += new Vector3(axisTransform.x, axisTransform.y, 0);
                assenkruis.transform.Rotate(0, 0, axisTransform.z);
                
                correctAnswerX = correctAnswerArray[0] * GameManager.worldScale;
                correctAnswerY = correctAnswerArray[1] * GameManager.worldScale;
                correctAnswer = "X: " + correctAnswerX + ", Y: " + correctAnswerY;
                Debug.Log(correctAnswerX + " , " + correctAnswerY);
                titleQuestionText.text = "Bepaal het coördinaat van punt P";
                questionText.text = "Het Assenstelsel is gedraaid met een bepaalde hoek, bepaal het coordinaat aan de hand van het rode assenkruis ten opzichte van het meettoestel.";
                break;

            case QuestionType.Afstand2Punten:
                //start oefening afstand
                lineController.SetVisibles(true, false, false, false, true, true, 2);
                correctAnswerArray = placer.PlaceCalculatePoints(2);
                correctAnswerH = Mathf.Sqrt(Mathf.Pow(correctAnswerArray[0] + correctAnswerArray[2], 2) + Mathf.Pow(correctAnswerArray[1] + correctAnswerArray[3], 2)) * GameManager.worldScale;
                correctAnswer = correctAnswerH.ToString();

                Debug.Log(correctAnswerH);
                titleQuestionText.text = "Bepaal de afstand tussen de punten P & A";
                questionText.text = "aan de hand van de kaarthoek, bepaal de afstand in vogelvlucht.";
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
            answerOutput.text = "Incorrect";
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
            answerOutput.text = "Incorrect";
        }

    }

    //displays the correct answer
    public void ShowAnswer()
    {
        answerOutput.text = "Het antwoord is: " + correctAnswer;
        answerInputH.color = falseColor;
        //answerInputH.text = "Het antwoord is: " + CorrectAnswer().ToString();
        //waterpassing.ShowAnswer();
        Debug.Log("showing answer");

    }



}
