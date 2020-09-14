using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundObjectMover : MonoBehaviour
{
    [SerializeField]
    Vector3 maxMove = Vector3.zero;
    [SerializeField]
    float maxScale = 0;
    [SerializeField]
    float movementSpeed = 0f;
    [SerializeField]
    bool moveLocal, move, scale;

    Vector3 startPosition;
    Vector3 startScale;

    float offset;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        startScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        MoveObject();
    }

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
