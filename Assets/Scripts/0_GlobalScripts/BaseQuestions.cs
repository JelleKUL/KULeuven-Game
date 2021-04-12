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
    protected string ID_maxError = "ID_max_error";
    protected string ID_TooManyTriesText = "ID_too_many_tries";


    protected GameManager gm;
    protected QuestionUIManager questionUI;
    protected float DEGtoGON = 4 / 3.6f;
    protected int currentTries = 0;

    /// <summary>
    /// finds the gameobjects, sets the text to their correct values
    /// </summary>
    protected virtual void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        questionUI = GameObject.FindGameObjectWithTag("QuestionUIController").GetComponent<QuestionUIManager>();

        if (questionUI)
        {
            questionUI.baseQuestions = this;
            questionUI.SetErrorDisplay(ID_maxError, errorMargin, errorUnit);
            if (controlText) questionUI.SetQuestionText(ID_questionHeader, ID_questionText);
            questionUI.SetAnswerOutput("");
        }
    }
    /// <summary>
    /// sets the current question, build upon this to do chapter specific stuff, call base at the end to add logging
    /// </summary>
    protected virtual void SetQuestionType()
    {
        LogAnswer();
    }

    /// <summary>
    /// checks if a given string equals the float value minus the error margin
    /// </summary>
    /// <param name="inputAnswer">the input from the player</param>
    /// <param name="correctAnswer">the current correct answer</param>
    /// <param name="errorMargin">the max error to count the value as correct</param>
    /// <returns>true if close enough</returns>
    protected bool CheckCorrectAnswer(float inputAnswer, float correctAnswer, float errorMargin)
    {
        if (GameManager.showDebugAnswer) Debug.Log(correctAnswer + " = " + inputAnswer + " ? -> " + (Mathf.Abs(correctAnswer - inputAnswer) < errorMargin) + ", with errormargin: " + errorMargin);
        if (Mathf.Abs(correctAnswer - inputAnswer) < errorMargin)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// adds a try, up until the max amount of tries
    /// </summary>
    /// <returns>false if the player has reached the max number of tries</returns>
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

    /// <summary>
    /// called externally to check the correct answer, depending on the inputtype and the current answertype
    /// if the answers is only one value long, it will check against the h value
    /// if the answers is 2 values long, it will check both the x and the y value
    /// </summary>
    public virtual void CheckAnswerInput()
    {
        List<float> correctAnswers = GetCorrectAnswer();

        if (correctAnswers.Count == 1) //only h input check
        {
            if (CheckCorrectAnswer(questionUI.GetAnswerInput(InputType.h), GetCorrectAnswer()[0], errorMargin)) //CorrectAnswer()))
            {
                Debug.Log("Correct answer!");
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
        else if (correctAnswers.Count == 2)
        {
            if (CheckCorrectAnswer(questionUI.GetAnswerInput(InputType.x), GetCorrectAnswer()[0], errorMargin) && CheckCorrectAnswer(questionUI.GetAnswerInput(InputType.y), GetCorrectAnswer()[1], errorMargin)) //CorrectAnswer()))
            {
                Debug.Log("Correct answer!");
                gm.IncreaseScore(scoreIncrease);
                questionUI.ActivateWinMenu();
            }
            else
            {
                questionUI.SetFalseAnswer("ID_incorrect_answer");
                if (!AddTry())
                {
                    questionUI.ShowCorrectAnswer(InputType.x, GetCorrectAnswer()[0], "ID_too_many_tries");
                    questionUI.ShowCorrectAnswer(InputType.y, GetCorrectAnswer()[1], "ID_too_many_tries");
                }
            }
        }
    }

    /// <summary>
    /// displays the correct answer, depending on the inputtype and the current answertype
    /// if the answers is only one value long, it will only show the h value
    /// if the answers is 2 values long, it will show both the x and the y value
    /// </summary>
    public virtual void ShowCorrectAnswer()
    {
        List<float> correctAnswers = GetCorrectAnswer();
        if (correctAnswers.Count == 1) //only h input check
        {
            questionUI.ShowCorrectAnswer(InputType.h, GetCorrectAnswer()[0], ID_answerText);
        }
        else if(correctAnswers.Count == 2)
        {
            questionUI.ShowCorrectAnswer(InputType.x, GetCorrectAnswer()[0], ID_answerText);
            questionUI.ShowCorrectAnswer(InputType.y, GetCorrectAnswer()[1], ID_answerText);
        }
    }
    /// <summary>
    /// logs the correct answers if it is checked in the Gamemanager (admin or not logged in)
    /// </summary>
    protected virtual void LogAnswer()
    {
        if (GameManager.showDebugAnswer)
        {
            string answerText = "Correct antwoord = ";
            foreach (var answer in GetCorrectAnswer())
            {
                answerText += answer.ToString() + ", ";
            }
            Debug.Log(answerText + errorUnit);
        }
    }

    /// <summary>
    /// get all the correct answers, depending on the current answerType
    /// </summary>
    /// <returns> a list of all the correct answers </returns>
    public virtual List <float> GetCorrectAnswer()
    {
        return new List<float>();
    }

}
