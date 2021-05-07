using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

//*********** The WaterPassingController controls most aspects of the waterpassing type exercise ******************//


public class WaterPassingController : MonoBehaviour
{
    [Header("Changeable Parameters")]
    [Range(0, 5)]
    [SerializeField] private int nrOfPoints;
    [Range(0, 3)]
    [SerializeField] private int nrOfTopPoints;
    [SerializeField] private int maxBeacons;
    [SerializeField] private int maxMeasures;

    [SerializeField] private bool lockMeasure;
    [SerializeField] private Vector2 lockedMeasureLocation;
    [SerializeField] private bool lockBeacon;
    [SerializeField] private List<Vector2> lockedBeaconLocations = new List<Vector2>();
    [Tooltip("create a height difference on the locked measures locations, only relevant if beacons are locked")]
    [SerializeField] private bool showLockedHeightDiff = false;
    [SerializeField] private bool loopAround;
    [SerializeField] private bool addPointOutLoop;


    [Header("Standard Parameters")]
    [SerializeField] private bool showAngleError;
    [Tooltip("the max deviation in 1 direction in degrees, before the scaling factor is applied")]
    [SerializeField] private float maxAngleError;
    [Tooltip("the minimum distance the points should be apart")]
    [SerializeField] private float minDistance;
    [Tooltip("the max height of the points in % of the screen height")]
    [Range(0, 0.5f)]
    [SerializeField] private float maxHeightGroundPoint;
    [SerializeField] private bool extremeHeightDiff;
    [Tooltip("the offset from the edge of the playarea for the edgepoints")]
    [SerializeField] private float startAndEndPointOffset = 0.1f;
    [SerializeField] private float topDownPointRaduis = 0.5f;

    [System.Serializable]
    private class Prefabs
    {
        public GameObject beacon;
        public GameObject measure;
        public GameObject groundPoint;
        public GameObject topPoint;
        public GameObject groundPointTopDown;
    }
    [System.Serializable]
    private class SceneObjects
    {
        public LayerMask pointMask;
        public Transform BeaconPlacer;
        public Transform MeasurePlacer;
        public Text angleErrorText;
        public Text rotationAngleText;
        public SpriteShapeController spriteShapeController;
        public SpriteShapeController topSpriteShapeController;
        public Transform groundPointTopDownCenter;
        public WaterpassingTabel waterpassingTabel;
    }
    [Header("Scene Objects")]
    [SerializeField]
    [Space(20)]
    private Prefabs prefabs;
    [SerializeField]
    private SceneObjects sceneObjects;

    //private Variables
    private GameManager gm;
    private List<GameObject> groundPoints = new List<GameObject>();
    private List<GameObject> topPoints = new List<GameObject>();
    private List<GameObject> measures = new List<GameObject>();
    private List<GameObject> beacons = new List<GameObject>();
    private List<GameObject> groundPointsTopDown = new List<GameObject>();

    private bool holdingObject;
    private GameObject hitObject;
    private bool hasStarted;
    private int pointOutLoopNr = -1;

    // values used by other scripts, the correct answers
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
    [HideInInspector]
    public float correctScaledErrorAngle;

    // Start is called before the first frame update
    private void Start()
    {
        if (!hasStarted) StartSetup(); //only calls when Start() hasn't gone yet
    }

