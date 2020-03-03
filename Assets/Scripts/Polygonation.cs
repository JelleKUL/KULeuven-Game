using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Polygonation : MonoBehaviour
{
    public Vector2 minPoint;
    public Vector2 maxPoint;
    public float randomAngle;
    public int scoreIncrease = 1;
    public GameObject obj1;
    public GameObject obj2;
    public GameObject answerLine;

    public Text Value;
    
    public Text answer;

    private Vector2 randomPos;
    private GameObject point1;
    private GameObject point2;
    private LineRenderer lineRend;

    public float correctLength;
    private GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        lineRend = answerLine.GetComponent<LineRenderer>();
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        randomPos = new Vector2(Random.Range(minPoint.x, maxPoint.x), Random.Range(minPoint.y, maxPoint.y));
        point1 = Instantiate(obj1, randomPos, Quaternion.Euler(0, 0, Random.Range(-randomAngle, randomAngle)));
        randomPos = new Vector2(Random.Range(minPoint.x, maxPoint.x), Random.Range(minPoint.y, maxPoint.y));
        point2 = Instantiate(obj2, randomPos, Quaternion.Euler(0, 0, Random.Range(-randomAngle, randomAngle)));

        correctLength = Vector2.Distance(point2.transform.position, point1.transform.position);
        Debug.Log(correctLength);
    }
    

    public void ChangeBuildings() 
    {
        ChangeTransform(point1);
        ChangeTransform(point2);
        correctLength = Vector2.Distance(point2.transform.position, point1.transform.position);
        Debug.Log(correctLength);
    }

    public void ChangeTransform(GameObject obj)
    {
        randomPos = new Vector2(Random.Range(minPoint.x, maxPoint.x), Random.Range(minPoint.y, maxPoint.y));
        obj.transform.SetPositionAndRotation(randomPos, Quaternion.Euler(0, 0, Random.Range(-randomAngle, randomAngle)));
        answerLine.SetActive(false);
    }

    public bool CorrectLocation(string answer)
    {
        return gm.CheckCorrectAnswer(answer, correctLength);
    }
    

    public void CheckAnswer()
    {
        if (CorrectLocation(Value.text))
        {
            gm.IncreaseScore(scoreIncrease);
            Debug.Log("true");
            ChangeBuildings();
        }
        else Debug.Log("false");

    }
    public void ShowAnswer()
    {
        answerLine.SetActive(true);
        
        SetLinePos(point1.transform.position, point2.transform.position);
        answer.text = "distance: " + correctLength.ToString();
    }
    void SetLinePos(Vector2 pS, Vector2 pE)
    {
        lineRend.SetPosition(0, pS);
        lineRend.SetPosition(1, pE);
    }
}
