using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//*********** The MapAngleQuestions sets the required parameters for a specific question ******************//


public class MapAngleQuestions : MonoBehaviour
{
    [Header("Predefined TextFields")]
    public Text questionText;
    public Text answerInputX;
    public Text answerInputY;
    public Text answerInputH;
    public Text answerOutput;
    public GameObject assenkruis;

    public enum QuestionType { Geen, BepaalMapAngle, BepaalCoordinaat, BepaalVorigPunt, AnderAssenStelsel }
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
                
                questionText.text = "Bepaal de Map Angle van punt A";
                correctAnswerH = lineController.GetMapAngle(new Vector2(correctAnswerArray[0], correctAnswerArray[1]), Vector2.up);

                Debug.Log(correctAnswerArray[0]+ "," + correctAnswerArray[1] + ",  " + correctAnswerH);
                

                break;
            case QuestionType.BepaalCoordinaat:
                //start oefening BepaalCoordinaat
                lineController.SetVisibles(true, false, false, false, true, true, 2);
                correctAnswerArray = placer.PlaceCalculatePoints(1);
                correctAnswerX = correctAnswerArray[0];
                correctAnswerY = correctAnswerArray[1];
                questionText.text = "Bepaal het coördinaat van punt A";
                break;

            case QuestionType.BepaalVorigPunt:
                //start oefening BepaalVorigPunt
                lineController.SetVisibles(false, false, false, false, true, true, 2);
                correctAnswerArray = placer.PlaceCalculatePoints(2);
                correctAnswerX = correctAnswerArray[0];
                correctAnswerY = correctAnswerArray[1];
                questionText.text = "Bepaal het coördinaat van punt A, via de verkregen meting van B: x:" + correctAnswerArray[2] + " / y:" + correctAnswerArray[3];
                break;

            case QuestionType.AnderAssenStelsel:
                lineController.SetVisibles(true, false, false, false, true, true, 2);
                correctAnswerArray = placer.PlaceCalculatePoints(1);
                
                placer.calculatePoints[0].transform.SetParent(assenkruis.transform);
                assenkruis.transform.position += new Vector3(axisTransform.x, axisTransform.y, 0);
                assenkruis.transform.Rotate(0, 0, axisTransform.z);
                
                correctAnswerX = correctAnswerArray[0];
                correctAnswerY = correctAnswerArray[1];
                Debug.Log(correctAnswerX + " , " + correctAnswerY);
                questionText.text = "Bepaal het coördinaat van punt A, Het Assenstelsel is gedraaid";
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
            gm.ReloadScene();


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
            gm.ReloadScene();

        }
        else Debug.Log("false");

    }
   

}
