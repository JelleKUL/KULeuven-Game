using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeasureController : MonoBehaviour
{
    public LayerMask beacons;
    public float errorAngle;
    public float maxDistance = 10;
    public LineRenderer laserline;
    public GameObject kruisDraden;

    private Vector3 beaconHitPoint;
    private GameObject kruisDraadL;
    private GameObject kruisDraadR;
    
    private Vector3[] laserLinePositions = new Vector3[3];



    // Start is called before the first frame update
    void Start()
    {
        kruisDraadL =  Instantiate(kruisDraden, transform.position, Quaternion.identity);
        kruisDraadR = Instantiate(kruisDraden, transform.position, Quaternion.identity);

        

    }

    // Update is called once per frame
    void Update()
    {
        laserLinePositions[1] = transform.position;
        kruisDraadL.transform.position = CastLaser(transform.position, new Vector2(-Mathf.Cos(errorAngle * Mathf.Deg2Rad), Mathf.Sin(errorAngle * Mathf.Deg2Rad)));
        kruisDraadR.transform.position = CastLaser(transform.position, new Vector2(Mathf.Cos(errorAngle * Mathf.Deg2Rad), Mathf.Sin(errorAngle * Mathf.Deg2Rad)));
        laserLinePositions[0] = kruisDraadL.transform.position;
        laserLinePositions[2] = kruisDraadR.transform.position;

        laserline.SetPositions(laserLinePositions);
    }

    public Vector4 PositionAndDistance()
    {

        return Vector4.zero;
    }

    // casts a ray in the direction of the laser
    Vector3 CastLaser(Vector2 origin, Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, maxDistance, beacons);

        if (hit.collider != null)
        {
            return new Vector3( hit.point.x, hit.point.y, -1);
            
        }
        return transform.position;
    }
}
