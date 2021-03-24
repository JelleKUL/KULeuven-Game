﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//*********** The WaterpassingQuestions sets the required parameters for a specific question ******************//


public class WaterpassingQuestions : BaseQuestions
{
    public enum AnswerType { Height, Distance, ErrorAngle, Table, none }

    [Tooltip("the type of answer a player has to enter")]
    [SerializeField]
    private AnswerType answerType;

    private WaterPassingController waterpassing;

    // awake is called before start
    protected override void Awake()
    {
        base.Awake();
        waterpassing = GetComponent<WaterPassingController>();
        SetQuestionType();//SoortVraag);
    }
    
    //sets the parameters for the type of question
    public void SetQuestionType()//QuestionType vraag)
    {
        if (controlController)
        {
            waterpassing.StartSetup();
            correctAnswer = CorrectAnswer();
            if (GameManager.showDebugAnswer) Debug.Log("Correct antwoord = " + correctAnswer + " m of gon");
        }
        
    }

    //checks if the given anwser is correct
    public override void CheckAnswerInput()
    {
        base.CheckAnswerInput();

        if(answerType == AnswerType.Table)
        {
            if (waterpassing.CheckTabelAnswer())
            {
                gm.IncreaseScore(scoreIncrease, 1);
                Debug.Log("true");

                questionUI.ActivateWinMenu();
                questionUI.SetAnswerOutput("De waarden die zijn ingevoerd zijn correct");
            }
            else
            {
                questionUI.SetAnswerOutput("De waarden die zijn ingevoerd zijn niet correct");
                Debug.Log("false");
            }
        }

        else if(answerType != AnswerType.none)
        {
            if (CheckCorrectAnswer(questionUI.GetAnswerInput(InputType.h), CorrectAnswer(), errorMargin)) //CorrectAnswer()))
            {
                Debug.Log("Correct answer!");
                gm.IncreaseScore(scoreIncrease, 1);
                questionUI.ActivateWinMenu();
            }
            else
            {
                questionUI.SetFalseAnswer("Incorrect answer...");
                if (!AddTry())
                {
                    questionUI.ShowCorrectAnswer(InputType.h, CorrectAnswer(), "Too many tries...");
                }
            }
        }
    }

    //displays the correct answer
    public override void ShowCorrectAnswer()
    {
        base.ShowCorrectAnswer();

        Debug.Log("showing answer");
        questionUI.ShowCorrectAnswer(InputType.h, CorrectAnswer(), ID_answerText);

        if (answerType == AnswerType.Table) //SoortVraag == QuestionType.KringWaterpassing || SoortVraag == QuestionType.Zijslag)
        {
            waterpassing.ShowAnswersTabel();
        }
    }

    public float CorrectAnswer()
    {
        switch (answerType)
        {
            case AnswerType.Height:
                return GameManager.RoundFloat(waterpassing.correctHeight, 3);

            case AnswerType.Distance:
                return GameManager.RoundFloat(waterpassing.correctDistance * GameManager.worldScale, 1);

            case AnswerType.ErrorAngle:
                return GameManager.RoundFloat(waterpassing.correctScaledErrorAngle * 4 / 3.6f, 3);

            default:
                return 0f;
        }
    }
}