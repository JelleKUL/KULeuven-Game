using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//*********** The ObjectPlacer manages the placement of random objects such as the to-be-calculated points and the obstacles ******************//


public class ObjectPlacer : BaseController
{
    [Header("Point Parameters")]
    [SerializeField]
    [Tooltip("Is the target point an obstructed point?")]
    private bool targetPointsAreObstructed = false;
    [Range(0,10)]
    [SerializeField] private int nrOfCalculatePoints = 0;
    [SerializeField]
    [Tooltip("are the other points obstructed?")]
    private bool otherPointsAreObstructed = true;
    [Range(0, 10)]
    [SerializeField] private int nrOfOtherPoints = 0;
    [Space(10)]
    [Tooltip("Should the points be spawned in a loop? if yes, the above sliders are ignored")]
    [SerializeField] private bool placeLoopedPoints = false;
    [SerializeField] private float loopedPointsEdgeLength = 1f;
    [SerializeField] private Vector2 loopedStartPos = Vector2.zero;
    [SerializeField] private float loopedMaxDistanceError = 0.001f;

    [Header("Obstacle Parameters")]
    [SerializeField] private bool PlaceObstacleBtwnPoints = false;
    [Range(0,10)]
    [SerializeField] private int nrOfObstacles = 0;
    [Range(0, 10)]
    [SerializeField] private int nrOfMobileObstacles = 0;
    [Tooltip("The max amount of rotation an obstacle can have")]
    [Range(0,180)]
    [SerializeField]private float maxRandomAngle = 0;

    [Header("Randomized Constrains")]
    [SerializeField] private float minDistanceBtwPoints = 1;
    [SerializeField] private float minDistanceToObstacles = 1;
    [Tooltip("The minimum distance each object should have from the origin")]
    [SerializeField] private float minDistanceFromOrigin = 1;
    [Tooltip("The minimum offset each point should be away from the edge of the playfield")]
    [SerializeField] private Vector2 minOffsetFromEdge = Vector2.one;
    [Tooltip("The maximum offset each point should be away from the edge of the playfield")]
    [SerializeField] private Vector2 maxOffsetFromEdge = Vector2.one;
    [SerializeField] private int maxItterations = 1000;


    [System.Serializable]
    private class Prefabs
    {
        public GameObject calculatePointPrefab;
        public GameObject obstructedCalculatePointPrefab;
        public GameObject[] obstaclePrefabs;
        public GameObject[] mobileObstaclePrefabs;
    }
    [Header("Prefabs")]
    [SerializeField]
    private Prefabs prefabs;

    //public variables for other scripts
    [HideInInspector]
    public List <float> calculatePointsPositions;
    [HideInInspector]
    public List <GameObject> calculatePoints = new List <GameObject>();
    [HideInInspector]
    public List<GameObject> otherPoints = new List<GameObject>();
    [HideInInspector]
    public List<float> otherPointsPositions = new List<float>();

    //private objects
    private List<GameObject> obstacles = new List<GameObject>();
    private int nrofPointsPlaced = 0;
    private GameManager gm;



    //starts the setup proces where points and obstacles are placed according to the parameters
    public override void StartSetup()
    {
        base.StartSetup();

        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        if (!placeLoopedPoints)
        {
            if(nrOfCalculatePoints > 0) calculatePointsPositions = PlacePoints(nrOfCalculatePoints, targetPointsAreObstructed? true : false, true);
            if(nrOfOtherPoints > 0) otherPointsPositions = PlacePoints(nrOfOtherPoints, otherPointsAreObstructed? true : false, false);
        }
        else
        {
            calculatePointsPositions = PlaceLoopedPoints(loopedPointsEdgeLength);
        }
        if (PlaceObstacleBtwnPoints) PlaceObstacleBtwn();
        if (nrOfObstacles > 0) PlaceObstacles(nrOfObstacles);
        if (nrOfMobileObstacles > 0) PlaceObstacles(nrOfMobileObstacles, true);
    }


