using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//*********** The PolygonLineController manages the Top down lines and displays all the measuring information ******************//
public enum ErrorType { Base, Ppm, Angle}

[RequireComponent(typeof(LineRenderer))]
public class PolygonLineController : MonoBehaviour
{
    [Header("Point Parameters")]
    [Tooltip("Should the first point be locked in place and where?")]
    [SerializeField] private bool lockFirstPoint;
    [SerializeField] private Vector2 firstPointPosition;
    [SerializeField] private int maxPoints;
    [SerializeField] private bool startCenterPoint;
    [SerializeField] private bool showSmallErrors;
    [Tooltip("if true, uses the angleError to offset each point based on the distance")]
    [SerializeField] private bool showVereffeningsErrors;

    [Header("Error Parameters")]
    [SerializeField] private ErrorClass baseError;
    [SerializeField] private ErrorClass ppmError;
    [SerializeField] private ErrorClass angleError;

    [Header("Visual Parameters")]
    [Range(0, 100)]
    [Tooltip("The visual size modifier of the error ellipses")]
    [SerializeField] private float visualEllipseSizeModifier = 1;
    [SerializeField] private bool showEllipses;
    [SerializeField] private bool showAngles;
    [SerializeField] private bool showLengths;
    [SerializeField] private bool showStartAngle;
    [Tooltip("show the length of only the first point (only relevant if showlengths is off)")]
    [SerializeField] private bool showStartLength;
    [SerializeField] private bool showEndLength = true;

    [System.Serializable]
    private class ErrorClass
    {
        [Tooltip("Can the error be changed at runtime by an other script?")]
        public bool lockError;
        [Tooltip("the base measure error of distance")]
        [Range(0, 5)]
        public float error = 0f;
        [Tooltip("Should the error be randomized, if so, it ignores the above value")]
        public bool randomizeError = true;
        [Tooltip("the min and max value of the random error range")]
        public Vector2 randomErrorRange = Vector2.one;
    }

    [System.Serializable]
    private class Prefabs
    {
        public GameObject linePoint;
        public GameObject firstPoint;
        public Material fullLine;
        public Material dottedLine;
    }
    [System.Serializable]
    private class LayerMasks
    {
        public LayerMask pointMask;
        public LayerMask Obstacles;
        public LayerMask ObstructionPoints;
    }
    [System.Serializable]
    private class SceneObjects
    {
        public ValueSlider baseDistanceErrorSlider;
        public ValueSlider ppmDistanceErrorSlider;
        public ValueSlider angleErrorSlider;
        [Space(10)]
        public Text errorEllipsDisplay;
    }
    [Header("Scene Objects")]
    [SerializeField]
    [Space(20)]
    private Prefabs prefabs;
    [SerializeField]
    private LayerMasks layerMasks;
    [SerializeField]
    private SceneObjects sceneObjects;

    private float[] correctAnswerArray;
    private float sigmaD = 0f;
    private float sigmaH = 0f;
    private float sigmaHExact = 0f;
    private float sigmaA = 0f;
    private float sigmaAExact = 0f;

    private List<PolygonPointController> linePoints = new List<PolygonPointController>();
    private bool holdingObject;
    private GameObject hitObject;
    private Vector2 obstacleHitPoint;
    private GameManager gm;
    private LineRenderer line;

    private bool hasStarted = false;

    
    private void Start()
    {
        if (!hasStarted) StartSetup();
    }

    // the startscript, can be called by the setparametersfunction to get the correct answers before the start function is called in this script
    public void StartSetup()
    {
        hasStarted = true;
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        line = GetComponent<LineRenderer>();

        // set random distance (fixed + ppm) and angle error
        if (baseError.randomizeError)  baseError.error  = Random.Range(baseError.randomErrorRange.x, baseError.randomErrorRange.y);
        if (ppmError.randomizeError)   ppmError.error   = Random.Range(ppmError.randomErrorRange.x, ppmError.randomErrorRange.y);
        if (angleError.randomizeError) angleError.error = Random.Range(angleError.randomErrorRange.x, angleError.randomErrorRange.y);

        // sets the type of line renderer
        if (showLengths || showStartLength)
        {
            line.material = prefabs.fullLine;
        }
        else line.material = prefabs.dottedLine;

        // places the first point 
        if (lockFirstPoint)
        {
            AddPoint(firstPointPosition);
            linePoints[0].GetComponent<CircleCollider2D>().enabled = false;
        }

        SetSliders();
    }

