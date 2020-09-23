using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*************** Manages the magnify glass ****************//

[RequireComponent (typeof(CircleCollider2D), typeof(BoxCollider2D))]
public class MagnifyGlass : MonoBehaviour
{
    [Header("Objects")]
    public GameObject ZoomedMagnify;
    public Transform zoomedBackground;
    public GameObject viewer;
    public Camera viewerCamera;
    public GameObject assenkruis;
    public GameObject assenkruisZoom;

    private BoxCollider2D zoomedCollider;
    private CircleCollider2D circleCollider;

    private GameManager gm;
    private bool ShowAssenkruis;

    private Vector3 viewerStartScale;

    private void Awake()
    {
        viewerStartScale = viewer.transform.localScale;
    }

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        circleCollider = GetComponent<CircleCollider2D>();
        zoomedCollider = GetComponent<BoxCollider2D>();


    }



    public void ToggleZoom()
    {
        Debug.Log("Toggle");
        ZoomedMagnify.SetActive(!ZoomedMagnify.activeSelf);
        zoomedBackground.gameObject.SetActive(!zoomedBackground.gameObject.activeSelf);

        setZoomLocation();
    }

    void setZoomLocation()
    {
        if (ZoomedMagnify.activeSelf)
        {
            ZoomedMagnify.transform.position = (gm.screenMax + gm.screenMin) / 2f;
            ZoomedMagnify.transform.position += Vector3.back * 5;
            zoomedBackground.position = Camera.main.transform.position + Vector3.back * Camera.main.transform.position.z + Vector3.forward;  //ZoomedMagnify.transform.position + Vector3.forward;
            zoomedCollider.offset = zoomedBackground.position - transform.position;

            ZoomedMagnify.transform.localScale = Vector3.one * Mathf.Min(gm.screenMax.x - gm.screenMin.x, gm.screenMax.y - gm.screenMin.y) / 2f;
            zoomedBackground.localScale = new Vector3(Camera.main.aspect, 1, 1) * Camera.main.orthographicSize * 2.2f;  //new Vector3(gm.screenMax.x - gm.screenMin.x, gm.screenMax.y - gm.screenMin.y, 1);
            zoomedCollider.size = zoomedBackground.localScale;

        }
        else
        {
            zoomedCollider.offset = Vector2.zero;

            zoomedCollider.size = Vector2.one;
        }
    }

    public void SetPositionAndScale(Vector2 position, float scale, bool assenkruisActive)
    {
        if (assenkruis.activeSelf != assenkruisActive)
        {
            assenkruis.SetActive(assenkruisActive);
            assenkruisZoom.SetActive(assenkruisActive);
        }

        if (circleCollider == null)
        {
            circleCollider = GetComponent<CircleCollider2D>();
        }

        viewerCamera.orthographicSize = viewerStartScale.x * scale;
        viewer.transform.localScale = viewerStartScale * scale;
        circleCollider.radius = scale * 1.1f;

        transform.position = position;
        transform.position += Vector3.back;
    }

   
}
