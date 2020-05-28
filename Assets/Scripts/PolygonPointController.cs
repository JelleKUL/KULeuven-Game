using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*********** The PolygonPointController manages the individual points of the lines and displays the information ******************//

public class PolygonPointController : MonoBehaviour
{
    public bool showName;
    public float maxLengthError;
    public float maxAngleError;

    [Header("Prefab Childeren")]
    public TextMesh nameText;
    public TextMesh distanceText;
    public TextMesh angleText;
    public GameObject errorEllipse;
    public GameObject angleDisplay;
    public GameObject spriteMask1;
    public GameObject spriteMask2;

    private float lengthError;
    private float angleError;

    //sets the name of the point as a number
    private void Start()
    {
        transform.rotation = Quaternion.identity;
        lengthError = Random.Range(-3 * maxLengthError, 3 * maxLengthError);
        angleError = Random.Range(-3 * maxAngleError, 3 * maxAngleError);
    }

    public void SetNameText (int nr)
    {
        if (showName)
        {
            if (nr < 0)
            {
                nameText.text = "0";
            }
            else if (nr == 0)
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

    //sets the name of the point as a letter
    public void SetNameNrText(int nr)
    {
        if (showName)
        {
            nameText.text = (nr).ToString();
        }
        else nameText.text = "";
    }

    //displays the distance to the previous point
    public void SetDistanceText (Vector3 prevPoint)
    {
        distanceText.text = (Mathf.Round(((transform.position - prevPoint).magnitude * GameManager.worldScale + lengthError) * 1000f ) / 1000f).ToString() + " m";
    }

    //displays the angle between the previous and next point
    public void SetAngleText (Vector3 prevPoint, Vector3 nextPoint)
    {
        angleDisplay.SetActive(true);
        Vector3 pos = transform.position;
        float angle = Vector3.Angle(prevPoint - pos, nextPoint - pos);
        spriteMask1.transform.right = prevPoint - spriteMask1.transform.position;
        spriteMask2.transform.right = -nextPoint + spriteMask2.transform.position;
        //Debug.Log((spriteMask1.transform.rotation * Quaternion.Inverse(spriteMask2.transform.rotation)).eulerAngles.z);

        if ((spriteMask1.transform.rotation * Quaternion.Inverse(spriteMask2.transform.rotation)).eulerAngles.z > 180)
        {
            spriteMask1.transform.right = nextPoint - spriteMask1.transform.position;
            spriteMask2.transform.right = -prevPoint + spriteMask2.transform.position;
        }
        angleText.transform.position = -Vector3.Normalize(Vector3.Normalize(nextPoint - transform.position) + Vector3.Normalize(prevPoint - transform.position)) * 0.7f + transform.position;
        distanceText.transform.position = -Vector3.Normalize(Vector3.Normalize(nextPoint - transform.position) + Vector3.Normalize(prevPoint - transform.position)) * 0.7f + transform.position;// + Vector3.down * 0.1f;
        angleText.text = (Mathf.Round((angle + angleError) /360 * 400 * 100) / 100f).ToString() + " gon";

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
    public void SetErrorEllips(Vector2 prevPoint,float prevX, float prevY, float prevAngle ,  float distanceError, float angleError)
    {
        errorEllipse.SetActive(true);
        errorEllipse.transform.right = prevPoint - new Vector2(errorEllipse.transform.position.x, errorEllipse.transform.position.y);
        float angleDif = (errorEllipse.transform.eulerAngles.z - prevAngle) * Mathf.Deg2Rad;
        //Debug.Log(angleDif);

        float newx = Mathf.Sqrt(Mathf.Pow(prevX * Mathf.Cos(angleDif), 2) + Mathf.Pow(prevY * Mathf.Sin(angleDif), 2));
        float newy = Mathf.Sqrt(Mathf.Pow(prevX * Mathf.Sin(angleDif), 2) + Mathf.Pow(prevY * Mathf.Cos(angleDif), 2));


        errorEllipse.transform.localScale = new Vector3((Vector2.Distance(errorEllipse.transform.position, prevPoint) * distanceError / 100f) + newx, (Vector2.Distance(errorEllipse.transform.position, prevPoint) * angleError / 100f) + newy, 1);
    }

    // returns the ellipse info so it can be used by the next point
    public Vector3 GetEllipseInfo()
    {
        return new Vector3(errorEllipse.transform.localScale.x, errorEllipse.transform.localScale.y, errorEllipse.transform.eulerAngles.z);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Beacon" || collision.gameObject.tag == "PolygonPoint")
        {
            collision.transform.position = transform.position;
        }
        
    }

    

}
