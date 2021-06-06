using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterPassingTabelTotaal : MonoBehaviour
{
    public Text hoogteVerschilText;
    public Text afstandText;
    public Text nieuwHoogteText;

    [SerializeField]
    private GameObject totaal, stationEmpty, hoogteMetingEmpty, hoogteVerschil, afstandMetingEmpty, afstand, peilEmpty, vereffendPeilEmpty, vertrouwensGrensEmpty;

    public void SetTitle(bool showStation, bool showHeightInput, bool showHeightOutput, bool showDistanceInput, bool showDistanceOutput, bool showPeilInput, bool showVereffenningsPeilInput, bool showVertrouwensgrensInput)
    {
        totaal.SetActive(showStation && (showHeightOutput || showDistanceOutput || showHeightInput || showDistanceInput));
        stationEmpty.SetActive(showStation && !totaal.activeSelf);
        hoogteMetingEmpty.SetActive(showHeightInput && !showHeightOutput);
        hoogteVerschil.SetActive(showHeightOutput || showHeightInput);
        afstandMetingEmpty.SetActive(showDistanceInput && !showDistanceOutput);
        afstand.SetActive(showDistanceOutput || showDistanceInput);
        peilEmpty.SetActive(showPeilInput);
        vereffendPeilEmpty.SetActive(showVereffenningsPeilInput);
        vertrouwensGrensEmpty.SetActive(showVertrouwensgrensInput);
    }

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
