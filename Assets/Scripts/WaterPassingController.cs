using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterPassingController : MonoBehaviour
{
    [Header ("Prefabs")]
    public GameObject beacon;
    public GameObject measure;
    public GameObject groundPoint;
    public GameObject magnifyGlass;
    public LayerMask pointMask;
    [Header ("Changeable Parameters")]
    [Range (2,5)]
    public int nrOfPoints = 2;
    public int maxBeacons;
    public int maxMeasures;
    public bool showDistanceLaser;
    public bool lockMeasure;
    public Vector2 lockedMeasureLocation;
    public bool lockBeacon;
    public Vector2 lockedBeaconLocation;
    [Header ("Standard Parameters")]
    [Tooltip ("the angle of the upper and lower laserline to determine the distance")]
    public float distanceMeasureAngle;
    [Tooltip("the maximum error of the measure laser")]
    public float maxAngleError;
    [Tooltip("the minimum distance the points should be apart")]
    public float minDistance;

    
    private GameManager gm;
    private List<GameObject> groundPoints = new List<GameObject>();
    private List<GameObject> measures = new List<GameObject>();
    private List<GameObject> beacons = new List<GameObject>();

    // determine what mode the player is in
    private bool measureMode;
    private bool beaconMode;
    private bool magnifyMode;

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
        if(nrOfPoints > 0)
        {
            ChangePoints();
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

    public void ToggleMeasure()
    {
        measureMode = true;
        beaconMode = false;
        magnifyMode = false;
        magnifyGlass.SetActive(false);
    }

    public void ToggleBeacon()
    {
        measureMode = false;
        beaconMode = true;
        magnifyMode = false;
        magnifyGlass.SetActive(false);
    }

    //toggles the magnifyglass
    public void ToggleMagnify()
    {
        measureMode = false;
        beaconMode = false;
        magnifyMode = true;
        magnifyGlass.SetActive(true);
    }

    //places a measure object
    public void AddMeasure(Vector2 location)
    {
        GameObject newMeasure = Instantiate(measure, location, Quaternion.identity);
        correctErrorAngle = Random.Range(-maxAngleError, maxAngleError);
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

        

        newMeasure.transform.GetChild(0).GetChild(0).SetPositionAndRotation(newMeasure.transform.GetChild(0).GetChild(0).position,Quaternion.Euler(0, 0, distanceMeasureAngle));
        newMeasure.transform.GetChild(0).GetChild(1).SetPositionAndRotation(newMeasure.transform.GetChild(0).GetChild(1).position, Quaternion.Euler(0, 0, -distanceMeasureAngle));
        newMeasure.transform.GetChild(1).GetChild(0).SetPositionAndRotation(newMeasure.transform.GetChild(1).GetChild(0).position, Quaternion.Euler(0, 0, distanceMeasureAngle));
        newMeasure.transform.GetChild(1).GetChild(1).SetPositionAndRotation(newMeasure.transform.GetChild(1).GetChild(1).position, Quaternion.Euler(0, 0, -distanceMeasureAngle));

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

    //place new groundpoints
    public void AddGroundPoints()
    {
        float Increment = (gm.screenMax.x - gm.screenMin.x) / (float)nrOfPoints;

        if (groundPoints.Count < nrOfPoints)
        {
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
                GameObject lastPoint = groundPoints[i -1];
                groundPoints.RemoveAt(i - 1);

                Destroy(lastPoint);
            }
        }

        for (int i = 0; i < nrOfPoints; i++)
        {
            Vector2 newPos = new Vector2(gm.screenMin.x + Increment * i + Random.Range(minDistance / 2f, Increment - minDistance / 2f), Random.Range(gm.screenMin.y + 0.5f, gm.screenMax.y - 3f));
            groundPoints[i].transform.position = newPos;
            Debug.Log("moved " + i);
        }
        

        correctHeight = groundPoints[groundPoints.Count - 1].transform.position.y - groundPoints[0].transform.position.y;
        correctDistance = groundPoints[groundPoints.Count - 1].transform.position.x - groundPoints[0].transform.position.x;
        Debug.Log(correctHeight);

    }
  

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
