using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//*************** Manages the scheefstand oefening ****************//

public class ScheefstandController : BaseController
{
    [Header("Variables")]
    public float maxSkewAngle;
    public float maxSkewError;
    public Vector2 skewBuildingLocation;

    [System.Serializable]
    private class Prefabs
    {
        public GameObject theodoliet;
        public GameObject skewBuilding;
    }
    [System.Serializable]
    private class SceneObjects
    {
        public LayerMask pointMask;
        public Transform MeasurePlacer;
    }
    [Header("Scene Objects")]
    [SerializeField]
    [Space(20)]
    private Prefabs prefabs;
    [SerializeField]
    private SceneObjects sceneObjects;

    [HideInInspector]
    public float correctDistance;
    [HideInInspector]
    public Transform[] points = new Transform[2];

    private GameObject building;
    private GameManager gm;
    private GameObject hitObject;
    private GameObject theodolietObject;
    private float skewError;
    private bool isFlipped;

    // the startscript, can be called by the setparametersfunction to get the correct answers before the start function is called in this script
    public override void StartSetup()
    {
        base.StartSetup();

        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        skewError = Random.Range(-maxSkewError, maxSkewError);

        PlaceBulding();
        float offset = 0f;
        if (prefabs.theodoliet.TryGetComponent(out BoxCollider2D box))
        {
            offset = box.offset.y;
        }
        theodolietObject = Instantiate(prefabs.theodoliet, sceneObjects.MeasurePlacer.position - sceneObjects.MeasurePlacer.up * offset, Quaternion.identity);
        theodolietObject.GetComponent<Theodoliet>().scheefstandController = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && gm.IsBetweenValues(gm.SetObjectToMouse(Input.mousePosition, 0)))
        {
            hitObject = CastMouseRay(sceneObjects.pointMask);

            if (hitObject != null && hitObject.tag == "MagnifyGlass")
            {

                hitObject.GetComponent<MagnifyGlass>().ToggleZoom();
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (!holdingObject)
            {
                hitObject = CastMouseRay(sceneObjects.pointMask);
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
                        float offset = 0f;
                        if (hitObject.TryGetComponent(out BoxCollider2D box))
                        {
                            offset = box.offset.y;
                        }
                        hitObject.transform.position = sceneObjects.MeasurePlacer.position - sceneObjects.MeasurePlacer.up * offset;
                    }
                    
                }
                hitObject.GetComponent<Physics2DObject>().isHeld = false;
            }

            holdingObject = false;

        }
    }

    void PlaceBulding()
    {
        building = Instantiate(prefabs.skewBuilding, skewBuildingLocation, Quaternion.Euler(0, 0, Random.Range(-maxSkewAngle, maxSkewAngle)));
        SkewBuildingController skewBuildingController = building.GetComponent<SkewBuildingController>();
        
        correctDistance = skewBuildingController.getDistance();
        points = skewBuildingController.beaconPoints;
        skewBuildingController.SetLine(skewError);
    }

    public void ToggleMagnify()
    {
        theodolietObject.GetComponent<Theodoliet>().ToggleMagnify() ;
    }

    public void FlipTheodoliet()
    {
        isFlipped = !isFlipped;

        building.GetComponent<SkewBuildingController>().SetLine(skewError * (isFlipped? -1: 1));
    }
}
