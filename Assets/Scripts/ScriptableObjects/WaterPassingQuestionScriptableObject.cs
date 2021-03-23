using UnityEngine;

[CreateAssetMenu(fileName = "WaterPassingQuestion", menuName = "ScriptableObjects/WaterPassingQuestionScriptableObject", order = 1)]
public class WaterPassingQuestionScriptableObject : ScriptableObject
{
    public enum AnswerType {Height, Distance, ErrorAngle, Table}


    [Header("Text Elements")]
    public string ID_questionHeader = "";
    public string ID_questionText = "";
    public string ID_answerText = "";

    [Header("Parameters")]
    [SerializeField]
    private AnswerType answerType;
    [SerializeField]
    private int nrPoints;
    [SerializeField]
    private int nrBeacons;
    [SerializeField]
    private int nrMeasures;
    [SerializeField]
    private bool ShowDistance;
    [SerializeField]
    private bool lockmeasure;
    [SerializeField]
    private Vector2 measureLocation;
    [SerializeField]
    private bool lockbeacon;
    [SerializeField]
    private Vector2 beaconLocation;
    [SerializeField]
    private bool loop;

    public void SetQuestion(WaterPassingController waterPassingController)
    {
        waterPassingController.SetParameters(nrPoints, nrBeacons, nrMeasures, ShowDistance, lockmeasure, measureLocation, lockbeacon, beaconLocation, loop);
    }


    public float GetCorrectAnswer(WaterPassingController waterPassingController)
    {
        switch (answerType)
        {
            case AnswerType.Height:
                return GameManager.RoundFloat(waterPassingController.correctHeight, 3);

            case AnswerType.Distance:
                return GameManager.RoundFloat(waterPassingController.correctDistance * GameManager.worldScale, 1);

            case AnswerType.ErrorAngle:
                return GameManager.RoundFloat(waterPassingController.correctScaledErrorAngle * 4/3.6f, 3);

            case AnswerType.Table:
                return 0f;

            default:
                return 0f;
        }
    }
}