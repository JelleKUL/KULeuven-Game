using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Physics2DObject : MonoBehaviour
{
    private GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < gm.screenMin.y)
        {
            transform.position = new Vector2(transform.position.x, gm.screenMax.y);
        }
    }
}
