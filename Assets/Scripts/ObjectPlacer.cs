using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectPlacer : MonoBehaviour
{
    public Vector2 minPoint;
    public Vector2 maxPoint;
    public float randomAngle;
    public int scoreIncrease = 1;
    public GameObject measureObject;

    public Text xValue;
    public Text yValue;

    private Vector2 objectPos;
    private GameObject building;
    private Vector2 correctPos;
    private GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        objectPos = new Vector2(Random.Range(minPoint.x, maxPoint.x), Random.Range(minPoint.y, maxPoint.y));
        building = Instantiate(measureObject, objectPos, Quaternion.Euler(0, 0, Random.Range(-randomAngle, randomAngle)));

        correctPos = GameObject.FindGameObjectWithTag("MeasurePoint").transform.position;
        Debug.Log(correctPos);
    }

    
    public void ChangeBuilding()
    {
        ChangeTransform(building);
    }

    public void ChangeTransform( GameObject obj)
    {
        objectPos = new Vector2(Random.Range(minPoint.x, maxPoint.x), Random.Range(minPoint.y, maxPoint.y));
        obj.transform.SetPositionAndRotation(objectPos, Quaternion.Euler(0, 0, Random.Range(-randomAngle, randomAngle)));
        correctPos = GameObject.FindGameObjectWithTag("MeasurePoint").transform.position;
        Debug.Log(correctPos);
    }

    public bool CorrectLocationX (string answer)
    {
        float answerNr = float.Parse(answer);
        Debug.Log(Mathf.Abs(correctPos.x - answerNr) < gm.errorMargin);
        if (Mathf.Abs(correctPos.x - answerNr) < gm.errorMargin)
        {
            return true; 
        }
        return false; 
    }
    public bool CorrectLocationY(string answer)
    {
        float answerNr = float.Parse(answer);
        Debug.Log(Mathf.Abs(correctPos.y - answerNr) < gm.errorMargin);
        if (Mathf.Abs(correctPos.y - answerNr) < gm.errorMargin)
        {

            return true;
        }
        return false;
    }

    public void CheckAnswer()
    {
        if(CorrectLocationX(xValue.text) && CorrectLocationY(yValue.text))
        {
            gm.IncreaseScore(scoreIncrease);
            Debug.Log("true");
            ChangeBuilding();
        }
        else Debug.Log("false");

    }

}
