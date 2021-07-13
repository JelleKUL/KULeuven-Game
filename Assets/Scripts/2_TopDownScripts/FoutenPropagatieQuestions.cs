using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//*********** The FoutenPropagatieQuestions sets the required parameters for a specific question ******************//

[RequireComponent(typeof(ObjectPlacer), typeof(PolygonLineController))]
public class FoutenPropagatieQuestions : BaseQuestions
{

    public enum AnswerType { geen, sigmaDH, sigmaA, MinimaleGrootte }
    [Space(10)]
    [Tooltip ("Kies het soort vraag voor de oefening")]
    public AnswerType answerType;


    //initiate scripts
    private PolygonLineController lineController;
    private ObjectPlacer placer;

    // awake is called before start functions
    protected override void Awake()
    {
        base.Awake();
        lineController = GetComponent<PolygonLineController>();
        placer = GetComponent<ObjectPlacer>();
        SetQuestionType();
    }

    //sets the type of question, can be altered by another script
    protected override void SetQuestionType()
    {
        if (controlController)
        {
            placer.StartSetup();
            lineController.SetAnswerArray(placer.calculatePointsPositions.ToArray());
            lineController.StartSetup();
        }
        base.SetQuestionType(); //does the base question stuff like logging

        if (questionUI) //set the answer input field according to the selected answertype
        {
            switch (answerType)
            {
                case AnswerType.sigmaDH:
                    questionUI.SetInputs(true, errorUnit); //set the input to one
                    break;
                case AnswerType.sigmaA:
                    questionUI.SetInputs(true, errorUnit); //set the input to one
                    break;
                default:
                    break;
            }
        }
    }
  
    //checks if the given anwser is correct
    public override void CheckAnswerInput()
    {
        if (lineController.CheckPoints() && lineController.CheckPointsVisibility())
        {
            if (answerType == AnswerType.MinimaleGrootte)
            {
                if (Mathf.Abs(lineController.GetSigmaA() - GetCorrectAnswer()[0]) <= 0.6 * GetCorrectAnswer()[0])
                {
                    Debug.Log("ID_correct_answer");
                    gm.IncreaseScore(scoreIncrease);
                    questionUI.ActivateWinMenu();
                }
                else
                {
                    questionUI.SetFalseAnswer("ID_incorrect_answer");
                    if (!AddTry())
                    {
                        questionUI.ShowCorrectAnswer(InputType.h, GetCorrectAnswer()[0], "ID_too_many_tries");
                    }
                }
            }
            else
            {

                if (GetCorrectAnswer().Count == 1 ?
                    (CheckCorrectAnswer(questionUI.GetAnswerInput(InputType.h), GetCorrectAnswer()[0], errorMargin)):
                    (CheckCorrectAnswer(questionUI.GetAnswerInput(InputType.h), GetCorrectAnswer()[0], errorMargin)|| CheckCorrectAnswer(questionUI.GetAnswerInput(InputType.h), GetCorrectAnswer()[1], errorMargin))
                   )
                {
                    Debug.Log("ID_correct_answer");
                    gm.IncreaseScore(scoreIncrease);
                    questionUI.ActivateWinMenu();
                }
                else
                {
                    questionUI.SetFalseAnswer("ID_incorrect_answer");
                    if (!AddTry())
                    {
                        questionUI.ShowCorrectAnswer(InputType.h, GetCorrectAnswer()[0], "ID_too_many_tries");
                    }
                }
            }

        }
        else questionUI.SetFalseAnswer( "Setup in P and measure to A before submitting");
    }

    //displays the correct answer
    public override void ShowCorrectAnswer()
    {
        //todo add show shortest path

        questionUI.ShowCorrectAnswer(InputType.h, GetCorrectAnswer()[0], ID_answerText);
    }

    public override List<float> GetCorrectAnswer()//use the exact formula *Pi/2 or rule of thumb *1.5
    {
        float[] newArray;

        switch (answerType)
        {
            case AnswerType.sigmaDH:

                newArray = lineController.GetErrorDH();
                return new List<float>() { Mathf.Max(newArray[0], newArray[1]) };

            case AnswerType.sigmaA:

                newArray = lineController.GetErrorDH();
                return new List<float>() { newArray[2], newArray[3] };

            case AnswerType.MinimaleGrootte:

                newArray = lineController.GetErrorDH();
                return new List<float>() {newArray[3]};

        }
        return new List<float>() { 0 };
    }


}
