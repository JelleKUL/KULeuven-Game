using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterPassingTabelTotaal : MonoBehaviour
{
    public Text hoogteVerschilText;
    public Text afstandText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetValues(float hoogte, float afstand)
    {
        hoogteVerschilText.text = hoogte.ToString();
        afstandText.text = afstand.ToString();
    }
}
