using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeasureController : MonoBehaviour
{
    public LayerMask beacons;
    public float scheefstandsHoek = 0f;
    public float errorAngle;
    public float maxDistance = 10;
    public float DistanceMultiplier = 0.01f;
    public LineRenderer laserline;
    public GameObject kruisDraden;
    public Transform measureHead;

    private Vector4 beaconHitPointL;
    private Vector4 beaconHitPointR;

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
        laserLinePositions[1] = measureHead.position;
        beaconHitPointL = CastLaser(measureHead.position, new Vector2(-Mathf.Cos((errorAngle - scheefstandsHoek)* Mathf.Deg2Rad), Mathf.Sin((errorAngle - scheefstandsHoek) * Mathf.Deg2Rad)));
        beaconHitPointR = CastLaser(measureHead.position, new Vector2(Mathf.Cos((errorAngle + scheefstandsHoek) * Mathf.Deg2Rad), Mathf.Sin((errorAngle + scheefstandsHoek) * Mathf.Deg2Rad)));
        kruisDraadL.transform.position = beaconHitPointL;
        kruisDraadL.transform.localScale = new Vector3(1,beaconHitPointL.w * DistanceMultiplier, 1);
        kruisDraadR.transform.position = beaconHitPointR;
        kruisDraadR.transform.localScale = new Vector3(1, beaconHitPointR.w * DistanceMultiplier, 1);
        laserLinePositions[0] = kruisDraadL.transform.position;
        laserLinePositions[2] = kruisDraadR.transform.position;

        laserline.SetPositions(laserLinePositions);
    }

    public Vector4 PositionAndDistance()
    {

        return Vector4.zero;
    }

    // casts a ray in the direction of the laser
    Vector4 CastLaser(Vector2 origin, Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, maxDistance, beacons);

        if (hit.collider != null)
        {
            return new Vector4( hit.point.x, hit.point.y, -1, hit.distance);
            
        }
        return measureHead.position;
    }

    public void ToggleMagnify()
    {

    }
}
