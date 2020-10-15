﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PolygonatieLoopTabel : MonoBehaviour
{

    public Color correctColor, falseColor;


    public GameObject[] tabelParts;
    public ObjectPlacer placer;
    public PolygonLineController polyline;

    public GameObject Title1;
    public GameObject Title2;

    private bool coordinateMode;

    // Start is called before the first frame update
    void Start()
    {
        coordinateMode = false;

    }

    // Update is called once per frame
    void Update()
    {
        
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
        bool correct = true;
        Vector2[] answerinputs = GetInputs();
        Vector2[] correctCoordinates = placer.GetCoordinates();

        for (int i = 0; i < correctCoordinates.Length; i++)
        {
            Debug.Log(Vector2.Distance(answerinputs[i], correctCoordinates[i] * GameManager.worldScale));
            tabelParts[i].GetComponent<PolygonatieLoopTabelDeel>().SetColor(correctColor);
            if (Vector2.Distance(answerinputs[i],correctCoordinates[i] * GameManager.worldScale) > 0.003)
            {
                correct = false;
                tabelParts[i].GetComponent<PolygonatieLoopTabelDeel>().SetColor(falseColor);
                Debug.Log("incorrect: " + i);
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


        for (int i = 0; i < correctValues.Length/2-1; i++)
        {
            Vector2 currentPoint = point;
            Vector2 nextPoint = new Vector2(correctValues[i * 2 + 2 ], correctValues[i * 2 + 3]) * GameManager.worldScale;
            tabelParts[i].GetComponent<PolygonatieLoopTabelDeel>().SetValues(currentPoint, nextPoint, false);
            //Debug.Log("this point: " + currentPoint + ", nextpoint: " + nextPoint + ", " + (i * 2 + 2) + ", " + (i * 2 + 3));
            point = nextPoint;


        }
        tabelParts[correctValues.Length / 2 - 1].GetComponent<PolygonatieLoopTabelDeel>().SetValues(point,firstPoint, false);
        tabelParts[correctValues.Length / 2].GetComponent<PolygonatieLoopTabelDeel>().SetValues(firstPoint,firstPoint,true);


    }
 


}
