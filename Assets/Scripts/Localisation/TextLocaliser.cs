using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

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
    /// <summary>
    /// Set the text component to a localised value
    /// </summary>
    /// <param name="key">the localisation key to search for the correct value</param>
    public void UpdateText(string key, int nr = -1, object parseObject = null)
    {
        if (!textObject)
        {
            if (TryGetComponent(out Text text))
            {
                textObject = text;
            }
        }
        if (textObject)
        {
            string localVal = (nr == -1 ? "" : nr.ToString() + ": ") + LocalisationManager.GetLocalisedValue(key);
            if (parseObject != null) localVal = localVal.ParseVariables(parseObject);
            localVal = Regex.Unescape(localVal); // replaces the escaped character back
            textObject.text = localVal;
        }
        else Debug.LogWarning(gameObject.name + ": no text component attached to this gameobject to localise.");

        
    }

    public void SetLocalisedText(string value)
    {
        if (!textObject)
        {
            if (TryGetComponent(out Text text))
            {
                textObject = text;
            }
        }
        if (textObject) textObject.text = value;
        else Debug.LogWarning(gameObject.name + ": no text component attached to this gameobject to localise.");
    }
}
