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
    public GameObject groundPointTopDown;
    public GameObject magnifyGlass;
    public LayerMask pointMask;
    public Button magnifyButton;
    public Button beaconButton;
    public Button measureButton;
    public Text angleErrorText;
    public Text distanceAngleText;
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
    public bool showDistanceLaser;
    public bool lockMeasure;
    public Vector2 lockedMeasureLocation;
    public bool lockBeacon;
    public Vector2 lockedBeaconLocation;
    public bool loopAround;
    public bool addPointOutLoop;
    [Header ("Standard Parameters")]
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
    
    public Color buttonBaseColor;
    public Color buttonActiveColor;

    
    private GameManager gm;
    private List<GameObject> groundPoints = new List<GameObject>();
    private List<GameObject> topPoints = new List<GameObject>();
    private List<GameObject> measures = new List<GameObject>();
    private List<GameObject> beacons = new List<GameObject>();
    private List<GameObject> groundPointsTopDown = new List<GameObject>();

    // determine what mode the player is in
    private bool measureMode;
    private bool beaconMode;
    private bool magnifyMode;
    private ColorBlock buttonColorBase;
    private ColorBlock buttonColorActive;

    private bool holdingObject;
    private GameObject hitObject;

    [HideInInspector]
    public float correctHeight;
    [HideInInspector]
    public float[] correctHeightDifferences;
    [HideInInspector]
    public float correctDistance;
    [HideInInspector]
    public float correctErrorAngle;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        magnifyGlass.SetActive(false);

        SetButtonColors();

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
        if (lockBeacon)
        {
            AddBeacon(lockedBeaconLocation);
            AddBeacon(new Vector2(lockedMeasureLocation.x * 2 - lockedBeaconLocation.x, lockedBeaconLocation.y));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (magnifyMode)
        {
            if (MouseOverBeacon())
            {
                magnifyGlass.SetActive(true);
                magnifyGlass.transform.position = gm.SetObjectToMouse(Input.mousePosition, -5);
            }
            else magnifyGlass.SetActive(false);
            

        }
        if (Input.GetMouseButton(0) && gm.IsBetweenValues(gm.SetObjectToMouse(Input.mousePosition, 0)))
        {
            if (!holdingObject)
            {
                hitObject = CastMouseRay();

                if (!holdingObject && beaconMode && beacons.Count < maxBeacons)
                {
                    AddBeacon(gm.SetObjectToMouse(Input.mousePosition, 0));
                }
                else if (!holdingObject && measureMode && measures.Count < maxMeasures)
                {
                    AddMeasure(gm.SetObjectToMouse(Input.mousePosition, 0));
                }
            }
            else
            {
                hitObject.transform.position = gm.SetObjectToMouse(Input.mousePosition, 0);
            }
            
        }
        else holdingObject = false;

    }
    public void SetButtonColors()
    {
        buttonColorBase = magnifyButton.colors;
        buttonColorBase.normalColor = buttonBaseColor;
        buttonColorBase.selectedColor = buttonBaseColor;
        buttonColorActive = magnifyButton.colors;
        buttonColorActive.normalColor = buttonActiveColor;
        buttonColorActive.selectedColor = buttonActiveColor;

        magnifyButton.colors = buttonColorBase;
        beaconButton.colors = buttonColorBase;
        measureButton.colors = buttonColorBase;
    }

    public void SetGroundSprite()
    {
        spriteShapeController.spline.Clear();
        spriteShapeController.spline.InsertPointAt(0, new Vector3(gm.screenMax.x, -0.5f));
        
        for (int i = nrOfPoints; i > 0 ; i--)
        {
            spriteShapeController.spline.InsertPointAt(0, groundPoints[i-1].transform.position + 0.5f * Vector3.down);
        }
        spriteShapeController.spline.InsertPointAt(0, new Vector3(gm.screenMin.x, -0.5f));

        for (int i = 0; i < spriteShapeController.spline.GetPointCount(); i++)
        {
            spriteShapeController.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
            spriteShapeController.spline.SetHeight(i, 0.1f);
            spriteShapeController.spline.SetLeftTangent(i, Vector3.left);
            spriteShapeController.spline.SetRightTangent(i, Vector3.right);
        }

    }
    public void SetTopSprite()
    {
        topSpriteShapeController.spline.Clear();
        topSpriteShapeController.spline.InsertPointAt(0, new Vector3(gm.screenMin.x,gm.screenMax.y -0.5f));

        for (int i = 0; i < nrOfTopPoints; i++)
        {
            topSpriteShapeController.spline.InsertPointAt(0, topPoints[i].transform.position + 0.5f * Vector3.up);
            
        }
        topSpriteShapeController.spline.InsertPointAt(0, new Vector3(gm.screenMax.x, gm.screenMax.y - 0.5f));

        for (int i = 0; i < topSpriteShapeController.spline.GetPointCount(); i++)
        {
            topSpriteShapeController.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
            topSpriteShapeController.spline.SetHeight(i, 0.1f);
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

    //toggles the different modes
    public void ToggleMeasure()
    {
        measureMode = true;
        beaconMode = false;
        magnifyMode = false;
        magnifyGlass.SetActive(false);

        magnifyButton.colors = buttonColorBase;
        beaconButton.colors = buttonColorBase;
        measureButton.colors = buttonColorActive;
    }

    public void ToggleBeacon()
    {
        measureMode = false;
        beaconMode = true;
        magnifyMode = false;
        magnifyGlass.SetActive(false);

        magnifyButton.colors = buttonColorBase;
        beaconButton.colors = buttonColorActive;
        measureButton.colors = buttonColorBase;
    }

    public void ToggleMagnify()
    {
        measureMode = false;
        beaconMode = false;
        magnifyMode = true;
        magnifyGlass.SetActive(true);

        magnifyButton.colors = buttonColorActive;
        beaconButton.colors = buttonColorBase;
        measureButton.colors = buttonColorBase;
    }
    


    //places a measure object
    public void AddMeasure(Vector2 location)
    {
        GameObject newMeasure = Instantiate(measure, location, Quaternion.identity);
        correctErrorAngle = Random.Range(-maxAngleError, maxAngleError);
        newMeasure.GetComponent<MeasureController>().errorAngle = correctErrorAngle;
        SetAngleErrorText();
        SetDistanceAngleText();
        
        /*
        if (nrOfPoints > 0)
        {
            laserlength = (gm.screenMax.x - gm.screenMin.x) / nrOfPoints;
        }
        else
        {
            laserlength = (gm.screenMax.x - gm.screenMin.x);
        }

        LineRenderer[] laserlines = newMeasure.transform.GetComponentsInChildren<LineRenderer>();
        foreach (var laserline in laserlines)
        {
            laserline.SetPosition(0, -laserlength * Vector3.right);
            laserline.SetPosition(1, laserlength * Vector3.right);
        }
        
        newMeasure.transform.GetChild(0).transform.Rotate(0, 0, correctErrorAngle);

        newMeasure.transform.GetChild(0).GetChild(0).localEulerAngles = new Vector3(0, 0, distanceMeasureAngle);
        newMeasure.transform.GetChild(0).GetChild(1).localEulerAngles = new Vector3(0, 0, -distanceMeasureAngle);
        newMeasure.transform.GetChild(1).GetChild(0).localEulerAngles = new Vector3(0, 0, distanceMeasureAngle);
        newMeasure.transform.GetChild(1).GetChild(1).localEulerAngles = new Vector3(0, 0, -distanceMeasureAngle);

        newMeasure.transform.GetChild(0).GetChild(0).gameObject.SetActive(showDistanceLaser);
        newMeasure.transform.GetChild(0).GetChild(1).gameObject.SetActive(showDistanceLaser);
        newMeasure.transform.GetChild(1).GetChild(0).gameObject.SetActive(showDistanceLaser);
        newMeasure.transform.GetChild(1).GetChild(1).gameObject.SetActive(showDistanceLaser);
        */
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

    public void RotateMeasure(string input)
    {
        float angleinput = float.Parse(input);
        for (int i = 0; i < measures.Count; i++)
        {
            measures[i].GetComponent<MeasureController>().scheefstandsHoek = angleinput * 4 / 3.6f;
            Debug.Log(angleinput * 4 / 3.6f);
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
        AddGroundPoints();
        if (placeTopPoints)
        {
            addTopPoints();

        }

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
        GameObject newPoint = Instantiate(groundPoint, new Vector2(gm.screenMin.x,0), Quaternion.identity);
        newPoint.GetComponent<PolygonPointController>().SetNameNrText(0);

        newPoint = Instantiate(groundPoint, new Vector2(gm.screenMax.x, 0), Quaternion.identity);
        newPoint.GetComponent<PolygonPointController>().SetNameNrText(0);
    }

    //place new groundpoints
    public void AddGroundPoints()
    {
        float Increment = (gm.screenMax.x - gm.screenMin.x - 2) / (float)nrOfPoints;

        if (groundPoints.Count < nrOfPoints)
        {
            Debug.Log(groundPoints.Count + " " + nrOfPoints);

            for (int i = groundPoints.Count; i < nrOfPoints; i++)
            {
                GameObject newPoint = Instantiate(groundPoint, Vector2.zero, Quaternion.identity);
                newPoint.GetComponent<PolygonPointController>().SetNameText(i + 1);
                groundPoints.Add(newPoint);
                Debug.Log("placed " + i);
            }
        }
        if (groundPoints.Count > nrOfPoints)
        {
            for (int i = groundPoints.Count; i > nrOfPoints; i--)
            {
                GameObject lastPoint = groundPoints[i - 1];
                groundPoints.RemoveAt(i - 1);

                Destroy(lastPoint);
                Debug.Log("destroyed" + i);
            }
        }

        for (int i = 0; i < nrOfPoints; i++)
        {
            Vector2 newPos = new Vector2(gm.screenMin.x + 1 + Increment * i + Random.Range(minDistance / 2f, Increment - minDistance / 2f), Random.Range(0, (gm.screenMax.y) * maxHeightGroundPoint));
            groundPoints[i].transform.position = newPos;
            Debug.Log("moved " + i);
        }


        correctHeight = groundPoints[groundPoints.Count - 1].transform.position.y - groundPoints[0].transform.position.y;
        correctDistance = groundPoints[groundPoints.Count - 1].transform.position.x - groundPoints[0].transform.position.x;
        Debug.Log(correctHeight);

        if (loopAround)
        {
            correctHeightDifferences = new float[nrOfPoints + 1];
            correctHeightDifferences[0] = groundPoints[0].transform.position.y;

            for (int i = 1; i < nrOfPoints; i++)
            {
                correctHeightDifferences[i] = groundPoints[i].transform.position.y - groundPoints[i - 1].transform.position.y;
            }
            correctHeightDifferences[nrOfPoints] = -groundPoints[nrOfPoints-1].transform.position.y;

            for (int i = 0; i < nrOfPoints+1; i++)
            {
                Debug.Log(correctHeightDifferences[i]);
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
                GameObject newPoint = Instantiate(groundPoint, Vector2.zero, Quaternion.identity);
                newPoint.GetComponent<PolygonPointController>().SetNameText(i+1);
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
            Vector2 newPos = new Vector2(gm.screenMin.x + 1 + Increment * i + Random.Range(minDistance / 2f, Increment - minDistance / 2f), Random.Range(gm.screenMax.y,  (gm.screenMax.y) * (1 - maxHeightGroundPoint)));
            topPoints[i].transform.position = newPos;
            Debug.Log("moved top" + i);
        }


        //correctHeight = groundPoints[groundPoints.Count - 1].transform.position.y - groundPoints[0].transform.position.y;
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
            GameObject newpoint = Instantiate(groundPointTopDown, groundPointTopDownCenter.position + Vector3.right * ((addPointOutLoop && i == pointOutLoopNr) ? 1.5f : 1), Quaternion.identity);
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
            

            line.SetPosition(i, groundPointsTopDown[j].transform.position - Vector3.forward * 10);
        }
        

        //Vector2 location = groundPointTopDownCenter.position +
    }


    public void SetAngleErrorText()
    {
        angleErrorText.text = "De Collimatiefout is: \n " + (Mathf.Round(correctErrorAngle * 100 * (4/3.6f)) / 100).ToString() + "gon";
    }

    public void SetDistanceAngleText()
    {
        distanceAngleText.text = "Divergentiecoefficient: \n " + (distanceMeasureAngle * 400/360f).ToString() + "gon";
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
        nrOfPoints = nrPoints;
        maxBeacons = nrBeacons;
        maxMeasures = nrMeasures;
        showDistanceLaser = ShowDistance;
        lockMeasure = lockmeasure;
        lockedMeasureLocation = measureLocation;
        lockBeacon = lockbeacon;
        lockedBeaconLocation = beaconLocation;
        loopAround = loop;
    }
    public bool CheckTabelAnswer()
    {
        return waterpassingTabel.CheckAnswers(correctHeightDifferences);
    }


}
