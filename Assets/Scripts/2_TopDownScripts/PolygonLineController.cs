﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

//*********** The PolygonLineController manages the Top down lines and displays all the measuring information ******************//

public class PolygonLineController : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject linePoint;
    public GameObject firstPoint;
    public LayerMask pointMask;
    public LayerMask Obstacles;
    public LayerMask ObstructionPoints;

    public Slider distanceError1Slider;
    public Slider distanceError2Slider;
    public Slider angleErrorSlider;

    public Text errorEllipsDisplay;
    public Material fullLine;
    public Material dottedLine;

    [Header("Changeable Parameters")]
    public int nrOfPoints;
    //public bool randomizeErrors;
    public bool lockDistanceError1;
    public bool lockDistanceError2;
    public bool lockAngleError;

    [Tooltip("the base measure error of distance")]
    [Range(0, 5)]
    public float distanceError1;
    [Tooltip("the ppm measure error of distance")]
    [Range(0, 5)]
    public float distanceError2;
    [Tooltip("the measure error of the angle")]
    [Range(0, 5)]
    public float angleError;
    [Tooltip("Should the first point be locked in place and where?")]
    public bool lockFirstPoint;
    public Vector2 firstPointPosition;
    public bool showEllipses;
    public bool showAngles;
    public bool showLengths;
    public bool showStartAngle;
    public bool showStartLength;
    public int maxPoints;
    public bool startCenterPoint;

    [HideInInspector]
    public float biggestEllips;
    [HideInInspector]
    public float ellipsX;
    [HideInInspector]
    public float ellipsY;



    private List<GameObject> linePoints = new List<GameObject>();
    private bool holdingObject;
    private GameObject hitObject;
    private Vector2 obstacleHitPoint;
    //private bool hasStarted;
    //private float[] correctAnswerArray;


    private GameManager gm;
    private LineRenderer line;
    private ObjectPlacer placer;

    //private PolygonPointController thisPoint;
    // Start is called before the first frame update
    //private void Start()
    //{
    //    if (!hasStarted) StartSetup(); //only calls when Start() hasn't gone yet
    //}

    // the startscript, can be called by the setparametersfunction to get the correct answers before the start function is called in this script
    void Start()
    {
        //hasStarted = true;

        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        line = GetComponent<LineRenderer>();
        
        // sets the type of line renderer
        if (showLengths || showStartLength)
        {
            line.material = fullLine;
        }
        else line.material = dottedLine;

        // places the first point 
        if (lockFirstPoint)
        {
            AddPoint(firstPointPosition);
            linePoints[0].GetComponent<CircleCollider2D>().enabled = false;
        }
        // initializes a random value
        //if (randomizeErrors)
        //{
        //    //distanceError1 = Mathf.Round(UnityEngine.Random.Range(1f, 5f)); // this change has no effect
        //    SetDistanceError1(Mathf.Round(UnityEngine.Random.Range(1f, 5f)));
        //    //distanceError2 = Mathf.Round(UnityEngine.Random.Range(1f, 5f));
        //    SetDistanceError2(Mathf.Round(UnityEngine.Random.Range(1f, 5f)));
        //    //angleError = Mathf.Round(UnityEngine.Random.Range(1f, 5f));
        //    SetAngleError(Mathf.Round(UnityEngine.Random.Range(1f, 5f)));
        //}
        distanceError1Slider.value = distanceError1;
        distanceError2Slider.value = distanceError2;
        angleErrorSlider.value = angleError;

        if (lockAngleError) angleErrorSlider.interactable = false;
        else angleErrorSlider.interactable = true;
        if (lockDistanceError1) distanceError1Slider.interactable = false;
        else distanceError1Slider.interactable = true;
        if (lockDistanceError2) distanceError2Slider.interactable = false;
        else distanceError2Slider.interactable = true;



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

                if (!holdingObject && linePoints.Count < maxPoints && Input.GetMouseButtonDown(0))
                {
                    Debug.Log("adding point");
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
                PolygonPointController thisPoint = linePoints[i].GetComponent<PolygonPointController>();
                thisPoint.SetErrorEllips(linePoints[i - 1].transform.position, prevEllipse.x, prevEllipse.y, prevEllipse.z, distanceError1 * 50f, angleError * 50f); // multiplied by 50 to increase visual size


                if (i == linePoints.Count - 1)
                {
                    Vector3 ellips = thisPoint.GetEllipseInfo(); // the ellips will be 50 times to large !
                    //biggestEllips = ellips.x * ellips.y * Mathf.PI / 4f;
                    biggestEllips = Mathf.Round((Mathf.Max(ellips.x, ellips.y) / 5 * 100f) / 100f);
                    biggestEllips = Mathf.Max(ellips.x, ellips.y) / 5;

                    ellipsX = Mathf.Round((ellips.x / 5 * 100f) / 100f);
                    ellipsY = Mathf.Round((ellips.y / 5 * 100f) / 100f);

                    if (errorEllipsDisplay)
                    {
                        errorEllipsDisplay.text = biggestEllips.ToString(); //(Mathf.Round(biggestEllips * 100f) / 100f).ToString()
                    }
                }

            }

            //checks if the line intersects with an obstacle
            if (i != linePoints.Count - 1 && !CheckVisible(linePoints[i], linePoints[i + 1], Obstacles))
            {

                line.positionCount = i + 2;
                line.SetPosition(i + 1, obstacleHitPoint);

                break;

            }
            //sets the anglevalues of the points
            if (showAngles && i != 0 && i != linePoints.Count - 1)
            {
                if (!CheckVisible(linePoints[1], linePoints[1], ObstructionPoints))
                {

                }
                else linePoints[i].GetComponent<PolygonPointController>().SetAngleText(linePoints[i - 1].transform.position, linePoints[i + 1].transform.position);
            }

            line.positionCount = linePoints.Count;

        }
    }


    // checks if next point is visible
    bool CheckVisible(GameObject point, GameObject nextPoint, LayerMask layerMask)
    {
        RaycastHit2D hit = Physics2D.Raycast(point.transform.position, nextPoint.transform.position - point.transform.position, (nextPoint.transform.position - point.transform.position).magnitude, layerMask);

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
        //correctAnswerArray = placer.PlaceCalculatePoints(1);
        SetDistanceError1(Mathf.Round(UnityEngine.Random.Range(1f, 5f)));
        SetDistanceError2(Mathf.Round(UnityEngine.Random.Range(1f, 5f)));
        SetAngleError(Mathf.Round(UnityEngine.Random.Range(1f, 5f)));


        lockFirstPoint = lock1stPoint;
        showEllipses = ellipses;
        showLengths = lengths;
        showAngles = angles;
        showStartAngle = startAngle;
        showStartLength = startLength;
        maxPoints = nrPoints;

        //StartSetup();
    }

    //returs the mapangle between two points
    public float GetMapAngle(Vector2 endPoint, Vector2 startPoint)
    {
        float angle = Vector2.SignedAngle(endPoint, startPoint);
        angle = Mathf.Round(Mathf.Abs(angle) / 360 * 400 * 1000) / 1000f + 200f;
        return angle;
    }


    //adds a new point
    public void AddPoint(Vector2 pos)
    {
        line.positionCount++;
        if (linePoints.Count == 0)
        {
            GameObject newPoint = Instantiate(firstPoint, pos, Quaternion.identity);
            newPoint.GetComponent<PolygonPointController>().SetNameNrText(line.positionCount);
            linePoints.Add(newPoint);
        }

        else if (startCenterPoint && line.positionCount == 2)
        {
            GameObject newPoint = Instantiate(linePoint, pos, Quaternion.identity);
            newPoint.GetComponent<PolygonPointController>().SetNameNrText(line.positionCount);
            linePoints.Insert(0, newPoint);
        }
        else
        {
            GameObject newPoint = Instantiate(linePoint, pos, Quaternion.identity);
            newPoint.GetComponent<PolygonPointController>().SetNameNrText(line.positionCount);
            linePoints.Add(newPoint);
        }

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
    public void SetPoints(float[] positions)
    {
        line = GetComponent<LineRenderer>();
        maxPoints = positions.Length / 2;
        for (int i = 0; i < maxPoints - 1; i++)
        {
            AddPoint(new Vector2(positions[i * 2], positions[i * 2 + 1]));
        }
        AddPoint(new Vector2(positions[0], positions[1]));
        AddPoint(new Vector2(positions[(maxPoints - 1) * 2], positions[(maxPoints - 1) * 2 + 1]));

    }

    public float GetErrorH(float[] correctAnswerArray) // compute largest axis of ellips
    {

        Vector2 startPoint = new Vector2(0f, 0f);
        Vector2 pointP = new Vector2(correctAnswerArray[0], correctAnswerArray[1]);

        float d = Vector2.Distance(pointP, startPoint) * GameManager.worldScale;
        float sigmaD = distanceError1 + (0.001f * d * distanceError2); // correctie m->mm
        float sigmaH = d * Mathf.Tan(AngleToRad(angleError));// correctie m->mm

        return GameManager.RoundFloat(Mathf.Max(sigmaD, sigmaH), 1);
   
    }

    public float GetErrorA(float[] correctAnswerArray) // compute sigmaA
    {

        Vector2 startPoint = new Vector2(0f, 0f);
        Vector2 pointP = new Vector2(correctAnswerArray[0], correctAnswerArray[1]);
        float angle = Vector2.SignedAngle(pointP, startPoint);

        float d = Vector2.Distance(pointP, startPoint) * GameManager.worldScale;
        float sigmaD = distanceError1 + (0.001f * d * distanceError2); // correctie m->mm
        float sigmaH = 1.5f * d * angleError / 100f ;// correctie m->mm
        float sigmaA = Mathf.Sqrt(Mathf.Pow(sigmaD, 2) + Mathf.Pow(sigmaH, 2));

        return GameManager.RoundFloat(sigmaA, 1);
        
    }

    public float AngleToRad(float value)
    {
        return value * 2 * Mathf.PI / 400f ;
    }

    public void SetAngleError(float value)
    {
        angleError = value;
    }
    public void SetDistanceError1(float value)
    {
        distanceError1 = value;
    }
    public void SetDistanceError2(float value)
    {
        distanceError2 = value;
    }
    public void CreateShortestPath()
    {

    }

    public bool LastPointSnapped()
    {
        return linePoints[linePoints.Count - 1].GetComponent<PolygonPointController>().IsSnapped;

    }
}



