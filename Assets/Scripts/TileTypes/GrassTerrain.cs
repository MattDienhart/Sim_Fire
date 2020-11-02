﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassTerrain : MonoBehaviour
{
    public Sprite[] sprites;

    public string getType()
    {
        return "Grass";
    }

    public Sprite getSprite()
    {
        return sprites[Random.Range(0, sprites.Length)];
    }

    public int getDryness()
    {
        return Random.Range(20, 50);
    }

    public int getSpeed()
    {
        return Random.Range(3, 5);
    }
}
