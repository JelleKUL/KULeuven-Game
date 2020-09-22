using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//**************** Controls the measure object ******************//

[RequireComponent (typeof(BoxCollider2D))]
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
    private Transform behindLegL;
    [SerializeField]
    private Transform behindLegR;
    [SerializeField]
    private LayerMask beacons;
    [SerializeField]
    private LayerMask groundMask;
    [SerializeField]
    private string groundTag = "Ground";

    [Header("Measure Variables")]
    public float scheefstandsHoek = 0f;
    public float errorAngle;
    public float maxDistance = 10;
    public float DistanceMultiplier = 0.01f;
    [SerializeField]
    private bool showBehindLegs;
    [SerializeField]
    private float maxLegDistance = 1f;
    [SerializeField]
    private float beaconOffset = 0.1f;
    public bool showMagnify = true;

    private Vector4 beaconHitPointL; // also includes the distance
    private Vector4 beaconHitPointR; // also includes the distance
    private GameObject MagnifyL;
    private MagnifyGlass magnifyLScript;
    private GameObject MagnifyR;
    private MagnifyGlass magnifyRScript;
    private Vector3[] laserLinePositions = new Vector3[3];
    private bool hasHitL;
    private bool hasHitR;

    private GameManager gm;


    // Start is called before the first frame update
    void Start()
    {
        MagnifyL =  Instantiate(magnifyGlassL, transform.position, Quaternion.identity);
        MagnifyR = Instantiate(magnifyGlassR, transform.position, Quaternion.identity);
        magnifyLScript = MagnifyL.GetComponent<MagnifyGlass>();
        magnifyRScript = MagnifyR.GetComponent<MagnifyGlass>();

        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

    }

    // Update is called once per frame
    void Update()
    {
        laserLinePositions[1] = measureHead.position;
        scaleMagnify(-1, magnifyLScript);
        scaleMagnify(1, magnifyRScript);
        laserline.SetPositions(laserLinePositions);

        MagnifyR.SetActive(hasHitR && showMagnify);
        MagnifyL.SetActive(hasHitL && showMagnify);
    }

    //rotates the measurehead to the desired location
    public void UpdateMeasureHeadRotation(float angle)
    {
        scheefstandsHoek = angle;
        measureHead.eulerAngles = Vector3.forward * angle;
    }


    // casts a ray in the direction of the laser
    Vector4 CastLaser(Vector2 origin, Vector2 directionVector, int direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, directionVector, maxDistance, beacons);
            
        if (hit.collider != null && gm.IsBetweenValues(hit.transform.position) && gm.IsBetweenValues(transform.position))
        {
            if(direction == 1)
            {
                hasHitR = true;
            }
            else
            {
                hasHitL = true;
            }
            return new Vector4( hit.transform.position.x, hit.point.y, -1, Mathf.Abs(origin.x - hit.transform.position.x));
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

    // toggles the visibility of the magnifyglass //obsolete
    public void ToggleMagnify()
    {
        showMagnify = !showMagnify;
    }

    
    // scale the small magnify the the correct scale according to the distance
    void scaleMagnify(int direction, MagnifyGlass magnify)
    {
        Vector4 beaconHitPoint = CastLaser(measureHead.position, new Vector2(direction * Mathf.Cos((errorAngle + direction * scheefstandsHoek) * Mathf.Deg2Rad), Mathf.Sin((errorAngle + direction * scheefstandsHoek) * Mathf.Deg2Rad)),direction);
        magnify.SetPositionAndScale(beaconHitPoint, beaconHitPoint.w * DistanceMultiplier, true);

        //magnify.transform.position = beaconHitPoint + new Vector4(1, 0,0,0) * direction * beaconOffset;
        //magnify.transform.localScale = new Vector3(beaconHitPoint.w * DistanceMultiplier, beaconHitPoint.w * DistanceMultiplier, 1);
        laserLinePositions[1 + direction] = magnify.transform.position;
    }

    //triggerd on a collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
      if(collision.gameObject.tag == groundTag && showBehindLegs)
        {
            SetBehindLegs(behindLegL);
            SetBehindLegs(behindLegR);
        }
    }

    // sets the behindlegs to the ground below
    private void SetBehindLegs(Transform leg)
    {
        // Cast a ray straight down.
        RaycastHit2D hit = Physics2D.Raycast(leg.position, -leg.up, maxLegDistance, groundMask);

        // If it hits something...
        if (hit.collider != null)
        {
            leg.localScale = hit.distance * Vector2.one;
        }
    }
}
