using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//**************** Moves background objects in a rythmic fashion ******************//

public class BackgroundObjectMover : MonoBehaviour
{
    [SerializeField]
    [Tooltip("the maximum offset in one direction")]
    Vector3 maxMove = Vector3.zero;
    [SerializeField]
    [Tooltip("the maximum scale offset in one direction")]
    float maxScale = 0;
    [SerializeField]
    [Tooltip("the speed at which the object moves")]
    float movementSpeed = 0f;
    [SerializeField]
    [Tooltip("toggle to use this property")]
    bool moveLocal, move, scale;

    //Private Variables

    private Vector3 startPosition;
    private Vector3 startScale;

    private float offset;

    // Start is called before the first frame update
    void Start()
    {
        // set the start values
        startPosition = transform.position;
        startScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        MoveObject();
    }

    // moves the object with a sin offset in time
    void MoveObject()
    {
        offset = Mathf.Sin(Time.time * movementSpeed);
        if(move)
        {
            transform.position = startPosition + offset * (moveLocal ? transform.TransformDirection(maxMove) : maxMove); //*  maxMove * moveLocal? transform.forward;
        }
        if (scale)
        {
            transform.localScale = startScale + offset * Vector3.one * maxScale;
        }
    }
}
