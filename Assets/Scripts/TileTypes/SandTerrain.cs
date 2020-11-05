using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandTerrain : TileScript
{
    public Sprite[] rocks;
    private void Start()
    {
        Transform[] children = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            if (Random.Range(0, 5) == 3)
                transform.GetChild(i).GetComponent<SpriteRenderer>().sprite =
                    rocks[Random.Range(0, rocks.Length)];
        }
        base.setBurning(true);
        // dryness = Random.Range(40, 60);
        // speed = Random.Range(4, 6);
        //terrian = "Sand";
        //Instantiate(prefab, new Vector3(2.0F, 0, 0), Quaternion.identity);
    }
}
