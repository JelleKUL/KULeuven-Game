using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TextLocaliser : MonoBehaviour
{
    [SerializeField]
    private bool UpdateAtStart = true;
    private Text textObject;
    private TextMeshProUGUI tmpTextObject;


    // Start is called before the first frame update
    void Start()
    {
        if(TryGetComponent(out Text text))
        {
            textObject = text;
            if(UpdateAtStart)UpdateText(textObject.text);
        }
        else if (TryGetComponent(out TextMeshProUGUI tmpText))
        {
            tmpTextObject = tmpText;
            if(UpdateAtStart)UpdateText(tmpTextObject.text);
        }
    }

    public void UpdateText(string key)
    {
        if (textObject) textObject.text = LocalisationManager.GetLocalisedValue(key);
        else if (tmpTextObject) tmpTextObject.text = LocalisationManager.GetLocalisedValue(key);
    }
}
