using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//*********** The MapAngleQuestions sets the required parameters for a specific question ******************//

[RequireComponent(typeof(ObjectPlacer), typeof(PolygonLineController))]
public class MapAngleQuestions : BaseQuestions
{
    public enum AnswerType { Geen, MapAngle, Coordinate, Distance }
    [Tooltip("the type of answer a player has to enter")]
    [SerializeField]
    private AnswerType answerType;

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
        base.SetQuestionType();
        lineController.StartSetup();
        placer.StartSetup();
    }

    //checks if the given anwser is correct
    public override void CheckAnswerInput()
    {
        
    }

    //displays the correct answer
    public override void ShowCorrectAnswer()
    {

    }

    public override List<float> GetCorrectAnswer()
    {
        switch (answerType)
        {
            case AnswerType.MapAngle:
                return new List<float>() { GameManager.RoundFloat(0, 3) };

            case AnswerType.Distance:
                return new List<float>() { GameManager.RoundFloat(0 * GameManager.worldScale, 1) };

            case AnswerType.Coordinate:
                return new List<float>() { GameManager.RoundFloat(0, 3) };

            default:
                return new List<float> { 0f };
        }
    }

}
