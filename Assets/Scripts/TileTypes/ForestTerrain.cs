using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestTerrain : TileScript
{
    public Sprite[] sprites;
    public int amountBurned = 0;

    private void Start()
    {
        base.setBurning(true);
        dryness = Random.Range(60, 100); 
        speed = Random.Range(1, 3);
        base.SetSprite(sprites[Random.Range(0, sprites.Length)]);
        terrian = "Forest";
    }
}

