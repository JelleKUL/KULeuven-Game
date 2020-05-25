using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchaalLat : MonoBehaviour
{
    public Transform lat;
    public TextMesh endText;


    // Start is called before the first frame update
    void Start()
    {
        endText.text = GameManager.worldScale.ToString() + "m";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
