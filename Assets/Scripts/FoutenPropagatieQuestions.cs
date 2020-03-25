using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoutenPropagatieQuestions : MonoBehaviour
{
    [Header("Predefined TextFields")]
    public Text questionText;
    //public Text answerInputX;
    //public Text answerInputY;
    //public Text answerOutput;

    public enum QuestionType { geen, Werking1Punt, WerkingMeerderePunten, MinimaleGrootte, DragEnDropEllips }
    [Tooltip ("Kies het soort vraag voor de oefening")]
    public QuestionType SoortVraag;

    private GameManager gm;
    private PolygonLineController lineController;
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
            case QuestionType.geen:

                break;

            case QuestionType.Werking1Punt:
                //demo van de foutenellips 1 punt
                lineController.SetVisibles(true, true, false, true, false, false, 2);
                questionText.text = "Kijk hoe de foutenellips eruit ziet";
                break;
            case QuestionType.WerkingMeerderePunten:
                //start oefening TekenFoutenEllips
                lineController.SetVisibles(true, true, false, true, false, false, 10);
                placer.PlaceObstacles(1);
                questionText.text = "Kijk hoe de foutenellips eruit ziet met meerdere punten";
                break;

            case QuestionType.DragEnDropEllips:
                //start oefening DragEnDropEllips
                
                break;

            case QuestionType.MinimaleGrootte:
                //start oefening MinimaleGrote
                lineController.SetVisibles(true, true, true, true, false, false, 10);
                placer.PlaceCalculatePoints(1);
                placer.PlaceObstacles(1);
                questionText.text = "Hoe kan je de foutenellips zo klein mogelijk maken?";
                break;
        }
    }
}
