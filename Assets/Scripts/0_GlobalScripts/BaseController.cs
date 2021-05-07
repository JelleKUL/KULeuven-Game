using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    protected bool hasStarted;
    protected bool holdingObject;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (!hasStarted) StartSetup(); //only calls when Start() hasn't gone yet
    }

    /// <summary>
    /// Starts the Setup proces of the controller
    /// </summary>
    public virtual void StartSetup()
    {
        hasStarted = true;

        //start the setup of the controller
    }

    //returns the gamobject the mouse has hit
    public virtual GameObject CastMouseRay(LayerMask mask)
    {
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), 20, mask);

        if (rayHit.collider != null)
        {
            holdingObject = true;
            //Debug.Log(rayHit.transform.gameObject.name);
            return rayHit.transform.gameObject;

        }
        holdingObject = false;
        return null;
    }
}
