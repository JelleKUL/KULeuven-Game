using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisTransformer : MonoBehaviour
{
    [Header("Axis transform controls")]
    [SerializeField] private bool rotateAxis;
    [Tooltip("the max number of quadrants the axis can rotate to")]
    [Range(1, 4)]
    [SerializeField] private int maxQuadrants = 1;
    [Tooltip("X & Y value: position offset, Z value rotation offset")]
    [SerializeField] private Vector3 minAxisTransform; // X & Y value: position offset, Z value rotation offset
    [Tooltip("X & Y value: position offset, Z value rotation offset")]
    [SerializeField] private Vector3 maxAxisTransform; // X & Y value: position offset, Z value rotation offset

    [Header("Scene Object")]
    [SerializeField] private GameObject assenkruis;


    public void RotatePoint(PolygonPointController point)
    {
        if(!assenkruis)
        {
            Debug.Log("No assenkruis set");
            return;
        }
        point.transform.SetParent(assenkruis.transform);
        assenkruis.transform.position += new Vector3(Random.Range(0f, 1f) * maxAxisTransform.x, Random.Range(0f, 1f) * maxAxisTransform.y, 0);

        assenkruis.transform.Rotate(0, 0, Random.Range(0, maxQuadrants) * 90f + Mathf.Sign(Random.Range(-1, 1)) * Random.Range(minAxisTransform.z, maxAxisTransform.z));
    }
}
