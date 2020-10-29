using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerBehavior : MonoBehaviour
{
    private GameObject[] allTiles;
    private string windDirection;
    private int money;
    private int happiness;
    private int fireCrewInstances;
    private int wildfireInstances;

    public GameObject fireCrewPrefab;
    public GameObject firePrefab;

    private GameObject selectedFireCrew;
    public GameObject SelectedFireCrew
    {
        get 
        {
            return selectedFireCrew;
        }
        set 
        {
            selectedFireCrew = value;
            print("This object was selected: Fire Crew " + selectedFireCrew.GetComponent<FireCrew>().CrewID);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        allTiles = GameObject.FindGameObjectsWithTag("Tile");

        // instantiate the first set of fire crews at the start of the game
        fireCrewInstances = 0;
        AddFireCrew(allTiles[45]);
        AddFireCrew(allTiles[110]);

        // Instantiate wildfire
        wildfireInstances = 0;
        lightTile(allTiles[1]);

        // Start wildfire behavior
        allTiles[1].GetComponent<TileScript>().setBurning(true);
        InvokeRepeating("wildfireBehavior", 5, 5);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Spawn new fireCrew instances from the FireCrew prefab
    void AddFireCrew(GameObject spawnLocation)
    {
        GameObject newFireCrew = (GameObject)Instantiate(fireCrewPrefab);
        newFireCrew.transform.position = spawnLocation.transform.position;
        newFireCrew.GetComponent<FireCrew>().CrewID = fireCrewInstances + 1;
        newFireCrew.GetComponent<FireCrew>().waterLevel = 100;
        newFireCrew.GetComponent<FireCrew>().energyLevel = 100;
        fireCrewInstances ++;
    }

    //Spawn new Wildfire instance from wildfire prefab
    void lightTile(GameObject spawnLocation)
    {
        GameObject newWildfire = (GameObject)Instantiate(firePrefab);
        newWildfire.transform.position = spawnLocation.transform.position;
        newWildfire.GetComponent<Wildfire>().hitPoints = 100;
        newWildfire.GetComponent<Wildfire>().windDirection = "North";
        newWildfire.GetComponent<Wildfire>().windSpeed = 10;
        wildfireInstances ++;
    }


    void _spreadfire(GameObject litTile, GameObject adjTile, string windDirection, int index) {
        //Make sure tile exists (litTile might be on edge)
        if(!GameObject.ReferenceEquals(litTile, adjTile))
        {
            Debug.Log("This adjacent tile exists");
            //Make sure tile isn't burning
            if(!adjTile.GetComponent<TileScript>().getBurning()) 
            {
                Debug.Log("And it's not on fire");
                int chanceToBurn = litTile.GetComponent<TileScript>().getDryness();

                //Check wind direction
                if((windDirection == "North") && (GameObject.ReferenceEquals(adjTile, litTile.GetComponent<TileScript>().GetSouth()))) chanceToBurn *= 2;
                if((windDirection == "South") && (GameObject.ReferenceEquals(adjTile, litTile.GetComponent<TileScript>().GetNorth()))) chanceToBurn *= 2;
                if((windDirection == "East") && (GameObject.ReferenceEquals(adjTile, litTile.GetComponent<TileScript>().GetWest()))) chanceToBurn *= 2;
                if((windDirection == "West") && (GameObject.ReferenceEquals(adjTile, litTile.GetComponent<TileScript>().GetEast()))) chanceToBurn *= 2;

                int multiplier = 0;

                //Check tiles adjacent to north adjacent tile
                if(adjTile.GetComponent<TileScript>().GetNorth().GetComponent<TileScript>().getBurning()) multiplier++;
                if(adjTile.GetComponent<TileScript>().GetSouth().GetComponent<TileScript>().getBurning()) multiplier++;
                if(adjTile.GetComponent<TileScript>().GetEast().GetComponent<TileScript>().getBurning()) multiplier++;
                if(adjTile.GetComponent<TileScript>().GetWest().GetComponent<TileScript>().getBurning()) multiplier++;

                chanceToBurn += 10 * multiplier;
                Debug.Log("Chance to burn is: " + chanceToBurn.ToString());

                int roll = Random.Range(0, 100);
                Debug.Log("Dice roll is: " + roll.ToString());

                //Roll die to see if fire spreads
                if (roll < chanceToBurn) {
                    adjTile.GetComponent<TileScript>().setBurning(true);
                    lightTile(adjTile);
                    Debug.Log("Tile " + index.ToString() + " is lit!");
                }
            }
        }
    }

    void spreadFire(GameObject litTile, string windDirection, int index) 
    {
        Debug.Log("Spreadfire function run with wind: " + windDirection + " at tile: " + index.ToString());
        GameObject northTile = litTile.GetComponent<TileScript>().GetNorth();
        GameObject southTile = litTile.GetComponent<TileScript>().GetSouth();
        GameObject eastTile = litTile.GetComponent<TileScript>().GetEast();
        GameObject westTile = litTile.GetComponent<TileScript>().GetWest();
        if(litTile.Equals(northTile)) Debug.Log("Lit and north are the same!");
        if(litTile.Equals(southTile)) Debug.Log("Lit and south are the same!");
        if(litTile.Equals(eastTile)) Debug.Log("Lit and east are the same!");
        if(litTile.Equals(westTile)) Debug.Log("Lit and west are the same!");

        _spreadfire(litTile, northTile, windDirection, index - 18);
        _spreadfire(litTile, southTile, windDirection, index + 18);
        _spreadfire(litTile, eastTile, windDirection, index + 1);
        _spreadfire(litTile, westTile, windDirection, index - 1);
    }

    void wildfireBehavior() {
        for(int i = 1; i < allTiles.Length; i++) 
        {
            Debug.Log("Checking tile #" + i.ToString());
            if(allTiles[i].GetComponent<TileScript>().getBurning())
            {
                Debug.Log("This tile is on fire!");
                spreadFire(allTiles[i], "North", i);
            }
        }
    }

    void pickEvent() {
        int dice = Random.Range(0, 100);

        //Calendar Sale
        if((dice > 40) && (dice < 55)) {

        }

        //Lightning Strikes
        if((dice > 55) && (dice < 60)) {

        }

        //Career fair
        if((dice > 60) && (dice < 75)) {

        }

        //Charitable donation
        if((dice > 75) && (dice < 80)) {

        }

        //Retirement
        if((dice > 80) && (dice < 90)) {

        }

        //Gender reveal party
        if((dice > 90) && (dice <= 100)) {

        }
    }
}