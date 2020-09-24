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
    public GameObject winMenu;

    public int maxPoints;


    //public Text answerInputX;
    //public Text answerInputY;
    //public Text answerOutput;

    public enum QuestionType { geen, Werking1Punt, Werking1Puntxy, MinimaleGrootte, WerkingMeerderePunten, DragEnDropEllips }
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
                titleQuestionText.text = "Bepaal de standaardafwijking van P";
                questionText.text = "Bereken a.d.v. de hoekfout en de afstandsfout de maximale standaardafwijking van de errorellips"; 
                break;
            case QuestionType.Werking1Puntxy:
                //demo van de foutenellips 1 punt
                lineController.SetVisibles(true, true, false, true, false, false, 2);
                lineController.lockAngleError = false;
                lineController.lockDistanceError = false;
                titleQuestionText.text = "Bepaal de standaardafwijking van P in X en Y";
                questionText.text = "Bereken de X-en Y-component van de errorellips.";
                break;
            
            case QuestionType.MinimaleGrootte:
                //start oefening MinimaleGrote
                lineController.randomizeErrors = false;
                lineController.SetVisibles(true, true, true, true, false, false, 10);
                lineController.lockAngleError = true;
                lineController.lockDistanceError = true;
                
                placer.PlaceCalculatePoints(1);
                placer.PlaceObstacleBtwn(1);
                titleQuestionText.text = "Bepaal P met een zo klein mogelijke errorellips";
                questionText.text = "Meet P via tussenopstellingen met een zo klein mogelijke fout.";
                break;
            case QuestionType.WerkingMeerderePunten:
                //start oefening TekenFoutenEllips
                lineController.SetVisibles(true, true, false, true, false, false, 10);
                placer.PlaceObstacles(1);
                titleQuestionText.text = "Bepaal de standaardafwijking van P in X en Y";
                questionText.text = "Bereken de X-en Y-component van de errorellips van P via meerdere punten.";
                break;

            case QuestionType.DragEnDropEllips:
                //start oefening DragEnDropEllips

                break;

        }
    }
    // checks the answer
    public void CheckAnswer()
    {
        switch (SoortVraag)
        {
            case QuestionType.geen:

                break;

            case QuestionType.Werking1Punt:
                gm.IncreaseScore(0, 2);
                winMenu.SetActive(true);
                break;
            case QuestionType.Werking1Puntxy:
                gm.IncreaseScore(0, 2);
                winMenu.SetActive(true);
                break;
           
            case QuestionType.MinimaleGrootte:

                Debug.Log(lineController.LastPointSnapped());
                if (lineController.LastPointSnapped())
                {
                    int points = Mathf.Max(0, maxPoints - Mathf.FloorToInt(lineController.biggestEllips / 10f));

                    gm.IncreaseScore(points, 2);
                    winMenu.SetActive(true);
                }
               
                break;
            case QuestionType.WerkingMeerderePunten:
                gm.IncreaseScore(0, 2);
                winMenu.SetActive(true);
                break;

            case QuestionType.DragEnDropEllips:

                break;

        }
    }
}
