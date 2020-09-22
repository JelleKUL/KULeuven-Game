using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//************* This maneges everything about the waterpassings tabel, there are child scripts for the seperate parts**********//

public class WaterpassingTabel : MonoBehaviour
{
    
    [Header ("Objets")]
    public GameObject waterPassingHeader;
    public GameObject waterPassingHeaderVereffening;

    public GameObject waterPassingTabelPart;
    public GameObject waterPassingTabelVereffening;
    public GameObject waterPassingTotaal;
    public GameObject waterPassingTotaalVereffening;

    [Header("Parameters")]
    
    public float titleHeight = 25f;
    public bool overrideErrorMargin = true;
    public float errormarginOverride = 0.002f;
    public Color correctColor, falseColor;

    private GameObject totaal;
    private GameObject totaalVereffening;

    private List<WaterpassigTabelDeel> tabelParts = new List<WaterpassigTabelDeel>();
    private List<WaterPassingTabelVereffening> tabelVereffeningParts = new List<WaterPassingTabelVereffening>();

    [HideInInspector]
    public float[] inputHoogteverschillen;
    [HideInInspector]
    public float[] inputHoogteverschillenVereffening;
    [HideInInspector]
    public float totalHoogte;
    [HideInInspector]
    public float totalAfstand;
    [HideInInspector]
    public float nieuwTotalHoogte;

    private int amount; 
    private bool VereffeningsMode;

    private WaterPassingTabelTotaal waterPassingTabelTotaal;
    private GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        //CreateTable(4);
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
        totalHoogte = 0f;
        totalAfstand = 0f;
        nieuwTotalHoogte = 0f;
        inputHoogteverschillen = new float[tabelParts.Count];
        inputHoogteverschillenVereffening = new float[tabelParts.Count];

        // add the values to totaal-hoogte and -afstand
        for (int i = 0; i < tabelParts.Count; i++)
        {
            totalHoogte += tabelParts[i].hoogteVerschil;
            inputHoogteverschillen[i] = tabelParts[i].hoogteVerschil;
            totalAfstand += tabelParts[i].afstand;
        }

        if (totaal != null)
        {
            waterPassingTabelTotaal.SetValues(totalHoogte, totalAfstand);
        }
        
        
        for (int i = 0; i < tabelVereffeningParts.Count; i++)
        {
            nieuwTotalHoogte += tabelVereffeningParts[i].vereffenigsHoogte;
            inputHoogteverschillenVereffening[i] = tabelVereffeningParts[i].vereffenigsHoogte;
            tabelVereffeningParts[i].GetComponent<WaterPassingTabelVereffening>().SetValues(tabelParts[i].hoogteVerschilText.text, tabelParts[i].afstand.ToString());

        }


        if (totaalVereffening != null)
        {
            totaalVereffening.GetComponent<WaterPassingTabelTotaal>().SetNieuwHoogte(nieuwTotalHoogte);
            totaalVereffening.GetComponent<WaterPassingTabelTotaal>().SetValues(totalHoogte, totalAfstand);
        }
            
    }

    public void CreateTable(int nrOfPoints)
    {
        amount = nrOfPoints;
        float size = waterPassingTabelPart.GetComponent<RectTransform>().rect.height;
       
        for (int i = 0; i < nrOfPoints; i++)
        {
            GameObject newPart = Instantiate(waterPassingTabelPart, transform, false);
            WaterpassigTabelDeel deel = newPart.GetComponent<WaterpassigTabelDeel>();
            newPart.GetComponent<RectTransform>().localPosition = new Vector2(0,-titleHeight - i * size);
            tabelParts.Add(deel);

            if(i+1 == nrOfPoints)
            {
                deel.SetNames(i + 1, SetNameText(i - 1), SetNameText(-1));
            }
            else 
                deel.SetNames(i + 1, SetNameText(i-1), SetNameText(i));
        }

        totaal = Instantiate(waterPassingTotaal, transform, false);
        waterPassingTabelTotaal = totaal.GetComponent<WaterPassingTabelTotaal>();
        totaal.GetComponent<RectTransform>().localPosition = new Vector2(0, -titleHeight - (nrOfPoints) * size);


        //creating the VereffeningsTable and setting it False
        
        size = waterPassingTabelVereffening.GetComponent<RectTransform>().rect.height;

        for (int i = 0; i < amount; i++)
        {
            GameObject newPart = Instantiate(waterPassingTabelVereffening, transform, false);
            WaterPassingTabelVereffening deel = newPart.GetComponent<WaterPassingTabelVereffening>();
            newPart.GetComponent<RectTransform>().localPosition = new Vector2(0, -titleHeight - i * size);
            tabelVereffeningParts.Add(deel);

            deel.SetName(i + 1);
            deel.gameObject.SetActive(false);

        }
        totaalVereffening = Instantiate(waterPassingTotaalVereffening, transform, false);
        totaalVereffening.GetComponent<RectTransform>().localPosition = new Vector2(0, -titleHeight - (amount) * size);
        totaalVereffening.GetComponent<WaterPassingTabelTotaal>().SetValues(totalHoogte, totalAfstand);
        totaalVereffening.SetActive(false);
    }

    public string SetNameText(int nr)
    {
        if (nr < 0)
        {
            return 0.ToString();
        }
        else
        {
            char c = (char)(65 + (nr));

            return  c.ToString();
        }

    }



    // set the tables active according to the input
    public void ActiveTable(bool input)
    {
        VereffeningsMode = !input;
        foreach (var tabelPart in tabelParts)
        {
            tabelPart.gameObject.SetActive(input);    
        }
        waterPassingHeader.SetActive(input);
        totaal.SetActive(input);

        foreach (var tabelPart in tabelVereffeningParts)
        {

            tabelPart.gameObject.SetActive(!input);
        }
        waterPassingHeaderVereffening.SetActive(!input);
        totaalVereffening.SetActive(!input);
    }


    public bool CheckAnswers(float[] heights, float[] distances)
    {
        bool correct = true;
        if (VereffeningsMode)
        {
            
            for (int i = 0; i < tabelVereffeningParts.Count; i++)
            {
                if (Mathf.Abs(tabelVereffeningParts[i].vereffenigsHoogte - heights[i]) > (overrideErrorMargin ? errormarginOverride : gm.errorMargin))
                {
                    tabelVereffeningParts[i].vereffeningsHoogteText.color = falseColor;
                    correct = false;
                }

                else
                {
                    tabelVereffeningParts[i].vereffeningsHoogteText.color = correctColor;

                }
                if (Mathf.Abs(tabelParts[i].afstand - distances[i]) > (overrideErrorMargin ? errormarginOverride: gm.errorMargin))
                {
                    tabelVereffeningParts[i].afstandText.color = falseColor;
                    correct = false;
                }

                else
                {
                    tabelVereffeningParts[i].afstandText.color = correctColor;

                }

            }

        }

        else
        { 
                
            for (int i = 0; i < tabelParts.Count; i++)
            {

                if (Mathf.Abs(tabelParts[i].hoogteVerschil - heights[i]) > (overrideErrorMargin ? errormarginOverride : gm.errorMargin))
                {
                    tabelParts[i].hoogteVerschilText.color = falseColor;
                    
                }
                else
                {
                    tabelParts[i].hoogteVerschilText.color = correctColor;
                }
                
            }
            correct = false;
        }
        return correct;
    }
}
