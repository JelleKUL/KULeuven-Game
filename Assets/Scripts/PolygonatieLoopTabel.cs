using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PolygonatieLoopTabel : MonoBehaviour
{
    private bool coordinateMode;


    public GameObject[] tabelParts;
    public ObjectPlacer placer;

    public GameObject Title1;
    public GameObject Title2;

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

    public bool checkAnswers()
    {
        bool correct = true;
        Vector2[] answerinputs = GetInputs();
        Vector2[] correctCoordinates = placer.GetCoordinates();

        for (int i = 0; i < correctCoordinates.Length; i++)
        {
            if(Vector2.SqrMagnitude(answerinputs[i+1] - correctCoordinates[i]) > 0.02)
            {
                correct = false;
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

}
