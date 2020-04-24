using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//*********** The ObjectPlacer manages the placement of random objects such as the to-be-calculated points and the obstacles ******************//


public class ObjectPlacer : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject calculatePoint;
    public GameObject obstacle;
    public GameManager gm;

    [Header("Randomized Constrains")]
    public bool placeBtwnPoints;
    public float minDistanceBtwPoints = 2;
    public Vector2 minOffset;
    public Vector2 maxOffset;
    public float maxRandomAngle;


    [HideInInspector]
    public List <GameObject> calculatePoints = new List <GameObject>();
    private List<GameObject> obstacles = new List<GameObject>();

    
    // generates a set amount of random points, making sure there is no overlap between any point or obstacle
    // and returns all the coordinates in an array so it can be evaluated by the questions
    public float[] PlaceCalculatePoints(int amount)
    {
        if(calculatePoints.Count > 0)
        {
            return (ChangeCalculatePoints());
        }
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

    // changes the position of the points and returns the updated value
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

    //places a set amount of obstacles keeping in mind the minimum distance
    public void PlaceObstacles (int amount)
    {
        if(obstacles.Count > 0)
        {
            ChangeObstacles();
            
        }
        else
        {
            for (int i = 0; i < amount; i++)
            {
                GameObject newObstacle = Instantiate(obstacle, FarEnoughRandomPoint(), RandomAngle());
                obstacles.Add(newObstacle);
            }
        }
        
    }

    //changes the location of the random obstacles
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


    //returns a random position that is far enough from all the other obstacles and points
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

    public void PlaceObstacleBtwn(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 obsPosition;
            if (i == 0) obsPosition = calculatePoints[0].transform.position / 2f;
            else obsPosition = (calculatePoints[i-1].transform.position + calculatePoints[i].transform.position) / 2f;
            GameObject newObstacle = Instantiate(obstacle, obsPosition, RandomAngle());
            obstacles.Add(newObstacle);
        }
        
    }

    

}
