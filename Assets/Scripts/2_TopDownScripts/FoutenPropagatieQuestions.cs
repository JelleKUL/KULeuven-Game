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

    public enum QuestionType { geen, Errorellips_sigmaDH, Errorellips_sigmaA, MinimaleGrootte, WerkingMeerderePunten, DragEnDropEllips }
    [Tooltip ("Kies het soort vraag voor de oefening")]
    public QuestionType SoortVraag;

    public int maxPoints;
    public int scoreIncrease;

    // internal answers
    private float[] correctAnswerArray;
    //private float correctAnswerA;
    private float correctAnswerH;
    //private float errorMargin = 0.001f;
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

            case QuestionType.Errorellips_sigmaDH:
                //demo van de foutenellips 1 punt
              
                lineController.SetVisibles(true, true, true, true, true, true, 2);
                titleQuestionText.text = "Bepaal de standaardafwijking van P";
                questionText.text = "Bereken a.d.v. de hoekfout en de afstandsfout de maximale standaardafwijking van de errorellips";

                correctAnswerArray = placer.PlaceCalculatePoints(1);// place the points on the field
                lineController.SetAnswerArray(correctAnswerArray);
                lineController.UpdateErrors();

                break;

            case QuestionType.Errorellips_sigmaA:
                //demo van de foutenellips 1 punt

                lineController.SetVisibles(true, true, true, true, true, true, 2);
                titleQuestionText.text = "Bepaal sigma a van P";
                questionText.text = "Bereken de sigma a-component van de errorellips.";

                correctAnswerArray = placer.PlaceCalculatePoints(1);// place the points on the field
                lineController.SetAnswerArray(correctAnswerArray);
                lineController.UpdateErrors();
                break;
            
            case QuestionType.MinimaleGrootte:
                //start oefening MinimaleGrote

                lineController.SetVisibles(true, true, true, true, true, true, 10);
                titleQuestionText.text = "Bepaal P met een zo klein mogelijke errorellips";
                questionText.text = "Meet P via tussenopstellingen met een zo klein mogelijke fout.";

                correctAnswerArray = placer.PlaceCalculatePoints(1);// place the points on the field
                placer.PlaceObstacleBtwn(1);
                lineController.SetAnswerArray(correctAnswerArray);
                lineController.UpdateErrors();

                break;

            case QuestionType.WerkingMeerderePunten:
                //start oefening TekenFoutenEllips

                lineController.SetVisibles(true, true, true, true, true, true, 10);
                titleQuestionText.text = "Bepaal sigma a van P";
                questionText.text = "Bereken via tussenopstellingen de sigma a-component van de errorellips.";

                correctAnswerArray = placer.PlaceCalculatePoints(1);// place the points on the field
                placer.PlaceObstacleBtwn(1);
                placer.PlaceObstacles(4);
                lineController.SetAnswerArray(correctAnswerArray);
                lineController.UpdateErrors();

                break;

            case QuestionType.DragEnDropEllips:
                //start oefening DragEnDropEllips

                break;

        }
        if (GameManager.showDebugAnswer) Debug.Log("Correct antwoord = " + CorrectAnswer() + " mm ");

    }
  
    //checks if the given anwser is correct
    public void CheckAnswer()
    {
        if (lineController.CheckPointP())
        {
            if (gm.CheckCorrectAnswer(answerInputH.text, CorrectAnswer()) )
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
        else answerOutput.text = "Measure to P before submitting";
    }

    //displays the correct answer
    public void ShowAnswer()
    {
        correctAnswer = CorrectAnswer().ToString();

        answerOutput.text = "Het antwoord is: " + correctAnswer + " mm ";
        answerInputH.color = falseColor;
         Debug.Log("showing answer");

    }

    public float CorrectAnswer()
    {
        switch (SoortVraag)
        {
            case QuestionType.Errorellips_sigmaDH:

                (float d, float h, float a) = lineController.GetErrorDH();

                return Mathf.Max(d, h);

            case QuestionType.Errorellips_sigmaA:

                return lineController.GetSigmaA();

            case QuestionType.MinimaleGrootte:

                (float dd, float hh, float aa) = lineController.GetErrorDH();
                answerInputH.text= lineController.GetSigmaA().ToString();

                return aa;

            case QuestionType.WerkingMeerderePunten:

                return lineController.GetSigmaA();
        }
        return 0;
    }

}
