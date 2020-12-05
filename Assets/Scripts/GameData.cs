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
    public List<string> crewTileLocations = new List<string>();
    public List<string> truckTileLocations = new List<string>();
    public List<string> helicopterTileLocations = new List<string>();
    // public int[] entireGrid;
    // public int waterColumn;

    public GameData (GameManager gameManager) 
    {
        money = gameManager.money;
        happiness = gameManager.happiness;
        windDirection = gameManager.windDirection;
        litTiles = gameManager.litTiles;
        crewTileLocations = gameManager.crewTileLocations;
        truckTileLocations = gameManager.truckTileLocations;
        helicopterTileLocations = gameManager.helicopterTileLocations;
        // entireGrid = gameManager.entireGrid;
        // waterColumn = gameManager.waterColumn;
    }
}
