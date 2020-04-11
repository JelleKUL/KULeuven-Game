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
        if (!VereffeningsMode)
        {
            totalHoogte = 0f;
            totalAfstand = 0f;
            for (int i = 0; i < tabelParts.Count; i++)
            {
                totalHoogte += tabelParts[i].hoogteVerschil;
                totalAfstand += tabelParts[i].afstand;
            }

            if (totaal != null)
            {
                totaal.GetComponent<WaterPassingTabelTotaal>().SetValues(totalHoogte, totalAfstand);
            }
        }
        else
        {
            nieuwTotalHoogte = 0f;
            for (int i = 0; i < tabelVereffeningParts.Count; i++)
            {
                nieuwTotalHoogte += tabelVereffeningParts[i].hoogteVerschil;
                
            }

            if (totaal != null)
            {
                totaalVereffening.GetComponent<WaterPassingTabelTotaal>().SetNieuwHoogte(nieuwTotalHoogte);
            }
            
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

    public void CreateVereffeningTabel()
    {
        VereffeningsMode = true;
        foreach (var tabelPart in tabelParts)
        {
            tabelPart.gameObject.SetActive(false);
            totaal.SetActive(false);
        }

        float size = waterPassingTabelVereffening.GetComponent<RectTransform>().rect.height;

        for (int i = 0; i < amount; i++)
        {
            GameObject newPart = Instantiate(waterPassingTabelVereffening, transform, false);
            WaterPassingTabelVereffening deel = newPart.GetComponent<WaterPassingTabelVereffening>();
            newPart.GetComponent<RectTransform>().localPosition = new Vector2(0, -20 - i * size);
            tabelVereffeningParts.Add(deel);

                deel.SetNames(i + 1, tabelParts[i].hoogteVerschilText.text, tabelParts[i].afstand.ToString());
            
        }
        totaalVereffening = Instantiate(waterPassingTotaalVereffening, transform, false);
        totaalVereffening.GetComponent<RectTransform>().localPosition = new Vector2(0, -20 - (amount) * size);
        totaalVereffening.GetComponent<WaterPassingTabelTotaal>().SetValues(totalHoogte, totalAfstand);
    }
}
