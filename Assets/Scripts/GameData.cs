using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int money;
    public int happiness;
    public string windDirection;

    public GameData (GameManager gameManager) 
    {
        money = gameManager.money;
        happiness = gameManager.happiness;
        windDirection = gameManager.windDirection;
    }
}
