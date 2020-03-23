using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapAngleQuestions : MonoBehaviour
{
    public enum QuestionType { Geen, BepaalMapAngle, BepaalCoordinaat, BepaalVorigPunt }
    [Tooltip("Kies het soort vraag voor de oefening")]
    public QuestionType SoortVraag;

    [Header ("Axis transform controls")]
    public bool rotateAxis;
    [Tooltip("X & Y value: position offset, Z value rotation offset")]
    public Vector3 axisTransform; // X & Y value: position offset, Z value rotation offset

    public int numberOfPoints;
   


    [HideInInspector]
    public float[] correctAnswer;

    private PolygonLineController lineController;
    private GameManager gm;
    private ObjectPlacer placer;

    // Start is called before the first frame update
    void Start()
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
                //start oefening VanANaarB
                lineController.SetVisibles(false, false, false, true, true, 2);
                correctAnswer = placer.PlaceCalculatePoints(numberOfPoints);
                placer.PlaceObstacles(1);

                foreach (var nr in correctAnswer)
                {
                    Debug.Log(nr + " , ");
                }
                

                break;
            case QuestionType.BepaalCoordinaat:
                //start oefening TekenFoutenEllips
                lineController.SetVisibles(false, false, false, true, true, 2);
                break;

            case QuestionType.BepaalVorigPunt:
                //start oefening DragEnDropEllips
                lineController.SetVisibles(false, false, false, true, true, 2);
                break;

         
        }
    }
}
