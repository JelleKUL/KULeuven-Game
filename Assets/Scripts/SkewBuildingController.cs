using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkewBuildingController : MonoBehaviour
{
    public Transform[] beaconPoints;
    public LineRenderer straightLine;


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


    public void SetLine( float error)
    {
        straightLine.SetPosition(0, beaconPoints[0].position + Vector3.back);
        straightLine.SetPosition(1, new Vector3(beaconPoints[0].position.x + error, beaconPoints[1].position.y, -1));
    }


}
