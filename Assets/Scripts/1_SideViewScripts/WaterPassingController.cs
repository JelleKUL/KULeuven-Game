using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

//*********** The WaterPassingController controls most aspects of the waterpassing type exercise ******************//


public class WaterPassingController : MonoBehaviour
{
    [Header ("Prefabs")]
    public GameObject beacon;
    public GameObject measure;
    public GameObject groundPoint;
    public GameObject topPoint;
    public GameObject groundPointTopDown;
    //public GameObject magnifyGlass;
    public LayerMask pointMask;
    public Transform BeaconPlacer;
    public Transform MeasurePlacer;
    public Button magnifyButton;
    public Button beaconButton;
    public Button measureButton;
    public Text angleErrorText;
    //public Text distanceAngleText;
    public Text rotationAngleText;
    public SpriteShapeController spriteShapeController;
    public SpriteShapeController topSpriteShapeController;
    public Transform groundPointTopDownCenter;
    public WaterpassingTabel waterpassingTabel;
    [Header ("Changeable Parameters")]
    [Range (2,5)]
    public int nrOfPoints;
    public bool placeTopPoints;
    [Range(0, 3)]
    public int nrOfTopPoints;
    public int maxBeacons;
    public int maxMeasures;
    //public bool showDistanceLaser;
    public bool lockMeasure;
    public Vector2 lockedMeasureLocation;
    public bool lockBeacon;
    public Vector2 lockedBeaconLocation;
    public bool loopAround;
    public bool addPointOutLoop;
    public float topDownPointRaduis = 0.5f;
    [Header("Standard Parameters")]
    public bool showDistanceMeasureAngle;
    [Tooltip ("the angle of the upper and lower laserline to determine the distance")]
    public float distanceMeasureAngle;
    public bool showAngleError;
    [Tooltip("the maximum error of the measure laser")]
    public float maxAngleError;
    [Tooltip("the minimum distance the points should be apart")]
    public float minDistance;
    [Tooltip("the max height of the points in % of the screen height")]
    [Range(0, 0.5f)]
    public float maxHeightGroundPoint;
    public bool extremeHeightDiff;
    public float startAndEndPointOffset = 0.1f;
  
    
    private GameManager gm;
    private List<GameObject> groundPoints = new List<GameObject>();
    private List<GameObject> topPoints = new List<GameObject>();
    private List<GameObject> measures = new List<GameObject>();
    private List<GameObject> beacons = new List<GameObject>();
    private List<GameObject> groundPointsTopDown = new List<GameObject>();



    private bool holdingObject;
    private GameObject hitObject;

    [HideInInspector]
    public float correctHeight;
    [HideInInspector]
    public float[] correctHeightDifferences;
    [HideInInspector]
    public float [] correctDistances;
    [HideInInspector]
    public float correctDistance;
    [HideInInspector]
    public float correctErrorAngle;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        //magnifyGlass.SetActive(false);
        

        if (nrOfPoints > 0)
        {
            ChangePoints();
        }
       
        if (loopAround)
        {
            AddStartAndEndGroundPoint();
            SetGroundPointsTopDown();
            waterpassingTabel.CreateTable(nrOfPoints + 1);
        }

        SetGroundSprite();
        SetTopSprite();

        if (lockMeasure)
        {
            AddMeasure(lockedMeasureLocation);
        }
        else
        {
            AddMeasure(MeasurePlacer.position);
        }

        if (lockBeacon)
        {
            AddBeacon(lockedBeaconLocation);
            AddBeacon(new Vector2(lockedMeasureLocation.x * 2 - lockedBeaconLocation.x, lockedBeaconLocation.y));
        }
        else
        {
            AddBeacon(BeaconPlacer.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0) && gm.IsBetweenValues(gm.SetObjectToMouse(Input.mousePosition, 0)))
        {
            hitObject = CastMouseRay();

            if (hitObject != null && hitObject.tag == "MagnifyGlass")
            {

                hitObject.GetComponent<MagnifyGlass>().ToggleZoom();
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (!holdingObject)
            {
                hitObject = CastMouseRay();
            }
            if (hitObject!= null && hitObject.tag != "MagnifyGlass")
            {
                hitObject.transform.position = gm.SetObjectToMouse(Input.mousePosition, 0);
                hitObject.GetComponent<Physics2DObject>().isHeld = true;
            }

        }

        if (Input.GetMouseButtonUp(0))
        {
            if (holdingObject && hitObject.tag != "MagnifyGlass")
            {
                if (gm.IsBetweenValues(gm.SetObjectToMouse(Input.mousePosition, 0)))
                {
                    if (hitObject.tag == "Measure" && measures.Count < maxMeasures)
                    {
                        AddMeasure(MeasurePlacer.position);
                    }
                    else if (hitObject.tag == "Beacon" && beacons.Count < maxBeacons)
                    {
                        AddBeacon(BeaconPlacer.position);
                    }
                }
                else
                {
                    if (hitObject.tag == "Measure")
                    {
                        hitObject.transform.position = MeasurePlacer.position;
                    }
                    else if (hitObject.tag == "Beacon")
                    {
                        hitObject.transform.position = BeaconPlacer.position;
                    }
                }
                hitObject.GetComponent<Physics2DObject>().isHeld = false;
            }

            holdingObject = false;

        }
       
    }
    

