using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeasureController : MonoBehaviour
{
    [Header ("Prefabs")]
    [SerializeField]
    private Transform measureHead;
    [SerializeField]
    private GameObject magnifyGlassR;
    [SerializeField]
    private GameObject magnifyGlassL;
    [SerializeField]
    private LineRenderer laserline;
    [SerializeField]
    private LayerMask beacons;

    [Header("Measure Variables")]
    public float scheefstandsHoek = 0f;
    public float errorAngle;
    public float maxDistance = 10;
    public float DistanceMultiplier = 0.01f;
    [SerializeField]
    private float beaconOffset = 0.1f;
    public bool showMagnify;

    private Vector4 beaconHitPointL;
    private Vector4 beaconHitPointR;
    private GameObject MagnifyL;
    private GameObject MagnifyR;
    private Vector3[] laserLinePositions = new Vector3[3];
    private bool hasHitL;
    private bool hasHitR;


    // Start is called before the first frame update
    void Start()
    {
        MagnifyL =  Instantiate(magnifyGlassL, transform.position, Quaternion.identity);
        MagnifyR = Instantiate(magnifyGlassR, transform.position, Quaternion.identity);

    }

    // Update is called once per frame
    void Update()
    {
        laserLinePositions[1] = measureHead.position;
        scaleMagnify(-1, MagnifyL);
        scaleMagnify(1, MagnifyR);
        laserline.SetPositions(laserLinePositions);

        MagnifyR.SetActive(hasHitR && showMagnify);
        MagnifyL.SetActive(hasHitL && showMagnify);
    }


    // casts a ray in the direction of the laser
    Vector4 CastLaser(Vector2 origin, Vector2 directionVector, int direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, directionVector, maxDistance, beacons);

        if (hit.collider != null)
        {
            if(direction == 1)
            {
                hasHitR = true;
            }
            else
            {
                hasHitL = true;
            }
            return new Vector4( hit.point.x, hit.point.y, -1, hit.distance);
        }
        if (direction == 1)
        {
            hasHitR = false;
        }
        else
        {
            hasHitL = false;
        }
        return measureHead.position;
    }

    public void ToggleMagnify()
    {
        showMagnify = !showMagnify;
    }

    

    void scaleMagnify(int direction, GameObject magnify)
    {
        Vector4 beaconHitPoint = CastLaser(measureHead.position, new Vector2(direction * Mathf.Cos((errorAngle + direction * scheefstandsHoek) * Mathf.Deg2Rad), Mathf.Sin((errorAngle + direction * scheefstandsHoek) * Mathf.Deg2Rad)),direction);
        magnify.transform.position = beaconHitPoint + new Vector4(1, 0,0,0) * direction * beaconOffset;
        magnify.transform.localScale = new Vector3(beaconHitPoint.w * DistanceMultiplier, beaconHitPoint.w * DistanceMultiplier, 1);
        laserLinePositions[1 + direction] = magnify.transform.position;
    }


}
