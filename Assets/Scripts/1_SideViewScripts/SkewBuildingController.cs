using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//***************** Holds the data from the skewbuilding *************//

public class SkewBuildingController : MonoBehaviour
{
    public Transform[] beaconPoints;
    public LineRenderer straightLine;

    // returns the correct distance between the two points
    public float getDistance()
    {
        return beaconPoints[0].position.x - beaconPoints[1].position.x;
    }

    // draws the line straight down with a small error
    public void SetLine( float error)
    {
        straightLine.SetPosition(0, beaconPoints[0].position + Vector3.back);
        straightLine.SetPosition(1, new Vector3(beaconPoints[0].position.x + error, beaconPoints[1].position.y, -1));
    }


}
