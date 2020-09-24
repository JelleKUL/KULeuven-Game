using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Text))]
public class SliderToText : MonoBehaviour
{
    private Text targetText;

    private void Awake()
    {
        targetText = GetComponent<Text>();
    }

    public void FloatToText(float input)
    {
        targetText.text = input.ToString(); //(Mathf.Round(input) / 1000f  ).ToString()
    }
}
