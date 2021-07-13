using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ValueSlider : MonoBehaviour
{
    [SerializeField]
    private Slider slider;
    [SerializeField]
    private Text sliderValue;

    // Class declaration
    [System.Serializable]
    public class MyEvent : UnityEvent<float> { }

    private float localVal = 0;


    // Somewhere in a component
    // Declare
    public MyEvent sliderEvent;

    // Start is called before the first frame update
    void Awake()
    {
        //slider = GetComponentInChildren<Slider>();
        //sliderValue = GetComponentInChildren<Text>();
    }

    public void SetSlider(float val, bool lockSlider)
    {
        localVal = val;

        slider.value = val;
        slider.interactable = !lockSlider;
        if (sliderValue) sliderValue.text = GameManager.RoundFloat(val, 1).ToString();
    }

    public void ChangeValue(float val)
    {
        if (!slider.interactable) return;
        localVal = val;

        sliderEvent.Invoke(val);

        if (sliderValue) sliderValue.text = GameManager.RoundFloat(val, 1).ToString();
    }
}
