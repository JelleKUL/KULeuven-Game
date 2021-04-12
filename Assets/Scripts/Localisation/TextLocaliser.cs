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
    /// <summary>
    /// Set the text component to a localised value
    /// </summary>
    /// <param name="key">the localisation key to search for the correct value</param>
    public void UpdateText(string key, int nr = -1)
    {
        if (!textObject)
        {
            if (TryGetComponent(out Text text))
            {
                textObject = text;
            }
        }
        if (textObject) textObject.text = (nr==-1?"":nr.ToString() + ": ") + LocalisationManager.GetLocalisedValue(key);
        else Debug.LogWarning(gameObject.name + ": no text component attached to this gameobject to localise.");
    }
}
