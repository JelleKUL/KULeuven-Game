using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PolygonatieLoopTabelDeel : MonoBehaviour
{
    private bool coordinateMode;
    public GameObject part1;
    public GameObject part2;

    public Text nr;
    public Text angleInput;
    public Text lengthInput;
    public Text kaartHoekInput;
    public Text vereffendeKaarthoekInput;

    public Text vereffendeKaarthoekText;
    public Text deltaXInput;
    public Text deltaYInput;
    public Text xInput;
    public Text yInput;


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

    }

    public Vector2 getAnswer()
    {
        if (float.TryParse(xInput.text, out float resultX) && float.TryParse(yInput.text, out float resultY))
        {
            return new Vector2(resultX, resultY);
        }
        else return Vector2.zero;

    }
}
