using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestTerrain : TileScript
{
   // public Sprite dirtSprite;
    public Sprite[] plants;
    private void Start()
    {
        Transform[] children = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            int r = Random.Range(0, 3);
            if (Random.Range(0, 3) == 2)
            {
                transform.GetChild(i).GetComponent<SpriteRenderer>().sprite =
                plants[Random.Range(0, plants.Length)];
            }
            
        }
       // base.SetSprite(dirtSprite);
        base.setBurning(true);
        dryness = Random.Range(60, 100); 
        speed = Random.Range(1, 3);
        terrian = "Forest";
        //Instantiate(prefab, new Vector3(2.0F, 0, 0), Quaternion.identity);
    }
}

