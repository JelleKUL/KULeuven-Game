using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//*************** Manages the scheefstand oefening ****************//

public class ScheefstandController : MonoBehaviour
{
    [Header("Prefabs")]
    
    public GameObject theodoliet;
    public GameObject skewBuilding;
    public GameObject winMenu, winMenuFree;
    public Transform MeasurePlacer;
    public Button magnifyButton;
    public LayerMask pointMask;
    public Text answerText;
    public Text answerOutput;

    public Color falseColor, CorrectColor;

    [Header("Variables")]
    public float maxSkewAngle;
    public float maxSkewError;
    public Vector2 skewBuildingLocation;
    public int scoreIncrease;

    [HideInInspector]
    public float correctDistance;

    private GameObject building;
    [HideInInspector]
    public Transform[] points = new Transform[2];
    private GameManager gm;
    private GameObject hitObject;
    private GameObject theodolietObject;
    private bool holdingObject;
    private float skewError;
    private bool isFlipped;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        skewError = Random.Range(-maxSkewError, maxSkewError);

        PlaceBulding();
        theodolietObject = Instantiate(theodoliet, MeasurePlacer.position, Quaternion.identity);
        theodolietObject.GetComponent<Theodoliet>().scheefstandController = this;

        if (GameManager.showDebugAnswer)
        {
            Debug.Log("Correct distance = " + Mathf.Abs(correctDistance));
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
        SkewBuildingController skewBuildingController = building.GetComponent<SkewBuildingController>();
        
        correctDistance = skewBuildingController.getDistance();
        points = skewBuildingController.beaconPoints;
        skewBuildingController.SetLine(skewError);
    }

    //returns the gamobject the mouse has hit
    public GameObject CastMouseRay()
    {
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), 20, pointMask);

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
        theodolietObject.GetComponent<Theodoliet>().ToggleMagnify() ;
    }

    public void FlipTheodoliet()
    {
        isFlipped = !isFlipped;

        building.GetComponent<SkewBuildingController>().SetLine(skewError * (isFlipped? -1: 1));
    }

    //checks if the given anwser is correct
    public void CheckAnswer()
    {

        if (gm.CheckCorrectAnswer(answerText.text, correctDistance) || gm.CheckCorrectAnswer(answerText.text, -correctDistance))
        {
            gm.IncreaseScore(scoreIncrease, 1);
            Debug.Log(answerText.text + " is correct!");

            if (GameManager.campaignMode)
            {
                winMenu.SetActive(true);
            }
            else
            {
                winMenuFree.SetActive(true);
            }
            //gm.ReloadScene();
        }
        else
        {
            answerText.color = falseColor;
            Debug.Log(answerText.text + " is False...");
            answerOutput.text = "Hou rekening met de collimatiefout.";
        }
    }


}
