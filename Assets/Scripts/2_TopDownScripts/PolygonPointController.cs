using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*********** The PolygonPointController manages the individual points of the lines and displays the information ******************//

public class PolygonPointController : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private bool showName;
    [SerializeField] private bool resetRotation = true;
    [SerializeField] private float maxLengthError;
    [SerializeField] private float maxAngleError;
    [SerializeField] private float angleDisplayMargin = 5;
    [SerializeField] private float angleDisplayRaduis = 0.7f;
    [SerializeField] private float distanceTextSwitchTreshold = 1.5f;
    
    [Header("Prefab Childeren")]
    [SerializeField] private TextMesh nameText;
    [SerializeField] private TextMesh distanceText;
    [SerializeField] private TextMesh angleText;
    [SerializeField] private GameObject anglePointer;
    [SerializeField] private GameObject errorEllipse;
    [SerializeField] private GameObject angleDisplay;

    [HideInInspector]
    public float errorEllipsSize;
    [HideInInspector]
    public bool IsSnapped;
    [HideInInspector]
    public bool displayError;
    [HideInInspector]
    public float displayRadiusModifier = 1;

    private float lengthError;
    private float angleError;

    private Material angleDisplayMaterial;

    //sets the name of the point as a number
    private void Start()
    {
        if(resetRotation) transform.rotation = Quaternion.identity;
        
        lengthError = displayError? Random.Range(-maxLengthError, maxLengthError) : 0f;
        angleError = displayError? Random.Range(-maxAngleError, maxAngleError) : 0f;
        if (angleDisplay) angleDisplayMaterial = angleDisplay.GetComponent<MeshRenderer>().material;
    }

    // set the name of the point, with a number input
    public void SetNameText (int nr)
    {
        if (showName)
        {
            if (nr < 0f)
            {
                nameText.text = "0";
            }
            else if (nr == 0f)
            {
                nameText.text = "P";
            }
            else
            {
                char c = (char)(64 + (nr));
                nameText.text = c.ToString();
            }
        }
        else nameText.text = "";
    }

    // sets the name of the point as a number
    public void SetNrText(int nr)
    {
        if (showName)
        {
            nameText.text = (nr).ToString();
        }
        else nameText.text = "";
    }

    //displays the distance to the previous point at the middle of the line
    public void SetDistanceText (Vector3 prevPoint)
    {
        float distance = Vector3.Distance(transform.position, prevPoint);

        distanceText.text = GameManager.RoundFloat(distance * GameManager.worldScale + lengthError,3).ToString() + " m";
        distanceText.transform.position = (transform.position + prevPoint) / 2f;

        if(distance < distanceTextSwitchTreshold)
        {
            distanceText.anchor = TextAnchor.MiddleLeft;
            Vector2 upVector = transform.position - prevPoint;
            float direction = Vector2.Dot(upVector, Vector2.up);
            distanceText.transform.up = upVector * direction;
            distanceText.transform.position += distanceText.transform.right * 0.2f;
        }
        else
        {
            distanceText.anchor = TextAnchor.UpperCenter;
            Vector2 upVector = new Vector2((transform.position.y - prevPoint.y), -(transform.position.x - prevPoint.x));
            float direction = Vector2.Dot(upVector, Vector2.up);
            distanceText.transform.up = upVector * direction;
            distanceText.transform.position -= distanceText.transform.up * 0.1f;

            if (direction == 0f)
            {
                distanceText.anchor = TextAnchor.MiddleLeft;
                distanceText.transform.position += distanceText.transform.right * 0.1f;
            }
        }
    }

    //displays the angle between the previous and next point
    public void SetAngleText (Vector3 prevPoint, Vector3 nextPoint)
    {
        angleDisplay.SetActive(true);

        Vector3 pos = transform.position;
        float angle = Vector2.SignedAngle(nextPoint - pos, prevPoint - pos);

        if (angle < 0) angle = 360f + angle;

        Vector2 dir = prevPoint - pos;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        angleDisplay.transform.localScale = Vector3.one * angleDisplayRaduis * displayRadiusModifier;
        angleDisplay.transform.rotation = Quaternion.AngleAxis(targetAngle-angleDisplayMargin, Vector3.forward);
        angleDisplayMaterial.SetFloat("_Arc2",360- angle + 2* angleDisplayMargin);

        angleText.transform.position = transform.position;
        angleText.text = GameManager.RoundFloat((angle + angleError) /360 * 400,3).ToString() + " gon";

        Vector2 upVector = Vector3.Normalize(transform.position - prevPoint) + Vector3.Normalize(transform.position - nextPoint);
        float direction = Vector2.Dot(upVector, Vector2.right);

        angleText.transform.right = upVector * direction;

        if(direction > 0f)
        {
            angleText.anchor = TextAnchor.MiddleLeft;
            angleText.transform.position += angleText.transform.right * 0.5f * angleDisplayRaduis/0.7f * displayRadiusModifier;
            anglePointer.transform.position = angleText.transform.position;
            anglePointer.transform.up = angleText.transform.right;
        }
        else
        {
            angleText.anchor = TextAnchor.MiddleRight;
            angleText.transform.position -= angleText.transform.right * 0.5f * angleDisplayRaduis/0.7f * displayRadiusModifier;
            anglePointer.transform.position = angleText.transform.position;
            anglePointer.transform.up = - angleText.transform.right;
        }
    }

    //hides the length and angle text
    public void HideInfo()
    {
        distanceText.text = "";
        angleText.text = "";
        errorEllipse.SetActive(false);
        angleDisplay.SetActive(false);
    }

    //scales the error ellips taking into account the previous one
    public void SetErrorEllips(Vector2 prevPoint,float prevX, float prevY, float prevAngle ,  float baseDistanceError, float ppmDistanceError, float angleError)
    {
        errorEllipse.SetActive(true);
        errorEllipse.transform.right = prevPoint - new Vector2(errorEllipse.transform.position.x, errorEllipse.transform.position.y);
        float angleDif = (errorEllipse.transform.eulerAngles.z - prevAngle) * Mathf.Deg2Rad;

        float newx = Mathf.Sqrt(Mathf.Pow(prevX * Mathf.Cos(angleDif), 2) + Mathf.Pow(prevY * Mathf.Sin(angleDif), 2));
        float newy = Mathf.Sqrt(Mathf.Pow(prevX * Mathf.Sin(angleDif), 2) + Mathf.Pow(prevY * Mathf.Cos(angleDif), 2));

        errorEllipse.transform.localScale = new Vector3( baseDistanceError/100f + (Vector2.Distance(errorEllipse.transform.position, prevPoint) * ppmDistanceError / 100f) + newx, (Vector2.Distance(errorEllipse.transform.position, prevPoint) * angleError / 100f) + newy, 1);
    }

    // returns the ellipse info so it can be used by the next point
    public Vector3 GetEllipseInfo()
    {
        return new Vector3(errorEllipse.transform.localScale.x, errorEllipse.transform.localScale.y, errorEllipse.transform.eulerAngles.z);
    }

    //collision handeling
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Beacon" || collision.gameObject.tag == "PolygonPoint")
        {
            collision.transform.position = transform.position;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "GroundPoint")
        {
            IsSnapped = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "GroundPoint")
        {
            IsSnapped = false;
        }
    }
}
