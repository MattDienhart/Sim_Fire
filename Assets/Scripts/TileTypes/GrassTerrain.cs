using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassTerrain : TileScript
{
    public override void BuildFireLine() => CheckHouse();

    public Sprite[] buildings;
    bool houseHere = false;
    private void Start()
    {
        if (Random.Range(0, 2) == 1)
        {
            Sprite house = buildings[Random.Range(0, buildings.Length)];
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = house;
            houseHere = true;
            SetOccupied(true);
        }

        dryness = Random.Range(40, 60);
        speed = Random.Range(4, 6);
        terrain = "grass";
        
    }

    private void CheckHouse()
    {
        if (houseHere)
        {
            TileNotificationText("Can't build fire line here.");
        }
        else
        {
            base.BuildFireLine();
        }
            
    }
        
}