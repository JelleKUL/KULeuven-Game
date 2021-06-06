using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterpassingTabelTitel : MonoBehaviour
{
    [SerializeField]
    private GameObject station, hoogteMeting, hoogteVerschil, afstandMeting, afstand, peil, vereffendPeil, vertrouwensGrens;

    public void SetTitle(bool showStation, bool showHeightInput,bool showHeightOutput, bool showDistanceInput, bool showDistanceOutput, bool showPeilInput, bool showVereffenningsPeilInput, bool showVertrouwensgrensInput)
    {
        station.SetActive(showStation);
        hoogteMeting.SetActive(showHeightInput && !showHeightOutput);
        hoogteVerschil.SetActive(showHeightOutput || showHeightInput);
        afstandMeting.SetActive(showDistanceInput && !showDistanceOutput);
        afstand.SetActive(showDistanceOutput || showDistanceInput);
        peil.SetActive(showPeilInput);
        vereffendPeil.SetActive(showVereffenningsPeilInput);
        vertrouwensGrens.SetActive(showVertrouwensgrensInput);
    }
}
