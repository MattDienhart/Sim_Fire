using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassTerrain : TileScript
{
    public Sprite[] buildings;
    private void Start()
    {
        if (Random.Range(0, 3) == 2)
        {
            Sprite house = buildings[Random.Range(0, buildings.Length)];
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = house;
        }
            
        base.setBurning(true);
        // dryness = Random.Range(40, 60);
        // speed = Random.Range(4, 6);
        //terrian = "Sand";
        //Instantiate(prefab, new Vector3(2.0F, 0, 0), Quaternion.identity);
    }
}
