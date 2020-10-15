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

    public GameObject winMenu, winMenuFree, submitBtn, restartBtn;
    public Color falseColor, CorrectColor;


    public enum QuestionType { Geen, BepaalMapAngle, BepaalCoordinaat, BepaalVorigPunt, AnderAssenStelsel, Afstand2Punten }
    [Tooltip("Kies het soort vraag voor de oefening")]
    public QuestionType SoortVraag;

    [Header ("Axis transform controls")]
    public bool rotateAxis;
    [Tooltip("X & Y value: position offset, Z value rotation offset")]
    public Vector3 maxAxisTransform; // X & Y value: position offset, Z value rotation offset

    public int numberOfPoints;
    public int scoreIncrease;

    [Tooltip("het aantal keren dat je mag proberen, 0 = oneindig")]
    public int nrOfTries = 3;


    // answers
    private float[] correctAnswerArray;
    private float correctAnswerX;
    private float correctAnswerY;
    private float correctAnswerH;
    private string correctAnswer;
    private string AnswerExplanation;
    private int currentTries = 0;


    //initiate scripts
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
                lineController.SetVisibles(true, false, false, false, true, false, 2);

                correctAnswerArray = placer.PlaceCalculatePoints(1);
                correctAnswerH = lineController.GetMapAngle( Vector2.up, new Vector2(correctAnswerArray[0], correctAnswerArray[1]));

                correctAnswerH = GameManager.RoundFloat(correctAnswerH,3);
                correctAnswer = correctAnswerH.ToString();

                titleQuestionText.text = "Bepaal de kaarthoek";
                questionText.text = "Van het punt P naar het opstelpunt";
                AnswerExplanation = "De kaarthoek wordt gemeten vanaf het noorden naar P in wijzerzin";

                if (GameManager.showDebugAnswer) Debug.Log( "Correct Mapangle: " + correctAnswerH);

                break;

            case QuestionType.BepaalCoordinaat:
                //start oefening BepaalCoordinaat
                lineController.SetVisibles(true, false, false, false, true, true, 2);

                correctAnswerArray = placer.PlaceCalculatePoints(1);
                correctAnswerX = GameManager.RoundFloat(correctAnswerArray[0] * GameManager.worldScale,3);
                correctAnswerY = GameManager.RoundFloat(correctAnswerArray[1] * GameManager.worldScale,3);
                correctAnswer = "X: " + correctAnswerX + ", Y: " + correctAnswerY;

                titleQuestionText.text = "Bepaal de coördinaten van punt P";
                questionText.text = "Het totaalstation staat opgesteld in (0,0)";
                AnswerExplanation = "De coördinaten kunnen bepaald worden via een hoekmeting en een afstand";

                if (GameManager.showDebugAnswer) Debug.Log(correctAnswerX + "," + correctAnswerY);

                break;

            case QuestionType.BepaalVorigPunt:
                //start oefening BepaalVorigPunt
                lineController.SetVisibles(false, false, false, false, true, true, 2);

                correctAnswerArray = placer.PlaceCalculatePoints(2);
                lineController.SetAnswerArray(correctAnswerArray);

                correctAnswerX = GameManager.RoundFloat(correctAnswerArray[0] * GameManager.worldScale,3);
                correctAnswerY = GameManager.RoundFloat(correctAnswerArray[1] * GameManager.worldScale,3);
                correctAnswer = "X: " + correctAnswerX + ", Y: " + correctAnswerY;
                AnswerExplanation = "De coördinaten kunnen bepaald worden via een hoekmeting en een afstand";

                titleQuestionText.text = "Bepaal de coördinaten van punt P";
				float xx = GameManager.RoundFloat(correctAnswerArray[2] * GameManager.worldScale,3);
                float yy = GameManager.RoundFloat(correctAnswerArray[3] * GameManager.worldScale,3);

                questionText.text = " Gegeven de coördinaten van A \n\u2022 x: " + xx + "m \n\u2022 y: " + yy + "m";
				if (GameManager.showDebugAnswer)
					Debug.Log(correctAnswerX + ", " + correctAnswerY);

                break;

            case QuestionType.AnderAssenStelsel:
                lineController.SetVisibles(true, false, false, false, true, true, 2);

                correctAnswerArray = placer.PlaceCalculatePoints(1);
                lineController.SetAnswerArray(correctAnswerArray);

                placer.calculatePoints[0].transform.SetParent(assenkruis.transform);
                assenkruis.transform.position += new Vector3(Random.Range(0f,1f) *  maxAxisTransform.x, Random.Range(0f, 1f) * maxAxisTransform.y, 0);
                assenkruis.transform.Rotate(0, 0, Random.Range(-1f, 1f) * maxAxisTransform.z);
                AnswerExplanation = "Gebruik de assen van het verdraaide assenstelsel om de kaarthoek te berekenen";

                correctAnswerX = GameManager.RoundFloat(correctAnswerArray[0] * GameManager.worldScale,3);
                correctAnswerY = GameManager.RoundFloat(correctAnswerArray[1] * GameManager.worldScale,3);
                correctAnswer = "X: " + correctAnswerX + ", Y: " + correctAnswerY;

                titleQuestionText.text = "Bepaal de coördinaten van punt P";
                questionText.text = "Bereken P in het rode assenstelsel. Bepaal hiervoor de verdraaing t.o.v. het blauwe assenstelsel";

                if (GameManager.showDebugAnswer) Debug.Log(correctAnswerX + " , " + correctAnswerY);

                break;

            case QuestionType.Afstand2Punten: // the point coordinates are incoherent with the distance and angles that are shown on screen.
                //start oefening afstand
                lineController.SetVisibles(true, false, false, false, true, true, 2);

                correctAnswerArray = placer.PlaceCalculatePoints(2);
                lineController.SetAnswerArray(correctAnswerArray);

                correctAnswerH = Mathf.Sqrt(Mathf.Pow(correctAnswerArray[0] - correctAnswerArray[2], 2) + Mathf.Pow(correctAnswerArray[1] - correctAnswerArray[3], 2)) * GameManager.worldScale;
                correctAnswerH = GameManager.RoundFloat(correctAnswerH,3);
                correctAnswer = correctAnswerH.ToString();
                AnswerExplanation = "Bepaal de Euclidische afstand door beide coördinaten te berekenen";



                titleQuestionText.text = "Bepaal de afstand tussen de punten P & A";
                questionText.text = "Bereken de Euclidische afstand ||AP||";

                if (GameManager.showDebugAnswer) Debug.Log("Correct answer: " + correctAnswer);

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
                    setRestartH();
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
            answerInputX.color = falseColor;
            answerInputY.color = falseColor;
            Debug.Log("false");
            answerOutput.text = "Incorrect";

            if (nrOfTries > 0)
            {
                currentTries++;
                if (currentTries >= nrOfTries)
                {
                    setRestartXY();
                    return;
                }
            }
        }

    }

    //displays the correct answer
    public void ShowAnswer()
    {
        if (answerInputH.transform.parent.GetComponent<InputField>())
        {
            answerInputH.color = falseColor;
            InputField answerDisplay = answerInputH.transform.parent.GetComponent<InputField>();
            answerDisplay.text = correctAnswer;
            answerDisplay.interactable = false;
        }

        answerOutput.text = AnswerExplanation;



        Debug.Log("showing answer");

    }

    //displays the correct answer
    public void ShowAnswerXY()
    {
        if (answerInputX.transform.parent.GetComponent<InputField>())
        {
            answerInputX.color = falseColor;
            InputField answerDisplay = answerInputX.transform.parent.GetComponent<InputField>();
            answerDisplay.text = correctAnswerX.ToString();
            answerDisplay.interactable = false;
        }
        if (answerInputY.transform.parent.GetComponent<InputField>())
        {
            answerInputY.color = falseColor;
            InputField answerDisplay = answerInputY.transform.parent.GetComponent<InputField>();
            answerDisplay.text = correctAnswerY.ToString();
            answerDisplay.interactable = false;
        }

        answerOutput.text = AnswerExplanation;



        Debug.Log("showing answer");

    }

    public void setRestartH()
    {
        ShowAnswer();
        submitBtn.SetActive(false);
        restartBtn.SetActive(true);
        answerOutput.text = "Te veel pogingen, probeer opnieuw.";
    }

    public void setRestartXY()
    {
        ShowAnswerXY();
        submitBtn.SetActive(false);
        restartBtn.SetActive(true);
        answerOutput.text = "Te veel pogingen, probeer opnieuw.";
    }



}
