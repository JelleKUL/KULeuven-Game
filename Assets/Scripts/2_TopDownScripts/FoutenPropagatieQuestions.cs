using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//*********** The FoutenPropagatieQuestions sets the required parameters for a specific question ******************//


public class FoutenPropagatieQuestions : MonoBehaviour
{
    [Header("Predefined Object")]
    public Text titleQuestionText;
    public Text questionText;
    

    // answer input
    public Text answerInputX;
    public Text answerInputY;
    public Text answerInputH;
    public Text answerOutput;

    public GameObject winMenu;
    public Color falseColor, CorrectColor;

    public enum QuestionType { geen, Werking1Punt, Werking1Puntxy, MinimaleGrootte, WerkingMeerderePunten, DragEnDropEllips }
    [Tooltip ("Kies het soort vraag voor de oefening")]
    public QuestionType SoortVraag;

    public int maxPoints;
    public int scoreIncrease;

    // internal answers
    private float[] correctAnswerArray;
    private float correctAnswerX;
    private float correctAnswerY;
    private float correctAnswerH;
    private float errorMargin = 0.001f;
    private string correctAnswer;

    //initiate scripts
    private GameManager gm;
    private PolygonLineController lineController;
    private PolygonPointController thisPoint;

    private ObjectPlacer placer;

    // awake is called before start functions
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
            case QuestionType.geen:

                break;

            case QuestionType.Werking1Punt:
                //demo van de foutenellips 1 punt
                
                lineController.SetVisibles(true, true, true, true, true, true, 2);
                titleQuestionText.text = "Bepaal de standaardafwijking van P";
                questionText.text = "Bereken a.d.v. de hoekfout en de afstandsfout de maximale standaardafwijking van de errorellips";
                //lineController.randomizeErrors = true;
                correctAnswerArray = placer.PlaceCalculatePoints(1);

                correctAnswerH = lineController.GetErrorH(correctAnswerArray);
                correctAnswer = correctAnswerH.ToString();
                break;

            case QuestionType.Werking1Puntxy:
                //demo van de foutenellips 1 punt
                lineController.SetVisibles(true, true, true, true, true, true, 2);
                titleQuestionText.text = "Bepaal de standaardafwijking van P in X en Y";
                questionText.text = "Bereken de X-en Y-component van de errorellips.";
                correctAnswerArray=placer.PlaceCalculatePoints(1);

                correctAnswerX = lineController.ellipsX+30;
                correctAnswerY = lineController.ellipsY+20;
                correctAnswer = "X: " + correctAnswerX + ", Y: " + correctAnswerY;
                break;
            
            case QuestionType.MinimaleGrootte:
                //start oefening MinimaleGrote
                lineController.SetVisibles(true, true, true, true, true, true, 10);
                titleQuestionText.text = "Bepaal P met een zo klein mogelijke errorellips";
                questionText.text = "Meet P via tussenopstellingen met een zo klein mogelijke fout.";
                placer.PlaceCalculatePoints(1);
                placer.PlaceObstacleBtwn(3);


                correctAnswerH = lineController.biggestEllips * 2; // error margin multiplier
                correctAnswer = "<" + correctAnswerH; 
                break;

            case QuestionType.WerkingMeerderePunten:
                //start oefening TekenFoutenEllips
                lineController.SetVisibles(true, true, true, true, true, true, 10);
                titleQuestionText.text = "Bepaal de standaardafwijking van P in X en Y";
                questionText.text = "Bereken de X-en Y-component van de errorellips van P via meerdere punten.";
                placer.PlaceCalculatePoints(1);
                placer.PlaceObstacles(3);

                correctAnswerX = lineController.ellipsX + 30;
                correctAnswerY = lineController.ellipsY + 20;
                correctAnswer = "X: " + correctAnswerX + ", Y: " + correctAnswerY;
                break;

            case QuestionType.DragEnDropEllips:
                //start oefening DragEnDropEllips

                break;

        }
        if (GameManager.showDebugAnswer) Debug.Log("Correct antwoord = " + correctAnswer + " mm ");

    }
    // checks the answer (old version)
    //public void CheckAnswer()
    //{
    //    switch (SoortVraag)
    //    {
    //        case QuestionType.geen:

    //            break;

    //        case QuestionType.Werking1Punt:
    //            gm.IncreaseScore(0, 2);
    //            winMenu.SetActive(true);
    //            break;

    //        case QuestionType.Werking1Puntxy:
    //            gm.IncreaseScore(0, 2);
    //            winMenu.SetActive(true);
    //            break;

    //        case QuestionType.MinimaleGrootte:

    //            Debug.Log(lineController.LastPointSnapped());
    //            if (lineController.LastPointSnapped())
    //            {
    //                int points = Mathf.Max(0, maxPoints - Mathf.FloorToInt(lineController.biggestEllips / 10f));

    //                gm.IncreaseScore(points, 2);
    //                winMenu.SetActive(true);
    //            }

    //            break;
    //        case QuestionType.WerkingMeerderePunten:
    //            gm.IncreaseScore(0, 2);
    //            winMenu.SetActive(true);
    //            break;

    //        case QuestionType.DragEnDropEllips:

    //            break;

    //    }
    //}

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
        
        answerOutput.text = "Het antwoord is: " + correctAnswer + " mm ";
        answerInputH.color = falseColor;
        //answerInputH.text = "Het antwoord is: " + CorrectAnswer().ToString();
        //waterpassing.ShowAnswer();
        Debug.Log("showing answer");

    }
}
