using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTerrain : TileScript
{
    public override void SetBurning(bool change) => burning = false;

    public override void BuildFireLine() => base.TileNotificationText("Can't build fire line here.");
    private void Start()
    {
        dryness = 0;
        SetOccupied(true);
    }
    public override void SetBorderSprite(Sprite sprite, float rotation)
    {
        if (sprite.name != "roadEdge" && sprite.name != "sandCorner" && sprite.name != "roadCorner")
        {
            base.SetBorderSprite(sprite, rotation);
        }
            
    }
}
