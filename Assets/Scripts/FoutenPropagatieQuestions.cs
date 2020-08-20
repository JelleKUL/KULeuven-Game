using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//*********** The FoutenPropagatieQuestions sets the required parameters for a specific question ******************//


public class FoutenPropagatieQuestions : MonoBehaviour
{
    [Header("Predefined TextFields")]
    public Text titleQuestionText;
    public Text questionText;

    public GameObject winMenu;


    //public Text answerInputX;
    //public Text answerInputY;
    //public Text answerOutput;

    public enum QuestionType { geen, Werking1Punt, WerkingMeerderePunten, MinimaleGrootte, DragEnDropEllips }
    [Tooltip ("Kies het soort vraag voor de oefening")]
    public QuestionType SoortVraag;

    private GameManager gm;
    private PolygonLineController lineController;
    private ObjectPlacer placer;

    // awake is called before start functions
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
            case QuestionType.geen:

                break;

            case QuestionType.Werking1Punt:
                //demo van de foutenellips 1 punt
                lineController.SetVisibles(true, true, false, true, false, false, 2);
                lineController.lockAngleError = false;
                lineController.lockDistanceError = false;
                titleQuestionText.text = "Kijk hoe de foutenellips eruit ziet";
                questionText.text = "Klik in het veld om een meetpunt te paatsen.";
                break;
            case QuestionType.WerkingMeerderePunten:
                //start oefening TekenFoutenEllips
                lineController.SetVisibles(true, true, false, true, false, false, 10);
                placer.PlaceObstacles(1);
                titleQuestionText.text = "Kijk hoe de foutenellips eruit ziet met meerdere punten";
                questionText.text = "Klik in het veld om meetpunten te paatsen met een maximum van 10 punten.";
                break;

            case QuestionType.DragEnDropEllips:
                //start oefening DragEnDropEllips
                
                break;

            case QuestionType.MinimaleGrootte:
                //start oefening MinimaleGrote
                lineController.SetVisibles(true, true, true, true, false, false, 10);
                lineController.lockAngleError = true;
                lineController.lockDistanceError = true;
                lineController.randomizeErrors = true;
                placer.PlaceCalculatePoints(1);
                placer.PlaceObstacleBtwn(1);
                titleQuestionText.text = "Hoe kan je de foutenellips zo klein mogelijk maken?";
                questionText.text = "Probeer via een zo kort mogelijke weg het punt te bereiken.";
                break;
        }
    }

    public void CheckAnswer()
    {
        switch (SoortVraag)
        {
            case QuestionType.geen:

                break;

            case QuestionType.Werking1Punt:
                
                break;
            case QuestionType.WerkingMeerderePunten:
                
                break;

            case QuestionType.DragEnDropEllips:
                

                break;

            case QuestionType.MinimaleGrootte:
                
                break;
        }
    }
}
