using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterpassingTabel : MonoBehaviour
{
    public GameObject waterPassingTabelPart;
    public GameObject waterPassingTotaal;

    private GameObject totaal;

    private List<WaterpassigTabelDeel> tabelParts = new List<WaterpassigTabelDeel>();

    public float totalHoogte;
    public float totalAfstand;
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
        for (int i = 0; i < tabelParts.Count; i++)
        {
            totalHoogte += tabelParts[i].hoogteVerschil;
            totalAfstand += tabelParts[i].afstand;
        }

        if(totaal != null)
        {
            totaal.GetComponent<WaterPassingTabelTotaal>().SetValues(totalHoogte, totalAfstand);
        }
    }

    public void CreateTable(int nrOfPoints)
    {
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
}
