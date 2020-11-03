using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandTerrain : MonoBehaviour
{
    public Sprite[] sprites;

    public string getType()
    {
        return "Sand";
    }

    public Sprite getSprite()
    {
        return sprites[Random.Range(0, sprites.Length)];
    }

    public int getDryness()
    {
        return Random.Range(50, 60);
    }

    public int getSpeed()
    {
        return Random.Range(4, 7);
    }
}
