using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wildfire : MonoBehaviour
{
    public int hitPoints;
    public int HitPoints 
    {
        get
        {
            return hitPoints;
        }
        set 
        {
            hitPoints = value;
        }
    }
    public string windDirection;
    public string WindDirection 
    {
        get
        {
            return windDirection;
        }
        set 
        {
            windDirection = value;
        }
    }
    public int windSpeed;
    public int WindSpeed 
    {
        get
        {
            return windSpeed;
        }
        set 
        {
            windSpeed = value;
        }
    }

    public int tileLocation;
    public int TileLocation 
    {
        get
        {
            return tileLocation;
        }
        set 
        {
            tileLocation = value;
        }
    }

    //Constructor
    public Wildfire()
    {
        // hitPoints = 100;
        // windDirection = "direction";
        // windSpeed = 10;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Change tile state to on fire
        //tile.burning = true;
        //Wait 5 seconds
        //StartCoroutine(waiter(5));
        //Spread fire AI
        //Debug.Log("testing");
    }

    // Update is called once per frame
    void Update()
    {
        //If fire dies
        if (hitPoints <= 0) 
        {
            //Destroy(this.Wildfire);
        }
    }

    //Coroutine to make fire wait
    IEnumerator waiter(int seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}