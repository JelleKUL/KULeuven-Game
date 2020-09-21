using UnityEngine;
using System.Collections;

//*************** Use this to sort 3D objecs into Unity's 2D sorting System ****************//

public class SortingLayer : MonoBehaviour
{
    [Header("Parameters")]
    [Tooltip("type one of the existing layers")]
    public string SortingLayerName = "Default";
    public int SortingOrder = 0;

    void Awake()
    {
        gameObject.GetComponent<MeshRenderer>().sortingLayerName = SortingLayerName;
        gameObject.GetComponent<MeshRenderer>().sortingOrder = SortingOrder;
    }
}