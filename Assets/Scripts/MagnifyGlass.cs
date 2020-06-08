using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnifyGlass : MonoBehaviour
{
    public GameObject ZoomedMagnify;
    private GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

    }

    // Update is called once per frame
    void Update()
    {
        if (ZoomedMagnify.activeSelf)
        {
            ZoomedMagnify.transform.position = (gm.screenMax + gm.screenMin) / 2f;
            if(transform.localScale.x * transform.localScale.y * transform.localScale.z != 0)
            {
                ZoomedMagnify.transform.localScale = new Vector3(1 / transform.localScale.x, 1 / transform.localScale.y, 1 / transform.localScale.z);

            }
        }
    }

    public void ToggleZoom()
    {
        Debug.Log("Toogle");
        ZoomedMagnify.SetActive(!ZoomedMagnify.activeSelf);
    }

   
}
