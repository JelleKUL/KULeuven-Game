using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionUIManager : MonoBehaviour
{
    [SerializeField] private TextLocaliser questionHeaderText;
    [SerializeField] private TextLocaliser questionText;
    [SerializeField] private Text errorDisplayText;

    [SerializeField] private InputField answerInputX;
    [SerializeField] private InputField answerInputY;
    [SerializeField] private InputField answerInputH;
    [SerializeField] private GameObject xInput, yInput, hInput;
    [SerializeField] private TextLocaliser answerOutput;

    [SerializeField] private GameObject winMenu;
    [SerializeField] private GameObject winMenuFree;
    [SerializeField] private GameObject submitBtn;
    [SerializeField] private GameObject restartBtn;

    [SerializeField] private Color falseColor, CorrectColor;

    [HideInInspector] public BaseQuestions baseQuestions;

    public void SetQuestionText(string ID_questionHeader, string ID_questionText)
    {
        if (questionHeaderText) questionHeaderText.UpdateText(ID_questionHeader);
        if (questionText) questionText.UpdateText(ID_questionText);
    }

    public void SetAnswerOutput(string ID_answerText)
    {
        if (answerOutput) answerOutput.UpdateText(ID_answerText);
    }

    // error display control
    public void SetErrorDisplay(string ID_maxError, float errorMargin, string errorUnit)
    {
        if (errorDisplayText) errorDisplayText.text = LocalisationManager.GetLocalisedValue(ID_maxError) + " " + errorMargin.ToString() + errorUnit;
    }

    //set the correct input

    public void SetInputs(bool one)
    {
        if (xInput) xInput.SetActive(!one);
        if (yInput) yInput.SetActive(!one);
        if (hInput) hInput.SetActive(one);
    }

    public void ActivateWinMenu()
    {
        if (GameManager.campaignMode)
        {
            if(winMenu) winMenu.SetActive(true);
        }
        else
        {
            if(winMenuFree) winMenuFree.SetActive(true);
        }
    }
    public void setRestartButtons()
    {
        if(submitBtn) submitBtn.SetActive(false);
        if(restartBtn) restartBtn.SetActive(true);
    }

    public void SetFalseAnswer(string ID_answerText)
    {
        SetAnswerOutput(ID_answerText);
        answerInputX.textComponent.color = falseColor;
        answerInputY.textComponent.color = falseColor;
        answerInputH.textComponent.color = falseColor;
    }

    public void ShowCorrectAnswer(InputType type, float correctValue, string ID_answerText)
    {
        InputField answerDisplay = answerInputH;
        switch (type)
        {
            case InputType.x:
                if (answerInputX) answerDisplay = answerInputX;
                break;
            case InputType.y:
                if (answerInputY) answerDisplay = answerInputY;
                break;
            case InputType.h:
                if (answerInputH) answerDisplay = answerInputH;
                break;
        }
        answerDisplay.textComponent.color = falseColor;
        answerDisplay.text = correctValue.ToString();
        answerDisplay.interactable = false;

        SetAnswerOutput(ID_answerText);
        setRestartButtons();
    }

    public float GetAnswerInput(InputType type)
    {
        string answer = "";

        switch (type)
        {
            case InputType.x:
                if (answerInputX) answer = answerInputX.text;
                break;
            case InputType.y:
                if (answerInputY) answer = answerInputY.text;
                break;
            case InputType.h:
                if (answerInputH) answer = answerInputH.text;
                break;
        }

        answer = answer.Replace(",", "."); //replace all the , with . to parse

        float answerNr = 0f;
        float.TryParse(answer, out answerNr);
        
        return answerNr;
    }

    public void SubmitAnswerInput()
    {
        if (baseQuestions) baseQuestions.CheckAnswerInput();
    }

    public void ShowBaseQuestionCorrectAnswer()
    {
        if (baseQuestions) baseQuestions.ShowCorrectAnswer();
    }
}
