using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextLocaliser : MonoBehaviour
{
    [SerializeField]
    private bool UpdateAtStart = true;
    private Text textObject;


    // Start is called before the first frame update
    void Start()
    {
        if(TryGetComponent(out Text text))
        {
            textObject = text;
            if(UpdateAtStart)UpdateText(textObject.text);
        }
    }

    public void UpdateText(string key)
    {
        if (textObject) textObject.text = LocalisationManager.GetLocalisedValue(key);
    }
}
