using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTerrain : TileScript
{
    public override void SetBurning(bool change) => burning = false;
    private void Start()
    {
        dryness = 0;
    }
    public override void SetBorderSprite(Sprite sprite, float rotation)
    {
        if (sprite.name != "roadEdge" && sprite.name != "sandCorner" && sprite.name != "roadCorner")
        {
            base.SetBorderSprite(sprite, rotation);
        }
            
    }
}
