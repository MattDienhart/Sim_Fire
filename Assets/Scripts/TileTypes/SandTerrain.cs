using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandTerrain : TileScript
{
    public Sprite[] rocks;

    private void Start()
    {
        Transform[] children = new Transform[transform.childCount];
        for (int i = 0; i < 4; i++)
        {
            if (Random.Range(0, 8) == 3)
            {
                transform.GetChild(i).GetComponent<SpriteRenderer>().sprite =
                    rocks[Random.Range(0, rocks.Length)];
                obstacles.Add(transform.GetChild(i).gameObject);
            }
                
        }
        dryness = Random.Range(40, 60);
        speed = Random.Range(4, 6);
        terrain = "Sand";
    }

    public override void SetBorderSprite(Sprite sprite, float rotation)
    {
        if (sprite.name == "dirtEdge" || sprite.name == "dirtCorner"
            || sprite.name == "grassEdge" || sprite.name == "grassCorner")
        {
            base.SetBorderSprite(sprite, rotation);
        }

    }



}