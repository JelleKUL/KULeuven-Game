using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//*********** The FoutenPropagatieQuestions sets the required parameters for a specific question ******************//


public class FoutenPropagatieQuestions : BaseQuestions
{

    public enum QuestionType { geen, Errorellips_sigmaDH, Errorellips_sigmaA, MinimaleGrootte, WerkingMeerderePunten, DragEnDropEllips }
    [Tooltip ("Kies het soort vraag voor de oefening")]
    public QuestionType SoortVraag;

    public int maxPoints;

    //initiate scripts
    private PolygonLineController lineController;
    private PolygonPointController thisPoint;
    private ObjectPlacer placer;

    // awake is called before start functions
    protected override void Awake()
    {
        base.Awake();
        lineController = GameObject.FindGameObjectWithTag("PolygonLine").GetComponent<PolygonLineController>();
        placer = GetComponent<ObjectPlacer>();
        SetQuestionType(SoortVraag);
    }

    //sets the type of question, can be altered by another script
    public void SetQuestionType(QuestionType vraag)
    {
        

    }
  
    //checks if the given anwser is correct
    public void CheckAnswer()
    {
        if (lineController.CheckPoints() && lineController.CheckPointsVisibility())
        {
            if (CheckCorrectAnswer(questionUI.GetAnswerInput(InputType.h), CorrectAnswer(true), errorMargin) || CheckCorrectAnswer(questionUI.GetAnswerInput(InputType.h), CorrectAnswer(false), errorMargin))
            {
                gm.IncreaseScore(scoreIncrease, 2);
                questionUI.ActivateWinMenu();
                questionUI.SetAnswerOutput("De waarden die zijn ingevoerd zijn correct");
            }
            else
            {

                Debug.Log("false");


                if (nrOfTries > 0)
                {
                    currentTries++;
                    if (currentTries >= nrOfTries)
                    {
                        //setRestart();
                        return;
                    }
                }
            }

        }
        else questionUI.SetFalseAnswer( "Setup in P and measure to A before submitting");
    }

    //checks if the given anwser is correct
    public void CheckAnswerMinimaal()
    {
        if (lineController.CheckPoints() && lineController.CheckPointsVisibility())
        {

            if (Mathf.Abs(lineController.GetSigmaA() - CorrectAnswer(false)) <=0.6* lineController.GetBaseDistanceError())
            {
                gm.IncreaseScore(scoreIncrease, 2);
                Debug.Log("true");
                questionUI.ActivateWinMenu();

        }
            else
            {
                questionUI.SetFalseAnswer("Incorrect");

                if (AddTry())
                {
                    questionUI.ShowCorrectAnswer(InputType.h, CorrectAnswer(true), ID_answerText);
                }
            }

        }
        else questionUI.SetFalseAnswer("Setup in P and measure to A before submitting");
    }

    //displays the correct answer
    public void ShowAnswer()
    {

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


}
