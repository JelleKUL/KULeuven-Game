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
    public GameObject winMenu, winMenuFree, submitBtn, restartBtn;
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
    [Tooltip("het aantal keren dat je mag proberen, 0 = oneindig")]
    public int nrOfTries = 3;


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
    private int currentTries = 0;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        skewError = Random.Range(-maxSkewError, maxSkewError);

        PlaceBulding();
        float offset = 0f;
        if (theodoliet.TryGetComponent(out BoxCollider2D box))
        {
            offset = box.offset.y;
        }
        theodolietObject = Instantiate(theodoliet, MeasurePlacer.position - MeasurePlacer.up * offset, Quaternion.identity);
        theodolietObject.GetComponent<Theodoliet>().scheefstandController = this;

        if (GameManager.showDebugAnswer)
        {
            Debug.Log("Correct distance = " + GameManager.RoundFloat(Mathf.Abs(correctDistance),3));
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
                        float offset = 0f;
                        if (hitObject.TryGetComponent(out BoxCollider2D box))
                        {
                            offset = box.offset.y;
                        }
                        hitObject.transform.position = MeasurePlacer.position - MeasurePlacer.up * offset;
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

            if (nrOfTries > 0)
            {
                currentTries++;
                if (currentTries >= nrOfTries)
                {
                    setRestart();
                    return;
                }
            }
        }
    }

    public void setRestart()
    {
        ShowAnswer();
        submitBtn.SetActive(false);
        restartBtn.SetActive(true);
        answerOutput.text = "Te veel pogingen, probeer opnieuw.";
    }

    //displays the correct answer
    public void ShowAnswer()
    {
        
        if (answerText.transform.parent.GetComponent<InputField>())
        {
            answerText.color = falseColor;
            InputField answerDisplay = answerText.transform.parent.GetComponent<InputField>();
            answerDisplay.text = GameManager.RoundFloat(Mathf.Abs(correctDistance),3).ToString();
            answerDisplay.interactable = false;
        }

        answerOutput.text = "Hou rekening met de collimatiefout.";



        Debug.Log("showing answer");

    }


}
