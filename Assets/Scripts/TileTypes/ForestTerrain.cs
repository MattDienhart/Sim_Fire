using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestTerrain : TileScript
{
    // public Sprite dirtSprite;
    public Sprite[] plants;

    private void Start()
    {
        if (transform.childCount > 0 && base.borderCount < 1)
        {
            Transform[] children = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                if (Random.Range(0, 3) == 1)
                {
                    Transform temp = transform.GetChild(i);
                    temp.GetComponent<SpriteRenderer>().sprite =
                        plants[Random.Range(0, plants.Length)];

                  obstacles.Add(transform.GetChild(i).gameObject);
                }
                
            }
        }
        base.SetBurning(false);
        dryness = Random.Range(60, 100);
        speed = Random.Range(1, 3);
        terrain = "forest";
    }

    public override void SetBorderSprite(Sprite sprite, float rotation)
    {
        if (sprite.name == "grassEdge" || sprite.name == "grassCorner")
        {
            base.SetBorderSprite(sprite, rotation);
        }
    }

}