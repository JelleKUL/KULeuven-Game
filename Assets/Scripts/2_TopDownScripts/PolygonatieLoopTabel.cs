using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PolygonatieLoopTabel : MonoBehaviour
{

    public Color correctColor, falseColor;
    [SerializeField]
    private float errorMargin = 0.001f;

    public GameObject[] tabelParts;
    public ObjectPlacer placer;
    public PolygonLineController polyline;

    public GameObject Title1;
    public GameObject Title2;

    private bool coordinateMode;

    private GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        coordinateMode = false;

        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

    }


    public Vector2[] GetInputs()
    {
        Vector2[] inputs = new Vector2[tabelParts.Length]; 
        for (int i = 0; i < tabelParts.Length; i++)
        {
            inputs[i] = tabelParts[i].GetComponent<PolygonatieLoopTabelDeel>().getAnswer();
        }

        return inputs;
    }

    public bool checkAnswers(float[] correctPoints )
    {
        if (!coordinateMode)
        {
            //check vereffende kaarthoek
            for (int i = 1; i < tabelParts.Length; i++)
            {
                PolygonatieLoopTabelDeel tabelPart = tabelParts[i].GetComponent<PolygonatieLoopTabelDeel>();
                tabelPart.SetColor(correctColor, false);
                Vector3 thisPoint;
                Vector3 targetPoint;

                if (i < tabelParts.Length - 2)
                {
                    thisPoint = placer.calculatePoints[i].transform.position;
                    targetPoint = placer.calculatePoints[i + 1].transform.position;
                }
                else if (i == tabelParts.Length - 2)
                {
                    thisPoint = placer.calculatePoints[i].transform.position;
                    targetPoint = placer.calculatePoints[1].transform.position;
                }
                else
                {
                    thisPoint = placer.calculatePoints[1].transform.position;
                    targetPoint = placer.calculatePoints[0].transform.position;
                }

                
                if (Mathf.Abs(GetMapAngle(thisPoint, targetPoint) - tabelPart.GetMapAngleInput()) > errorMargin)
                {
                    tabelParts[i].GetComponent<PolygonatieLoopTabelDeel>().SetColor(falseColor, false);
                    
                    if(GameManager.showDebugAnswer) Debug.Log(i+ ": Incorrect inputvalue: " + tabelPart.GetMapAngleInput() + ", Correct answer: " + GetMapAngle(thisPoint, targetPoint) + ", Difference: " +
                                                                (Mathf.Abs(GetMapAngle(thisPoint, targetPoint) - tabelPart.GetMapAngleInput())));
                    else Debug.Log("Mapangle incorrect: " + i);
                }
                else
                {
                    Debug.Log("Mapangle correct: " + i);
                }

            }
            return false;
        }



        bool correct = true;
        Vector2[] answerinputs = GetInputs();
        Vector2[] correctCoordinates = placer.GetCoordinates();

        for (int i = 1; i < correctCoordinates.Length; i++)
        {
            if(GameManager.showDebugAnswer) Debug.Log("Point " + (i+1) + ": " + Vector2.Distance(answerinputs[i], correctCoordinates[i] * GameManager.worldScale));
            tabelParts[i].GetComponent<PolygonatieLoopTabelDeel>().SetColor(correctColor, true);
            if (Vector2.Distance(answerinputs[i],correctCoordinates[i] * GameManager.worldScale) > errorMargin)
            {
                correct = false;
                tabelParts[i].GetComponent<PolygonatieLoopTabelDeel>().SetColor(falseColor, true);
                Debug.Log("coordinate incorrect: " + i);
            }
            else
            {
                Debug.Log("coordinate correct: " + i);
            }
           
        }

        return correct;
    }

    public void SwitchModes()
    {

        coordinateMode = !coordinateMode;

        Title1.SetActive(!coordinateMode);
        Title2.SetActive(coordinateMode);
        for (int i = 0; i < tabelParts.Length; i++)
        {
            tabelParts[i].GetComponent<PolygonatieLoopTabelDeel>().SwitchModes();
        }
    }
    
    public void ShowCorrectValues(float[] correctValues)
    {
        coordinateMode = true;
        Title1.SetActive(false);
        Title2.SetActive(true);

        Vector2 firstPoint = new Vector2(correctValues[0], correctValues[1]) * GameManager.worldScale;
        Vector2 point = firstPoint;
        Vector2 secondPoint = new Vector2(correctValues[2], correctValues[3]) * GameManager.worldScale;


        for (int i = 0; i < correctValues.Length/2-1; i++)
        {
            Vector2 currentPoint = point;
            Vector2 nextPoint = new Vector2(correctValues[i * 2 + 2 ], correctValues[i * 2 + 3]) * GameManager.worldScale;
            tabelParts[i].GetComponent<PolygonatieLoopTabelDeel>().SetValues(currentPoint, nextPoint, false);
            //Debug.Log("this point: " + currentPoint + ", nextpoint: " + nextPoint + ", " + (i * 2 + 2) + ", " + (i * 2 + 3));
            point = nextPoint;


        }
        tabelParts[correctValues.Length / 2 - 1].GetComponent<PolygonatieLoopTabelDeel>().SetValues(point,secondPoint, false);
        tabelParts[correctValues.Length / 2].GetComponent<PolygonatieLoopTabelDeel>().SetValues(secondPoint,firstPoint,true);


    }

    float GetMapAngle(Vector2 point, Vector2 targetPoint)
    {
        float angle = Vector2.SignedAngle(targetPoint - point, Vector2.up);
        if (angle < 0) angle = 360 + angle;
        angle = GameManager.RoundFloat(angle / 360f * 400, 3);
        return angle;
    }



}
