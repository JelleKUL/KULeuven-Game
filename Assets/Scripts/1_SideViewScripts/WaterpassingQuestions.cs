using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//*********** The WaterpassingQuestions sets the required parameters for a specific question ******************//

[RequireComponent(typeof(WaterPassingController))]
public class WaterpassingQuestions : BaseQuestions
{
    public enum AnswerType { Height, Distance, ErrorAngle, Table, none }
    [Tooltip("the type of answer a player has to enter")]
    [SerializeField]
    private AnswerType answerType;

    //initiate scripts
    private WaterPassingController waterpassing;

    // awake is called before start
    protected override void Awake()
    {
        base.Awake();
        waterpassing = GetComponent<WaterPassingController>();
        SetQuestionType();
    }
    
    //sets the parameters for the type of question
    protected override void SetQuestionType()
    {
        if (controlController)
        {
            waterpassing.StartSetup();
            
        }
        base.SetQuestionType(); //does the base question stuff like logging
    }

    //checks if the given anwser is correct
    public override void CheckAnswerInput()
    {
        //special case: table other type of answer checking
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
        else
        {
            base.CheckAnswerInput();
        }
    }

    //displays the correct answer
    public override void ShowCorrectAnswer()
    {
        if (answerType == AnswerType.Table) //SoortVraag == QuestionType.KringWaterpassing || SoortVraag == QuestionType.Zijslag)
        {
            waterpassing.ShowAnswersTabel();
        }
        else
        {
            base.ShowCorrectAnswer();
        }
    }

    public override List <float> GetCorrectAnswer()
    {
        switch (answerType)
        {
            case AnswerType.Height:
                return new List<float>() { GameManager.RoundFloat(waterpassing.correctHeight, 3) };

            case AnswerType.Distance:
                return new List<float>() { GameManager.RoundFloat(waterpassing.correctDistance * GameManager.worldScale, 1) };

            case AnswerType.ErrorAngle:
                return new List<float>() { GameManager.RoundFloat(waterpassing.correctScaledErrorAngle * 4 / 3.6f, 3) };

            default:
                return new List<float> { 0f };
        }
    }
}