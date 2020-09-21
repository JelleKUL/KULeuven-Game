using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//**************** sets the top down background to the correct scale according to the gamemanager ******************//

[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundTopDownScaler : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        SetPositionAndScale(GameObject.FindGameObjectWithTag("GameController"));
    }

    // sets the sprite to the correct position and scale
    void SetPositionAndScale(GameObject target)
    {
        transform.position = target.transform.position;
        

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();

        sprite.drawMode = SpriteDrawMode.Tiled;
        sprite.size = target.transform.localScale;
    }
}
