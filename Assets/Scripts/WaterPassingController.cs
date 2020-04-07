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
    public SpriteShapeController spriteShapeController;
    public Transform groundPointTopDownCenter;
    public WaterpassingTabel waterpassingTabel;
    [Header ("Changeable Parameters")]
    [Range (2,5)]
    public int nrOfPoints;
    public int maxBeacons;
    public int maxMeasures;
    public bool showDistanceLaser;
    public bool lockMeasure;
    public Vector2 lockedMeasureLocation;
    public bool lockBeacon;
    public Vector2 lockedBeaconLocation;
    public bool loopAround;
    [Header ("Standard Parameters")]
    [Tooltip ("the angle of the upper and lower laserline to determine the distance")]
    public float distanceMeasureAngle;
    public bool showAngleError;
    [Tooltip("the maximum error of the measure laser")]
    public float maxAngleError;
    [Tooltip("the minimum distance the points should be apart")]
    public float minDistance;
    [Tooltip("the max height of the points in % of the screen height")]
    [Range(0, 1)]
    public float maxHeightGroundPoint;
    public Color buttonBaseColor;
    public Color buttonActiveColor;

    
    private GameManager gm;
    private List<GameObject> groundPoints = new List<GameObject>();
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

        if (lockMeasure)
        {
            AddMeasure(lockedMeasureLocation);
        }
        if (lockBeacon)
        {
            AddBeacon(lockedBeaconLocation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (magnifyMode)
        {

            magnifyGlass.transform.position = gm.SetObjectToMouse(Input.mousePosition, -5);

        }
        else if (Input.GetMouseButton(0) && gm.IsBetweenValues(gm.SetObjectToMouse(Input.mousePosition, 0)))
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
        SetAngleErrorText();
        float laserlength;

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
            measures[i].transform.GetChild(0).transform.Rotate(0,0, -2 * measures[i].transform.GetChild(0).transform.eulerAngles.z);
            Debug.Log(measures[i].transform.GetChild(0).transform.eulerAngles);
        }

        
    }


    //places a measure object
    public void AddBeacon(Vector2 location)
    {
        GameObject newBeacon = Instantiate(beacon, location, Quaternion.identity);
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
        float Increment = (gm.screenMax.x - gm.screenMin.x -2) / (float)nrOfPoints;

        if (groundPoints.Count < nrOfPoints)
        {
            Debug.Log(groundPoints.Count + " " + nrOfPoints);
            
            for (int i = groundPoints.Count; i < nrOfPoints; i++)
            {
                GameObject newPoint = Instantiate(groundPoint, Vector2.zero, Quaternion.identity);
                newPoint.GetComponent<PolygonPointController>().SetNameText(i);
                groundPoints.Add(newPoint);
                Debug.Log("placed " + i);
            }
        }
        if (groundPoints.Count > nrOfPoints)
        {
            for (int i = groundPoints.Count; i > nrOfPoints; i--)
            {
                GameObject lastPoint = groundPoints[i -1];
                groundPoints.RemoveAt(i - 1);

                Destroy(lastPoint);
                Debug.Log("destroyed" + i);
            }
        }

        for (int i = 0; i < nrOfPoints; i++)
        {
            Vector2 newPos = new Vector2(gm.screenMin.x + 1  + Increment * i + Random.Range(minDistance / 2f, Increment - minDistance / 2f), Random.Range(0 , gm.screenMax.y * maxHeightGroundPoint));
            groundPoints[i].transform.position = newPos;
            Debug.Log("moved " + i);
        }

        
        correctHeight = groundPoints[groundPoints.Count - 1].transform.position.y - groundPoints[0].transform.position.y;
        correctDistance = groundPoints[groundPoints.Count - 1].transform.position.x - groundPoints[0].transform.position.x;
        Debug.Log(correctHeight);

    }
    public void SetGroundPointsTopDown()
    {
        float radius = (gm.screenMax.x - gm.screenMin.x) / ( 2 * Mathf.PI);
        //groundPointTopDownCenter.localScale = Vector3.one * radius;
        float cummAngle = 0f;
        

        for (int i = 0; i < nrOfPoints + 1; i++)
        {
            GameObject newpoint = Instantiate(groundPointTopDown, groundPointTopDownCenter.position + Vector3.right, Quaternion.identity);
            newpoint.GetComponent<PolygonPointController>().SetNameText(i-1);
            groundPointsTopDown.Add(newpoint);
        }

        cummAngle += (groundPoints[0].transform.position.x - gm.screenMin.x) / radius;
        groundPointsTopDown[1].transform.RotateAround(groundPointTopDownCenter.position, Vector3.back, cummAngle * Mathf.Rad2Deg);

        for (int i = 2; i < groundPointsTopDown.Count; i++)
        {

            cummAngle += (groundPoints[i-1].transform.position.x - groundPoints[i-2].transform.position.x) / radius;
            groundPointsTopDown[i].transform.RotateAround(groundPointTopDownCenter.position, Vector3.back, cummAngle * Mathf.Rad2Deg);
              
        }
        

        //Vector2 location = groundPointTopDownCenter.position +
    }


    public void SetAngleErrorText()
    {
        angleErrorText.text = "De hoekfout is: \n " + correctErrorAngle.ToString();
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
    public void SetParameters(int nrPoints, int nrBeacons, int nrMeasures, bool ShowDistance, bool lockmeasure, Vector2 measureLocation, bool lockbeacon, Vector2 beaconLocation)
    {
        nrOfPoints = nrPoints;
        maxBeacons = nrBeacons;
        maxMeasures = nrMeasures;
        showDistanceLaser = ShowDistance;
        lockMeasure = lockmeasure;
        lockedMeasureLocation = measureLocation;
        lockBeacon = lockbeacon;
        lockedBeaconLocation = beaconLocation;
    }

}
