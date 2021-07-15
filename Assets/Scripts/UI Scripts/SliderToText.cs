using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class SliderToText : MonoBehaviour
{
    [SerializeField] private string unit = "";
    [SerializeField] private int roundingIndex = 3;

    private Text targetText;

    private void Awake()
    {
        targetText = GetComponent<Text>();
    }

    public void FloatToText(float input)
    {
        if(targetText)targetText.text = GameManager.RoundFloat(input, roundingIndex).ToString() +" "+ unit; //(Mathf.Round(input) / 1000f  ).ToString()
    }
}
