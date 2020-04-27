﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterPassingTabelVereffening : MonoBehaviour
{
    public Text station;
    public Text hoogteVerschilText;
    public Text afstandText;
    public Text VereffeningsHoogte;

    public float hoogteVerschil;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        hoogteVerschil = 0f;
        if (float.TryParse(VereffeningsHoogte.text, out float result))
        {
            hoogteVerschil = result;
        }
        else hoogteVerschil = 0f;

    }

    public void SetNames(int nr, string hoogteVerschilVoor, string afstandVoor)
    {
        station.text = nr.ToString();
        hoogteVerschilText.text = hoogteVerschilVoor;
        afstandText.text = afstandVoor;
    }
}