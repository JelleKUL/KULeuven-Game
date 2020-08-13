﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterPassingTabelVereffening : MonoBehaviour
{
    public Text station;
    public Text hoogteVerschilText;
    public Text afstandText;
    public Text vereffeningsHoogteText;

    public float hoogteVerschil;
    public float vereffenigsHoogte;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        vereffenigsHoogte = 0f;
        if (float.TryParse(vereffeningsHoogteText.text, out float result))
        {
            vereffenigsHoogte = result;
        }
        else vereffenigsHoogte = 0f;

    }

    public void SetName(int nr)
    {
        station.text = nr.ToString();
    }
    public void SetValues(string hoogteVerschilVoor, string afstandVoor)
    {
        hoogteVerschilText.text = hoogteVerschilVoor;

        hoogteVerschil = 0f;
        if (float.TryParse(hoogteVerschilText.text, out float result))
        {
            hoogteVerschil = result;
        }
        else hoogteVerschil = 0f;

        afstandText.text = afstandVoor + " m";
    }
}