    /// <summary>
    // generates a set amount of random points, making sure there is no overlap between any point or obstacle
    // and returns all the coordinates in an array so it can be evaluated by the questions
    /// </summary>
    /// <param name="amount">the amount of points to place</param>
    /// <param name="obstructed">are the points that need to be placed reachable by the measurestation</param>
    /// <param name="calculate">are the points asked or, extra info</param>
    /// <returns> a list of floats in {x,y,x,y,...}</returns>
    private List <float> PlacePoints(int amount, bool obstructed = false, bool calculate = true)
    {
        List <float> positions = new List <float>();



        for (int i = 0; i < amount; i++)
        {
            GameObject newPoint = Instantiate(obstructed? prefabs.obstructedCalculatePointPrefab:prefabs.calculatePointPrefab, FarEnoughRandomPoint(), Quaternion.identity);
            if (calculate) calculatePoints.Add(newPoint);
            else otherPoints.Add(newPoint);

            newPoint.GetComponent<PolygonPointController>().SetNameText(nrofPointsPlaced);
            nrofPointsPlaced++;

            positions.Add(newPoint.transform.position.x);
            positions.Add(newPoint.transform.position.y);
        }

        return positions;
    }

    //places the calculate points in a looped position
    private List <float> PlaceLoopedPoints(float edgeLength)
    {
        int[,] angleArray = new int[,] {    { 90, 90, 180, 90, 90,180 }, { 90, 90, 225, 45, 135, 135 }, { 45, 135, 180, 45, 135, 180 }, { 90, 90, 135, 135,45, 225 },
                                            { 135, 135, 90, 135, 135, 90 }, { 90, 135, 135, 90, 135, 135 }, { 135, 90, 135, 135, 90, 135 } }; //{ 135,45, 180, 135,45, 180 },
        /*
        int totalAngle = 180 * (amount - 3); // substract one extra point for the first external point to compare
        int amountOfQuarters = 4 * (amount - 3);
        int[] angles = new int[amount - 1]; // also substract the first point
        */
        int shape = Random.Range(0, angleArray.GetLength(1));
        Debug.Log("ShapeNr: " + shape);

        List <float> positions = new List <float>();
        for (int i = 0; i < 7; i++)
        {
            Vector2 position = Vector2.zero;
            if (i == 0) //place the first point out of the loop at the start position
            {
                position = loopedStartPos - Vector2.one.normalized * edgeLength;
            }
            else if(i == 1) //place the first point 45 deg away to start the loop
            {
                position = loopedStartPos;
            }
            else if (i == 2) //place the first point 45 deg away to start the loop
            {
                position = calculatePoints[i - 1].transform.position + Vector3.right * edgeLength;
            }
            else if(i > 2 && i < 7)
            {
                position = calculatePoints[i - 2].transform.position;
            }
            else
            {
                position = calculatePoints[i-1].transform.position;
            }

            GameObject newPoint = Instantiate(prefabs.calculatePointPrefab, position, Quaternion.identity);
            if(i> 2)
            {
                newPoint.transform.RotateAround(calculatePoints[i - 1].transform.position, Vector3.back, angleArray[shape, i-1]);
            }

            if(i >1) newPoint.transform.position += new Vector3(Random.Range(-1f,1f) * loopedMaxDistanceError, Random.Range(-1f, 1f) * loopedMaxDistanceError, 0);
            calculatePoints.Add(newPoint);
            newPoint.GetComponent<PolygonPointController>().SetNameText( i==0? 0 : nrofPointsPlaced);
            nrofPointsPlaced++;

            //positions[i * 2] = newPoint.transform.position.x;
            //positions[(i * 2) + 1] = newPoint.transform.position.y;
            positions.Add(newPoint.transform.position.x);
            positions.Add(newPoint.transform.position.y);
        }

        return positions;
    }

