using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadScript : TileScript
{
    // public override void SetBurning(bool change) => burning = false;
    public override void BuildFireLine() => 
        base.TileNotificationText("Can't build fire line here.");

    // Start is called before the first frame update
    void Start()
    {
        dryness = 0;
        speed = Random.Range(8, 10);
        terrain = "Road";
    }

}