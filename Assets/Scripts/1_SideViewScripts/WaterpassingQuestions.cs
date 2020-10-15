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

    public GameObject winMenu, winMenuFree;
    public Color falseColor, CorrectColor;

    
    public enum QuestionType { Geen, Hoogteverschil2Punten, HoogteVerschilMeerPunten, Afstand2Punten, Hoekfout, KringWaterpassing, Scheefstand, OmgekeerdeBaak, ScheveWaterpassing, AfstandMeerPunten }
    [Tooltip("Kies het soort vraag voor de oefening")]
    public QuestionType SoortVraag;

    public int scoreIncrease = 1;

    private WaterPassingController waterpassing;
    private GameManager gm;
    private float correctAnswer;
    private float[] correctPoints;
    private string AnswerExplanation;

    private float DEGtoGON = 4 / 3.6f;



    // awake is called before start
    void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        waterpassing = GetComponent<WaterPassingController>();

        SetQuestionType(SoortVraag);
    }
    
    

    //sets the parameters for the type of question
    public void SetQuestionType(QuestionType vraag)
    {
        switch (vraag)
        {
            case QuestionType.Geen:

                break;

            case QuestionType.Hoogteverschil2Punten:
                waterpassing.SetParameters(2, 2, 1, false, false, Vector2.zero, false, Vector2.zero, false);
                correctAnswer = GameManager.RoundFloat(waterpassing.correctHeight,3);
                questionHeaderText.text = "Bepaal het hoogteveschil tussen A & B";
                questionText.text = "Plaats de meetbaken op de meetpunten en meet met het waterpastoestel het verschil tussen beide punten";
                AnswerExplanation = "Het hoogteverschil kan gevonden worden door beide middendraden te vergelijken.";

                break;

            case QuestionType.HoogteVerschilMeerPunten:
                waterpassing.SetParameters(4, 4, 1, false, false, Vector2.zero, false, Vector2.zero, false);
                correctAnswer = GameManager.RoundFloat(waterpassing.correctHeight,3);
                questionHeaderText.text = "Bepaal het hoogteveschil tussen A & D";
                questionText.text = "Plaats de meetbaken op de meetpunten en meet met het waterpastoestel het hoogteverschil tussen beide punten";
                AnswerExplanation = "Het hoogteverschil kan gevonden worden door de middendraden te vergelijken.";

                break;

            case QuestionType.Afstand2Punten:
                waterpassing.SetParameters(2, 2, 1, true, false, Vector2.zero, false, Vector2.zero, false);
                correctAnswer = GameManager.RoundFloat(waterpassing.correctDistance * GameManager.worldScale,1);
                questionHeaderText.text = "Bepaal de afstand tussen A & B";
                questionText.text = "Plaats de meetbaken op de meetpunten en gebruik het vizier om de afstand te berekenen.";
                AnswerExplanation = "De Afstand kan gevonden worden door de afstand tussen de top en onderdraad te meten";

                break;

            case QuestionType.AfstandMeerPunten:
                waterpassing.SetParameters(4, 4, 1, true, false, Vector2.zero, false, Vector2.zero, false);
                correctAnswer = GameManager.RoundFloat(waterpassing.correctDistance * GameManager.worldScale, 1);
                questionHeaderText.text = "Bepaal de afstand tussen A & D";
                questionText.text = "Plaats de meetbaken op de meetpunten en gebruik het vizier om de afstand te berekenen.";
                AnswerExplanation = "De Afstand kan gevonden worden door de afstand tussen de top en onderdraad te meten";

                break;

            case QuestionType.Hoekfout:
                waterpassing.SetParameters(0, 1, 1, true, true, new Vector2(6,1), true, new Vector2(9,1), false);
                correctAnswer = GameManager.RoundFloat(waterpassing.correctScaledErrorAngle * DEGtoGON, 3);
                questionHeaderText.text = "Bepaal de collimatiefout van het toestel";
                questionText.text = "Bereken de collimatiefout d.m.v. een exentrieke plaatsing en de afstanden tot de meetbaak.";
                AnswerExplanation = "De collimatiefout kan bepaald worden door het veschil in hoogte te vergeijken in functie van de afstand";

                break;

            case QuestionType.KringWaterpassing:
                waterpassing.SetParameters(3, 5, 1, false, false, Vector2.zero, false, Vector2.zero, true);
                correctPoints = waterpassing.correctHeightDifferences;
                questionHeaderText.text = "Vervolledig de waterpassingtabel";
                questionText.text = "Voer alle noodzakelijke metingen uit en vul de juiste waardes in in de tabel.";
                AnswerExplanation = "Het hoogteverschil kan gevonden worden door de middendraden te vergelijken.";

                break;

            case QuestionType.Scheefstand:
                //waterpassing.SetParameters(2, 2, 1, false, false, Vector2.zero, false, Vector2.zero, false);
                //correctAnswer = waterpassing.correctHeight; calculated in the scheefstandsmanager
                questionHeaderText.text = "Bepaal de scheefstand van het gebouw";
                questionText.text = "Plaats de theodoliet en meet de horizontale verschuiving tussen beide punten, gemeten van onder naar boven.";
                AnswerExplanation = "de scheefstand kan gevonden worden door de ondermeetband te vergelijken met de laserlijn.";

                break;

            case QuestionType.OmgekeerdeBaak:
                waterpassing.SetParameters(1, 2, 1, false, false, Vector2.zero, false, Vector2.zero, false);
                correctAnswer = GameManager.RoundFloat(waterpassing.correctHeight,3);
                questionHeaderText.text = "Bepaal het hoogteveschil tussen A & B";
                questionText.text = "Plaats de meetbaken op de meetpunten en meet met het waterpastoestel het verschil tussen beide punten." +
                    " Opgelet, één van de baken staat omgekeerd.";
                AnswerExplanation = "Het hoogteverschil kan gevonden worden door beide middendraden te vergelijken.";

                break;
            case QuestionType.ScheveWaterpassing:
                waterpassing.SetParameters(2, 2, 1, false, false, Vector2.zero, false, Vector2.zero, false);
                correctAnswer = GameManager.RoundFloat(waterpassing.correctHeight,3);
                questionHeaderText.text = "Bepaal het hoogteveschil tussen A & B";
                questionText.text = "Plaats de meetbaken op de meetpunten en meet met de theodoliet het verschil tussen beide punten.";
                AnswerExplanation = "Het hoogteverschil kan gevonden worden door beide middendraden te vergelijken.";

                break;
        }
        if(GameManager.showDebugAnswer && vraag != QuestionType.Scheefstand) Debug.Log("Correct antwoord = " + correctAnswer + " m of gon");
    }



    //checks if the given anwser is correct
    public void CheckAnswer()
    {

        if (gm.CheckCorrectAnswer(answerInputH.text, CorrectAnswer()))
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
            answerOutput.text = "De Waarden die zijn ingevoerd zijn correct";
        }
        else
        {
            answerOutput.text = "De Waarden die zijn ingevoerd zijn niet correct";
            Debug.Log("false");
        }
    }

    //displays the correct answer
    public void ShowAnswer()
    {
        if(SoortVraag == QuestionType.KringWaterpassing)
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

            case QuestionType.Scheefstand:

                return GameManager.RoundFloat(waterpassing.correctHeight, 3);

            case QuestionType.OmgekeerdeBaak:

                return GameManager.RoundFloat(waterpassing.correctHeight, 3);

            case QuestionType.ScheveWaterpassing:

                return GameManager.RoundFloat(waterpassing.correctHeight, 3);

        }
        return 0;
    }
}