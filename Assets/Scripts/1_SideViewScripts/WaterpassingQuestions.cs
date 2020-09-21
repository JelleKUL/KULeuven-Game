﻿using System.Collections;
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

    public GameObject winMenu;
    public Color falseColor, CorrectColor;


    public enum QuestionType { Geen, Hoogteverschil2Punten, HoogteVerschilMeerPunten, Afstand2Punten, Hoekfout, KringWaterpassing, Scheefstand, OmgekeerdeBaak, ScheveWaterpassing }
    [Tooltip("Kies het soort vraag voor de oefening")]
    public QuestionType SoortVraag;

    public int scoreIncrease = 1;

    private WaterPassingController waterpassing;
    private GameManager gm;
    private float correctAnswer;
    private float[] correctPoints;

    // awake is called before start
    void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        waterpassing = GetComponent<WaterPassingController>();

        SetQuestionType(SoortVraag);
    }
    public void ResetCurrentQuestion()
    {
        switch (SoortVraag)
        {
            case QuestionType.Geen:
                break;

            case QuestionType.Hoogteverschil2Punten:
                waterpassing.ChangePoints();
                correctAnswer = waterpassing.correctHeight;

                break;

            case QuestionType.HoogteVerschilMeerPunten:
                waterpassing.ChangePoints();
                correctAnswer = waterpassing.correctHeight;

                break;

            case QuestionType.Afstand2Punten:
                waterpassing.ChangePoints();
                correctAnswer = waterpassing.correctDistance;

                break;

            case QuestionType.Hoekfout:
                waterpassing.ChangePoints();
                correctAnswer = waterpassing.correctErrorAngle;

                break;

            case QuestionType.KringWaterpassing:
                waterpassing.ChangePoints();
                correctAnswer = waterpassing.correctHeight;

                break;

            case QuestionType.Scheefstand:
                waterpassing.ChangePoints();
                correctAnswer = waterpassing.correctHeight;

                break;
            case QuestionType.ScheveWaterpassing:
                waterpassing.ChangePoints();
                correctAnswer = waterpassing.correctHeight;

                break;
        }
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
                correctAnswer = waterpassing.correctHeight;
                questionHeaderText.text = "Bepaal het hoogteveschil tussen A & B";
                questionText.text = "Plaats de meetbaken op de meetpunten en meet met het waterpastoestel het verschil tussen beide punten";

                break;

            case QuestionType.HoogteVerschilMeerPunten:
                waterpassing.SetParameters(4, 4, 1, false, false, Vector2.zero, false, Vector2.zero, false);
                correctAnswer = waterpassing.correctHeight;
                questionHeaderText.text = "Bepaal het hoogteveschil tussen A & D";
                questionText.text = "Plaats de meetbaken op de meetpunten en meet met het waterpastoestel het hoogteverschil tussen beide punten";

                break;

            case QuestionType.Afstand2Punten:
                waterpassing.SetParameters(2, 2, 1, true, false, Vector2.zero, false, Vector2.zero, false);
                correctAnswer = waterpassing.correctDistance * GameManager.worldScale;
                questionHeaderText.text = "Bepaal de afstand tussen A & B";
                questionText.text = "Plaats de meetbaken op A en B en het waterpastoestel tussen beide meetpunten." +
                    " Gebruik de boven en onderlijn van het vizier om de afstanden te berekenen.";

                break;

            case QuestionType.Hoekfout:
                waterpassing.SetParameters(0, 1, 1, true, true, new Vector2(4,1), true, new Vector2(7,1), false);
                correctAnswer = waterpassing.correctErrorAngle * 4/3.6f;
                questionHeaderText.text = "Bepaal de collimatiefout van het toestel";
                questionText.text = "Bereken de collimatiefout d.m.v. een exentrieke plaatsing en de afstanden tot de meetbaak.";

                break;

            case QuestionType.KringWaterpassing:
                waterpassing.SetParameters(3, 5, 1, false, false, Vector2.zero, false, Vector2.zero, true);
                correctPoints = waterpassing.correctHeightDifferences;
                questionHeaderText.text = "Vervolledig de waterpassingtabel";
                questionText.text = "voer alle noodzakelijke metingen uit en vul de juiste waardes in in de tabel.";

                break;

            case QuestionType.Scheefstand:
                //waterpassing.SetParameters(2, 2, 1, false, false, Vector2.zero, false, Vector2.zero, false);
                //correctAnswer = waterpassing.correctHeight;
                questionHeaderText.text = "Bepaal De scheefstand van het gebouw";
                questionText.text = "de theodoliet geeft de meting weer op beide punten.";

                break;

            case QuestionType.OmgekeerdeBaak:
                waterpassing.SetParameters(1, 2, 1, false, false, Vector2.zero, false, Vector2.zero, false);
                correctAnswer = waterpassing.correctHeight;
                questionHeaderText.text = "Bepaal het hoogteveschil tussen A & B";
                questionText.text = "Plaats de meetbaken op de meetpunten en meet met het meettoestel het verschil tussen beide punten." +
                    "Opgelet, één van de punten staat omgekeerd.";

                break;
            case QuestionType.ScheveWaterpassing:
                waterpassing.SetParameters(2, 2, 1, false, false, Vector2.zero, false, Vector2.zero, false);
                correctAnswer = waterpassing.correctHeight;
                questionHeaderText.text = "Bepaal het hoogteveschil tussen A & B";
                questionText.text = "Plaats de meetbaken op de meetpunten en meet met het meettoestel het verschil tussen beide punten, gebruik de rotatiehoek om het toestel te draaien";

                break;
        }
        if(GameManager.showDebugAnswer) Debug.Log("Correct antwoord = " + correctAnswer);
    }



    //checks if the given anwser is correct
    public void CheckAnswer()
    {

        if (gm.CheckCorrectAnswer(answerInputH.text, CorrectAnswer()))
        {
            gm.IncreaseScore(scoreIncrease, 1);
            Debug.Log(answerInputH.text + " is correct!");
            winMenu.SetActive(true);
            //gm.ReloadScene();
        }
        else
        {
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
            winMenu.SetActive(true);
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
        answerOutput.text = "Het antwoord is: " + CorrectAnswer().ToString();
        answerInputH.color = falseColor;
        //answerInputH.text = "Het antwoord is: " + CorrectAnswer().ToString();
        //waterpassing.ShowAnswer();
        Debug.Log("showing answer");

    }

    public float CorrectAnswer()
    {
        switch (SoortVraag)
        {
            case QuestionType.Hoogteverschil2Punten:

                return waterpassing.correctHeight;

            case QuestionType.HoogteVerschilMeerPunten:

                return waterpassing.correctHeight;


            case QuestionType.Afstand2Punten:

                return waterpassing.correctDistance * GameManager.worldScale;


            case QuestionType.Hoekfout:

                return waterpassing.correctErrorAngle * 4/3.6f;


            case QuestionType.KringWaterpassing:

                return waterpassing.correctHeight;

            case QuestionType.Scheefstand:

                return waterpassing.correctHeight;

        }
        return 0;
    }
}