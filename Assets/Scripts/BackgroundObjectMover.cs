using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundObjectMover : MonoBehaviour
{
    [SerializeField]
    Vector3 maxMove = Vector3.zero;
    [SerializeField]
    float movementSpeed = 0f;
    [SerializeField]
    bool moveLocal;

    Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        MoveObject();
    }

    void MoveObject()
    {
        transform.position = startPosition + Mathf.Sin(Time.time * movementSpeed) * (moveLocal ? transform.TransformDirection(maxMove) : maxMove); //*  maxMove * moveLocal? transform.forward;
    }
}
