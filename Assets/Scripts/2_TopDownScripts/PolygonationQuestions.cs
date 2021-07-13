using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//*********** The PolygonationQuestions sets the required parameters for a specific question ******************//


public class PolygonationQuestions : BaseQuestions
{

    public enum AnswerType { geen,Coordinate, Distance, Table }
    [Space(10)]
    [Tooltip("Kies het soort vraag voor de oefening")]
    public AnswerType answerType;


    private PolygonLineController lineController;
    private ObjectPlacer placer;
    public PolygonatieLoopTabel tabel;

    // Start is called before the first frame update
    protected override void Awake()
    {
        lineController = GetComponent<PolygonLineController>();
        placer = GetComponent<ObjectPlacer>();
        SetQuestionType();

        base.Awake();
    }

    //sets the type of question, can be altered by another script
    protected override void SetQuestionType()//QuestionType vraag)
    {
        if (controlController)
        {
            placer.StartSetup();
            lineController.SetAnswerArray(placer.calculatePointsPositions.ToArray());
            lineController.StartSetup();

            if (answerType == AnswerType.Table) lineController.SetPoints(placer.calculatePointsPositions.ToArray());
        }
        base.SetQuestionType(); //does the base question stuff like logging

        if (questionUI) //set the answer input field according to the selected answertype
        {
            switch (answerType)
            {
                case AnswerType.Coordinate:
                    questionUI.SetInputs(false, "m"); //set the input to one
                    break;
                case AnswerType.Distance:
                    questionUI.SetInputs(true, "m"); //set the input to one
                    break;
                default:
                    break;
            }
        }
    }

