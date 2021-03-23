﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//*********** The WaterpassingQuestions sets the required parameters for a specific question ******************//


public class WaterpassingQuestions : MonoBehaviour
{
    [Header("Predefined TextFields")]
    public Text questionHeaderText;
    public Text questionText;

    public Text answerInputH;   
    public Text answerOutput;

    public GameObject winMenu, winMenuFree, submitBtn, restartBtn;
    public Color falseColor, CorrectColor;

    
    public enum QuestionType { Geen, Hoogteverschil2Punten, HoogteVerschilMeerPunten, Afstand2Punten, Hoekfout, KringWaterpassing,Zijslag, Scheefstand, OmgekeerdeBaak, ScheveWaterpassing, AfstandMeerPunten }
    [Tooltip("Kies het soort vraag voor de oefening")]
    private QuestionType SoortVraag;
    [SerializeField]
    private WaterPassingQuestionScriptableObject waterpassingQuestion;

    public int scoreIncrease = 1;

    [Tooltip("het aantal keren dat je mag proberen, 0 = oneindig")]
    public int nrOfTries = 0;

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

        SetQuestionType(SoortVraag);

        if (answerOutput) answerOutput.text = "";

    }
    
    

    //sets the parameters for the type of question
    public void SetQuestionType(QuestionType vraag)
    {
        if (waterpassingQuestion)
        {
            waterpassingQuestion.SetQuestion(waterpassing);
            correctAnswer = waterpassingQuestion.GetCorrectAnswer(waterpassing);
            questionHeaderText.text = waterpassingQuestion.ID_questionHeader;
            questionText.text = waterpassingQuestion.ID_questionText;
            AnswerExplanation = waterpassingQuestion.ID_answerText;
        }
        /*
        switch (vraag)
        {
            case QuestionType.Geen:

                break;

            case QuestionType.Hoogteverschil2Punten:
                waterpassing.SetParameters(2, 2, 1, false, false, Vector2.zero, false, Vector2.zero, false);
                correctAnswer = GameManager.RoundFloat(waterpassing.correctHeight,3);
                questionHeaderText.text = "II. Bepaal het hoogteveschil tussen A & B";
                questionText.text = "Plaats de meetbaken op de meetpunten en meet met het waterpastoestel het verschil tussen beide punten";
                AnswerExplanation = "Het hoogteverschil kan gevonden worden door beide middendraden te vergelijken";

                break;

            case QuestionType.HoogteVerschilMeerPunten:
                waterpassing.SetParameters(4, 4, 1, false, false, Vector2.zero, false, Vector2.zero, false);
                correctAnswer = GameManager.RoundFloat(waterpassing.correctHeight,3);
                questionHeaderText.text = "III. Bepaal het hoogteveschil tussen A & D";
                questionText.text = "Plaats de meetbaken op de meetpunten en meet met het waterpastoestel het hoogteverschil tussen de punten A en D";
                AnswerExplanation = "Het hoogteverschil kan gevonden worden door de middendraden te vergelijken van elk punt, hou rekening met het teken van de verschillen";

                break;

            case QuestionType.Afstand2Punten:
                waterpassing.SetParameters(2, 2, 1, true, false, Vector2.zero, false, Vector2.zero, false);
                correctAnswer = GameManager.RoundFloat(waterpassing.correctDistance * GameManager.worldScale,1);
                questionHeaderText.text = "V. Bepaal de afstand tussen A & B";
                questionText.text = "Plaats de meetbaken op de meetpunten en gebruik het vizier om de afstand te berekenen";
                AnswerExplanation = "De afstand kan gevonden worden door de afstand tussen de boven-en onderdraad te meten";

                break;

            case QuestionType.AfstandMeerPunten:
                waterpassing.SetParameters(4, 4, 1, true, false, Vector2.zero, false, Vector2.zero, false);
                correctAnswer = GameManager.RoundFloat(waterpassing.correctDistance * GameManager.worldScale, 1);
                questionHeaderText.text = "VI. Bepaal de afstand tussen A & D";
                questionText.text = "Plaats de meetbaken op de meetpunten en gebruik het vizier om de afstand te berekenen";
                AnswerExplanation = "De afstand kan gevonden worden door de afstand tussen de boven-en onderdraad te meten";

                break;

            case QuestionType.Hoekfout:
                waterpassing.SetParameters(0, 1, 1, true, true, new Vector2(6,1), true, new Vector2(9,1), false);
                correctAnswer = GameManager.RoundFloat(waterpassing.correctScaledErrorAngle * DEGtoGON, 3);
                questionHeaderText.text = "I. Bepaal de collimatiefout van het toestel";
                questionText.text = "Bereken de collimatiefout d.m.v. een exentrieke plaatsing en de afstanden tot de meetbaak";
                AnswerExplanation = "De collimatiefout kan bepaald worden door het veschil in hoogte te vergelijken in functie van de afstand";

                break;

            case QuestionType.KringWaterpassing:
                waterpassing.SetParameters(3, 5, 1, false, false, Vector2.zero, false, Vector2.zero, true);
                correctPoints = waterpassing.correctHeightDifferences;
                questionHeaderText.text = "VII. Vervolledig de waterpassingtabel";
                questionText.text = "Voer alle noodzakelijke metingen uit en vul de juiste waardes in in de tabel";
                AnswerExplanation = "Het hoogteverschil kan gevonden worden door de middendraden te vergelijken";

                break;

            case QuestionType.Zijslag:
                waterpassing.SetParameters(3, 5, 1, false, false, Vector2.zero, false, Vector2.zero, true);
                correctPoints = waterpassing.correctHeightDifferences;
                questionHeaderText.text = "VIII. Vervolledig de waterpassingtabel met zijslag";
                questionText.text = "Voer alle noodzakelijke metingen uit en vul de juiste waardes in in de tabel";
                AnswerExplanation = "Het hoogteverschil kan gevonden worden door de middendraden te vergelijken";

                break;

            case QuestionType.Scheefstand:
                //waterpassing.SetParameters(2, 2, 1, false, false, Vector2.zero, false, Vector2.zero, false);
                //correctAnswer = waterpassing.correctHeight; calculated in the scheefstandsmanager
                questionHeaderText.text = "X. Bepaal de scheefstand van het gebouw";
                questionText.text = "Plaats de theodoliet en meet de horizontale verschuiving tussen beide punten, gemeten van onder naar boven. Gebruik de 'flip' knop om een tweede meting te maken en een eventuele inclinatiefout op te heffen.";
                AnswerExplanation = "De scheefstand kan gevonden worden door de onderste meetband te vergelijken met de laserlijn";

                break;

            case QuestionType.OmgekeerdeBaak:
                waterpassing.SetParameters(1, 2, 1, false, false, Vector2.zero, false, Vector2.zero, false);
                correctAnswer = GameManager.RoundFloat(waterpassing.correctHeight,3);
                questionHeaderText.text = "IV. Bepaal het hoogteveschil tussen A & B";
                questionText.text = "Plaats de meetbaken op de meetpunten en meet met het waterpastoestel het verschil tussen beide punten" +
                    " Opgelet, één van de baken staat omgekeerd.";
                AnswerExplanation = "Het hoogteverschil kan gevonden worden door beide middendraden te vergelijken";

                break;
            case QuestionType.ScheveWaterpassing:
                waterpassing.SetParameters(2, 2, 1, false, false, Vector2.zero, false, Vector2.zero, false);
                correctAnswer = GameManager.RoundFloat(waterpassing.correctHeight,3);
                questionHeaderText.text = "IX. Bepaal het hoogteveschil tussen A & B";
                questionText.text = "Plaats de meetbaken op de meetpunten en meet met de theodoliet het verschil tussen beide punten";
                AnswerExplanation = "Het hoogteverschil kan gevonden worden door beide middendraden te vergelijken";

                break;
        }
        */
        if(GameManager.showDebugAnswer && vraag != QuestionType.Scheefstand) Debug.Log("Correct antwoord = " + correctAnswer + " m of gon");
    }



    //checks if the given anwser is correct
    public void CheckAnswer()
    {

        if (gm.CheckCorrectAnswer(answerInputH.text, waterpassingQuestion.GetCorrectAnswer(waterpassing))) //CorrectAnswer()))
        {
            gm.IncreaseScore(scoreIncrease, 1);
            Debug.Log(answerInputH.text + " is correct!");

            if (GameManager.campaignMode)
            {
                winMenu.SetActive(true);
            }
            else
            {
                winMenuFree.SetActive(true);
            }
            
            //gm.ReloadScene();
        }
        else
        {
            answerOutput.text = "Waarde incorrect...";
            answerInputH.color = falseColor;
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
                winMenu.SetActive(true);
            }
            else
            {
                winMenuFree.SetActive(true);
            }
            //gm.ReloadScene();
            answerOutput.text = "De waarden die zijn ingevoerd zijn correct";
        }
        else
        {
            answerOutput.text = "De waarden die zijn ingevoerd zijn niet correct";
            Debug.Log("false");
        }
    }

    //displays the correct answer
    public void ShowAnswer()
    {
        if(SoortVraag == QuestionType.KringWaterpassing || SoortVraag == QuestionType.Zijslag)
        {
            waterpassing.ShowAnswersTabel();
        }

        if (answerInputH.transform.parent.GetComponent<InputField>())
        {
            answerInputH.color = falseColor;
            InputField answerDisplay = answerInputH.transform.parent.GetComponent<InputField>();
            answerDisplay.text = CorrectAnswer().ToString();
            answerDisplay.interactable = false;
        }

        answerOutput.text = AnswerExplanation;



        Debug.Log("showing answer");

    }

    public void setRestart()
    {
        ShowAnswer();
        submitBtn.SetActive(false);
        restartBtn.SetActive(true);
        answerOutput.text = "Te veel pogingen, probeer opnieuw.";
    }
    
    public float CorrectAnswer()
    {
        switch (SoortVraag)
        {
            case QuestionType.Hoogteverschil2Punten:

                return GameManager.RoundFloat(waterpassing.correctHeight, 3);

            case QuestionType.HoogteVerschilMeerPunten:

                return GameManager.RoundFloat(waterpassing.correctHeight, 3);


            case QuestionType.Afstand2Punten:


                return GameManager.RoundFloat(waterpassing.correctDistance * GameManager.worldScale, 1);

            case QuestionType.AfstandMeerPunten:


                return GameManager.RoundFloat(waterpassing.correctDistance * GameManager.worldScale, 1);

            case QuestionType.Hoekfout:

                return GameManager.RoundFloat(waterpassing.correctScaledErrorAngle * DEGtoGON, 3);


            case QuestionType.KringWaterpassing:

                return GameManager.RoundFloat(waterpassing.correctHeight, 3);

            case QuestionType.Zijslag:

                return GameManager.RoundFloat(waterpassing.correctHeight, 3);


            case QuestionType.OmgekeerdeBaak:

                return GameManager.RoundFloat(waterpassing.correctHeight, 3);

            case QuestionType.ScheveWaterpassing:

                return GameManager.RoundFloat(waterpassing.correctHeight, 3);

        }
        return 0;
    }
}