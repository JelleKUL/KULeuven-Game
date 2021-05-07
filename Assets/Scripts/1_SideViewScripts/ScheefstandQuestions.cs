using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//*********** The WaterpassingQuestions sets the required parameters for a specific question ******************//


public class ScheefstandQuestions : BaseQuestions
{
    public enum AnswerType { offset  } // add the available answerTypes here
    [Tooltip("the type of answer a player has to enter")]
    [SerializeField]
    private AnswerType answerType;

    
    private ScheefstandController scheefstandController;
 

    // awake is called before start
    protected override void Awake()
    {
        base.Awake();
        scheefstandController = (ScheefstandController)controller;
        SetQuestionType();
    }

    //sets the parameters for the type of question
    protected override void SetQuestionType()
    {
        if (controlController)
        {
            scheefstandController.StartSetup();

        }
        base.SetQuestionType(); //does the base question stuff like logging
        if (questionUI)
        {
            questionUI.SetInputs(true); //set the input to one
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
            case AnswerType.offset:
                return new List<float>() { GameManager.RoundFloat(scheefstandController.correctDistance, 3) };

            default:
                return new List<float> { 0f };
        }
    }
}