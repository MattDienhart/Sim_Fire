using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wildfire : MonoBehaviour
{
    public int hitPoints;
    public Tile * tile;
    public string windDirection;
    public int windSpeed;


    //Constructor
    public Wildfire(Tile * tileParameter, string direction, int speed)
    {
        hitPoints = 100;
        tile = tileParameter;
        windDirection = direction;
        windSpeed = speed;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Change tile state to on fire
        tile.onFire = true;
        //Wait 5 seconds
        StartCoroutine(waiter(5));
        //Check if adjacent tile can be lit aflame

    }

    // Update is called once per frame
    void Update()
    {
        //If fire dies
        if (hitPoints <= 0) 
        {
            Destroy(this.Wildfire);
        }
    }

    //Coroutine to make fire wait
    IEnumerator waiter(int seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    
}
