using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkewBeaconController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RotateStraight();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        
    }

    void RotateStraight()
    {
        transform.rotation = Quaternion.identity;
    }
}
