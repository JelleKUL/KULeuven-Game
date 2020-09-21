using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*************** Sets the scale value to the world scale ****************//

public class SchaalLat : MonoBehaviour
{
    [Header("Objects")]
    public Transform lat;
    public TextMesh endText;

    // Start is called before the first frame update
    void Start()
    {
        endText.text = GameManager.worldScale.ToString() + "m";
    }

}
