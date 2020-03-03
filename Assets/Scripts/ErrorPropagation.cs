using UnityEngine;
using System.Collections;

public class ErrorPropagation : MonoBehaviour
{
    [Header("percentage")]
    [Range(0,100)]
    public float distanceError;
    [Range(0, 100)]
    public float angleError;

    public Transform startPoint;


    private void Start()
    {
        
    }

    private void Update()
    {
        transform.right = startPoint.position - transform.position;
        transform.localScale = new Vector3(Vector2.Distance(transform.position, startPoint.position) * distanceError / 100f, Vector2.Distance(transform.position, startPoint.position) * angleError / 100f, 1);
    }
}