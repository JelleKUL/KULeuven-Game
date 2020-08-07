using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundTopDownScaler : MonoBehaviour
{
    

    // Start is called before the first frame update
    void Start()
    {
        SetPositionAndScale(GameObject.FindGameObjectWithTag("GameController"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetPositionAndScale(GameObject target)
    {
        transform.position = target.transform.position;
        

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();

        sprite.drawMode = SpriteDrawMode.Tiled;
        sprite.size = target.transform.localScale;
    }
}
