using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTerrain : TileScript
{
    public override void setBurning(bool change) => burning = false;
    private void Start()
    {
        dryness = 0;
    }

}
