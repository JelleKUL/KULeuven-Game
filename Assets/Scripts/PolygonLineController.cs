﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//*********** The PolygonLineController manages the Top down lines and displays all the measuring information ******************//

public class PolygonLineController : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject linePoint;
    public LayerMask pointMask;
    public LayerMask Obstacles;
    public Slider distanceErrorSlider;
    public Slider angleErrorSlider;

    [Header("Changeable Parameters")]
    public bool lockDistanceError;
    public bool lockAngleError;
    [Tooltip ("the measure error of distance")]
    [Range(0, 100)]
    public float distanceError;
    [Tooltip("the measure error of the angle")]
    [Range(0, 100)]
    public float angleError;
    [Tooltip ("Should the first point be locked in place an where?")]
    public bool lockFirstPoint;
    public Vector2 firstPointPosition;
    public bool showEllipses;
    public bool showAngles;
    public bool showLengths;
    public bool showStartAngle;
    public bool showStartLength;
    public int maxPoints;

    
    private List<GameObject> linePoints = new List<GameObject>();
    private bool holdingObject;
    private GameObject hitObject;
    private Vector2 obstacleHitPoint;

    private GameManager gm;
    private LineRenderer line;


    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        line = GetComponent<LineRenderer>();

        if (lockFirstPoint)
        {
            AddPoint(firstPointPosition);
            linePoints[0].GetComponent<CircleCollider2D>().enabled = false;
        }
        if(distanceErrorSlider != null && angleErrorSlider != null)
        {
            distanceErrorSlider.value = distanceError;
            angleErrorSlider.value = angleError;

            if (lockAngleError) angleErrorSlider.interactable = false;
            else angleErrorSlider.interactable = true;
            if (lockDistanceError) distanceErrorSlider.interactable = false;
            else distanceErrorSlider.interactable = true;
        }
        
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
            if (!holdingObject)
            {
                hitObject = CastMouseRay();

                if (!holdingObject && linePoints.Count < maxPoints)
                {
                    AddPoint(gm.SetObjectToMouse(Input.mousePosition, 0));
                }

            }
            else
            {
                hitObject.transform.position = gm.SetObjectToMouse(Input.mousePosition, 0);

            }

        }
        else holdingObject = false;


    }

    //returns the gamobject the mouse has hit
    public GameObject CastMouseRay()
    {
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), 100, pointMask);

        if (rayHit.collider != null)
        {
            holdingObject = true;
            Debug.Log(rayHit.transform.gameObject.name);
            return rayHit.transform.gameObject;

        }
        holdingObject = false;
        return null;
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
            linePoints[0].GetComponent<PolygonPointController>().SetAngleText(linePoints[0].transform.position + Vector3.up, linePoints[1].transform.position);
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
        RaycastHit2D hit = Physics2D.Raycast(point.transform.position, nextPoint.transform.position - point.transform.position, (nextPoint.transform.position - point.transform.position).magnitude, Obstacles);

        if (hit.collider != null)
        {
            obstacleHitPoint = hit.point;
            return false;
        }
        return true;
    }

    //sets the parameters to a specific value so it matches the question
    public void SetVisibles(bool lock1stPoint, bool ellipses, bool angles, bool lengths, bool startAngle, bool startLength, int nrPoints)
    {
        lockFirstPoint = lock1stPoint;
        showEllipses = ellipses;
        showLengths = lengths;
        showAngles = angles;
        showStartAngle = startAngle;
        showStartLength = startLength;
        maxPoints = nrPoints;
    }

    //returs the mapangle between two points
    public float GetMapAngle(Vector2 endPoint, Vector2 startPoint)
    {
        float angle = Vector2.SignedAngle(endPoint, startPoint);

        return (Mathf.Round(angle / 360 * 400 * 100) / 100f);
    }


    //adds a new point
    public void AddPoint(Vector2 pos)
    {
        line.positionCount++;
        GameObject newPoint = Instantiate(linePoint, pos, Quaternion.identity);
        newPoint.GetComponent<PolygonPointController>().SetNameNrText(line.positionCount);
        linePoints.Add(newPoint);
    }

    //removes the last point
    public void RemovePoint()
    {
        if (linePoints.Count > 1)
        {
            line.positionCount--;
            GameObject removed = linePoints[linePoints.Count - 1];
            linePoints.RemoveAt(linePoints.Count - 1);

            Destroy(removed);
        }

    }

    public void SetAngleError(float value)
    {
        angleError = value;
    }
    public void SetDistanceError(float value)
    {
        distanceError = value;
    }

}
