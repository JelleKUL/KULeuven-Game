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

    public GameObject winMenu, winMenuFree, submitBtn, restartBtn;
    public Color falseColor, CorrectColor;

    public enum QuestionType { geen, Errorellips_sigmaDH, Errorellips_sigmaA, MinimaleGrootte, WerkingMeerderePunten, DragEnDropEllips }
    [Tooltip ("Kies het soort vraag voor de oefening")]
    public QuestionType SoortVraag;

    public int maxPoints;
    public int scoreIncrease;
    [Tooltip("het aantal keren dat je mag proberen, 0 = oneindig")]
    public int nrOfTries = 3;

    // internal answers
    private float[] correctAnswerArray;
    private float correctAnswerH;
    private string correctAnswer;
    private string AnswerExplanation;
    private int currentTries = 0;



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

        if (answerOutput) answerOutput.text = "";

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
              
                lineController.SetVisibles(false, true, true, true, true, true, 2);
                titleQuestionText.text = "I. Bepaal de standaardafwijking van A gemeten vanaf P";
                questionText.text = "Bereken a.d.v. de hoekfout en de afstandsfout de maximale standaardafwijking van de errorellips";
                AnswerExplanation = "de grootste waarde kan je ook waarnemen op het speelveld.";

                correctAnswerArray = placer.PlaceCalculatePoints(2);// place the points on the field
                lineController.SetAnswerArray(correctAnswerArray);
                lineController.UpdateErrors();

                break;

            case QuestionType.Errorellips_sigmaA:
                //demo van de foutenellips 1 punt

                lineController.SetVisibles(false, true, true, true, true, true, 2);
                titleQuestionText.text = "II. Bepaal sigma a van A gemeten vanaf P";
                questionText.text = "Bereken de sigma a-component van de errorellips.";
                AnswerExplanation = "gebruik de vaste en variablele afstandsfout.";

                correctAnswerArray = placer.PlaceCalculatePoints(2);// place the points on the field
                lineController.SetAnswerArray(correctAnswerArray);
                lineController.UpdateErrors();
                break;
            
            case QuestionType.MinimaleGrootte:
                //start oefening MinimaleGrote

                lineController.SetVisibles(false, true, true, true, true, true, 10);
                titleQuestionText.text = "III. Bepaal A gemeten vanaf P met een zo klein mogelijke errorellips";
                questionText.text = "Kies zorgvuldig tussenopstelling om de fout zo klein mogelijk te houden.";
                AnswerExplanation = "probeer de afstand zo laag mogelijk te houden";

                correctAnswerArray = placer.PlaceCalculatePoints(2);// place the points on the field
                placer.PlaceObstacleBtwn();
                placer.PlaceRandomObstacles(2);
                placer.PlaceRandomMobileObstacles(2);

                lineController.SetAnswerArray(correctAnswerArray);
                lineController.UpdateErrors();

                break;

            case QuestionType.WerkingMeerderePunten:
                //start oefening TekenFoutenEllips

                lineController.SetVisibles(false, true, true, true, true, true, 10);
                titleQuestionText.text = "IV. Bepaal sigma a van A gemeten vanaf P";
                questionText.text = "Bereken via tussenopstellingen de sigma a-component van de errorellips.";
                AnswerExplanation = " houd rekening met de verschillende componenten van te opstelpunten.";

                correctAnswerArray = placer.PlaceCalculatePoints(2);// place the points on the field
                placer.PlaceObstacleBtwn();
                placer.PlaceRandomObstacles(2);
                placer.PlaceRandomMobileObstacles(2);
                lineController.SetAnswerArray(correctAnswerArray);
                lineController.UpdateErrors();

                break;

            case QuestionType.DragEnDropEllips:
                //start oefening DragEnDropEllips

                break;

        }
        if (GameManager.showDebugAnswer) Debug.Log("Correct antwoord = " + CorrectAnswer(false) + " mm ");

    }
  
    //checks if the given anwser is correct
    public void CheckAnswer()
    {
        if (lineController.CheckPoints() && lineController.CheckPointsVisibility())
        {
            if (gm.CheckCorrectAnswer(answerInputH.text, CorrectAnswer(true)) || gm.CheckCorrectAnswer(answerInputH.text, CorrectAnswer(false)))
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

                if (nrOfTries > 0)
                {
                    currentTries++;
                    if (currentTries >= nrOfTries)
                    {
                        setRestart();
                        return;
                    }
                }
            }

        }
        else answerOutput.text = "Setup in P and measure to A before submitting";
    }

    //checks if the given anwser is correct
    public void CheckAnswerMinimaal()
    {
        if (lineController.CheckPoints() && lineController.CheckPointsVisibility())
        {

            if (Mathf.Abs(lineController.GetSigmaA() - CorrectAnswer(false)) <=0.6* lineController.GetDistanceError1() )
            {
                gm.IncreaseScore(scoreIncrease, 2);
                Debug.Log("true");
                if (GameManager.campaignMode)
            {
                winMenu.SetActive(true);
            }
            else
            {
                winMenuFree.SetActive(true);
            }

        }
        else
            {
                answerInputH.color = falseColor;
                Debug.Log("false");
                answerOutput.text = "Incorrect";

                if (nrOfTries > 0)
                {
                    currentTries++;
                    if (currentTries >= nrOfTries)
                    {
                        setRestart();
                        return;
                    }
                }
            }

        }
        else answerOutput.text = "Setup in P and measure to A before submitting";
    }

    //displays the correct answer
    public void ShowAnswer()
    {
        if (answerInputH.transform.parent.GetComponent<InputField>())
        {
            answerInputH.color = falseColor;
            InputField answerDisplay = answerInputH.transform.parent.GetComponent<InputField>();
            answerDisplay.text = CorrectAnswer(false).ToString();
            answerDisplay.interactable = false;
        }

        answerOutput.text = AnswerExplanation;



        Debug.Log("showing answer");

    }

    public float CorrectAnswer( bool exact)//use the exact formula *Pi/2 or rule of thumb *1.5
    {
        switch (SoortVraag)
        {
            case QuestionType.Errorellips_sigmaDH:

                (float d, float h, float a) = lineController.GetErrorDH();

                return Mathf.Max(d, h);

            case QuestionType.Errorellips_sigmaA:

                if(exact) return lineController.GetSigmaAExact();
                else return lineController.GetSigmaA();

            case QuestionType.MinimaleGrootte:

                (float dd, float hh, float aa) = lineController.GetErrorDH();
                return aa;

            case QuestionType.WerkingMeerderePunten:

                if(exact) return lineController.GetSigmaAExact();
                else return lineController.GetSigmaA();


        }
        return 0;
    }

    public void setRestart()
    {
        ShowAnswer();
        submitBtn.SetActive(false);
        restartBtn.SetActive(true);
        answerOutput.text = "Te veel pogingen, probeer opnieuw.";
    }

}
