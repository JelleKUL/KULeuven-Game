using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    [Range(0, 100)]
    public float distanceError;
    [Range(0, 100)]
    public float angleError;
    public GameObject linePoint;
    public bool showEllipses;
    public bool showAngles;
    public bool showLengths;
    public bool showStartAngle;
    public bool showStartLength;
    public int maxPoints;
    

    private List<GameObject> linePoints = new List<GameObject>();
    private bool mouseOnPoint;
    private Vector2 obstacleHitPoint;

    private GameManager gm;
    private LineRenderer line;




    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        line = GetComponent<LineRenderer>();
        
    }

    // Update is called once per frame
    void Update()
    {

        // checks is mousebutton is clicked and sets the point to that position or adds a new point
        SetPointsToMouse();

        //draws the line and sets the values of the points.
        //also stops when it encounters an object.
        DrawLineAndValues();

    }


    //sets the point to the mouse position
    void SetPointsToMouse()
    {
        // checks is mousebutton is clicked and sets the point to that position
        if (Input.GetMouseButton(0) && gm.IsBetweenValues(gm.SetObjectToMouse(Input.mousePosition, 0)))
        {
            for (int i = linePoints.Count-1; i >= 0; i--)
            {
                // if the mouse position is close to a point, the point will move to that position
                if ((linePoints[i].transform.position - gm.SetObjectToMouse(Input.mousePosition, 0)).magnitude < 1f)
                {
                    mouseOnPoint = true;
                    linePoints[i].transform.position = gm.SetObjectToMouse(Input.mousePosition, 0);
                    break;

                }

            }
            // adds new point if mouse clicks away from existing point
            if (mouseOnPoint == false && linePoints.Count < maxPoints)
            {
                AddPoint(gm.SetObjectToMouse(Input.mousePosition, 0));
            }
            mouseOnPoint = false;
        }
    }


    //draws the line and values
    void DrawLineAndValues()
    {
        

        for (int i = 0; i < linePoints.Count; i++)
        {
            linePoints[i].GetComponent<PolygonPointController>().HideInfo();
        }

        if (showStartLength && linePoints.Count > 1)
        {
            linePoints[0].GetComponent<PolygonPointController>().SetDistanceText(linePoints[1].transform.position);
        }
        if (showStartAngle && linePoints.Count > 1)
        {
            linePoints[0].GetComponent<PolygonPointController>().SetAngleText(linePoints[0].transform.position + Vector3.right, linePoints[1].transform.position);
        }

        for (int i = 0; i < linePoints.Count; i++)
        {
            line.SetPosition(i, linePoints[i].transform.position);

            //sets the lengthvalues of the points
            if (showLengths && i != 0)
            {
                linePoints[i].GetComponent<PolygonPointController>().SetDistanceText(linePoints[i - 1].transform.position);
            }
            //sets the errorEllipses
            if (showEllipses && i != 0)
            {
                Vector3 prevEllipse = linePoints[i - 1].GetComponent<PolygonPointController>().GetEllipseInfo();
                //Debug.Log(prevEllipse.z);
                linePoints[i].GetComponent<PolygonPointController>().SetErrorEllips(linePoints[i - 1].transform.position, prevEllipse.x, prevEllipse.y, prevEllipse.z, distanceError, angleError);
            }

            //checks if the line intersects with an obstacle
            if (i != linePoints.Count - 1 && !CheckVisible(linePoints[i], linePoints[i + 1]))
            {
                line.positionCount = i + 2;
                line.SetPosition(line.positionCount - 1, obstacleHitPoint);

                break;
            }
            //sets the anglevalues of the points
            if (showAngles && i != 0 && i != linePoints.Count - 1)
            {
                linePoints[i].GetComponent<PolygonPointController>().SetAngleText(linePoints[i - 1].transform.position, linePoints[i + 1].transform.position);
            }

            line.positionCount = linePoints.Count;

        }
    }


    // checks if next point is visible
    bool CheckVisible(GameObject point, GameObject nextPoint)
    {
        RaycastHit2D hit = Physics2D.Raycast(point.transform.position, nextPoint.transform.position - point.transform.position, (nextPoint.transform.position - point.transform.position).magnitude);

        if (hit.collider != null)
        {
            obstacleHitPoint = hit.point;
            return false;
        }
        return true;
    }


    //adds new point
    public void AddPoint(Vector2 pos)
    {
        line.positionCount++;
        GameObject newPoint = Instantiate(linePoint, pos, Quaternion.identity);
        newPoint.GetComponent<PolygonPointController>().SetNameText(line.positionCount);
        linePoints.Add(newPoint);
    }


}
