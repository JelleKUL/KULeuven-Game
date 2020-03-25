using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectPlacer : MonoBehaviour
{
    
    
    //[Header ("Predefined Points")]
    //public Vector3[] pointLocationAndRotation;

    [Header("Randomized Constrains")]
    //public bool randomizePoint;
    //public int nrRandomizedPoints;
    public float minDistanceBtwPoints = 2;
    public Vector2 minOffset;
    public Vector2 maxOffset;
    public float maxRandomAngle;

    [Header ("Prefabs")]
    public GameObject calculatePoint;
    public GameObject obstacle;
    public GameManager gm;

    [HideInInspector]
    public List <GameObject> calculatePoints = new List <GameObject>();
    private List<GameObject> obstacles = new List<GameObject>();

    

    public float[] PlaceCalculatePoints(int amount)
    {
        float[] positions = new float[amount * 2];
        for (int i = 0; i < amount; i++)
        {
            GameObject newPoint = Instantiate(calculatePoint, FarEnoughRandomPoint(), Quaternion.identity);
            calculatePoints.Add(newPoint);
            newPoint.GetComponent<PolygonPointController>().SetNameText(i);

            positions[i * 2] = newPoint.transform.position.x;
            positions[(i * 2) + 1] = newPoint.transform.position.y;
        }

        return positions;
    }

    public float[] ChangeCalculatePoints()
    {
        float[] positions = new float[calculatePoints.Count * 2];
        for (int i = 0; i < calculatePoints.Count; i++)
        {
            calculatePoints[i].transform.position = Vector2.zero;
        }

        for (int i = 0; i < calculatePoints.Count; i++)
        {
            calculatePoints[i].transform.position = FarEnoughRandomPoint();
            positions[i * 2] = calculatePoints[i].transform.position.x;
            positions[(i * 2) + 1] = calculatePoints[i].transform.position.y;
        }

        return positions;
    }

    public void PlaceObstacles (int amount)
    {
        
        for (int i = 0; i < amount; i++)
        {
            GameObject newObstacle = Instantiate(obstacle, FarEnoughRandomPoint(), RandomAngle());
            obstacles.Add(newObstacle);
        }
    }

    public void ChangeObstacles()
    {
        for (int i = 0; i < obstacles.Count; i++)
        {
            obstacles[i].transform.position = Vector2.zero;
        }

        for (int i = 0; i < obstacles.Count; i++)
        {
            obstacles[i].transform.SetPositionAndRotation(FarEnoughRandomPoint(), RandomAngle());
        }
    }


    //returns a random position that is far enugh from all the others
    public Vector2 FarEnoughRandomPoint()
    {
        float minDist = Mathf.Infinity;
        Vector2 randPos;
        do
        {
            randPos = new Vector2(Random.Range(gm.screenMin.x + minOffset.x, gm.screenMax.x - maxOffset.x), Random.Range(gm.screenMin.y + minOffset.y, gm.screenMax.y - maxOffset.y));
            minDist = Mathf.Infinity;

            for (int i = 0; i < calculatePoints.Count; i++)
            {
                if (Vector2.Distance(randPos, calculatePoints[i].transform.position) < minDist)
                {
                    minDist = Vector2.Distance(randPos, calculatePoints[i].transform.position);
                }
            }
            for (int i = 0; i < obstacles.Count; i++)
            {
                if (Vector2.Distance(randPos, calculatePoints[i].transform.position) < minDist)
                {
                    minDist = Vector2.Distance(randPos, calculatePoints[i].transform.position);
                }
            }
        } while (minDist < minDistanceBtwPoints);
        
        return randPos;
    }

    //returns a random rotationvalue
    public Quaternion RandomAngle()
    {
        return Quaternion.Euler(0, 0, Random.Range(-maxRandomAngle, maxRandomAngle));
    } 

    

}