    // Place a set amount of random obstacles, mobile= wether to place te mobile obstacles 
    private void PlaceObstacles(int amount, bool mobile = false)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject newObstacle = Instantiate(
                mobile? prefabs.mobileObstaclePrefabs[Random.Range(0, prefabs.mobileObstaclePrefabs.Length)]: prefabs.obstaclePrefabs[Random.Range(0, prefabs.obstaclePrefabs.Length)],
                FarEnoughRandomPoint(),
                RandomAngle()
            );
            obstacles.Add(newObstacle);
        }
    }

    // place an obstacle in between the calculate points
    private void PlaceObstacleBtwn()
    {
        for (int i = 1; i < calculatePoints.Count; i++)
        {
            Vector3 obsPosition;
            obsPosition = (calculatePoints[i - 1].transform.position + calculatePoints[i].transform.position) / 2f;
            GameObject newObstacle = Instantiate(prefabs.obstaclePrefabs[Random.Range(0, prefabs.obstaclePrefabs.Length)], obsPosition, RandomAngle());
            obstacles.Add(newObstacle);
        }
    }

    //returns a random position that is far enough from all the other obstacles and points
    private Vector2 FarEnoughRandomPoint()
    {
        float minDistPoints = Mathf.Infinity;
        float minDistObstacles = Mathf.Infinity;
        float minDistObstructed = Mathf.Infinity;
        bool repeat = true;

        Vector2 randPos;
        int maxRepeat = 0;
        do
        {
            randPos = new Vector2(Random.Range(gm.screenMin.x + minOffsetFromEdge.x, gm.screenMax.x - maxOffsetFromEdge.x) , Random.Range(gm.screenMin.y + minOffsetFromEdge.y, gm.screenMax.y - maxOffsetFromEdge.y));
            minDistPoints = Mathf.Infinity;
            minDistObstacles = Mathf.Infinity;
            minDistObstructed = Mathf.Infinity;

            maxRepeat++;

            if (Vector2.Distance(randPos, Vector2.zero) < minDistanceFromOrigin)
            {
                minDistPoints = 0f;
            }
            else
            {
                for (int i = 0; i < calculatePoints.Count; i++)
                {
                    if (Vector2.Distance(randPos, calculatePoints[i].transform.position) < minDistPoints)
                    {
                        minDistPoints = Vector2.Distance(randPos, calculatePoints[i].transform.position);

                    }
                }
            }

            if (otherPoints.Count > 0)
            {
                for (int i = 0; i < otherPoints.Count; i++)
                {
                    if (Vector2.Distance(randPos, otherPoints[i].transform.position) < minDistObstructed)
                    {
                        minDistObstructed = Vector2.Distance(randPos, otherPoints[i].transform.position);
                    }
                }
            }

            if (obstacles.Count > 0)
            {
                for (int i = 0; i < obstacles.Count; i++)
                {
                    if (Vector2.Distance(randPos, obstacles[i].transform.position) < minDistObstacles)
                    {
                        minDistObstacles = Vector2.Distance(randPos, obstacles[i].transform.position);
                    }
                }
            }

            if ((minDistPoints >= minDistanceBtwPoints) && (minDistObstructed >= minDistanceBtwPoints) && (minDistObstacles >= minDistanceToObstacles))
            {
                repeat = false;
            }

        } while (repeat && maxRepeat < maxItterations);

        return randPos;
    }

    //returns a random rotationvalue
    private Quaternion RandomAngle()    
    {
        return Quaternion.Euler(0, 0, Random.Range(-maxRandomAngle, maxRandomAngle));
    }

    //returns a Vector2 array of the calculate points
    public Vector2[] GetCoordinates()
    {
        Vector2[] coordinates = new Vector2[calculatePoints.Count];

        for (int i = 0; i < calculatePoints.Count; i++)
        {
            coordinates[i] = calculatePoints[i].transform.position;
        }

        return coordinates;
    }

    //set the desired points and obstacles for the placer
    public void SetParameters(int nrPoints, int nrOtherPoints, bool loop, int nrObs, int nrmobileObs)
    {
        nrOfCalculatePoints = nrPoints;
        nrOfOtherPoints = nrOtherPoints;
        placeLoopedPoints = loop;
        nrOfObstacles = nrObs;
        nrOfMobileObstacles = nrmobileObs;
        StartSetup();
    }
}
