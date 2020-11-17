using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PolygonatieLoopTabelDeel : MonoBehaviour
{
    private bool coordinateMode;
    public GameObject part1;
    public GameObject part2;
    [Header("Title")]
    public Text nr;
    [Header("Part 1")]
    public Text angleInput;
    public Text lengthInput;
    public Text kaartHoekInput;
    public InputField vereffendeKaarthoekInput;

    [Header ("Part 2")]
    public Text vereffendeKaarthoekOutput;
    public Text afstandsOutput;
    public InputField deltaXInput;
    public InputField deltaYInput;
    public InputField xInput;
    public InputField yInput;




    public float vereffendeKaarthoek;


    // Start is called before the first frame update
    void Start()
    {
        coordinateMode = false;
        part1.SetActive(!coordinateMode);
        part2.SetActive(coordinateMode);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchModes()
    {
        coordinateMode = !coordinateMode;
        part1.SetActive(!coordinateMode);
        part2.SetActive(coordinateMode);

        UpdateOutputs();

    }

    public Vector2 getAnswer()
    {
        xInput.text = xInput.text.Replace(",", ".");
        yInput.text = yInput.text.Replace(",", ".");


        if (float.TryParse(xInput.text, out float resultX) && float.TryParse(yInput.text, out float resultY))
        {
            return new Vector2(resultX, resultY);
        }
        else return Vector2.zero;

    }
    public float GetMapAngleInput()
    {
        vereffendeKaarthoekInput.text = vereffendeKaarthoekInput.text.Replace(",", ".");

        if (float.TryParse(vereffendeKaarthoekInput.text, out float angle))
        {
            vereffendeKaarthoek = angle;
            return angle;
        }
        else return 0;
    }

    public void SetValues(Vector2 coordinate, Vector3 nextPoint, bool last)
    {
        coordinateMode = true;
        part1.SetActive(false);
        part2.SetActive(true);
        UpdateOutputs();

        vereffendeKaarthoekOutput.text = GetMapAngle(coordinate, nextPoint).ToString();

        if (!last)
        {
            afstandsOutput.text = GameManager.RoundFloat(Vector2.Distance(coordinate, nextPoint), 3).ToString();
            

            deltaXInput.text = GameManager.RoundFloat(nextPoint.x - coordinate.x, 3).ToString();
            deltaXInput.GetComponentInParent<InputField>().interactable = false;

            deltaYInput.text = GameManager.RoundFloat(nextPoint.y - coordinate.y, 3).ToString();
            deltaYInput.GetComponentInParent<InputField>().interactable = false;

        }

        xInput.text = GameManager.RoundFloat(coordinate.x, 3).ToString();
        xInput.GetComponentInParent<InputField>().interactable = false;

        yInput.text = GameManager.RoundFloat(coordinate.y, 3).ToString();
        yInput.GetComponentInParent<InputField>().interactable = false;


    }

    void UpdateOutputs()
    {
        vereffendeKaarthoekInput.text = vereffendeKaarthoekInput.text.Replace(",", ".");

        if (float.TryParse(vereffendeKaarthoekInput.text, out float kaartHoek))
        {
            vereffendeKaarthoek = GameManager.RoundFloat(kaartHoek, 3);
            vereffendeKaarthoekOutput.text = vereffendeKaarthoek.ToString() + " gon";  
        }
        else
        {
            vereffendeKaarthoek = 0;
            vereffendeKaarthoekOutput.text = " / ";
        }
        lengthInput.text = lengthInput.text.Replace(",", ".");

        if (float.TryParse(lengthInput.text, out float length))
        {
            afstandsOutput.text = GameManager.RoundFloat(length, 3).ToString() + " m";
        }
        else afstandsOutput.text = " / ";

    }

    //returns the mapangle of a targetpoint from a referencepoint
    public float GetMapAngle(Vector2 point, Vector2 targetPoint)
    {
        float angle = Vector2.SignedAngle(targetPoint - point, Vector2.up);
        if (angle < 0) angle = 360 + angle;
        angle = GameManager.RoundFloat(angle / 360f * 400, 3);
        return angle;
    }

    public void SetColor(Color color, bool coordinate)
    {
        if (coordinate)
        {
            Text[] values = xInput.GetComponentsInChildren<Text>();
            foreach (var text in values)
            {
                text.color = color;
            }
            values = yInput.GetComponentsInChildren<Text>();
            foreach (var text in values)
            {
                text.color = color;
            }
        }
        else
        {
            Text[] values = vereffendeKaarthoekInput.GetComponentsInChildren<Text>();
            foreach (var text in values)
            {
                text.color = color;
            }
        }
        
    }


}