    // the startscript, can be called by the setparametersfunction to get the correct answers before the start function is called in this script
    public void StartSetup()
    {
        hasStarted = true; 

        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        correctErrorAngle = Random.Range(0, maxAngleError); //in degrees
        correctScaledErrorAngle = Mathf.Atan(Mathf.Tan(correctErrorAngle * Mathf.Deg2Rad) / (float)GameManager.worldScale) * Mathf.Rad2Deg; //in degrees

        if (nrOfPoints > 0)
        {
            ChangePoints(); // place the points on the field
        }
       
        if (loopAround) //make the loop
        {
            AddStartAndEndGroundPoint(); 
            SetGroundPointsTopDown();
            sceneObjects.waterpassingTabel.CreateTable(nrOfPoints + 1, pointOutLoopNr);
        }

        SetGroundSprite();
        SetTopSprite();

        if (lockMeasure)
        {
            AddMeasure(lockedMeasureLocation);
        }
        else
        {
            float offset = 0f;
            if(prefabs.measure.TryGetComponent(out BoxCollider2D box))
            {
                offset = box.offset.y;
            }
            AddMeasure(sceneObjects.MeasurePlacer.position - sceneObjects.MeasurePlacer.up * offset);
        }

        if (lockBeacon)
        {
            foreach (var location in lockedBeaconLocations)
            {
                AddBeacon(location);
            }
        }
        else
        {
            float offset = 0f;
            if (prefabs.beacon.TryGetComponent(out BoxCollider2D box))
            {
                offset = box.offset.y;
            }
            AddBeacon(sceneObjects.BeaconPlacer.position - sceneObjects.BeaconPlacer.up * offset);
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
                        float offset = 0f;
                        if (prefabs.measure.TryGetComponent(out BoxCollider2D box))
                        {
                            offset = box.offset.y;
                        }
                        AddMeasure(sceneObjects.MeasurePlacer.position - sceneObjects.MeasurePlacer.up * offset);
                    }
                    else if (hitObject.tag == "Beacon" && beacons.Count < maxBeacons)
                    {
                        float offset = 0f;
                        if (prefabs.beacon.TryGetComponent(out BoxCollider2D box))
                        {
                            offset = box.offset.y;
                        }
                        AddBeacon(sceneObjects.BeaconPlacer.position - sceneObjects.BeaconPlacer.up * offset);
                    }
                }
                else
                {
                    //calculate the spawn offset
                    float offset = 0f;
                    if (hitObject.TryGetComponent(out BoxCollider2D box))
                    {
                        offset = box.offset.y;
                    }

                    if (hitObject.tag == "Measure")
                    {
                        hitObject.transform.position = sceneObjects.MeasurePlacer.position - sceneObjects.MeasurePlacer.up * offset;
                    }
                    else if (hitObject.tag == "Beacon")
                    {
                        
                        hitObject.transform.position = sceneObjects.BeaconPlacer.position - sceneObjects.BeaconPlacer.up * offset;
                    }
                }
                hitObject.GetComponent<Physics2DObject>().isHeld = false;
            }

            holdingObject = false;

        }
       
    }
    

    public void SetGroundSprite()
    {
        //add the anchorpoints at the under and right side
        sceneObjects.spriteShapeController.spline.Clear();
        sceneObjects.spriteShapeController.spline.InsertPointAt(0, new Vector3(-10,-3));
        sceneObjects.spriteShapeController.spline.InsertPointAt(0, new Vector3(20, -3));
        sceneObjects.spriteShapeController.spline.InsertPointAt(0, new Vector3(20, -sceneObjects.spriteShapeController.colliderOffset));
        sceneObjects.spriteShapeController.spline.InsertPointAt(0, new Vector3(gm.screenMax.x, -sceneObjects.spriteShapeController.colliderOffset));

        // add locked beacon positions
        if(lockBeacon && showLockedHeightDiff)
        {
            for (int i = lockedBeaconLocations.Count; i > 0; i--)
            {
                sceneObjects.spriteShapeController.spline.InsertPointAt(0, (Vector3)lockedBeaconLocations[i-1] + sceneObjects.spriteShapeController.colliderOffset * Vector3.down);
            }
        }
        //or add the points positions
        else
        {
            for (int i = nrOfPoints; i > 0; i--)
            {
                sceneObjects.spriteShapeController.spline.InsertPointAt(0, groundPoints[i - 1].transform.position + sceneObjects.spriteShapeController.colliderOffset * Vector3.down);
            }
        }
        // add the left points
        sceneObjects.spriteShapeController.spline.InsertPointAt(0, new Vector3(gm.screenMin.x, -sceneObjects.spriteShapeController.colliderOffset));
        sceneObjects.spriteShapeController.spline.InsertPointAt(0, new Vector3(-10, -sceneObjects.spriteShapeController.colliderOffset));

        // set the tangents for a smooth shape
        for (int i = 0; i < sceneObjects.spriteShapeController.spline.GetPointCount(); i++)
        {
            sceneObjects.spriteShapeController.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
            sceneObjects.spriteShapeController.spline.SetLeftTangent(i, Vector3.left);
            sceneObjects.spriteShapeController.spline.SetRightTangent(i, Vector3.right);
        }

    }
    public void SetTopSprite()
    {
        sceneObjects.topSpriteShapeController.spline.Clear();

        sceneObjects.topSpriteShapeController.spline.InsertPointAt(0, new Vector3(-2, gm.screenMax.y - 0.5f));
        sceneObjects.topSpriteShapeController.spline.InsertPointAt(0, new Vector3(gm.screenMin.x, gm.screenMax.y - 0.5f));

        for (int i = 0; i < nrOfTopPoints; i++)
        {
            sceneObjects.topSpriteShapeController.spline.InsertPointAt(0, topPoints[i].transform.position + 0.5f * Vector3.up);
            
        }

        sceneObjects.topSpriteShapeController.spline.InsertPointAt(0, new Vector3(gm.screenMax.x, gm.screenMax.y - 0.5f));
        sceneObjects.topSpriteShapeController.spline.InsertPointAt(0, new Vector3(18, gm.screenMax.y - 0.5f));
        sceneObjects.topSpriteShapeController.spline.InsertPointAt(0, new Vector3(18, 10));
        sceneObjects.topSpriteShapeController.spline.InsertPointAt(0, new Vector3(-2, 10));

        for (int i = 0; i < sceneObjects.topSpriteShapeController.spline.GetPointCount(); i++)
        {
            sceneObjects.topSpriteShapeController.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
            //topSpriteShapeController.spline.SetHeight(i, 0.1f);
            sceneObjects.topSpriteShapeController.spline.SetLeftTangent(i, Vector3.right);
            sceneObjects.topSpriteShapeController.spline.SetRightTangent(i, Vector3.left);
        }

    }

    //returns the gamobject the mouse has hit
    public GameObject CastMouseRay()
    {
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition),20, sceneObjects.pointMask);

        if (rayHit.collider != null)
        {
            holdingObject = true;
            //Debug.Log(rayHit.transform.gameObject.name);
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
        GameObject newMeasure = Instantiate(prefabs.measure, location, Quaternion.identity);
        MeasureController measureController = newMeasure.GetComponent<MeasureController>();
        measureController.errorAngle = correctErrorAngle;

        if (nrOfPoints > 1 && !loopAround)
        {
            measureController.maxDistance = maxDistanceBetweenPoints(1) * 0.6f; //(gm.screenMax.x - gm.screenMin.x) / (nrOfPoints * 2) + 0.4f * minDistance; //- 0.7f*minDistance;
        }
        else if (loopAround)
        {
            measureController.maxDistance = maxDistanceBetweenPoints(2) * 0.6f; //(gm.screenMax.x - gm.screenMin.x) / (nrOfPoints * 2) + 0.4f * minDistance; //- 0.7f*minDistance;
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
            //.Log(measures[i].GetComponent<MeasureController>().errorAngle);
        }

        
    }

    //sets the scheefstandangle of all the measures

    public void RotateMeasure(float angleInput)
    {
        if (sceneObjects.rotationAngleText)
        {
            sceneObjects.rotationAngleText.text = GameManager.RoundFloat(angleInput,3).ToString() + " gon";
        }
        for (int i = 0; i < measures.Count; i++)
        {
            measures[i].GetComponent<MeasureController>().UpdateMeasureHeadRotation(angleInput * 3.6f / 4f);
            //Debug.Log(angleInput);
        }


    }


    //places a measure object
    public void AddBeacon(Vector2 location)
    {

        GameObject newBeacon = Instantiate(prefabs.beacon, location, Quaternion.identity);
        newBeacon.GetComponent<Physics2DObject>().allowUpsideDown = nrOfTopPoints > 0;
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
        AddGroundPoints(nrOfTopPoints > 0);
        

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
            foreach (var location in lockedBeaconLocations)
            {
                AddBeacon(location);
            }

            
        }

    }

    public void AddStartAndEndGroundPoint()
    {
        GameObject newPoint = Instantiate(prefabs.groundPoint, new Vector2(gm.screenMin.x+ startAndEndPointOffset, 0), Quaternion.identity);
        newPoint.GetComponent<PolygonPointController>().SetNrText(0);

        newPoint = Instantiate(prefabs.groundPoint, new Vector2(gm.screenMax.x - startAndEndPointOffset, 0), Quaternion.identity);
        newPoint.GetComponent<PolygonPointController>().SetNrText(0);
    }

    //place new groundpoints
    public void AddGroundPoints( bool placeTop)
    {
        
        float Increment = (gm.screenMax.x - gm.screenMin.x - 2) / (float)(nrOfPoints + (placeTop? nrOfTopPoints : 0));
        int pointSkipper = 0;

        // creating the groundpoints
        if (groundPoints.Count < nrOfPoints)
        {
            //Debug.Log(groundPoints.Count + " placed -> " + nrOfPoints + " to Place");
            pointSkipper = 0;
            for (int i = groundPoints.Count; i < nrOfPoints; i++)
            {
                GameObject newPoint = Instantiate(prefabs.groundPoint, Vector2.zero, Quaternion.identity);
                
                newPoint.GetComponent<PolygonPointController>().SetNameText(i + 1 + pointSkipper);
                pointSkipper += (nrOfTopPoints > i) ? 1 : 0;
                groundPoints.Add(newPoint);
                //Debug.Log("placed " + i + 1 + pointSkipper);
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
                //Debug.Log("moved " + i);
            }
        }
        

        // creating the toppoints
        if (topPoints.Count < nrOfTopPoints && placeTop)
        {
            //Debug.Log(topPoints.Count + " " + nrOfTopPoints);
            pointSkipper = 1;
            for (int i = topPoints.Count; i < nrOfTopPoints; i++)
            {
                GameObject newPoint = Instantiate(prefabs.topPoint, Vector2.zero, Quaternion.identity);
                newPoint.GetComponent<PolygonPointController>().SetNameText(i + 1 + pointSkipper);
                pointSkipper += (nrOfPoints > i) ? 1 : 0;
                topPoints.Add(newPoint);
                //Debug.Log("placed " + i);
            }

            // moving the toppoints to the correct position
            pointSkipper = 1;
            for (int i = 0; i < nrOfTopPoints; i++)
            {
                Vector2 newPos = new Vector2(gm.screenMin.x + 1 + Increment * (i + pointSkipper) + Random.Range(minDistance / 2f, Increment - minDistance / 2f), Random.Range(gm.screenMax.y - 0.5f, (gm.screenMax.y - 0.5f) * (1 - maxHeightGroundPoint)));
                topPoints[i].transform.position = newPos;
                pointSkipper += (nrOfPoints > i) ? 1 : 0;
                //Debug.Log("moved top " + i);
            }
        }

        if (!placeTop)
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
        //Debug.Log(correctHeight);


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

            if (GameManager.showDebugAnswer)
            {
                for (int i = 0; i < nrOfPoints + 1; i++)
                {
                    Debug.Log(i + "-> HeightDiff: " + correctHeightDifferences[i] + " m, Distance: " + correctDistances[i] * GameManager.worldScale);
                }
            }
            
        }
        

        

    }
    
    //sets the top down points for visual reference
    public void SetGroundPointsTopDown()
    {
        float radius = (gm.screenMax.x - gm.screenMin.x) / ( 2 * Mathf.PI);
        //groundPointTopDownCenter.localScale = Vector3.one * radius;
        float cummAngle = 0f;
        pointOutLoopNr = -1;
        //chooses one point to leave out of the loop
        if (addPointOutLoop)
        {
            pointOutLoopNr = Random.Range(1, nrOfPoints);
        }
        

        for (int i = 0; i < nrOfPoints + 1; i++)
        {
            // instantiates a point and moves a point out of the loop further away
            GameObject newpoint = Instantiate(prefabs.groundPointTopDown, sceneObjects.groundPointTopDownCenter.position + Vector3.right * topDownPointRaduis * ((addPointOutLoop && i == pointOutLoopNr) ? 1.5f : 1), Quaternion.identity);
            newpoint.GetComponent<PolygonPointController>().SetNameText(i==0? -1:i);
            groundPointsTopDown.Add(newpoint);
        }

        cummAngle += (groundPoints[0].transform.position.x - gm.screenMin.x) / radius;
        groundPointsTopDown[1].transform.RotateAround(sceneObjects.groundPointTopDownCenter.position, Vector3.back, cummAngle * Mathf.Rad2Deg);

        for (int i = 2; i < groundPointsTopDown.Count; i++)
        {

            cummAngle += (groundPoints[i-1].transform.position.x - groundPoints[i-2].transform.position.x) / radius;
            groundPointsTopDown[i].transform.RotateAround(sceneObjects.groundPointTopDownCenter.position, Vector3.back, cummAngle * Mathf.Rad2Deg);
              
        }
        LineRenderer line = sceneObjects.groundPointTopDownCenter.gameObject.GetComponent<LineRenderer>();
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
            sceneObjects.angleErrorText.text = "Collimatiefout: \n " + GameManager.RoundFloat(correctScaledErrorAngle * (4 / 3.6f), 3).ToString() + " gon";
        }
        else sceneObjects.angleErrorText.text = "Collimatiefout: \n " + "? gon";


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
    public void SetParameters(int nrPoints, int nrBeacons, int nrMeasures, bool lockmeasure, Vector2 measureLocation, bool lockbeacon, List <Vector2> beaconLocation, bool loop)
    {
        nrOfPoints = nrPoints;
        maxBeacons = nrBeacons;
        maxMeasures = nrMeasures;
        lockMeasure = lockmeasure;
        lockedMeasureLocation = measureLocation;
        lockBeacon = lockbeacon;
        lockedBeaconLocations = beaconLocation;
        loopAround = loop;

        StartSetup();
    }
    public bool CheckTabelAnswer()
    {
        return sceneObjects.waterpassingTabel.CheckAnswers(correctHeightDifferences, correctDistances);
    }

    public void ShowAnswersTabel()
    {
        sceneObjects.waterpassingTabel.ShowCorrectValues(correctHeightDifferences, correctDistances);
    }

    float maxDistanceBetweenPoints(int nrBtwn)
    {
        float maxDist = 0f;

        for (int i = 0; i < nrOfPoints - nrBtwn; i++)
        {
            float newDist = Vector2.Distance(groundPoints[i].transform.position, groundPoints[(i + nrBtwn)].transform.position);
            //Debug.Log(i + "newDist: " + newDist + " coor: " + groundPoints[i].transform.position + " 2: " + groundPoints[(i) % (nrOfPoints)].transform.position);
            if ( newDist > maxDist)
            {
                maxDist = newDist;
            }
        }
        //Debug.Log(maxDist);
        return maxDist;
    }


}
