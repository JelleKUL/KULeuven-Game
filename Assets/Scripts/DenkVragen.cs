using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class DenkVragen : MonoBehaviour
{
    public Text vraagText;
    public Text uitlegText;

    public DenkVraag[] vragen;
    private static List<DenkVraag> testVragen;
    private GameManager gm;

    private DenkVraag denkVraag;

    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();


        if (testVragen == null || testVragen.Count == 0)
        {
            testVragen = vragen.ToList<DenkVraag>();
        }

        SetCurrentQuestion();

    }

    public void SetCurrentQuestion()
    {
        if(testVragen.Count == 0) testVragen = vragen.ToList<DenkVraag>();
        uitlegText.text = "";
        int randomIndex = Random.Range(0,testVragen.Count);
        denkVraag = testVragen[randomIndex];
        vraagText.text = denkVraag.vraagText;
        Debug.Log(denkVraag.vraagText + " is " + denkVraag.isWaar);

        testVragen.RemoveAt(randomIndex);
    }

    public void MultipleChoiceSelection(bool isTrue)
    {
        if(denkVraag.isWaar == isTrue)
        {
            uitlegText.text = "Correct!";
            Debug.Log("juist");
            gm.IncreaseScore(1);
            Invoke("SetCurrentQuestion", 1f);
        }
        else
        {
            Debug.Log("fout");
            uitlegText.text = "Fout: " + denkVraag.uitleg;

        }
    }
    
}
