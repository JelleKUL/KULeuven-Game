using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//***************** Manages info of a part of the waterpassingtabel *************//


public class WaterpassigTabelDeel : MonoBehaviour
{
    [Header("Objects")]
    public Text station;
    public Text meetpuntAchter;
    public Text meetpuntVoor;
    public Text metingAchter;
    public Text metingVoor;
    public Text hoogteVerschilText;
    public Text afstandVoor;
    public Text afstandAchter;

    [HideInInspector]
    public float hoogteVerschil;
    [HideInInspector]
    public float afstand;


    // Update is called once per frame
    void Update()
    {
        afstand = 0f;
        if (float.TryParse(afstandAchter.text, out float result) )
        {
            afstand += result;
        }
        
        if (float.TryParse(afstandVoor.text, out float result2))
        {
            afstand += result2;
        }
        

        if (float.TryParse(metingAchter.text, out result) && float.TryParse(metingVoor.text, out result2))
        {
            hoogteVerschil = result - result2;
            
        }
        else hoogteVerschil = 0f;

        hoogteVerschilText.text = (Mathf.Round(hoogteVerschil * 1000) / 1000f).ToString() + " m";

    }

    public void SetNames(int nr, string achterMeetpunt, string voorMeetpunt)
    {
        station.text = nr.ToString();
        meetpuntVoor.text = voorMeetpunt;
        meetpuntAchter.text = achterMeetpunt;
    }

    public void SetColors()
    {

    }
}
