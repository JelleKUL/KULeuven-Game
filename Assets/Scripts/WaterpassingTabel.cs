using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterpassingTabel : MonoBehaviour
{
    public GameObject waterPassingTabelPart;
    public GameObject waterPassingTabelVereffening;
    public GameObject waterPassingTotaal;
    public GameObject waterPassingTotaalVereffening;

    private GameObject totaal;
    private GameObject totaalVereffening;

    private List<WaterpassigTabelDeel> tabelParts = new List<WaterpassigTabelDeel>();
    private List<WaterPassingTabelVereffening> tabelVereffeningParts = new List<WaterPassingTabelVereffening>();

    public float[] inputHoogteverschillen;
    public float[] inputHoogteverschillenVereffening;

    public float totalHoogte;
    public float totalAfstand;
    public float nieuwTotalHoogte;
    private int amount; 

    private bool VereffeningsMode;
    // Start is called before the first frame update
    void Start()
    {
        //CreateTable(4);
    }

    // Update is called once per frame
    void Update()
    {
        
        totalHoogte = 0f;
        totalAfstand = 0f;
        nieuwTotalHoogte = 0f;
        inputHoogteverschillen = new float[tabelParts.Count];
        inputHoogteverschillenVereffening = new float[tabelParts.Count];

        for (int i = 0; i < tabelParts.Count; i++)
        {
            totalHoogte += tabelParts[i].hoogteVerschil;
            inputHoogteverschillen[i] = tabelParts[i].hoogteVerschil;
            totalAfstand += tabelParts[i].afstand;
        }

        if (totaal != null)
        {
            totaal.GetComponent<WaterPassingTabelTotaal>().SetValues(totalHoogte, totalAfstand);
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
            newPart.GetComponent<RectTransform>().localPosition = new Vector2(0,-20 -i * size);
            tabelParts.Add(deel);

            if(i+1 == nrOfPoints)
            {
                deel.SetNames(i + 1, SetNameText(i - 1), SetNameText(-1));
            }
            else 
                deel.SetNames(i + 1, SetNameText(i-1), SetNameText(i));
        }
        totaal = Instantiate(waterPassingTotaal, transform, false);
        totaal.GetComponent<RectTransform>().localPosition = new Vector2(0, -20 - (nrOfPoints) * size);

        //creating the VereffeningsTable and setting it False
        
        size = waterPassingTabelVereffening.GetComponent<RectTransform>().rect.height;

        for (int i = 0; i < amount; i++)
        {
            GameObject newPart = Instantiate(waterPassingTabelVereffening, transform, false);
            WaterPassingTabelVereffening deel = newPart.GetComponent<WaterPassingTabelVereffening>();
            newPart.GetComponent<RectTransform>().localPosition = new Vector2(0, -20 - i * size);
            tabelVereffeningParts.Add(deel);

            deel.SetName(i + 1);
            deel.gameObject.SetActive(false);

        }
        totaalVereffening = Instantiate(waterPassingTotaalVereffening, transform, false);
        totaalVereffening.GetComponent<RectTransform>().localPosition = new Vector2(0, -20 - (amount) * size);
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
        totaal.SetActive(input);

        foreach (var tabelPart in tabelVereffeningParts)
        {

            tabelPart.gameObject.SetActive(!input);
        }
        totaalVereffening.SetActive(!input);
    }

    public bool CheckAnswers(float[] inputs)
    {
        bool correct = true;
        if (VereffeningsMode)
        {
            if (Mathf.Abs(nieuwTotalHoogte) <  0.02)
            {
                Debug.Log("correct");
                for (int i = 0; i < tabelVereffeningParts.Count; i++)
                {
                    if(Mathf.Abs(tabelVereffeningParts[i].vereffenigsHoogte - inputs[i]) > 0.02)
                    {
                        correct = false;
                    }
                    
                 }
            }
            else correct = false;
        }
        else
        { 
                
            for (int i = 0; i < tabelParts.Count; i++)
            {
                if (Mathf.Abs(tabelParts[i].hoogteVerschil - inputs[i]) > 0.02)
                {
                    correct = false;
                }

            }
           
        }
        return correct;
    }
}
