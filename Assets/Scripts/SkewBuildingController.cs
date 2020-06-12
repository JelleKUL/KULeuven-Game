using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkewBuildingController : MonoBehaviour
{
    public Transform[] beaconPoints;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float getDistance()
    {
        return beaconPoints[1].position.x - beaconPoints[0].position.x;
    }
}
