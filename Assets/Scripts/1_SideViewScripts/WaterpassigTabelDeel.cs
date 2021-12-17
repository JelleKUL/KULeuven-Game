using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//***************** Manages info of a part of the waterpassingtabel *************//


public class WaterpassigTabelDeel : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private GameObject stationPanel;
    [SerializeField] private Text station;
    [SerializeField] private Text meetpuntAchter;
    [SerializeField] private Text meetpuntVoor;
    [Space(10)]
    [SerializeField] private GameObject hoogteMetingPanel;
    [SerializeField] private InputField metingAchterInput;
    [SerializeField] private InputField metingVoorInput;
    [SerializeField] private GameObject hoogteVerschilPanel;
    [SerializeField] private Text hoogteVerschilText;
    [Space(10)]
    [SerializeField] private GameObject afstandMetingPanel;
    [SerializeField] private InputField afstandAchterInput;
    [SerializeField] private InputField afstandVoorInput;
    [SerializeField] private GameObject afstandPanel;
    [SerializeField] private Text afstandText;
    [Space(10)]
    [SerializeField] private GameObject peilPanel;
    [SerializeField] private InputField peilInput;
    [SerializeField] private GameObject NullOutputPanel;
    [SerializeField] private Text nullPeilOutput;
    [SerializeField] private GameObject peilOutputPanel;
    [SerializeField] private Text peilOutputText;
    [Space(10)]
    [SerializeField] private GameObject vereffendPeilPanel;
    [SerializeField] private InputField vereffendPeilInput;
    [SerializeField] private GameObject vereffendNullOutputPanel;
    [SerializeField] private Text vereffendNullPeilOutput;
    [SerializeField] private GameObject vereffendPeilOutputPanel;
    [SerializeField] private Text vereffendPeilOutputText;
    [Space(10)]
    [SerializeField] private GameObject vertrouwensgrensPanel;
    [SerializeField] private InputField vertrouwensgrensInput;

    [HideInInspector]
    public float inputHoogteVerschil;
    [HideInInspector]
    public float inputAfstand;
    [HideInInspector]
    public float inputPeil;
    [HideInInspector]
    public float inputVereffendPeil;
    [HideInInspector]
    public float inputVertrouwensgrens;

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateHeightDiff()
    {
        inputHoogteVerschil = 0f;
        if (float.TryParse(metingAchterInput.text.Replace(",", "."), out float result))
        {
            inputHoogteVerschil += result;
        }

        if (float.TryParse(metingVoorInput.text.Replace(",", "."), out float result2))
        {
            inputHoogteVerschil -= result2;
        }

        hoogteVerschilText.text = (Mathf.Round(inputHoogteVerschil * 1000) / 1000f).ToString() + " m";
    }

    public void UpdateDistance()
    {
        inputAfstand = 0f;
        if (float.TryParse(afstandAchterInput.text.Replace(",", "."), out float result))
        {
            inputAfstand += result;
        }

        if (float.TryParse(afstandVoorInput.text.Replace(",", "."), out float result2))
        {
            inputAfstand += result2;
        }
        afstandText.text = (Mathf.Round(inputAfstand * 10) / 10f).ToString() + " m";
    }

    public void UpdatePeil(string input)
    {
        if (float.TryParse(input.Replace(",", "."), out float result))
        {
            inputPeil = result;
        }
        else inputPeil = 0;
    }

    public void UpdateVereffendPeil(string input)
    {
        if (float.TryParse(input.Replace(",", "."), out float result))
        {
            inputVereffendPeil = result;
        }
        else inputVereffendPeil = 0;
    }


    //start values management
    public void SetNames(int nr, string achterMeetpunt, string voorMeetpunt, bool showHeightInput, bool showDistanceInput, bool showPeilInput, bool showVereffenningsPeilInput, bool showVertrouwensgrensInput)
    {
        station.text = nr.ToString();
        meetpuntVoor.text = voorMeetpunt;
        meetpuntAchter.text = achterMeetpunt;

        hoogteMetingPanel.SetActive(showHeightInput);
        hoogteVerschilPanel.SetActive(showHeightInput);

        afstandMetingPanel.SetActive(showDistanceInput);
        afstandPanel.SetActive(showDistanceInput);

        peilPanel.SetActive(showPeilInput);
        peilInput.gameObject.SetActive(showPeilInput);

        vereffendPeilPanel.SetActive(showVereffenningsPeilInput);
        vereffendPeilInput.gameObject.SetActive(showVereffenningsPeilInput);

        vertrouwensgrensPanel.SetActive(showVertrouwensgrensInput);
    }

    public void SetHeight(float hoogteVerschil)
    {
        hoogteMetingPanel.SetActive(false);
        hoogteVerschilPanel.SetActive(true);
        hoogteVerschilText.text = (Mathf.Round(hoogteVerschil * 1000) / 1000f).ToString() + " m";
        inputHoogteVerschil = (Mathf.Round(hoogteVerschil * 1000) / 1000f);
    }
    public void SetDistance(float afstand)
    {
        afstandMetingPanel.SetActive(false);
        afstandPanel.SetActive(true);
        afstandText.text = (Mathf.Round(afstand * 10) / 10f).ToString() + " m";
        inputAfstand = (Mathf.Round(afstand * 10) / 10f);
    }
    public void SetPeil(float peil)
    {
        peilPanel.SetActive(true);
        peilOutputPanel.SetActive(true);
        peilInput.gameObject.SetActive(false);
        peilOutputText.text = (Mathf.Round(peil * 1000) / 1000f).ToString() + " m";
        inputPeil = (Mathf.Round(peil * 1000) / 1000f);
    }
    public void SetVereffendPeil(float vereffendPeil)
    {
        vereffendPeilPanel.SetActive(true);
        vereffendPeilOutputPanel.SetActive(true);
        vereffendPeilInput.gameObject.SetActive(false);
        vereffendPeilOutputText.text = (Mathf.Round(vereffendPeil * 1000) / 1000f).ToString() + " m";
        inputVereffendPeil = (Mathf.Round(vereffendPeil * 1000) / 1000f);
    }
    public void SetNullPeil(float startPeil)
    {
        NullOutputPanel.SetActive(true);
        nullPeilOutput.text = (Mathf.Round(startPeil * 1000) / 1000f).ToString() + " m";

        vereffendNullOutputPanel.SetActive(true);
        vereffendNullPeilOutput.text = nullPeilOutput.text;
    }
    public void SetVertrouwensgrens(float grens)
    {
        vertrouwensgrensPanel.SetActive(true);
        vertrouwensgrensInput.text = (Mathf.Round(grens * 1000) / 1000f).ToString() + " m";
        vertrouwensgrensInput.interactable = false;
        inputVertrouwensgrens = (Mathf.Round(grens * 1000) / 1000f);
    }


    //show correct answers
    public void ShowCorrectHeight(float height)
    {
        foreach (Transform child in hoogteMetingPanel.transform)
        {
            child.GetComponent<InputField>().interactable = false;
        }
        hoogteVerschilText.text = (Mathf.Round(height * 1000) / 1000f).ToString() + " m";
    }
    //show correct answers
    public void ShowCorrectDistance(float distance)
    {
        foreach (Transform child in afstandMetingPanel.transform)
        {
            child.GetComponent<InputField>().interactable = false;
        }
        afstandText.text = (Mathf.Round(distance * 10) / 10f).ToString() + " m";
    }
    //show correct answers
    public void ShowCorrectPeil(float peil)
    {
        peilInput.interactable = false;
        peilInput.text = (Mathf.Round(peil * 1000) / 1000f).ToString() + " m";
    }
    //show correct answers
    public void ShowCorrectVereffenPeil(float peil)
    {
        vereffendPeilInput.interactable = false;
        vereffendPeilInput.text = (Mathf.Round(peil * 1000) / 1000f).ToString() + " m";
    }
    //show correct answers
    public void ShowCorrectVertrouwenGrens(float grens)
    {
        vertrouwensgrensInput.interactable = false;
        vertrouwensgrensInput.text = (Mathf.Round(grens * 1000) / 1000f).ToString() + " m";
    }

    public void SetAfstandColor(Color color)
    {
        afstandText.color = color;
    }
    public void SetHoogteColor(Color color)
    {
        hoogteVerschilText.color = color;
    }

}
