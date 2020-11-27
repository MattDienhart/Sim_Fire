using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestTerrain : MonoBehaviour
{
    public Sprite[] sprites;
    public int dryness = 50;
    public int amountBurned = 0;
    public Sprite terrainSprite;

    public string getType()
    {
        return "Forest";
    }

    public Sprite getSprite()
    {
        return sprites[Random.Range(0, sprites.Length)];
    }

    public int getDryness()
    {
        return Random.Range(60, 100);
    }

    public int getSpeed()
    {
        return Random.Range(1, 3);
    }
}

