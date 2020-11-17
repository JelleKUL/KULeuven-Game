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

    public GameObject winMenu, winMenuFree, submitBtn, restartBtn;
    public Color falseColor, CorrectColor;


    public enum QuestionType { Geen, Coordinaat1Punt, Afstand2PuntenPolygoon, VoorwaardseInsnijding, AchterwaardseInsnijding, Tabel, Bilateratie, VrijePolygonatie }
    [Tooltip("Kies het soort vraag voor de oefening")]
    public QuestionType SoortVraag;

    public int scoreIncrease;
    [Tooltip("het aantal keren dat je mag proberen, 0 = oneindig")]
    public int nrOfTries = 0;


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
    private string AnswerExplanation;
    private int currentTries = 0;



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
                correctAnswerX = GameManager.RoundFloat(correctAnswerArray[0] * GameManager.worldScale, 3);
                correctAnswerY = GameManager.RoundFloat(correctAnswerArray[1] * GameManager.worldScale , 3);
                correctAnswer = "X: " + correctAnswerX + ", Y: " + correctAnswerY;

                titleQuestionText.text = "Bepaal het coördinaat van punt P";
                questionText.text = "Met behulp van de afstand en map angle vanaf het meetpunt.";
                AnswerExplanation = "Gebruik trigoniometrische formules om het coordinaat te berekenen";
                break;

            case QuestionType.Afstand2PuntenPolygoon:
                //start oefening DragEnDropEllips
                lineController.SetVisibles(true, false, false, false, true, true, 2);
                correctAnswerArray = placer.PlaceCalculatePoints(2);
                correctAnswerH = GameManager.RoundFloat(GameManager.worldScale * Mathf.Sqrt(Mathf.Pow(correctAnswerArray[2] - correctAnswerArray[0], 2) + Mathf.Pow(correctAnswerArray[3] - correctAnswerArray[1], 2)), 3);
                correctAnswer = correctAnswerH.ToString();

                titleQuestionText.text = "Bepaal de afstand tussen A en B";
                questionText.text = "Met behulp van de afstand en map angle vanaf het meetpunt.";
                AnswerExplanation = "Bepaal beide coordinaten en bereken dan de afstand";

                break;

            case QuestionType.VoorwaardseInsnijding:
                //start oefening voorwaardse insnijding
                lineController.SetVisibles(false, false, true, false, false, false, 3);
                obsructedPointsArray = placer.PlaceObstructedCalculatePoints(1);
                correctAnswerArray = placer.PlaceCalculatePoints(2);
                
                //placer.PlaceObstacles(2);
                correctAnswerX = GameManager.RoundFloat(obsructedPointsArray[0] * GameManager.worldScale, 3);
                correctAnswerY = GameManager.RoundFloat(obsructedPointsArray[1] * GameManager.worldScale, 3);
                correctAnswer = "X: " + correctAnswerX + ", Y: " + correctAnswerY;

                titleQuestionText.text = "I. Voorwaardse Insnijding bepaal P";
                questionText.text = "\n\u2022 A = x:" + GameManager.RoundFloat(correctAnswerArray[0] * GameManager.worldScale, 3) + ", y:" + GameManager.RoundFloat(correctAnswerArray[1] * GameManager.worldScale, 3) + 
                                    "\n\u2022 B = x:" + GameManager.RoundFloat(correctAnswerArray[2] * GameManager.worldScale, 3) + ", y:" + GameManager.RoundFloat(correctAnswerArray[3] * GameManager.worldScale, 3);
                AnswerExplanation = "meet de hoeken en bereken het coordinaat via de sinusregel en Pythagoras";

                break;

            case QuestionType.AchterwaardseInsnijding:
                //start oefening achterwaardse insnijding
                lineController.SetVisibles(false, false, true, false, false, false, 3);
                correctAnswerArray = placer.PlaceCalculatePoints(1);
                obsructedPointsArray = placer.PlaceObstructedCalculatePoints(3);
                correctAnswerX = GameManager.RoundFloat(correctAnswerArray[0] * GameManager.worldScale, 3);
                correctAnswerY = GameManager.RoundFloat(correctAnswerArray[1] * GameManager.worldScale, 3);
                correctAnswer = "X: " + correctAnswerX + ", Y: " + correctAnswerY;

                //placer.PlaceObstacles(1);
                titleQuestionText.text = "II. Achterwaardse Insnijding bepaal P";
                questionText.text = "\n\u2022 A = x: " + GameManager.RoundFloat(obsructedPointsArray[0] * GameManager.worldScale, 3) + ", y: " + GameManager.RoundFloat(obsructedPointsArray[1] * GameManager.worldScale, 3) + 
                                    "\n\u2022 B = x: " + GameManager.RoundFloat(obsructedPointsArray[2] * GameManager.worldScale, 3) + ", y: " + GameManager.RoundFloat(obsructedPointsArray[3] * GameManager.worldScale, 3) + 
                                    "\n\u2022 C = x: " + GameManager.RoundFloat(obsructedPointsArray[4] * GameManager.worldScale, 3) + ", y: " + GameManager.RoundFloat(obsructedPointsArray[5] * GameManager.worldScale, 3) ;
                AnswerExplanation = "meet de hoeken en bereken het coordinaat via de sinusregel en Pythagoras";
                break;

            case QuestionType.Tabel:
                //start oefening tabel
                lineController.SetVisibles(false, false, true, true, true, false, 8);
                correctAnswerArray = placer.placeLoopedPoints(3);
                
                correctAnswerX = GameManager.RoundFloat(correctAnswerArray[10] * GameManager.worldScale, 3);
                correctAnswerY = GameManager.RoundFloat(correctAnswerArray[11] * GameManager.worldScale, 3);
                correctAnswer = "X: " + correctAnswerX + ", Y: " + correctAnswerY;

                lineController.SetPoints(correctAnswerArray);
                //placer.PlaceObstacles(2);
                titleQuestionText.text = "III. Vervolledig onderstaande tabel";
                questionText.text =     "De punten P en A zijn gegeven:" +
                                        "\n\u2022 P (" + GameManager.RoundFloat(correctAnswerArray[0] * GameManager.worldScale, 3) + ", " + GameManager.RoundFloat(correctAnswerArray[1] * GameManager.worldScale, 3)+ ") m" +
                                        "\n\u2022 A (" + GameManager.RoundFloat(correctAnswerArray[2] * GameManager.worldScale, 3) + ", " + GameManager.RoundFloat(correctAnswerArray[3] * GameManager.worldScale, 3) + ") m";
                AnswerExplanation = "";
                if (GameManager.showDebugAnswer)
                {
                    Debug.Log(  "<b>Correct points:</b>" +
                                "\n\u2022 P = x: " + GameManager.RoundFloat(correctAnswerArray[0] * GameManager.worldScale, 3) + ", y: " + GameManager.RoundFloat(correctAnswerArray[1] * GameManager.worldScale, 3) +
                                "\n\u2022 A = x: " + GameManager.RoundFloat(correctAnswerArray[2] * GameManager.worldScale, 3) + ", y: " + GameManager.RoundFloat(correctAnswerArray[3] * GameManager.worldScale, 3) +
                                "\n\u2022 B = x: " + GameManager.RoundFloat(correctAnswerArray[4] * GameManager.worldScale, 3) + ", y: " + GameManager.RoundFloat(correctAnswerArray[5] * GameManager.worldScale, 3) +
                                "\n\u2022 C = x: " + GameManager.RoundFloat(correctAnswerArray[6] * GameManager.worldScale, 3) + ", y: " + GameManager.RoundFloat(correctAnswerArray[7] * GameManager.worldScale, 3) +
                                "\n\u2022 D = x: " + GameManager.RoundFloat(correctAnswerArray[8] * GameManager.worldScale, 3) + ", y: " + GameManager.RoundFloat(correctAnswerArray[9] * GameManager.worldScale, 3) +
                                "\n\u2022 E = x: " + GameManager.RoundFloat(correctAnswerArray[10] * GameManager.worldScale, 3) + ", y: " + GameManager.RoundFloat(correctAnswerArray[11] * GameManager.worldScale, 3) +
                                "\n\u2022 F = x: " + GameManager.RoundFloat(correctAnswerArray[12] * GameManager.worldScale, 3) + ", y: " + GameManager.RoundFloat(correctAnswerArray[13] * GameManager.worldScale, 3)
                            );
                }
                break;

            case QuestionType.Bilateratie:
                //start oefening Bilateratie
                lineController.SetVisibles(false, false, true, true, false, false, 3);
                correctAnswerArray = placer.PlaceCalculatePoints(1);
                obsructedPointsArray = placer.PlaceObstructedCalculatePoints(2);
                correctAnswerX = GameManager.RoundFloat(correctAnswerArray[0] * GameManager.worldScale, 3);
                correctAnswerY = GameManager.RoundFloat(correctAnswerArray[1] * GameManager.worldScale, 3);
                correctAnswer = "X: " + correctAnswerX + ", Y: " + correctAnswerY;

                //placer.PlaceObstacles(2);
                titleQuestionText.text = "IV. Bilateratie bepaal P";
                questionText.text = "De punten A en B zijn gegeven:" +
                                    "\n\u2022 A (" + GameManager.RoundFloat(obsructedPointsArray[0] * GameManager.worldScale, 3) + ", " + GameManager.RoundFloat(obsructedPointsArray[1] * GameManager.worldScale, 3)+ ") m" +
                                    "\n\u2022 B (" + GameManager.RoundFloat(obsructedPointsArray[2] * GameManager.worldScale, 3) + ", " + GameManager.RoundFloat(obsructedPointsArray[3] * GameManager.worldScale, 3) + ") m";
                AnswerExplanation = "Je hebt een gegeven te veel, gebruik dat om te vereffenen";
                

                break;

            case QuestionType.VrijePolygonatie:

                AnswerExplanation = "";

                break;
  
        }
        if (GameManager.showDebugAnswer) Debug.Log(correctAnswer);
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

    // checks if a given coordinate is correct
    public void CheckAnswerXY()
    {
        if (gm.CheckCorrectAnswer(answerInputX.text, correctAnswerX) && gm.CheckCorrectAnswer(answerInputY.text, correctAnswerY))
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
            answerOutput.text = "Waarde incorrect...";
            answerInputX.color = falseColor;
            answerInputY.color = falseColor;
            Debug.Log("false");

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
    
    public void CheckAnswerArray()
    {
        if (tabel.checkAnswers(correctAnswerArray))
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
            answerOutput.text = "De waarden die zijn ingevoerd zijn niet correct";
            Debug.Log("false");
        }
    }
    

    public void ShowAnswerArray()
    {
        tabel.ShowCorrectValues(correctAnswerArray);
    }

    //displays the correct answer
    public void ShowAnswer()
    {
        answerOutput.text = AnswerExplanation;
        answerInputH.color = falseColor;
        answerInputX.color = falseColor;
        answerInputY.color = falseColor;
        Debug.Log("showing answer");

        if (answerInputH.transform.parent.GetComponent<InputField>())
        {
            InputField answerDisplay = answerInputH.transform.parent.GetComponent<InputField>();
            answerDisplay.text = correctAnswer.ToString();
            answerDisplay.interactable = false;
        }
        if (answerInputX.transform.parent.GetComponent<InputField>())
        {
            InputField answerDisplay = answerInputX.transform.parent.GetComponent<InputField>();
            answerDisplay.text = correctAnswerX.ToString();
            answerDisplay.interactable = false;
        }
        if (answerInputY.transform.parent.GetComponent<InputField>())
        {
            InputField answerDisplay = answerInputY.transform.parent.GetComponent<InputField>();
            answerDisplay.text = correctAnswerY.ToString();
            answerDisplay.interactable = false;
        }
    }
    public void setRestart()
    {
        ShowAnswer();
        submitBtn.SetActive(false);
        restartBtn.SetActive(true);
        answerOutput.text = "Te veel pogingen, probeer opnieuw.";
    }

}
