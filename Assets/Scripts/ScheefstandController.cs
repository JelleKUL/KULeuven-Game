using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScheefstandController : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject theodoliet;
    public GameObject skewBuilding;
    public Transform MeasurePlacer;
    public Button magnifyButton;
    public LayerMask pointMask;

    [Header("Variables")]
    public float maxSkewAngle;
    public Vector2 skewBuildingLocation;

    [HideInInspector]
    public float correctDistance;

    private GameObject building;
    public Transform[] points = new Transform[2];
    private GameManager gm;
    private GameObject hitObject;
    private GameObject theodolietObject;
    private bool holdingObject;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();


        PlaceBulding();
        theodolietObject = Instantiate(theodoliet, MeasurePlacer.position, Quaternion.identity);
        theodolietObject.GetComponent<Theodoliet>().scheefstandController = this;
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
            if (hitObject != null && hitObject.tag != "MagnifyGlass")
            {
                hitObject.transform.position = gm.SetObjectToMouse(Input.mousePosition, 0);
                hitObject.GetComponent<Physics2DObject>().isHeld = true;
            }

        }

        if (Input.GetMouseButtonUp(0))
        {
            if (holdingObject && hitObject.tag != "MagnifyGlass")
            {
                if (!gm.IsBetweenValues(gm.SetObjectToMouse(Input.mousePosition, 0)))
                {
                    
                    if (hitObject.tag == "Measure")
                    {
                        hitObject.transform.position = MeasurePlacer.position;
                    }
                    
                }
                hitObject.GetComponent<Physics2DObject>().isHeld = false;
            }

            holdingObject = false;

        }
    }
        void PlaceBulding()
        {
            building = Instantiate(skewBuilding, skewBuildingLocation, Quaternion.Euler(0, 0, Random.Range(-maxSkewAngle, maxSkewAngle)));
            correctDistance = building.GetComponent<SkewBuildingController>().getDistance();
            points = building.GetComponent<SkewBuildingController>().beaconPoints;
        }

        //returns the gamobject the mouse has hit
        public GameObject CastMouseRay()
        {
            RaycastHit2D rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), 20, pointMask);

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
        theodolietObject.GetComponent<Theodoliet>().ToggleMagnify() ;
    }


}