    public void SetGroundSprite()
    {
        spriteShapeController.spline.Clear();
        spriteShapeController.spline.InsertPointAt(0, new Vector3(-2,-3));
        spriteShapeController.spline.InsertPointAt(0, new Vector3(18, -3));
        spriteShapeController.spline.InsertPointAt(0, new Vector3(18, -0.5f));
        spriteShapeController.spline.InsertPointAt(0, new Vector3(gm.screenMax.x, -0.5f));
        
        for (int i = nrOfPoints; i > 0 ; i--)
        {
            spriteShapeController.spline.InsertPointAt(0, groundPoints[i-1].transform.position + 0.5f * Vector3.down);
        }
        spriteShapeController.spline.InsertPointAt(0, new Vector3(gm.screenMin.x, -0.5f));
        spriteShapeController.spline.InsertPointAt(0, new Vector3(-2, -0.5f));

        for (int i = 0; i < spriteShapeController.spline.GetPointCount(); i++)
        {
            spriteShapeController.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
            //spriteShapeController.spline.SetHeight(i, 1);
            spriteShapeController.spline.SetLeftTangent(i, Vector3.left);
            spriteShapeController.spline.SetRightTangent(i, Vector3.right);
        }

    }
    public void SetTopSprite()
    {
        topSpriteShapeController.spline.Clear();

        topSpriteShapeController.spline.InsertPointAt(0, new Vector3(-2, gm.screenMax.y - 0.5f));
        topSpriteShapeController.spline.InsertPointAt(0, new Vector3(gm.screenMin.x, gm.screenMax.y - 0.5f));

        for (int i = 0; i < nrOfTopPoints; i++)
        {
            topSpriteShapeController.spline.InsertPointAt(0, topPoints[i].transform.position + 0.5f * Vector3.up);
            
        }

        topSpriteShapeController.spline.InsertPointAt(0, new Vector3(gm.screenMax.x, gm.screenMax.y - 0.5f));
        topSpriteShapeController.spline.InsertPointAt(0, new Vector3(18, gm.screenMax.y - 0.5f));
        topSpriteShapeController.spline.InsertPointAt(0, new Vector3(18, 10));
        topSpriteShapeController.spline.InsertPointAt(0, new Vector3(-2, 10));

        for (int i = 0; i < topSpriteShapeController.spline.GetPointCount(); i++)
        {
            topSpriteShapeController.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
            //topSpriteShapeController.spline.SetHeight(i, 0.1f);
            topSpriteShapeController.spline.SetLeftTangent(i, Vector3.right);
            topSpriteShapeController.spline.SetRightTangent(i, Vector3.left);
        }

    }

