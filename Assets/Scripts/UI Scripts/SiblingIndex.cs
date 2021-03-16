using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode()]
public class SiblingIndex : MonoBehaviour
{
    public int index;

    private void Awake()
    {
        transform.SetSiblingIndex(index);
    }

}