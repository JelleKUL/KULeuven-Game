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

    // rotates the beacon straight even when the building is rotated
    void RotateStraight()
    {
        transform.rotation = Quaternion.identity;
    }
}
