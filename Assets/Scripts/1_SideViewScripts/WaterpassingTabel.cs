using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//************* This manages everything about the waterpassings tabel, there are child scripts for the seperate parts **********//


public class WaterpassingTabel : MonoBehaviour
{
    public enum AnswerType { Peil, VereffendPeil, Sluitfout, Vertrouwensgrens, Afstanden, HoogteVerschillen }
    [Header ("Objets")]
    public WaterpassingTabelTitel waterPassingTitle;
    [SerializeField]
    public GameObject waterPassingTabelPart;
    public GameObject waterPassingTotaal;
    [SerializeField]
    private GameObject sluitFoutInput;

    [Header("Parameters")]
    [SerializeField] private AnswerType answerType;
    [SerializeField] private bool showHeightInput;
    [SerializeField] private bool showHeightOutput;
    [SerializeField] private bool showDistanceInput;
    [SerializeField] private bool showDistanceOutput;
    [SerializeField] private bool showPeilInput;
    [SerializeField] private bool showPeilOutput;
    [Tooltip("The max error the normal peil output can have, which the students will have to correct (m)")]
    [SerializeField] private float maxPeilOutputError = 0.1f;
    [SerializeField] private bool showVereffeningsPeilInput;
    [SerializeField] private bool showVereffeningsPeilOutput;
    [SerializeField] private bool showVertrouwensgrensInput;
    [Space(10)]

    public float titleHeight = 25f;
    public bool overrideErrorMargin = true;
    public float errormarginOverride = 0.002f;
    public float lengthErrormarginOverride = 0.2f;
    public Color correctColor, falseColor;

    private GameObject totaal;

    private List<WaterpassigTabelDeel> tabelParts = new List<WaterpassigTabelDeel>();

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

    float[] correctHeights;
    float[] correctDistances;

    private int amount; 
    private bool VereffeningsMode;
    private float peilOutputError;

    private WaterPassingTabelTotaal waterPassingTabelTotaal;
    private WaterPassingController waterPassingController;
    private GameManager gm;

    private bool playing = true;
    private int pointOutLoop = -10;

    private float inputSluitFout;
    

    // Start is called before the first frame update
    void Start()
    {
        //CreateTable(4);
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        sluitFoutInput.SetActive(answerType == AnswerType.Sluitfout);
    }

    // Update is called once per frame
    void Update()
    {
        if (playing)
        {
            totalHoogte = 0f;
            totalAfstand = 0f;
            nieuwTotalHoogte = 0f;
            inputHoogteverschillen = new float[tabelParts.Count];
            inputHoogteverschillenVereffening = new float[tabelParts.Count];

            // add the values to totaal-hoogte and -afstand
            for (int i = 0; i < tabelParts.Count; i++)
            {
                totalHoogte += tabelParts[i].inputHoogteVerschil;
                inputHoogteverschillen[i] = tabelParts[i].inputHoogteVerschil;
                totalAfstand += tabelParts[i].inputAfstand;
            }

            if (totaal != null)
            {
                waterPassingTabelTotaal.SetValues(totalHoogte, totalAfstand);
            }
        }
  
    }

