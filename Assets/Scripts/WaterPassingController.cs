using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterPassingController : MonoBehaviour
{
    public GameObject beacon;
    public GameObject measure;
    public GameObject correctMeasure;
    public GameObject magnifyGlass;
    public GameObject startPoint;
    public GameObject endPoint;
    public Text heightAnswer;
    public Text answerText;
    public Vector2 minPoint;
    public Vector2 maxPoint;
    public float distanceMeasureAngle;
    public float maxAngleError;
    public float minDistance;
    public int scoreIncrease = 1;

    private Vector2 objectPos;
    private GameManager gm;

    private float correctHeight;
    private float meanX;
    private float errorAngle;
    private bool measurePlaced;
    private bool magnifyPlaced;
    private bool useMagnify;
    private int posErrorAngle = 1;
    private Vector2 mousePos;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        measure.transform.GetChild(1).gameObject.SetActive(false);
        

        meanX = (maxPoint.x - minPoint.x) / 2;
        errorAngle = Random.Range(-maxAngleError, maxAngleError);
        startPoint = Instantiate(startPoint, objectPos, Quaternion.identity);
        endPoint = Instantiate(endPoint, objectPos, Quaternion.identity);


        ChangeTransform(startPoint, 0, 1);
        ChangeTransform(endPoint, 1, 0);

        correctHeight = endPoint.transform.position.y - startPoint.transform.position.y;
        Debug.Log(correctHeight);
    }

    // Update is called once per frame
    void Update()
    {
        if (!measurePlaced && Input.GetMouseButtonDown(0))
        {
            PlaceMeasure();
        }
        else if (Input.GetMouseButton(0) && gm.IsBetweenValues(gm.SetObjectToMouse(Input.mousePosition, 0)) && !useMagnify)
        {
            measure.transform.position = gm.SetObjectToMouse(Input.mousePosition, 0);
        }
        else if (useMagnify && !magnifyPlaced)
        {
            PlaceMagnify();
        }
        else if (useMagnify)
        {
            magnifyGlass.transform.position = gm.SetObjectToMouse(Input.mousePosition, -5);
        }
        
    }

    public void ChangePoints()
    {
        measure.transform.GetChild(1).gameObject.SetActive(false);
        ChangeTransform(startPoint,0,1);
        ChangeTransform(endPoint,1,0);
        errorAngle = Random.Range(-maxAngleError, maxAngleError);
        measure.transform.GetChild(0).transform.SetPositionAndRotation(measure.transform.GetChild(0).transform.position, Quaternion.Euler(0, 0, errorAngle));
    }

    public void ChangeTransform(GameObject obj, int start, int end)
    {
        objectPos = new Vector2(Random.Range(minPoint.x + (meanX + minDistance) * start, maxPoint.x - (meanX + minDistance) * end), Random.Range(minPoint.y, maxPoint.y));
        obj.transform.SetPositionAndRotation(objectPos, Quaternion.identity);
        correctHeight = endPoint.transform.position.y - startPoint.transform.position.y;
        Debug.Log(correctHeight);

    }

    public void PlaceMeasure()
    {
        measure = Instantiate(measure, gm.SetObjectToMouse(Input.mousePosition, 0), Quaternion.identity);
        measure.transform.GetChild(0).transform.Rotate(0, 0, errorAngle);

        measure.transform.GetChild(0).GetChild(0).Rotate(0, 0, distanceMeasureAngle);
        measure.transform.GetChild(0).GetChild(1).Rotate(0, 0, -distanceMeasureAngle);
        measurePlaced = true;
    }
    public void FlipMeasure()
    {
        posErrorAngle = -posErrorAngle;
        measure.transform.GetChild(0).transform.Rotate(0,0, errorAngle * 2 * posErrorAngle);
        Debug.Log(measure.transform.GetChild(0).transform.rotation.eulerAngles);
    }
    public void PlaceMagnify()
    {
        magnifyGlass = Instantiate(magnifyGlass, gm.SetObjectToMouse(Input.mousePosition, -5), Quaternion.identity);
        magnifyPlaced = true;
    }
    public void ToggleMagnify()
    {
        if (useMagnify)
        {
            magnifyGlass.SetActive(false);
            useMagnify = false;
        }
        else
        {
            magnifyGlass.SetActive(true);
            useMagnify = true;
        }
    }

    public void CheckAnswer()
    {
        if (gm.CheckCorrectAnswer(heightAnswer.text, correctHeight))
        {
            gm.IncreaseScore(scoreIncrease);
            Debug.Log("true");
            ChangePoints();
        }
        else Debug.Log("false");

    }
    public void ShowAnswer()
    {
        measure.transform.GetChild(1).gameObject.SetActive(true);
        measure.transform.GetChild(1).GetChild(0).Rotate(0, 0, distanceMeasureAngle);
        measure.transform.GetChild(1).GetChild(1).Rotate(0, 0, -distanceMeasureAngle);

        answerText.text = "errorAngle: " + errorAngle.ToString() + " & Height Diff: " + correctHeight.ToString();
    }

}
