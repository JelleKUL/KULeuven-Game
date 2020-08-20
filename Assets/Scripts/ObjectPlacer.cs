using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//*********** The ObjectPlacer manages the placement of random objects such as the to-be-calculated points and the obstacles ******************//


public class ObjectPlacer : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject calculatePoint;
    public GameObject obstructedCalculatePoint;
    public GameObject[] obstaclePrefabs;
    public GameObject[] mobileObstaclePrefabs;
    public GameManager gm;

    [Header("Randomized Constrains")]
    public bool placeBtwnPoints;
    public float minDistanceBtwPoints = 2;
    public Vector2 minOffset;
    public Vector2 maxOffset;
    public float maxRandomAngle;
    


    [HideInInspector]
    public List <GameObject> calculatePoints = new List <GameObject>();
    public List<GameObject> obstructedCalculatePoints = new List<GameObject>();
    private List<GameObject> obstacles = new List<GameObject>();
    private int nrofPointsPlaced = 0;

    
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
            newPoint.GetComponent<PolygonPointController>().SetNameText(nrofPointsPlaced);
            nrofPointsPlaced++;

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
    // places obstructed points
    public float[] PlaceObstructedCalculatePoints(int amount)
    {
        if (obstructedCalculatePoints.Count > 0)
        {
            return (ChangeCalculatePoints());
        }
        float[] positions = new float[amount * 2];
        for (int i = 0; i < amount; i++)
        {
            GameObject newPoint = Instantiate(obstructedCalculatePoint, FarEnoughRandomPoint(), Quaternion.identity);
            obstructedCalculatePoints.Add(newPoint);
            newPoint.GetComponent<PolygonPointController>().SetNameText(nrofPointsPlaced);
            nrofPointsPlaced++;

            positions[i * 2] = newPoint.transform.position.x;
            positions[(i * 2) + 1] = newPoint.transform.position.y;
        }

        return positions;
    }

    // changes the position of the points and returns the updated value
    public float[] ChangeObstructedCalculatePoints()
    {
        float[] positions = new float[obstructedCalculatePoints.Count * 2];
        for (int i = 0; i < obstructedCalculatePoints.Count; i++)
        {
            obstructedCalculatePoints[i].transform.position = Vector2.zero;
        }

        for (int i = 0; i < obstructedCalculatePoints.Count; i++)
        {
            obstructedCalculatePoints[i].transform.position = FarEnoughRandomPoint();
            positions[i * 2] = obstructedCalculatePoints[i].transform.position.x;
            positions[(i * 2) + 1] = obstructedCalculatePoints[i].transform.position.y;
        }

        return positions;
    }

    //places the calculate points in a looped position
    public float[] placeLoopedPoints(float edgeLength)
    {
        int[,] angleArray = new int[,] {    { 90, 90, 180, 90, 90,180 }, { 90, 90, 225, 45, 135, 135 }, { 45, 135, 180, 45, 135, 180 }, { 90, 90, 135, 135,45, 225 },
                                            { 135,45, 180, 135,45, 180 }, { 135, 135, 90, 135, 135, 90 }, { 90, 135, 135, 90, 135, 135 }, { 135, 90, 135, 135, 90, 135 } };
        /*
        int totalAngle = 180 * (amount - 3); // substract one extra point for the first external point to compare
        int amountOfQuarters = 4 * (amount - 3);
        int[] angles = new int[amount - 1]; // also substract the first point
        */
        int shape = Random.Range(0, 8);
        Debug.Log(shape);
        Vector2 centerPoint = (gm.screenMax + gm.screenMin)/2 + Vector2.down + Vector2.left;

        float[] positions = new float[7 * 2];
        for (int i = 0; i < 7; i++)
        {
            Vector2 position = Vector2.zero;
            if (i == 0)
            {
                position = centerPoint;
            }
            else if(i == 1)
            {
                position = centerPoint + Vector2.right * edgeLength;
            }
            else if(i > 1 && i < 6)
            {
                position = calculatePoints[i - 2].transform.position;
            }
            else
            {
                position = calculatePoints[i-1].transform.position;
            }

            GameObject newPoint = Instantiate(calculatePoint, position, Quaternion.identity);
            if(i> 1 && i< 6)
            {
                newPoint.transform.RotateAround(calculatePoints[i - 1].transform.position, Vector3.back, angleArray[shape, i-1]);
            }
            else if (i == 6)
            {
                newPoint.transform.RotateAround(calculatePoints[0].transform.position, Vector3.forward, Random.Range(1,5) * 45);
            }
            calculatePoints.Add(newPoint);
            newPoint.GetComponent<PolygonPointController>().SetNameText( i==6? 0 : nrofPointsPlaced+1);
            nrofPointsPlaced++;

            positions[i * 2] = newPoint.transform.position.x;
            positions[(i * 2) + 1] = newPoint.transform.position.y;
        }

        return positions;
    }

    public Vector2[] GetCoordinates()
    {
        Vector2[] coordinates = new Vector2[calculatePoints.Count];

        for (int i = 0; i < calculatePoints.Count; i++)
        {
            coordinates[i] = calculatePoints[i].transform.position;
        }

        return coordinates;
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
                GameObject newObstacle = Instantiate(obstaclePrefabs[Random.Range(0,obstaclePrefabs.Length)], FarEnoughRandomPoint(), RandomAngle());
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
            for (int i = 0; i < obstructedCalculatePoints.Count; i++)
            {
                if (Vector2.Distance(randPos, obstructedCalculatePoints[i].transform.position) < minDist)
                {
                    minDist = Vector2.Distance(randPos, obstructedCalculatePoints[i].transform.position);
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
            GameObject newObstacle = Instantiate(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)], obsPosition, RandomAngle());
            obstacles.Add(newObstacle);
        }
        
    }

    

}
