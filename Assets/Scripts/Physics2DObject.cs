using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Physics2DObject : MonoBehaviour
{
    
    [Tooltip ("Check-off if you want to allow the object to be placed upside down")]
    public bool allowUpsideDown;
    private Rigidbody2D rb;
    private GameManager gm;

    public bool isHeld;


    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isHeld)
        {
            rb.simulated = true;
            if (!allowUpsideDown)
            {
                if (transform.position.y < gm.screenMin.y)
                {
                    transform.position = new Vector2(transform.position.x, (gm.screenMax.y + gm.screenMin.y) / 2f);
                }
            }
            else
            {
                if (transform.position.y < ((gm.screenMax.y + gm.screenMin.y) / 2f))
                {
                    setGravityDownWards(true);
                }
                else setGravityDownWards(false);

                if (rb.gravityScale == -1)
                {

                    if (transform.position.y > gm.screenMax.y)
                    {
                        transform.position = new Vector2(transform.position.x, (gm.screenMax.y + gm.screenMin.y) / 2f);
                    }
                }
                else
                {
                    if (transform.position.y < gm.screenMin.y)
                    {
                        transform.position = new Vector2(transform.position.x, (gm.screenMax.y + gm.screenMin.y) / 2f);
                    }
                }
            }
        }
        else
        {
            rb.simulated = false;
        }
        

    }
    // swaps the direction of the gravity
    private void setGravityDownWards(bool direction)
    {
        if (direction)
        {
            rb.gravityScale = 1;
            //transform.up = Vector3.up;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            rb.gravityScale = -1;
            //transform.up = Vector3.down;
            transform.rotation = Quaternion.Euler(0, 0, 180);


        }
    }
    
    
}
