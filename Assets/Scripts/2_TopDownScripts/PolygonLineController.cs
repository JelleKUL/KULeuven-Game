using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//*********** The PolygonLineController manages the Top down lines and displays all the measuring information ******************//

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

    [Header("Error Parameters")]
    [SerializeField] private bool lockBaseDistanceError;
    [Tooltip("the base measure error of distance")]
    [Range(0, 5)]
    [SerializeField] private float baseDistanceError = 0f;
    [SerializeField] private bool lockPpmDistanceError;
    [Tooltip("the ppm measure error of distance")]
    [Range(0, 5)]
    [SerializeField] private float ppmDistanceError = 0f;
    [SerializeField] private bool lockAngleError;
    [Tooltip("the measure error of the angle")]
    [Range(0, 5)]
    [SerializeField] private float angleError = 0f;
    
    [Header("Visual Parameters")]
    [Range(0, 100)]
    [Tooltip("The visual size modifier of the error ellipses")]
    [SerializeField] private float visualEllipseSizeModifier = 1;
    [SerializeField] private bool showEllipses;
    [SerializeField] private bool showAngles;
    [SerializeField] private bool showLengths;
    [SerializeField] private bool showStartAngle;
    [SerializeField] private bool showStartLength;
    [SerializeField] private bool showEndLength = true;


    [System.Serializable]
    private class Prefabs
    {
        public GameObject linePoint;
        public GameObject firstPoint;
        public LayerMask pointMask;
        public LayerMask Obstacles;
        public LayerMask ObstructionPoints;
        public Material fullLine;
        public Material dottedLine;
    }
    [System.Serializable]
    private class SceneObjects
    {
        public Slider baseDistanceErrorSlider;
        public Slider ppmDistanceErrorSlider;
        public Slider angleErrorSlider;
        [Space(10)]
        public Text errorEllipsDisplay;
    }
    [Header("Scene Objects")]
    [SerializeField]
    [Space(20)]
    private Prefabs prefabs;
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
            sceneObjects.baseDistanceErrorSlider.value = baseDistanceError;
            sceneObjects.baseDistanceErrorSlider.interactable = !lockBaseDistanceError;
        }
        if (sceneObjects.ppmDistanceErrorSlider)
        {
            sceneObjects.ppmDistanceErrorSlider.value = ppmDistanceError;
            sceneObjects.ppmDistanceErrorSlider.interactable = !lockPpmDistanceError;
        }
        if (sceneObjects.angleErrorSlider)
        {
            sceneObjects.angleErrorSlider.value = angleError;
            sceneObjects.angleErrorSlider.interactable = !lockAngleError;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // checks is mousebutton is clicked and sets the point to that position or adds a new point
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
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), 100, prefabs.pointMask);

        if (rayHit.collider != null)
        {
            holdingObject = true;
            //Debug.Log(rayHit.transform.gameObject.name);
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

        if (showStartLength && linePoints.Count > 1)
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

            //sets the lengthvalues of the points
            if (showLengths && i != 0)
            {
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
                    if((i == linePoints.Count - 1 || i == 1) && (!showEndLength))
                    {
                        
                    }
                    else
                    {
                        linePoints[i].SetDistanceText(linePoints[i - 1].transform.position);
                    }
                }

            }
            //sets the errorEllipses
            if (showEllipses && i != 0)
            {
                Vector3 prevEllipse = linePoints[i - 1].GetEllipseInfo();
                linePoints[i].SetErrorEllips(linePoints[i - 1].transform.position, prevEllipse.x, prevEllipse.y, prevEllipse.z, baseDistanceError * visualEllipseSizeModifier, angleError * visualEllipseSizeModifier); // multiplied by 50 to increase visual size

                if (i == linePoints.Count - 1) //&& !CheckVisible(linePoints[i - 1], linePoints[i], Obstacles)
                {
                    //Vector3 ellips = thisPoint.GetEllipseInfo(); // the ellips will be 50 times to large !
                    //biggestEllips = ellips.x * ellips.y * Mathf.PI / 4f;
                    if (sceneObjects.errorEllipsDisplay)
                    {
                        sceneObjects.errorEllipsDisplay.text = GetSigmaA().ToString();
                    }
                }
            }

            //checks if the line intersects with an obstacle
            if (i != linePoints.Count - 1 && !CheckVisible(linePoints[i], linePoints[i + 1], prefabs.Obstacles))
            {

                line.positionCount = i + 2;
                line.SetPosition(i + 1, obstacleHitPoint);

                break;

            }
            //sets the anglevalues of the points
            if (showAngles && i != 0 && i != linePoints.Count - 1)
            {
                if (CheckVisible(linePoints[1], linePoints[1], prefabs.ObstructionPoints))
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

                    linePoints[i].SetAngleText(linePoints[i - 1].transform.position, linePoints[i + 1].transform.position);
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

                sigmaD      += baseDistanceError + (dist * ppmDistanceError / 1000f); // correctie m->mm
                sigmaH      += dist * angleError / 100f * 1.5f;
                sigmaHExact += dist * angleError / 100f * (Mathf.PI / 2f);
            }
            sigmaA      = Mathf.Sqrt(Mathf.Pow(sigmaD, 2) + Mathf.Pow(sigmaH, 2));
            sigmaAExact = Mathf.Sqrt(Mathf.Pow(sigmaD, 2) + Mathf.Pow(sigmaHExact, 2));
        }
        else if(!startCenterPoint)
        {
            Vector2 startPoint = new Vector2(correctAnswerArray[0], correctAnswerArray[1]);
            Vector2 pointP = new Vector2(correctAnswerArray[2], correctAnswerArray[3]);

            float dist = Vector2.Distance(pointP, startPoint) * GameManager.worldScale;

            sigmaD      = baseDistanceError + (dist * ppmDistanceError / 1000f);    // correctie m->mm
            sigmaH      = dist * angleError / 100f * 1.5f;                          // correctie m->mm
            sigmaHExact = dist * angleError / 100f * (Mathf.PI / 2f);               // correctie m->mm

            sigmaA      = Mathf.Sqrt(Mathf.Pow(sigmaD, 2) + Mathf.Pow(sigmaH, 2));
            sigmaAExact = Mathf.Sqrt(Mathf.Pow(sigmaD, 2) + Mathf.Pow(sigmaHExact, 2));
        }

    }

    // checks if next point is visible
    private bool CheckVisible(PolygonPointController point, PolygonPointController nextPoint, LayerMask layerMask)
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
        // set random distance (fixed + ppm) and angle error
        baseDistanceError = Random.Range(1, 5); 
        ppmDistanceError  = Random.Range(1, 5);
        angleError        = Random.Range(1, 5);

        lockFirstPoint = lock1stPoint;
        showEllipses = ellipses;
        showLengths = lengths;
        showAngles = angles;
        showStartAngle = startAngle;
        showStartLength = startLength;
        maxPoints = nrPoints;
    }

 


    //adds a new point
    private void AddPoint(Vector2 pos)
    {
        line.positionCount++;

        PolygonPointController newPoint;

        if (linePoints.Count == 0)
        {
            newPoint = Instantiate(prefabs.firstPoint, pos, Quaternion.identity).GetComponent<PolygonPointController>();
            newPoint.SetNameNrText(line.positionCount);
            linePoints.Add(newPoint);
        }

        else if (startCenterPoint && line.positionCount == 2)
        {
            newPoint = Instantiate(prefabs.linePoint, pos, Quaternion.identity).GetComponent<PolygonPointController>();
            newPoint.SetNameNrText(line.positionCount);
            linePoints.Insert(0, newPoint);
        }
        else
        {
            newPoint = Instantiate(prefabs.linePoint, pos, Quaternion.identity).GetComponent<PolygonPointController>();
            newPoint.SetNameNrText(line.positionCount);
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
        //AddPoint(new Vector2(positions[(maxPoints - 1) * 2], positions[(maxPoints - 1) * 2 + 1]));

    }

    //returs the mapangle between two points
    public float GetMapAngle(Vector2 endPoint, Vector2 startPoint)
    {
        float angle = Vector2.SignedAngle(endPoint, startPoint);
        angle = Mathf.Round(Mathf.Abs(angle) / 360f * 400 * 1000) / 1000f + 200f;
        return angle;
    }

    public (float,float,float) GetErrorDH() // compute distance, angle, and sigmaA error for P to A
    {
        Vector2 startPoint = new Vector2(correctAnswerArray[0], correctAnswerArray[1]);
        Vector2 pointP = new Vector2(correctAnswerArray[2], correctAnswerArray[3]);

        float d = Vector2.Distance(pointP, startPoint) * GameManager.worldScale;
        float sigmaD = baseDistanceError + (0.001f * d * ppmDistanceError); // correctie m->mm
        float sigmaH = 1.5f * d * angleError / 100f;// correctie m->mm
        float errorA = Mathf.Sqrt(Mathf.Pow(sigmaD, 2) + Mathf.Pow(sigmaD, 2));

        return (GameManager.RoundFloat(sigmaD, 1),GameManager.RoundFloat(sigmaH, 1), GameManager.RoundFloat(errorA, 1));

    }

    public float GetAngleError()
    {
        return angleError;
    }
    public void SetAngleError(float value)
    {
        if (!lockAngleError) angleError = value;
        else Debug.Log("Angle Error is locked");
    }

    public float GetBaseDistanceError()
    {
        return baseDistanceError;
    }
    public void SetBaseDistanceError(float value)
    {
        if(!lockBaseDistanceError) baseDistanceError = value;
        else Debug.Log("base Distance error is locked");
    }

    public float GetPpmDistanceError()
    {
        return ppmDistanceError;
    }
    public void SetPpmDistanceError(float value)
    {
        if(!lockPpmDistanceError) ppmDistanceError = value;
        else Debug.Log("ppm distance error is locked");
    }
   
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

    public bool CheckPoints() // check whether first and last linepoint are equal to the start (P) and endpoint(A)
    {
        if (correctAnswerArray.Length < 4) return true;

        bool match = true;

        if (linePoints.Count >= 2)
        {
            Vector2 startPoint = new Vector2(correctAnswerArray[0], correctAnswerArray[1]);
            Vector2 endPoint = new Vector2(correctAnswerArray[correctAnswerArray.Length - 2], correctAnswerArray[correctAnswerArray.Length - 1]);

            if ((Vector2.Distance(linePoints[0].transform.position, startPoint) > 0.001) || (Vector2.Distance(linePoints.Last().transform.position, endPoint) > 0.001))
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
            if (!CheckVisible(linePoints[i], linePoints[i + 1], prefabs.Obstacles))
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



