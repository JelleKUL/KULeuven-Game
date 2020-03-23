using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonLineController))]
public class FoutenPropagatieQuestions : MonoBehaviour
{
    public enum QuestionType { VanANaarB, TekenFoutenEllips, DragEnDropEllips, MinimaleGrote }
    [Tooltip ("Kies het soort vraag voor de oefening")]
    public QuestionType SoortVraag;

    private PolygonLineController lineController;

    // Start is called before the first frame update
    void Start()
    {
        lineController = GetComponent<PolygonLineController>();
        SetQuestionType(SoortVraag);



    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //sets the type of question, can be altered by another script
    public void SetQuestionType(QuestionType vraag)
    {
        switch (vraag)
        {
            case QuestionType.VanANaarB:
                //start oefening VanANaarB
                lineController.SetVisibles( true, false, true, false, false, 4);

                break;
            case QuestionType.TekenFoutenEllips:
                //start oefening TekenFoutenEllips
                lineController.SetVisibles(true, false, true, false, false, 4);
                break;

            case QuestionType.DragEnDropEllips:
                //start oefening DragEnDropEllips
                lineController.SetVisibles(true, false, true, false, false, 4);
                break;

            case QuestionType.MinimaleGrote:
                //start oefening MinimaleGrote
                lineController.SetVisibles(true, true, true, false, false, 10);

                break;
        }
    }
}
