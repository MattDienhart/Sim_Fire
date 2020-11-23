using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int money;
    public int happiness;
    public string windDirection;
    public List<int> litTiles = new List<int>();

    public GameData (GameManager gameManager) 
    {
        money = gameManager.money;
        happiness = gameManager.happiness;
        windDirection = gameManager.windDirection;
        litTiles = gameManager.litTiles;
    }
}
