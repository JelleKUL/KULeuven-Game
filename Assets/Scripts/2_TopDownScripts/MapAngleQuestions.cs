using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//*********** The MapAngleQuestions sets the required parameters for a specific question ******************//

[RequireComponent(typeof(ObjectPlacer), typeof(PolygonLineController))]
public class MapAngleQuestions : BaseQuestions
{
    public enum AnswerType { Geen, MapAngle, Coordinate, Distance }
    [Space(10)]
    [Tooltip("the type of answer a player has to enter")]
    [SerializeField]
    private AnswerType answerType;
    [SerializeField] private bool transformAxis = false;

    //initiate scripts
    private PolygonLineController lineController;
    private ObjectPlacer placer;
    private AxisTransformer axisTransformer;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        lineController = GetComponent<PolygonLineController>();
        placer = GetComponent<ObjectPlacer>();
        axisTransformer = GetComponent<AxisTransformer>();
        SetQuestionType();
    }


    //sets the type of question, can be altered by another script
    protected override void SetQuestionType()//QuestionType vraag)
    {
        if (controlController)
        {
            lineController.StartSetup();
            placer.StartSetup();

            //rotates the calculate point randomly
            if (transformAxis && axisTransformer && (placer.calculatePoints.Count > 0)) axisTransformer.RotatePoint(placer.calculatePoints[0]);
        }
        base.SetQuestionType(); //does the base question stuff like logging
    }

    //checks if the given anwser is correct
    public override void CheckAnswerInput()
    {
        base.CheckAnswerInput();
    }

    //displays the correct answer
    public override void ShowCorrectAnswer()
    {
        base.ShowCorrectAnswer();
    }

    public override List<float> GetCorrectAnswer()
    {
        float val = 0f;
        switch (answerType)
        {
            case AnswerType.MapAngle: //returns the map angle of the first point in the calculatepoints array
                val = lineController.GetMapAngle(Vector2.up, new Vector2(placer.calculatePointsPositions[0], placer.calculatePointsPositions[1]));
                return new List<float>() { GameManager.RoundFloat(val, 3) };

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