using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterPassingTabelTotaal : MonoBehaviour
{
    public Text hoogteVerschilText;
    public Text afstandText;
    public Text nieuwHoogteText;


    public void SetValues(float hoogte, float afstand)
    {
        hoogteVerschilText.text = GameManager.RoundFloat(hoogte,3).ToString() + " m";
        afstandText.text = GameManager.RoundFloat(afstand,3).ToString() + " m";
    }
    public void SetNieuwHoogte(float nieuwhoogte)
    {
        nieuwHoogteText.text = GameManager.RoundFloat(nieuwhoogte,3).ToString() + " m";
    }

}