    /// <summary>
    /// this function creates the table accordingto the selected fields
    /// </summary>
    /// <param name="nrOfPoints"></param>
    /// <param name="pointOutLoopNr"></param>
    /// <param name="controller"></param>
    public void CreateTable(int nrOfPoints, int pointOutLoopNr, WaterPassingController controller)
    {
        waterPassingController = controller;
        peilOutputError = Random.Range(-maxPeilOutputError, maxPeilOutputError);

        waterPassingTitle.SetTitle(true, showHeightInput, showHeightOutput, showDistanceInput, showDistanceOutput, showPeilInput || showPeilOutput, showVereffeningsPeilInput || showVereffeningsPeilOutput, showVertrouwensgrensInput || showVereffeningsPeilOutput);

        amount = nrOfPoints;
        pointOutLoop = pointOutLoopNr;
        float size = waterPassingTabelPart.GetComponent<RectTransform>().rect.height;
       
        for (int i = 0; i < nrOfPoints; i++) //create a new part for every point
        {
            //spawn a new part and set the transform under the parent and at the correct position
            GameObject newPart = Instantiate(waterPassingTabelPart, transform, false);
            WaterpassigTabelDeel deel = newPart.GetComponent<WaterpassigTabelDeel>();
            newPart.GetComponent<RectTransform>().localPosition = new Vector2(0,-titleHeight - i * size);
            tabelParts.Add(deel);

            //set the names and inputfields
            if(i+1 == nrOfPoints)
            {
                deel.SetNames(i + 1, SetNameText(i - 1), SetNameText(-1), showHeightInput, showDistanceInput, showPeilInput, showVereffeningsPeilInput, showVertrouwensgrensInput);
            }
            else if(i == pointOutLoopNr)
            {
                deel.SetNames(i + 1, SetNameText(i - 2), SetNameText(i), showHeightInput, showDistanceInput, showPeilInput, showVereffeningsPeilInput, showVertrouwensgrensInput);
            }
            else 
                deel.SetNames(i + 1, SetNameText(i-1), SetNameText(i), showHeightInput, showDistanceInput, showPeilInput, showVereffeningsPeilInput, showVertrouwensgrensInput);

            //if the first one show the startPeil
            if(i == 0)
            {
                deel.SetNullPeil(0);
            }

            //set what outputs to show
            if (showHeightOutput)
            {
                deel.SetHeight(controller.correctHeightDifferences[i]);
            }
            if (showDistanceOutput)
            {
                deel.SetDistance(controller.correctDistances[i] * GameManager.worldScale);
            }
            if (showPeilOutput)
            {
                float errorOffset = peilOutputError * controller.correctDistances[i] / controller.correctDistance;
                deel.SetPeil(controller.correctHeights[i] + errorOffset);
            }
            if (showVereffeningsPeilOutput)
            {
                deel.SetVereffendPeil(controller.correctHeights[i]);
            }
        }

        totaal = Instantiate(waterPassingTotaal, transform, false);
        waterPassingTabelTotaal = totaal.GetComponent<WaterPassingTabelTotaal>();
        totaal.GetComponent<RectTransform>().localPosition = new Vector2(0, -titleHeight - (nrOfPoints) * size);
        waterPassingTabelTotaal.SetTitle(true, showHeightInput, showHeightOutput, showDistanceInput, showDistanceOutput, showPeilInput || showPeilOutput, showVereffeningsPeilInput || showVereffeningsPeilOutput, showVertrouwensgrensInput || showVereffeningsPeilOutput);
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


    public bool CheckAnswers() //todo add color feedback && zijslag
    {

        bool correct = true;

        switch (answerType)
        {
            case AnswerType.Peil:
                //check the peil
                for (int i = 0; i < tabelParts.Count; i++)
                {
                    if(Mathf.Abs(tabelParts[i].inputPeil - waterPassingController.correctHeights[i]) > errormarginOverride)
                    {
                        correct = false;
                    }
                }

                break;
            // check the heights
            case AnswerType.VereffendPeil:
                //
                for (int i = 0; i < tabelParts.Count; i++)
                {
                    if (Mathf.Abs(tabelParts[i].inputVereffendPeil - waterPassingController.correctHeightDifferences[i]) > errormarginOverride)
                    {
                        correct = false;
                    }
                }

                break;
            // check the heights
            case AnswerType.HoogteVerschillen:
                //
                for (int i = 0; i < tabelParts.Count; i++)
                {
                    if (Mathf.Abs(tabelParts[i].inputHoogteVerschil - waterPassingController.correctHeightDifferences[i]) > errormarginOverride)
                    {
                        correct = false;
                    }
                }

                break;
            // check the heights
            case AnswerType.Afstanden:
                //
                for (int i = 0; i < tabelParts.Count; i++)
                {
                    if (Mathf.Abs(tabelParts[i].inputAfstand - waterPassingController.correctDistances[i]) > lengthErrormarginOverride)
                    {
                        correct = false;
                    }
                }

                break;
            // check the heights
            case AnswerType.Vertrouwensgrens:
                //
                for (int i = 0; i < tabelParts.Count; i++)
                {
                    //answered in mm
                    if (Mathf.Abs(tabelParts[i].inputVertrouwensgrens - GetVertrouwensGrens(waterPassingController.correctDistances[i])) > errormarginOverride)
                    {
                        correct = false;
                    }
                }
                break;
            // check the heights
            case AnswerType.Sluitfout:
                //
                for (int i = 0; i < tabelParts.Count; i++)
                {
                    //answered in mm
                    if (Mathf.Abs(inputSluitFout - GetVertrouwensGrens(waterPassingController.correctDistance)) > errormarginOverride)
                    {
                        correct = false;
                    }
                }

                break;

            default:
                break;
        }
        return correct;
        
    }
    // start is to indicate weither to show the values at start or not
    public void ShowCorrectValues()
    {
        playing = false;

        for (int i = 0; i < tabelParts.Count; i++)
        {
            tabelParts[i].ShowCorrectHeight(waterPassingController.correctHeightDifferences[i]);
            tabelParts[i].ShowCorrectDistance(waterPassingController.correctDistances[i]);
            tabelParts[i].ShowCorrectPeil(waterPassingController.correctHeights[i]);
            tabelParts[i].ShowCorrectVereffenPeil(waterPassingController.correctHeights[i]);
            tabelParts[i].ShowCorrectVertrouwenGrens(GetVertrouwensGrens(waterPassingController.correctDistances[i]));
        }
    }

    public void SetSluitFout(string input)
    {
        if(float.TryParse(input, out float output))
        {
            inputSluitFout = output;
        }
    }

    float GetVertrouwensGrens(float distance)
    {
        return 2.56f * Mathf.Sqrt(2) * waterPassingController.sigma1kmHT * Mathf.Sqrt(distance);
    }

    void ShowHeightDifferences()
    {

    }

    void ShowDistances()
    {

    }


}
