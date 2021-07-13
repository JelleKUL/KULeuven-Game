using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode()]
public class SiblingIndex : MonoBehaviour
{
    public int index;
    public bool Update = false;

    private void Awake()
    {
        transform.SetSiblingIndex(index);
    }

    private void OnDrawGizmosSelected()
    {
        if (Update)
        {
            Update = false;
            transform.SetSiblingIndex(Mathf.Min(index, transform.parent.childCount-1));
        }
    }

}