    // set the values of the sliders if they are available
    private void SetSliders()
    {
        if (sceneObjects.baseDistanceErrorSlider)
        {
            sceneObjects.baseDistanceErrorSlider.SetSlider(baseError.error, baseError.lockError);
        }
        if (sceneObjects.ppmDistanceErrorSlider)
        {
            sceneObjects.ppmDistanceErrorSlider.SetSlider(ppmError.error, ppmError.lockError);
        }
        if (sceneObjects.angleErrorSlider)
        {
            sceneObjects.angleErrorSlider.SetSlider(angleError.error, angleError.lockError);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // checks if mousebutton is clicked and sets the point to that position or adds a new point
        SetPointsToMouse();

        //draws the line and sets the values of the points.
        //also stops when it encounters an object.
        DrawLineAndValues();
    }

    //sets the point to the mouse position
    private void SetPointsToMouse()
    {
        // checks if mousebutton is clicked and sets the point to that position
        if (Input.GetMouseButton(0) && gm.IsBetweenValues(gm.SetObjectToMouse(Input.mousePosition, 0)))
        {
            if (!holdingObject)
            {
                hitObject = CastMouseRay();

                if (!holdingObject && linePoints.Count < maxPoints && Input.GetMouseButtonDown(0))
                {
                    //Debug.Log("adding point");
                    AddPoint(gm.SetObjectToMouse(Input.mousePosition, 0));
                    UpdateErrors();
                }
            }
            else
            {
                hitObject.transform.position = gm.SetObjectToMouse(Input.mousePosition, 0);
                UpdateErrors();
            }
        }
        else holdingObject = false;

        if (Input.GetMouseButtonDown(1)){
            RemovePoint();
        }
    }

    //returns the gamobject the mouse has hit
    private GameObject CastMouseRay()
    {
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), 100, layerMasks.pointMask);

