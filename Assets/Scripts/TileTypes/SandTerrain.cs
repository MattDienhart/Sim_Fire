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
                transform.GetChild(i).GetComponent<SpriteRenderer>().sprite =
                    rocks[Random.Range(0, rocks.Length)];
        }
        base.setBurning(true);
        // dryness = Random.Range(40, 60);
        // speed = Random.Range(4, 6);
        //terrian = "Sand";
        //Instantiate(prefab, new Vector3(2.0F, 0, 0), Quaternion.identity);
    }

    public override void SetBorderSprite(Sprite sprite, float rotation)
    {
        Debug.Log(" -- Ov 2boom" + this.name + " name " + sprite.name);
        if (string.Equals(sprite.name, "dirtCorner")) Debug.Log("TRUE" + this.name);
        if (sprite.name == "dirtEdge" || sprite.name == "dirtCorner" 
            || sprite.name == "grassEdge" || sprite.name == "grassCorner")
        {
            if(sprite.name == "dirtCorner" ) Debug.Log("2222dirt corner " + this.name);
            base.SetBorderSprite(sprite, rotation);
        }
            
    }



}
