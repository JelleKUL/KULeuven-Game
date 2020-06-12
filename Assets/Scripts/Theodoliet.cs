using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Theodoliet : MonoBehaviour
{
  
    [Header("Prefabs")]
    [SerializeField]
    private Transform measureHead;
    [SerializeField]
    private GameObject magnifyGlassR;
    [SerializeField]
    private GameObject magnifyGlassL;
    [SerializeField]
    private LineRenderer laserline;
    public ScheefstandController scheefstandController;

    [Header("Measure Variables")]
    public float scheefstandsHoek = 0f;
    public float DistanceMultiplier = 0.01f;
    [SerializeField]
    private float beaconOffset = 0.1f;
    public bool showMagnify;

    private GameObject MagnifyL;
    private GameObject MagnifyR;
    private Vector3[] laserLinePositions = new Vector3[3];

    


    // Start is called before the first frame update
    void Start()
    {
        MagnifyL = Instantiate(magnifyGlassL, transform.position, Quaternion.identity);
        MagnifyR = Instantiate(magnifyGlassR, transform.position, Quaternion.identity);

    }

    // Update is called once per frame
    void Update()
    {
        laserLinePositions[1] = measureHead.position;
        scaleMagnify(0, MagnifyL);
        scaleMagnify(1, MagnifyR);
        laserline.SetPositions(laserLinePositions);

        MagnifyR.SetActive( showMagnify);
        MagnifyL.SetActive( showMagnify);
    }


    public void ToggleMagnify()
    {
        showMagnify = !showMagnify;
    }



    void scaleMagnify(int point, GameObject magnify)
    {
        //Vector4 beaconHitPoint = CastLaser(measureHead.position, new Vector2(direction * Mathf.Cos((errorAngle + direction * scheefstandsHoek) * Mathf.Deg2Rad), Mathf.Sin((errorAngle + direction * scheefstandsHoek) * Mathf.Deg2Rad)), direction);
        magnify.transform.position = scheefstandController.points[point].position;//beaconHitPoint + new Vector4(1, 0, 0, 0) * direction * beaconOffset;
        magnify.transform.localScale = Vector3.one * Vector2.Distance(scheefstandController.points[point].position, measureHead.position) * DistanceMultiplier; //new Vector3(beaconHitPoint.w * DistanceMultiplier, beaconHitPoint.w * DistanceMultiplier, 1);
        laserLinePositions[2*point] = magnify.transform.position;
    }



}
