using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//*********** The WaterpassingQuestions sets the required parameters for a specific question ******************//


public class WaterpassingQuestions : MonoBehaviour
{
    public enum AnswerType { Height, Distance, ErrorAngle, Table, none }


    [Header("Question Parameters")]
    [SerializeField]
    private bool controlText = true;
    [SerializeField]
    private bool controlController = true;
    [Tooltip("the type of answer a player has to enter")]
    [SerializeField]
    private AnswerType answerType;
    [Tooltip("amount of points a player gets by entering the right answer")]
    [SerializeField]
    private int scoreIncrease = 1;
    [Tooltip("the amount of tries a player has to anter a wrong answer, 0 = infinite")]
    [SerializeField]
    private int nrOfTries = 0;
    [Header("Localised Text Strings")]
    [Tooltip("The title of the question")]
    [SerializeField]
    [TextArea(1, 5)]
    private string ID_questionHeader = "";
    [Tooltip("the explaination of the question")]
    [SerializeField]
    [TextArea(2, 5)]
    private string ID_questionText = "";
    [Tooltip("The feedbacktext of the question")]
    [SerializeField]
    [TextArea(2, 3)]
    private string ID_answerText = "";

    [Serializable]
    private class SceneObjects
    {
        public string ID_TooManyTriesText = "too many tries, try again";
        public Text questionHeaderText;
        public Text questionText;
        public Text answerInputH;
        public Text answerOutput;
        public GameObject winMenu;
        public GameObject winMenuFree;
        public GameObject submitBtn;
        public GameObject restartBtn;
        public Color falseColor, CorrectColor;
    }
    [Header("Scene Objects")]
    [Space(20)]
    [SerializeField]
    private SceneObjects sceneObjects;
    
    private WaterPassingController waterpassing;
    private GameManager gm;
    private ScheefstandController scheefstandController;
    private float correctAnswer;
    private float[] correctPoints;
    private string AnswerExplanation;
    private float DEGtoGON = 4 / 3.6f;
    private int currentTries = 0;

    // awake is called before start
    void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        waterpassing = GetComponent<WaterPassingController>();

        SetQuestionType();//SoortVraag);

        if (sceneObjects.answerOutput) sceneObjects.answerOutput.text = "";

    }
    
    //sets the parameters for the type of question
    public void SetQuestionType()//QuestionType vraag)
    {
        if (controlController)
        {
            waterpassing.StartSetup();
            correctAnswer = CorrectAnswer();
        }
        if (controlText)
        {
            sceneObjects.questionHeaderText.text = ID_questionHeader;
            sceneObjects.questionText.text = ID_questionText;
            AnswerExplanation = ID_answerText;
        }

        //if (GameManager.showDebugAnswer && vraag != QuestionType.Scheefstand) Debug.Log("Correct antwoord = " + correctAnswer + " m of gon");
    }

    //checks if the given anwser is correct
    public void CheckAnswer()
    {

        if (gm.CheckCorrectAnswer(sceneObjects.answerInputH.text, CorrectAnswer())) //CorrectAnswer()))
        {
            gm.IncreaseScore(scoreIncrease, 1);
            Debug.Log(sceneObjects.answerInputH.text + " is correct!");

            if (GameManager.campaignMode)
            {
                sceneObjects.winMenu.SetActive(true);
            }
            else
            {
                sceneObjects.winMenuFree.SetActive(true);
            }
            
            //gm.ReloadScene();
        }
        else
        {
            sceneObjects.answerOutput.text = "Waarde incorrect...";
            sceneObjects.answerInputH.color = sceneObjects.falseColor;
            Debug.Log("false");

            if (nrOfTries > 0)
            {
                currentTries++;
                if (currentTries >= nrOfTries)
                {
                    setRestart();
                    return;
                }
            }
        }

    }

    public void CheckAnswerArray()
    {
        if (waterpassing.CheckTabelAnswer())
        {
            gm.IncreaseScore(scoreIncrease, 1);
            Debug.Log("true");
            if (GameManager.campaignMode)
            {
                sceneObjects.winMenu.SetActive(true);
            }
            else
            {
                sceneObjects.winMenuFree.SetActive(true);
            }
            //gm.ReloadScene();
            sceneObjects.answerOutput.text = "De waarden die zijn ingevoerd zijn correct";
        }
        else
        {
            sceneObjects.answerOutput.text = "De waarden die zijn ingevoerd zijn niet correct";
            Debug.Log("false");
        }
    }

    //displays the correct answer
    public void ShowAnswer()
    {
        if(answerType == AnswerType.Table) //SoortVraag == QuestionType.KringWaterpassing || SoortVraag == QuestionType.Zijslag)
        {
            waterpassing.ShowAnswersTabel();
        }

        if (sceneObjects.answerInputH.transform.parent.GetComponent<InputField>())
        {
            sceneObjects.answerInputH.color = sceneObjects.falseColor;
            InputField answerDisplay = sceneObjects.answerInputH.transform.parent.GetComponent<InputField>();
            answerDisplay.text = CorrectAnswer().ToString();
            answerDisplay.interactable = false;
        }

        sceneObjects.answerOutput.text = AnswerExplanation;
        Debug.Log("showing answer");
    }

    public void setRestart()
    {
        ShowAnswer();
        sceneObjects.submitBtn.SetActive(false);
        sceneObjects.restartBtn.SetActive(true);
        sceneObjects.answerOutput.text = sceneObjects.ID_TooManyTriesText;
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

            case AnswerType.Table:
                return 0f;

            default:
                return 0f;
        }
    }
}