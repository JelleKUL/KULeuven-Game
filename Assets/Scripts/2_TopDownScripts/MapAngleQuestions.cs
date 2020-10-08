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

    public GameObject winMenu, winMenuFree;
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


    // answers
    private float[] correctAnswerArray;
    private float correctAnswerX;
    private float correctAnswerY;
    private float correctAnswerH;
    private string correctAnswer;

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
                lineController.SetVisibles(true, false, false, false, true, true, 2);

                correctAnswerArray = placer.PlaceCalculatePoints(1);
                correctAnswerH = lineController.GetMapAngle( Vector2.up, new Vector2(correctAnswerArray[0], correctAnswerArray[1]));

                correctAnswerH = Mathf.Round(correctAnswerH * 1000f) / 1000f;
                correctAnswer = correctAnswerH.ToString();

                titleQuestionText.text = "Bepaal de kaarthoek";
                questionText.text = "Van het punt P naar het opstelpunt.";

                if (GameManager.showDebugAnswer) Debug.Log(correctAnswerArray[0]+ "," + correctAnswerArray[1] + ",  " + correctAnswerH);

                break;

            case QuestionType.BepaalCoordinaat:
                //start oefening BepaalCoordinaat
                lineController.SetVisibles(true, false, false, false, true, true, 2);

                correctAnswerArray = placer.PlaceCalculatePoints(1);
                correctAnswerX = Mathf.Round(correctAnswerArray[0] * GameManager.worldScale * 1000f) / 1000f;
                correctAnswerY = Mathf.Round(correctAnswerArray[1] * GameManager.worldScale * 1000f) / 1000f;
                correctAnswer = "X: " + correctAnswerX + ", Y: " + correctAnswerY;

                titleQuestionText.text = "Bepaal de coördinaten van punt P";
                questionText.text = "Het totaalstation staat opgesteld in (0,0).";

                if (GameManager.showDebugAnswer) Debug.Log(correctAnswerX + "," + correctAnswerY);

                break;

            case QuestionType.BepaalVorigPunt:
                //start oefening BepaalVorigPunt
                lineController.SetVisibles(false, false, false, false, true, true, 2);

                correctAnswerArray = placer.PlaceCalculatePoints(2);
                lineController.SetAnswerArray(correctAnswerArray);

                correctAnswerX = Mathf.Round(correctAnswerArray[0] * GameManager.worldScale * 1000f) / 1000f;
                correctAnswerY = Mathf.Round(correctAnswerArray[1] * GameManager.worldScale * 1000f) / 1000f;
                correctAnswer = "X: " + correctAnswerX + ", Y: " + correctAnswerY;

                titleQuestionText.text = "Bepaal de coördinaten van punt P";
                // vorige code:
				//questionText.text = " Via de verkregen meting van A: \n\u2022 x: " + (Mathf.Round(correctAnswerArray[2] * 1000)/1000f) * GameManager.worldScale + "m \n\u2022 y: " + (Mathf.Round(correctAnswerArray[3] * 1000) / 1000f) * GameManager.worldScale + "m";
                
				// nieuw (met string interpolation $), kan ook via x.ToString("F2"):
				float xx = Mathf.Round(correctAnswerArray[2] * GameManager.worldScale * 1000f) / 1000f;
                float yy = Mathf.Round(correctAnswerArray[3] * GameManager.worldScale * 1000f) / 1000f;

                questionText.text = " Gegeven de coördinaten van A \n\u2022 \n\u2022 x: {xx:F2}m \n\u2022 y: {yy:F2}m";
				if (GameManager.showDebugAnswer)
					Debug.Log("{x} ,{y}");

                break;

            case QuestionType.AnderAssenStelsel:
                lineController.SetVisibles(true, false, false, false, true, true, 2);

                correctAnswerArray = placer.PlaceCalculatePoints(1);
                lineController.SetAnswerArray(correctAnswerArray);

                placer.calculatePoints[0].transform.SetParent(assenkruis.transform);
                assenkruis.transform.position += new Vector3(Random.Range(0f,1f) *  maxAxisTransform.x, Random.Range(0f, 1f) * maxAxisTransform.y, 0);
                assenkruis.transform.Rotate(0, 0, Random.Range(-1f, 1f) * maxAxisTransform.z);
                

                correctAnswerX = Mathf.Round(correctAnswerArray[0] * GameManager.worldScale * 1000f) / 1000f;
                correctAnswerY = Mathf.Round(correctAnswerArray[1] * GameManager.worldScale * 1000f) / 1000f;
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
                correctAnswerH = Mathf.Round(correctAnswerH * 1000f) / 1000f;
                correctAnswer = correctAnswerH.ToString();
                //var a = (correctAnswerArray[0] *GameManager.worldScale).ToString();
                    
                //var b = (correctAnswerArray[1] * GameManager.worldScale).ToString();

                //correctAnswer = a + "," +b;


                titleQuestionText.text = "Bepaal de afstand tussen de punten P & A";
                questionText.text = "Bereken de Euclidische afstand ||AB||.";

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