    public bool MouseOverBeacon()
    {
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), 20);
        if (rayHit.collider != null)
        {
            if (rayHit.collider.tag == "Beacon")
            {
                return true;
            }
        }
        return false;
    }
    //returns the gamobject the mouse has hit
    public GameObject CastMouseRay()
    {
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition),20, pointMask);

        if (rayHit.collider != null)
        {
            holdingObject = true;
            Debug.Log(rayHit.transform.gameObject.name);
            return rayHit.transform.gameObject;
            
        }
        holdingObject = false;
        return null;
    }


    public void ToggleMagnify()
    {
        
        foreach (var measureObject in measures)
        {
            measureObject.GetComponent<MeasureController>().ToggleMagnify();
        }
        

    }
    
    //places a measure object
    public void AddMeasure(Vector2 location)
    {
        GameObject newMeasure = Instantiate(measure, location, Quaternion.identity);
        MeasureController measureController = newMeasure.GetComponent<MeasureController>();
        measureController.errorAngle = correctErrorAngle;
        if (nrOfPoints > 0)
        {
            measureController.maxDistance = (gm.screenMax.x - gm.screenMin.x) / nrOfPoints - minDistance;
        }
        else
        {
            measureController.maxDistance = (gm.screenMax.x - gm.screenMin.x);
        }
        SetAngleErrorText();
        //SetDistanceAngleText();
        
        newMeasure.transform.GetChild(1).gameObject.SetActive(false);
        measures.Add(newMeasure);
     
    }


    //removes the last measure object
    public void RemoveMeasure()
    {
        GameObject lastMeasure = measures[measures.Count - 1];
        measures.RemoveAt(measures.Count - 1);

        Destroy(lastMeasure);
    }

    //flips the measure
    public void FlipMeasure()
    {
        for (int i = 0; i < measures.Count; i++)
        {
            measures[i].GetComponent<MeasureController>().errorAngle *= -1;
            Debug.Log(measures[i].GetComponent<MeasureController>().errorAngle);
        }

        
    }

    //sets the scheefstandangle of all the measures

    public void RotateMeasure(float angleInput)
    {
        if (rotationAngleText)
        {
            rotationAngleText.text = (Mathf.Round(angleInput * 100) / 100f).ToString() + " gon";
        }
        for (int i = 0; i < measures.Count; i++)
        {
            measures[i].GetComponent<MeasureController>().UpdateMeasureHeadRotation(angleInput * 3.6f / 4f);
            Debug.Log(angleInput);
        }


    }


    //places a measure object
    public void AddBeacon(Vector2 location)
    {
        GameObject newBeacon = Instantiate(beacon, location, Quaternion.identity);
        newBeacon.GetComponent<Physics2DObject>().allowUpsideDown = placeTopPoints;
        beacons.Add(newBeacon);
        
    }


    //removes the last measure object
    public void RemoveBeacon()
    {
        GameObject lastBeacon = beacons[beacons.Count - 1];
        beacons.RemoveAt(beacons.Count - 1);

        Destroy(lastBeacon);
    }



    // randomizes the playfield
    public void ChangePoints()
    {
        AddGroundPoints( placeTopPoints);
        

        for (int i = 0; i < measures.Count; i++)
        {
            RemoveMeasure();

        }
        for (int i = 0; i < beacons.Count; i++)
        {
            RemoveBeacon();

        }

        if (lockMeasure)
        {
            AddMeasure(lockedMeasureLocation);
        }
        if (lockBeacon)
        {
            AddBeacon(lockedBeaconLocation);
        }

    }

    public void AddStartAndEndGroundPoint()
    {
        GameObject newPoint = Instantiate(groundPoint, new Vector2(gm.screenMin.x+ startAndEndPointOffset, 0), Quaternion.identity);
        newPoint.GetComponent<PolygonPointController>().SetNameNrText(0);

        newPoint = Instantiate(groundPoint, new Vector2(gm.screenMax.x - startAndEndPointOffset, 0), Quaternion.identity);
        newPoint.GetComponent<PolygonPointController>().SetNameNrText(0);
    }

    //place new groundpoints
    public void AddGroundPoints( bool placeTop)
    {
        
        float Increment = (gm.screenMax.x - gm.screenMin.x - 2) / (float)(nrOfPoints + (placeTop? nrOfTopPoints : 0));
        int pointSkipper = 0;

        // creating the groundpoints
        if (groundPoints.Count < nrOfPoints)
        {
            Debug.Log(groundPoints.Count + " placed -> " + nrOfPoints + " to Place");
            pointSkipper = 0;
            for (int i = groundPoints.Count; i < nrOfPoints; i++)
            {
                GameObject newPoint = Instantiate(groundPoint, Vector2.zero, Quaternion.identity);
                
                newPoint.GetComponent<PolygonPointController>().SetNameText(i + 1 + pointSkipper);
                pointSkipper += (nrOfTopPoints > i) ? 1 : 0;
                groundPoints.Add(newPoint);
                Debug.Log("placed " + i + 1 + pointSkipper);
            }

            // moving the points to the correct location
            pointSkipper = 0;
            for (int i = 0; i < nrOfPoints; i++)
            {
                Vector2 newPos;
                if (extremeHeightDiff)
                {
                    newPos = new Vector2(gm.screenMin.x + 1 + Increment * (i + pointSkipper) + Random.Range(minDistance / 2f, Increment - minDistance / 2f), (gm.screenMax.y) * maxHeightGroundPoint * i * 2 / (float)nrOfPoints + Random.Range(0, 0.5f));
                }
                else newPos = new Vector2(gm.screenMin.x + 1 + Increment * (i + pointSkipper) + Random.Range(minDistance / 2f, Increment - minDistance / 2f), Random.Range(0, (gm.screenMax.y) * maxHeightGroundPoint));
                groundPoints[i].transform.position = newPos;
                pointSkipper += (nrOfTopPoints > i) ? 1 : 0;
                Debug.Log("moved " + i);
            }
        }
        

        // creating the toppoints
        if (topPoints.Count < nrOfTopPoints && placeTopPoints)
        {
            Debug.Log(topPoints.Count + " " + nrOfTopPoints);
            pointSkipper = 1;
            for (int i = topPoints.Count; i < nrOfTopPoints; i++)
            {
                GameObject newPoint = Instantiate(topPoint, Vector2.zero, Quaternion.identity);
                newPoint.GetComponent<PolygonPointController>().SetNameText(i + 1 + pointSkipper);
                pointSkipper += (nrOfPoints > i) ? 1 : 0;
                topPoints.Add(newPoint);
                Debug.Log("placed " + i);
            }

            // moving the toppoints to the correct position
            pointSkipper = 1;
            for (int i = 0; i < nrOfTopPoints; i++)
            {
                Vector2 newPos = new Vector2(gm.screenMin.x + 1 + Increment * (i + pointSkipper) + Random.Range(minDistance / 2f, Increment - minDistance / 2f), Random.Range(gm.screenMax.y - 0.5f, (gm.screenMax.y - 0.5f) * (1 - maxHeightGroundPoint)));
                topPoints[i].transform.position = newPos;
                pointSkipper += (nrOfPoints > i) ? 1 : 0;
                Debug.Log("moved top " + i);
            }
        }

        if (!placeTopPoints)
        {
            correctHeight = groundPoints[groundPoints.Count - 1].transform.position.y - groundPoints[0].transform.position.y;
            correctDistance = groundPoints[groundPoints.Count - 1].transform.position.x - groundPoints[0].transform.position.x;
        }
        else
        {
            // check what the last point is, the first point is alsways the groundpoint
            Vector2 lastPoint = nrOfPoints > nrOfTopPoints ? groundPoints[groundPoints.Count - 1].transform.position : topPoints[topPoints.Count - 1].transform.position;
            correctHeight = lastPoint.y - groundPoints[0].transform.position.y;
            correctDistance = lastPoint.x - groundPoints[0].transform.position.x;
        }
        Debug.Log(correctHeight);


        if (loopAround)
        {
            correctHeightDifferences = new float[nrOfPoints + 1];
            correctDistances = new float[nrOfPoints + 1];

            // correct heights
            correctHeightDifferences[0] = groundPoints[0].transform.position.y;
            for (int i = 1; i < nrOfPoints; i++)
            {
                correctHeightDifferences[i] = groundPoints[i].transform.position.y - groundPoints[i - 1].transform.position.y;
                
            }
            correctHeightDifferences[nrOfPoints] = -groundPoints[nrOfPoints-1].transform.position.y;

            //correct distances
            correctDistances[0] = groundPoints[0].transform.position.x - (gm.screenMin.x + startAndEndPointOffset);
            for (int i = 1; i < nrOfPoints; i++)
            {
                correctDistances[i] = groundPoints[i].transform.position.x - groundPoints[i-1].transform.position.x;
            }
            correctDistances[nrOfPoints] = (gm.screenMax.x - startAndEndPointOffset) - groundPoints[nrOfPoints-1].transform.position.x;

            if (GameManager.showDebugAnswer || true)
            {
                for (int i = 0; i < nrOfPoints + 1; i++)
                {
                    Debug.Log(i + "-> Height: " + correctHeightDifferences[i] + " m, Distance: " + correctDistances[i]);
                }
            }
            
        }
        

        

    }
    //adds the top points
    public void addTopPoints()
    {
        float Increment = (gm.screenMax.x - gm.screenMin.x - 2) / (float)nrOfTopPoints;

        if (topPoints.Count < nrOfTopPoints)
        {
            Debug.Log(topPoints.Count + " " + nrOfTopPoints);

            for (int i = topPoints.Count; i < nrOfTopPoints; i++)
            {
                GameObject newPoint = Instantiate(topPoint, Vector2.zero, Quaternion.identity);
                newPoint.GetComponent<PolygonPointController>().SetNameText(i+1+nrOfPoints);
                topPoints.Add(newPoint);
                Debug.Log("placed " + i);
            }
        }
        if (topPoints.Count > nrOfTopPoints)
        {
            for (int i = topPoints.Count; i > nrOfTopPoints; i--)
            {
                GameObject lastPoint = topPoints[i - 1];
                topPoints.RemoveAt(i - 1);

                Destroy(lastPoint);
                Debug.Log("destroyed" + i);
            }
        }

        for (int i = 0; i < nrOfTopPoints; i++)
        {
            Vector2 newPos = new Vector2(gm.screenMin.x + 1 + Increment * i + Random.Range(minDistance / 2f, Increment - minDistance / 2f), Random.Range(gm.screenMax.y - 0.5f,  (gm.screenMax.y - 0.5f) * (1 - maxHeightGroundPoint)));
            topPoints[i].transform.position = newPos;
            Debug.Log("moved top " + i);
        }
    

        correctHeight = topPoints[topPoints.Count - 1].transform.position.y - groundPoints[0].transform.position.y;
        //correctDistance = groundPoints[groundPoints.Count - 1].transform.position.x - groundPoints[0].transform.position.x;
        //Debug.Log(correctHeight);
    }

    public void SetGroundPointsTopDown()
    {
        float radius = (gm.screenMax.x - gm.screenMin.x) / ( 2 * Mathf.PI);
        //groundPointTopDownCenter.localScale = Vector3.one * radius;
        float cummAngle = 0f;
        int pointOutLoopNr = 0;
        //chooses one point to leave out of the loop
        if (addPointOutLoop)
        {
            pointOutLoopNr = Random.Range(1, nrOfPoints);
        }
        

        for (int i = 0; i < nrOfPoints + 1; i++)
        {
            // instantiates a point and moves a point out of the loop further away
            GameObject newpoint = Instantiate(groundPointTopDown, groundPointTopDownCenter.position + Vector3.right * topDownPointRaduis * ((addPointOutLoop && i == pointOutLoopNr) ? 1.5f : 1), Quaternion.identity);
            newpoint.GetComponent<PolygonPointController>().SetNameText(i==0? -1:i);
            groundPointsTopDown.Add(newpoint);
        }

        cummAngle += (groundPoints[0].transform.position.x - gm.screenMin.x) / radius;
        groundPointsTopDown[1].transform.RotateAround(groundPointTopDownCenter.position, Vector3.back, cummAngle * Mathf.Rad2Deg);

        for (int i = 2; i < groundPointsTopDown.Count; i++)
        {

            cummAngle += (groundPoints[i-1].transform.position.x - groundPoints[i-2].transform.position.x) / radius;
            groundPointsTopDown[i].transform.RotateAround(groundPointTopDownCenter.position, Vector3.back, cummAngle * Mathf.Rad2Deg);
              
        }
        LineRenderer line = groundPointTopDownCenter.gameObject.GetComponent<LineRenderer>();
        line.positionCount = ((addPointOutLoop) ? -1 : 0) + groundPointsTopDown.Count;

        for (int i = 0; i < line.positionCount; i++)
        {
            int j = (i < pointOutLoopNr || ! addPointOutLoop) ? i : i + 1;
            

            line.SetPosition(i, groundPointsTopDown[j].transform.position - Vector3.forward * 2);
        }
        

        //Vector2 location = groundPointTopDownCenter.position +
    }


    public void SetAngleErrorText()
    {
        if (showAngleError)
        {
            angleErrorText.text = "De Collimatiefout is: \n " + (Mathf.Round(correctErrorAngle * 100 * (4 / 3.6f)) / 100).ToString() + " gon";
        }
        else angleErrorText.text = "De Collimatiefout is: \n " + "? gon";


    }
 
    //shows the correct answer (replaced in the questionscript)
    public string ShowAnswer()
    {
        string answer =  "Height Diff: " + correctHeight.ToString();

        for (int i = 0; i < measures.Count; i++)
        {
            float errorAngleEach = measures[i].transform.GetChild(0).transform.eulerAngles.z;
            answer += " & errorAngle " + (i + 1) + ": " + errorAngleEach.ToString();
            measures[i].transform.GetChild(1).gameObject.SetActive(true);
            measures[i].transform.GetChild(0).gameObject.SetActive(false);
        }

        return answer;

    }

    //sets the parameters so they match the given question
    public void SetParameters(int nrPoints, int nrBeacons, int nrMeasures, bool ShowDistance, bool lockmeasure, Vector2 measureLocation, bool lockbeacon, Vector2 beaconLocation, bool loop)
    {
        correctErrorAngle = Random.Range(-maxAngleError, maxAngleError);

        nrOfPoints = nrPoints;
        maxBeacons = nrBeacons;
        maxMeasures = nrMeasures;
        //showDistanceLaser = ShowDistance;
        lockMeasure = lockmeasure;
        lockedMeasureLocation = measureLocation;
        lockBeacon = lockbeacon;
        lockedBeaconLocation = beaconLocation;
        loopAround = loop;
    }
    public bool CheckTabelAnswer()
    {
        return waterpassingTabel.CheckAnswers(correctHeightDifferences, correctDistances);
    }


}
