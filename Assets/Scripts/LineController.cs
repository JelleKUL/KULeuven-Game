using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public Vector2 startCanvas;
    public Vector2 endCanvas;

    private LineRenderer line;
    private TextMesh distanceText;
    private TextMesh angleText;
    private Vector2 p1;
    private Vector2 p2;

    // Start is called before the first frame update
    void Start()
    {

        line = GetComponent<LineRenderer>();
        distanceText = endPoint.GetChild(0).GetComponent<TextMesh>();
        angleText = startPoint.GetChild(0).GetComponent<TextMesh>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && IsBetweenValues(SetObjectToMouse(Input.mousePosition),startCanvas, endCanvas))
        {
            endPoint.position = SetObjectToMouse(Input.mousePosition);
            p1 = startPoint.position;
            p2 = endPoint.position;
            SetLinePos(p1, p2);
            SetText(p1, p2);
        }
    }

    void SetLinePos(Vector2 pS, Vector2 pE)
    {
        line.SetPosition(0, pS);
        line.SetPosition(1, pE);
    }
    void SetText(Vector2 pS, Vector2 pE)
    {
        distanceText.text = (Mathf.Round((pE - pS).magnitude * 100f) / 100f).ToString() + " m";
        angleText.text = (Mathf.Round(Mathf.Atan2(pE.y - pS.y, pE.x - pS.x) / (Mathf.PI * 2) * 400 * 100)/100f).ToString() + " gon";
    }

    Vector3 SetObjectToMouse(Vector2 mousePos)
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -Camera.main.transform.position.z));
    }

    bool IsBetweenValues(Vector2 check, Vector2 min, Vector2 max)
    {
        if (check.x < max.x && check.y < max.y && check.x > min.x && check.y > min.y) return true;
        else return false;
    }
}
