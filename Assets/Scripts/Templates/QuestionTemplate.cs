using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//*********** The WaterpassingQuestions sets the required parameters for a specific question ******************//

public class QuestionTemplate : BaseQuestions
{
    public enum AnswerType {  } // add the available answerTypes here
    [Tooltip("the type of answer a player has to enter")]
    [SerializeField]
    private AnswerType answerType;

    //initiate scripts
    private BaseController controller; 

    // awake is called before start
    protected override void Awake()
    {
        base.Awake();
        controller = GetComponent<BaseController>();
        SetQuestionType();
    }

    //sets the parameters for the type of question
    protected override void SetQuestionType()
    {
        if (controlController)
        {
            controller.StartSetup();

        }
        base.SetQuestionType(); //does the base question stuff like logging

        if (questionUI) //set the answer input field according to the selected answertype
        {
            switch (answerType)
            {
                // add the different cases here.

                default:
                    questionUI.SetInputs(true, errorUnit);
                    break;
            }
        }
    }

    //checks if the given anwser is correct
    public override void CheckAnswerInput()
    {
        //add special cases for the aswerchecking here
        base.CheckAnswerInput();

    }

    //displays the correct answer
    public override void ShowCorrectAnswer()
    {
        //add special cases for the answerShowing here
        base.ShowCorrectAnswer();
    }

    public override List<float> GetCorrectAnswer()
    {
        switch (answerType)
        {
            // add the different cases here.

            default:
                return new List<float> { 0f };
        }
    }
}