    //checks if the given anwser is correct
    public override void CheckAnswerInput()
    {
        if(answerType == AnswerType.Table)
        {
            if (tabel.checkAnswers(GetCorrectAnswer().ToArray()))
            {
                gm.IncreaseScore(scoreIncrease);
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
            tabel.ShowCorrectValues(GetCorrectAnswer().ToArray());
        }
        else
        {
            base.ShowCorrectAnswer();
        }
    }

    public override List<float> GetCorrectAnswer()
    {
        float val = 0f;
        switch (answerType)
        {
            case AnswerType.Table: //returns the map angle of the first point in the calculatepoints array
                return placer.calculatePointsPositions;

            case AnswerType.Distance: //returns the distance between the first 2 points in the calculatepoints array
                val = Vector3.Distance(placer.calculatePoints[0].transform.position, placer.calculatePoints[1].transform.position) * GameManager.worldScale;
                return new List<float>() { GameManager.RoundFloat(val, 3) };

            case AnswerType.Coordinate: //return the coordinates of the first point
                return new List<float>() { GameManager.RoundFloat(placer.calculatePoints[0].transform.position.x * GameManager.worldScale, 3), GameManager.RoundFloat(placer.calculatePoints[0].transform.position.y * GameManager.worldScale, 3) };

            default:
                return new List<float> { 0f };
        }
    }

}
/*
 * AnswerType { Geen, Coordinaat1Punt, Afstand2PuntenPolygoon, VoorwaardseInsnijding, AchterwaardseInsnijding, Tabel, Bilateratie, VrijePolygonatie }
//sets the type of question, can be altered by another script
public void SetQuestionType(QuestionType vraag)
{
    switch (vraag)
    {
        case QuestionType.Geen:

            break;
        case QuestionType.Coordinaat1Punt:
            //start oefening Coordinate1Point
            lineController.SetVisibles(true, false, false, false, true, true, 2);
            //correctAnswerArray = placer.PlacePoints(1);
            correctAnswerX = GameManager.RoundFloat(correctAnswerArray[0] * GameManager.worldScale, 3);
            correctAnswerY = GameManager.RoundFloat(correctAnswerArray[1] * GameManager.worldScale , 3);
            correctAnswerString = "X: " + correctAnswerX + ", Y: " + correctAnswerY;

            titleQuestionText.text = "Bepaal het coördinaat van punt P";
            questionText.text = "Met behulp van de afstand en map angle vanaf het meetpunt.";
            AnswerExplanation = "Gebruik trigoniometrische formules om het coordinaat te berekenen";
            break;

        case QuestionType.Afstand2PuntenPolygoon:
            //start oefening DragEnDropEllips
            lineController.SetVisibles(true, false, false, false, true, true, 2);
            //correctAnswerArray = placer.PlacePoints(2);
            correctAnswerH = GameManager.RoundFloat(GameManager.worldScale * Mathf.Sqrt(Mathf.Pow(correctAnswerArray[2] - correctAnswerArray[0], 2) + Mathf.Pow(correctAnswerArray[3] - correctAnswerArray[1], 2)), 3);
            correctAnswerString = correctAnswerH.ToString();

            titleQuestionText.text = "Bepaal de afstand tussen A en B";
            questionText.text = "Met behulp van de afstand en map angle vanaf het meetpunt.";
            AnswerExplanation = "Bepaal beide coordinaten en bereken dan de afstand";

            break;

        case QuestionType.VoorwaardseInsnijding:
            //start oefening voorwaardse insnijding
            lineController.SetVisibles(false, false, true, false, false, false, 3);
            //obsructedPointsArray = placer.PlacePoints(1, true);
            //correctAnswerArray = placer.PlacePoints(2);

            //placer.PlaceObstacles(2);
            correctAnswerX = GameManager.RoundFloat(obsructedPointsArray[0] * GameManager.worldScale, 3);
            correctAnswerY = GameManager.RoundFloat(obsructedPointsArray[1] * GameManager.worldScale, 3);
            correctAnswerString = "X: " + correctAnswerX + ", Y: " + correctAnswerY;

            titleQuestionText.text = "I. Voorwaardse Insnijding bepaal P";
            questionText.text = "\n\u2022 A = x:" + GameManager.RoundFloat(correctAnswerArray[0] * GameManager.worldScale, 3) + ", y:" + GameManager.RoundFloat(correctAnswerArray[1] * GameManager.worldScale, 3) + 
                                "\n\u2022 B = x:" + GameManager.RoundFloat(correctAnswerArray[2] * GameManager.worldScale, 3) + ", y:" + GameManager.RoundFloat(correctAnswerArray[3] * GameManager.worldScale, 3);
            AnswerExplanation = "meet de hoeken en bereken het coordinaat via de sinusregel en Pythagoras";

            break;

        case QuestionType.AchterwaardseInsnijding:
            //start oefening achterwaardse insnijding
            lineController.SetVisibles(false, false, true, false, false, false, 3);
            //correctAnswerArray = placer.PlacePoints(1);
            //obsructedPointsArray = placer.PlacePoints(3, true);
            correctAnswerX = GameManager.RoundFloat(correctAnswerArray[0] * GameManager.worldScale, 3);
            correctAnswerY = GameManager.RoundFloat(correctAnswerArray[1] * GameManager.worldScale, 3);
            correctAnswerString = "X: " + correctAnswerX + ", Y: " + correctAnswerY;

            //placer.PlaceObstacles(1);
            titleQuestionText.text = "II. Achterwaardse Insnijding bepaal P";
            questionText.text = "\n\u2022 A = x: " + GameManager.RoundFloat(obsructedPointsArray[0] * GameManager.worldScale, 3) + ", y: " + GameManager.RoundFloat(obsructedPointsArray[1] * GameManager.worldScale, 3) + 
                                "\n\u2022 B = x: " + GameManager.RoundFloat(obsructedPointsArray[2] * GameManager.worldScale, 3) + ", y: " + GameManager.RoundFloat(obsructedPointsArray[3] * GameManager.worldScale, 3) + 
                                "\n\u2022 C = x: " + GameManager.RoundFloat(obsructedPointsArray[4] * GameManager.worldScale, 3) + ", y: " + GameManager.RoundFloat(obsructedPointsArray[5] * GameManager.worldScale, 3) ;
            AnswerExplanation = "meet de hoeken en bereken het coordinaat via de sinusregel en Pythagoras";
            break;

        case QuestionType.Tabel:
            //start oefening tabel
            lineController.SetVisibles(false, false, true, true, true, false, 8);
            //correctAnswerArray = placer.PlaceLoopedPoints(3);

            correctAnswerX = GameManager.RoundFloat(correctAnswerArray[10] * GameManager.worldScale, 3);
            correctAnswerY = GameManager.RoundFloat(correctAnswerArray[11] * GameManager.worldScale, 3);
            correctAnswerString = "X: " + correctAnswerX + ", Y: " + correctAnswerY;

            lineController.SetPoints(correctAnswerArray);
            //placer.PlaceObstacles(2);
            titleQuestionText.text = "III. Vervolledig onderstaande tabel";
            questionText.text =     "De punten P en A zijn gegeven:" +
                                    "\n\u2022 P (" + GameManager.RoundFloat(correctAnswerArray[0] * GameManager.worldScale, 3) + ", " + GameManager.RoundFloat(correctAnswerArray[1] * GameManager.worldScale, 3)+ ") m" +
                                    "\n\u2022 A (" + GameManager.RoundFloat(correctAnswerArray[2] * GameManager.worldScale, 3) + ", " + GameManager.RoundFloat(correctAnswerArray[3] * GameManager.worldScale, 3) + ") m";
            AnswerExplanation = "";
            if (GameManager.showDebugAnswer)
            {
                Debug.Log(  "<b>Correct points:</b>" +
                            "\n\u2022 P = x: " + GameManager.RoundFloat(correctAnswerArray[0] * GameManager.worldScale, 3) + ", y: " + GameManager.RoundFloat(correctAnswerArray[1] * GameManager.worldScale, 3) +
                            "\n\u2022 A = x: " + GameManager.RoundFloat(correctAnswerArray[2] * GameManager.worldScale, 3) + ", y: " + GameManager.RoundFloat(correctAnswerArray[3] * GameManager.worldScale, 3) +
                            "\n\u2022 B = x: " + GameManager.RoundFloat(correctAnswerArray[4] * GameManager.worldScale, 3) + ", y: " + GameManager.RoundFloat(correctAnswerArray[5] * GameManager.worldScale, 3) +
                            "\n\u2022 C = x: " + GameManager.RoundFloat(correctAnswerArray[6] * GameManager.worldScale, 3) + ", y: " + GameManager.RoundFloat(correctAnswerArray[7] * GameManager.worldScale, 3) +
                            "\n\u2022 D = x: " + GameManager.RoundFloat(correctAnswerArray[8] * GameManager.worldScale, 3) + ", y: " + GameManager.RoundFloat(correctAnswerArray[9] * GameManager.worldScale, 3) +
                            "\n\u2022 E = x: " + GameManager.RoundFloat(correctAnswerArray[10] * GameManager.worldScale, 3) + ", y: " + GameManager.RoundFloat(correctAnswerArray[11] * GameManager.worldScale, 3) +
                            "\n\u2022 F = x: " + GameManager.RoundFloat(correctAnswerArray[12] * GameManager.worldScale, 3) + ", y: " + GameManager.RoundFloat(correctAnswerArray[13] * GameManager.worldScale, 3)
                        );
            }
            break;

        case QuestionType.Bilateratie:
            //start oefening Bilateratie
            lineController.SetVisibles(false, false, true, true, false, false, 3);
            //correctAnswerArray = placer.PlacePoints(1);
            //obsructedPointsArray = placer.PlacePoints(2, true);
            correctAnswerX = GameManager.RoundFloat(correctAnswerArray[0] * GameManager.worldScale, 3);
            correctAnswerY = GameManager.RoundFloat(correctAnswerArray[1] * GameManager.worldScale, 3);
            correctAnswerString = "X: " + correctAnswerX + ", Y: " + correctAnswerY;

            //placer.PlaceObstacles(2);
            titleQuestionText.text = "IV. Bilateratie bepaal P";
            questionText.text = "De punten A en B zijn gegeven:" +
                                "\n\u2022 A (" + GameManager.RoundFloat(obsructedPointsArray[0] * GameManager.worldScale, 3) + ", " + GameManager.RoundFloat(obsructedPointsArray[1] * GameManager.worldScale, 3)+ ") m" +
                                "\n\u2022 B (" + GameManager.RoundFloat(obsructedPointsArray[2] * GameManager.worldScale, 3) + ", " + GameManager.RoundFloat(obsructedPointsArray[3] * GameManager.worldScale, 3) + ") m";
            AnswerExplanation = "Je hebt een gegeven te veel, gebruik dat om te vereffenen";


            break;

        case QuestionType.VrijePolygonatie:

            AnswerExplanation = "";

            break;

    }
    base.SetQuestionType();
}

//checks if the given anwser is correct
public void CheckAnswerH()
{
    if (CheckCorrectAnswer(questionUI.GetAnswerInput(InputType.h), correctAnswerH, errorMargin))
    {
        gm.IncreaseScore(scoreIncrease, 2);
        Debug.Log("true");
        winMenu.SetActive(true);

    }
    else
    {
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

// checks if a given coordinate is correct
public void CheckAnswerXY()
{
    if (CheckCorrectAnswer(questionUI.GetAnswerInput(InputType.x), correctAnswerX, errorMargin) && CheckCorrectAnswer(questionUI.GetAnswerInput(InputType.y), correctAnswerY, errorMargin))
    {
        gm.IncreaseScore(scoreIncrease, 2);
        Debug.Log("true");
        if (GameManager.campaignMode)
        {
            winMenu.SetActive(true);
        }
        else
        {
            winMenuFree.SetActive(true);
        }

    }
    else
    {
        answerOutput.text = "Waarde incorrect...";
        answerInputX.color = falseColor;
        answerInputY.color = falseColor;
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
    if (tabel.checkAnswers(correctAnswerArray))
    {
        gm.IncreaseScore(scoreIncrease, 2);
        Debug.Log("true");
        if (GameManager.campaignMode)
        {
            winMenu.SetActive(true);
        }
        else
        {
            winMenuFree.SetActive(true);
        }

    }
    else
    {
        answerOutput.text = "De waarden die zijn ingevoerd zijn niet correct";
        Debug.Log("false");
    }
}


public void ShowAnswerArray()
{
    tabel.ShowCorrectValues(correctAnswerArray);
}

//displays the correct answer
public void ShowAnswer()
{
    answerOutput.text = AnswerExplanation;
    answerInputH.color = falseColor;
    answerInputX.color = falseColor;
    answerInputY.color = falseColor;
    Debug.Log("showing answer");

    if (answerInputH.transform.parent.GetComponent<InputField>())
    {
        InputField answerDisplay = answerInputH.transform.parent.GetComponent<InputField>();
        //answerDisplay.text = correctAnswer.ToString();
        answerDisplay.interactable = false;
    }
    if (answerInputX.transform.parent.GetComponent<InputField>())
    {
        InputField answerDisplay = answerInputX.transform.parent.GetComponent<InputField>();
        answerDisplay.text = correctAnswerX.ToString();
        answerDisplay.interactable = false;
    }
    if (answerInputY.transform.parent.GetComponent<InputField>())
    {
        InputField answerDisplay = answerInputY.transform.parent.GetComponent<InputField>();
        answerDisplay.text = correctAnswerY.ToString();
        answerDisplay.interactable = false;
    }
}


}
*/