        if (rayHit.collider != null)
        {
            holdingObject = true;
            return rayHit.transform.gameObject;
        }
        holdingObject = false;
        return null;
    }

    //draws the line and values
    private void DrawLineAndValues()
    {
        for (int i = 0; i < linePoints.Count; i++)
        {
            linePoints[i].HideInfo();
        }

        if (showStartLength && !showLengths && linePoints.Count > 1)
        {
            linePoints[0].SetDistanceText(linePoints[1].transform.position);
        }
        if (showStartAngle && linePoints.Count > 1)
        {
            linePoints[0].SetAngleText(linePoints[0].transform.position + Vector3.up, linePoints[1].transform.position);
        }

        for (int i = 0; i < linePoints.Count; i++)
        {
            line.SetPosition(i, linePoints[i].transform.position);

            //sets the lengthvalues of the points taking into account potential overlap of multiple points
            if (showLengths && i != 0)
            {
                linePoints[i].SetTotalDistance(linePoints[i - 1].distanceFromStart, linePoints[i-1].transform.position);

                bool overlap = false;
                for (int j = 0; j < i; j++)
                {
                    if(linePoints[j].transform.position == linePoints[i].transform.position)
                    {
                        if (j > 0)
                        {
                            if (linePoints[j - 1].transform.position == linePoints[i - 1].transform.position || linePoints[j + 1].transform.position == linePoints[i - 1].transform.position) overlap = true;
                        }
                        else
                        {
                            if (linePoints[j + 1].transform.position == linePoints[i - 1].transform.position) overlap = true;
                        }
                    }
                }
                if (!overlap)
                {
                    if((i != linePoints.Count - 1 && i != 1) || showEndLength)
                    {
                        linePoints[i].SetDistanceText(linePoints[i - 1].transform.position);
                    }
                }
            }

            //sets the errorEllipses
            if (showEllipses && i != 0)
            {
                Vector3 prevEllipse = linePoints[i - 1].GetEllipseInfo();
                linePoints[i].SetErrorEllips(linePoints[i - 1].transform.position, prevEllipse.x, prevEllipse.y, prevEllipse.z, baseError.error * visualEllipseSizeModifier, ppmError.error * visualEllipseSizeModifier, angleError.error * visualEllipseSizeModifier);

                if (i == linePoints.Count - 1)
                {
                    if (sceneObjects.errorEllipsDisplay)
                    {
                        sceneObjects.errorEllipsDisplay.text = GetSigmaA().ToString();
                    }
                }
            }

            //checks if the line intersects with an obstacle
            if (i != linePoints.Count - 1 && !CheckVisible(linePoints[i], linePoints[i + 1], layerMasks.Obstacles))
            {
                line.positionCount = i + 2;
                line.SetPosition(i + 1, obstacleHitPoint);

                break; //breaks the current loop to skip the next points
            }
            //sets the anglevalues of the points
            if (showAngles && i != 0 && i != linePoints.Count - 1)
            {
                if (CheckVisible(linePoints[1], linePoints[1], layerMasks.ObstructionPoints))
                {
                    bool overlap = false;
                    for (int j = 0; j < i; j++)
                    {
                        if (linePoints[j].transform.position == linePoints[i].transform.position)
                        {
                            overlap = true;
                        }
                    }
                    if (overlap) linePoints[i].displayRadiusModifier = 1.3f;
                    else linePoints[i].displayRadiusModifier = 1;

                    linePoints[i].SetAngleText(linePoints[i - 1].transform.position, linePoints[i + 1].transform.position, showVereffeningsErrors? angleError.error/100f:0);
                }
            }
            line.positionCount = linePoints.Count;
        }
    }

    //updates the errors of the foutenpropagatie
    private void UpdateErrors()
    {
        if (linePoints.Count > 1)
        {
            sigmaD      = 0f;
            sigmaH      = 0f;
            sigmaHExact = 0f;
            sigmaA      = 0f;
            sigmaAExact = 0f;

            for (int i = 0; i < linePoints.Count-1; i++)
            {
                float dist = Vector2.Distance(linePoints[i+1].transform.position, linePoints[i].transform.position) * GameManager.worldScale;

                sigmaD      += baseError.error + (dist * ppmError.error / 1000f); // correctie m->mm
                sigmaH      += dist * angleError.error / 100f * 1.5f;
                sigmaHExact += dist * angleError.error / 100f * (Mathf.PI / 2f);
            }
            sigmaA      = Mathf.Sqrt(Mathf.Pow(sigmaD, 2) + Mathf.Pow(sigmaH, 2));
            sigmaAExact = Mathf.Sqrt(Mathf.Pow(sigmaD, 2) + Mathf.Pow(sigmaHExact, 2));
        }
        /*
        else if(!startCenterPoint)
        {
            Vector2 startPoint = new Vector2(correctAnswerArray[0], correctAnswerArray[1]);
            Vector2 pointP = new Vector2(correctAnswerArray[2], correctAnswerArray[3]);

            float dist = Vector2.Distance(pointP, startPoint) * GameManager.worldScale;

            sigmaD      = baseError.error + (dist * ppmError.error / 1000f);    // correctie m->mm
            sigmaH      = dist * angleError.error / 100f * 1.5f;                // correctie m->mm
            sigmaHExact = dist * angleError.error / 100f * (Mathf.PI / 2f);     // correctie m->mm

            sigmaA      = Mathf.Sqrt(Mathf.Pow(sigmaD, 2) + Mathf.Pow(sigmaH, 2));
            sigmaAExact = Mathf.Sqrt(Mathf.Pow(sigmaD, 2) + Mathf.Pow(sigmaHExact, 2));
        }
        */

    }

    // checks if next point is visible
    private bool CheckVisible(PolygonPointController point, PolygonPointController nextPoint, LayerMask layerMask)
    {
        Vector3 direction = nextPoint.transform.position - point.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(point.transform.position, direction.normalized, direction.magnitude, layerMask);

        if (hit.collider != null)
        {
            obstacleHitPoint = hit.point;
            return false;
        }
        return true;
    }

    //sets the parameters to a specific value so it matches the question and starts the setup
    public void SetVisibles(bool lock1stPoint, bool ellipses, bool angles, bool lengths, bool startAngle, bool startLength, int nrPoints)
    {
        lockFirstPoint = lock1stPoint;
        showEllipses = ellipses;
        showLengths = lengths;
        showAngles = angles;
        showStartAngle = startAngle;
        showStartLength = startLength;
        maxPoints = nrPoints;

        StartSetup();
    }

    //adds a new point
    private void AddPoint(Vector2 pos)
    {
        line.positionCount++;

        PolygonPointController newPoint;

        if (linePoints.Count == 0)
        {
            newPoint = Instantiate(prefabs.firstPoint, pos, Quaternion.identity).GetComponent<PolygonPointController>();
            newPoint.SetNrText(line.positionCount);
            linePoints.Add(newPoint);
        }

        else if (startCenterPoint && line.positionCount == 2)
        {
            newPoint = Instantiate(prefabs.linePoint, pos, Quaternion.identity).GetComponent<PolygonPointController>();
            newPoint.SetNrText(line.positionCount);
            linePoints.Insert(0, newPoint);
        }
        else
        {
            newPoint = Instantiate(prefabs.linePoint, pos, Quaternion.identity).GetComponent<PolygonPointController>();
            newPoint.SetNrText(line.positionCount);
            linePoints.Add(newPoint);
        }

        newPoint.displayError = showSmallErrors;

    }

    //removes the last point
    public void RemovePoint()
    {
        if (linePoints.Count == 2 && startCenterPoint)
        {
            line.positionCount--;
            PolygonPointController removed = linePoints[0];
            linePoints.RemoveAt(0);

            Destroy(removed.gameObject);
        }
        else if (linePoints.Count > 1)
        {
            line.positionCount--;
            PolygonPointController removed = linePoints[linePoints.Count - 1];
            linePoints.RemoveAt(linePoints.Count - 1);

            Destroy(removed.gameObject);
        }
    }

    //set all the line points to a specific value, useful for predetermined points
    public void SetPoints(float[] positions)
    {
        if(!line) line = GetComponent<LineRenderer>();
        maxPoints = Mathf.Max(positions.Length / 2, maxPoints);
        int maxPlacedpoints = positions.Length / 2;
        for (int i = 0; i < maxPlacedpoints; i++)
        {
            AddPoint(new Vector2(positions[i * 2], positions[i * 2 + 1]));
        }
        AddPoint(new Vector2(positions[2], positions[3]));
        AddPoint(new Vector2(positions[0], positions[1]));
    }

    //returs the mapangle between two points
    public float GetMapAngle(Vector2 endPoint, Vector2 startPoint)
    {
        float angle = Vector2.SignedAngle(endPoint, startPoint);
        angle = Mathf.Round(Mathf.Abs(angle) / 360f * 400 * 1000) / 1000f + 200f;
        return angle;
    }

    /// <summary>
    /// return the distance, angle, sigmaA and sigmaA exact error for P to A
    /// </summary>
    /// <returns></returns>
    public float[] GetErrorDH() 
    {
        Vector2 startPoint = new Vector2(correctAnswerArray[0], correctAnswerArray[1]);
        Vector2 pointP = new Vector2(correctAnswerArray[2], correctAnswerArray[3]);

        float d = Vector2.Distance(pointP, startPoint) * GameManager.worldScale;
        float sigmaD = baseError.error + (0.001f * d * ppmError.error); // correctie m->mm
        float sigmaH      = 1.5f *        d * angleError.error / 100f;// correctie m->mm
        float sigmaHExact = Mathf.PI/2f * d * angleError.error / 100f;// correctie m->mm
        float errorA = Mathf.Sqrt(Mathf.Pow(sigmaD, 2) + Mathf.Pow(sigmaH, 2));
        float errorAExact = Mathf.Sqrt(Mathf.Pow(sigmaD, 2) + Mathf.Pow(sigmaHExact, 2));

        return new float[] { GameManager.RoundFloat(sigmaD, 1), GameManager.RoundFloat(sigmaH, 1), GameManager.RoundFloat(errorA, 1), GameManager.RoundFloat(errorAExact, 1), linePoints.Count() };

    }

    //returns on of the errors
    public float GetError(ErrorType errorType)
    {
        switch (errorType)
        {
            case ErrorType.Base: return baseError.error;
            case ErrorType.Ppm: return ppmError.error;
            case ErrorType.Angle: return angleError.error;
            default: return 0;  
        }
    }

    // sets one of the errors
    public void SetError(ErrorType errorType ,float value)
    {
        ErrorClass error;
        switch (errorType)
        {
            case ErrorType.Base: error = baseError;
                break;
            case ErrorType.Ppm: error = baseError;
                break;
            case ErrorType.Angle: error = baseError;
                break;
            default: error = new ErrorClass();
                break;
        }
        if (!error.lockError) error.error = value;
        else Debug.Log("Angle Error is locked");
    }
    public void SetAngleError(float value)
    {
        if (!angleError.lockError) angleError.error = value;
        else Debug.Log("Angle Error is locked");
    }
    public void SetBaseDistanceError(float value)
    {
        if(!baseError.lockError) baseError.error = value;
        else Debug.Log("base Distance error is locked");
    }
    public void SetPpmDistanceError(float value)
    {
        if(!ppmError.lockError) ppmError.error = value;
        else Debug.Log("ppm distance error is locked");
    }

    //get specific errors
    public float GetSigmaD() // compute distance error of last linePoint
    {
        return GameManager.RoundFloat(sigmaD, 1);
    }

    public float GetSigmaH() // compute angle error of last linePoint
    {
        return GameManager.RoundFloat(sigmaH, 1);
    }

    public float GetSigmaHExact() // compute angle error of last linePoint
    {
        return GameManager.RoundFloat(sigmaHExact, 1);
    }

    public float GetSigmaA() // compute sigmaA of last linePoint
    {
        return GameManager.RoundFloat(sigmaA, 1);
    }

    public float GetSigmaAExact() // compute sigmaA of last linePoint
    {
        return GameManager.RoundFloat(sigmaAExact, 1);
    }

    public void SetAnswerArray(float[] array) 
    {
        correctAnswerArray = array;
    }

    // check whether first and last linepoint are equal to the start (A) or endpoint(P)
    public bool CheckPoints() 
    {
        if (correctAnswerArray.Length < 4) return true;

        bool match = true;

        if (linePoints.Count >= 2)
        {
            Vector2 startPoint = new Vector2(correctAnswerArray[0], correctAnswerArray[1]);
            Vector2 endPoint = new Vector2(correctAnswerArray[correctAnswerArray.Length - 2], correctAnswerArray[correctAnswerArray.Length - 1]);

            if ((Vector2.Distance(linePoints[0].transform.position, endPoint) > 0.001) || (Vector2.Distance(linePoints.Last().transform.position, startPoint) > 0.001))
            {
                match = false;
            }
        }
        else match = false;
        return match;
    }

    // check visibility of all consecutive points (returns true if this is the case)
    public bool CheckPointsVisibility() 
    {
        for (int i = 0; i < linePoints.Count-1; i++)
        {
            if (!CheckVisible(linePoints[i], linePoints[i + 1], layerMasks.Obstacles))
            {
                return false;
            }
        }
        return true;
    }



    //todo
    public void CreateShortestPath()
    {

    }
}



