using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterpassigTabelDeel : MonoBehaviour
{
    public Text station;
    public Text meetpuntAchter;
    public Text meetpuntVoor;
    public Text metingAchter;
    public Text metingVoor;
    public Text hoogteVerschilText;
    public Text afstandVoor;
    public Text afstandAchter;

    public float hoogteVerschil;
    public float afstand;

    // Start is called before the first frame update
    void Start()
    {
        
    }

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
            hoogteVerschil = result2 - result;
            
        }
        else hoogteVerschil = 0f;

        hoogteVerschilText.text = hoogteVerschil.ToString() + " m";

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
