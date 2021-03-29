using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public enum InputType { x, y, h}

public class BaseQuestions : MonoBehaviour
{
    [Header("Basic Question Parameters")]
    [SerializeField]
    protected bool controlText = true;
    [SerializeField]
    protected bool controlController = true;

    [Tooltip("amount of points a player gets by entering the right answer")]
    [SerializeField]
    protected int scoreIncrease = 1;
    [Tooltip("the amount of tries a player has to anter a wrong answer, 0 = infinite")]
    [SerializeField]
    protected int nrOfTries = 0;
    [Tooltip("the errormargin for answers")]
    [SerializeField]
    protected float errorMargin = 0.001f;
    [SerializeField]
    [Tooltip("the unit of the errormargin")]
    protected string errorUnit = "m";

    [Header("Localised Text Strings")]
    [Tooltip("The title of the question")]
    [SerializeField]
    [TextArea(1, 5)]
    protected string ID_questionHeader = "";
    [Tooltip("the explaination of the question")]
    [SerializeField]
    [TextArea(2, 5)]
    protected string ID_questionText = "";
    [Tooltip("The feedbacktext of the question")]
    [SerializeField]
    [TextArea(2, 3)]
    protected string ID_answerText = "";
    [SerializeField]
    protected string ID_maxError = "max error: ";
    [SerializeField]
    protected string ID_TooManyTriesText = "too many tries, try again";


    protected GameManager gm;
    protected QuestionUIManager questionUI;
    protected float DEGtoGON = 4 / 3.6f;
    protected int currentTries = 0;
    protected float correctAnswer;

    protected virtual void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        questionUI = GameObject.FindGameObjectWithTag("QuestionUIController").GetComponent<QuestionUIManager>();

        if (questionUI)
        {
            questionUI.baseQuestions = this;
            questionUI.SetErrorDisplay(ID_maxError, errorMargin, errorUnit);
            if (controlText) questionUI.SetQuestionText(ID_questionHeader, ID_questionText);
            questionUI.SetAnswerOutput();
        }
    }

    protected virtual void SetQuestionType()
    {

    }

    // checks if a given string equals the float value minus the error margin
    protected bool CheckCorrectAnswer(float inputAnswer, float correctAnswer, float errorMargin)
    {
        if (GameManager.showDebugAnswer) Debug.Log(correctAnswer + " = " + inputAnswer + " ? -> " + (Mathf.Abs(correctAnswer - inputAnswer) < errorMargin) + ", with errormargin: " + errorMargin);
        if (Mathf.Abs(correctAnswer - inputAnswer) < errorMargin)
        {
            return true;
        }
        return false;
    }
    //returns false if the player is out of tries
    protected virtual bool AddTry()
    {
        if (nrOfTries > 0)
        {
            currentTries++;
            if (currentTries >= nrOfTries)
            {
                return false;
            }
        }
        return true;
    }

    //called externally to check the correct answer, depending on the inputtype
    public virtual void CheckAnswerInput()
    {

    }

    //displays the correct answer
    public virtual void ShowCorrectAnswer()
    {

    }

    //returns all the correct answers
    public virtual List <float> GetCorrectAnswer()
    {
        return new List<float>();
    }

}
