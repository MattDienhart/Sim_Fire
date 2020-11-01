using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private GameObject[] allTiles;
    private GameObject[] fireStations;
    private GameObject[] wildFires;
    private string windDirection;
    public int money;
    public int happiness;
    private int fireCrewInstances;
    private int helicopterInstances;
    private int wildfireInstances;

    public GameObject fireCrewPrefab;
    public GameObject firePrefab;
    
    private GameObject selectedFireCrew;
    

    [Header("HUD")]
    public Text selectedText;
    public Text notificationText;
    public Text windDirectionText;
    public Button crewBtn;
    public Button dispatchBtn;
    public Button infoBtn;


    // Start is called before the first frame update
    void Start()
    {
        allTiles = GameObject.FindGameObjectsWithTag("Tile");
        wildFires =  new GameObject[181];
        windDirection = pickWindDirection();
        windDirectionText.text = "The wind blows: \n" + windDirection;

        // Set on click listeners
        crewBtn.onClick.AddListener(() => CrewClicked());
        dispatchBtn.onClick.AddListener(() => DispatchClicked());
        infoBtn.onClick.AddListener(() => InfoClicked());
        // instantiate the first set of fire crews at the start of the game
        fireCrewInstances = 0;
        AddFireCrew(allTiles[45]);
        AddFireCrew(allTiles[110]);

        // Instantiate wildfire
        wildfireInstances = 0;
        lightTile(allTiles[29], 29);
        lightTile(allTiles[138], 138);
        StartCoroutine(sendNotification("Oh no, there are two wildfires! Put them out!", 3));

        // Start wildfire behavior
        InvokeRepeating("wildfireBehavior", 10, 10);
        //InvokeRepeating("pickEvent", 20, 20);
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

    void CrewClicked()
    {
        Debug.Log("Crew button has been clicked.");
        selectedText.text = "Crew";
    }

    void DispatchClicked()
    {
        Debug.Log("Dispatch button has been clicked.");
        selectedText.text = "Dispatch";
    }

    void InfoClicked()
    {
        Debug.Log("Info button has been clicked.");
        selectedText.text = "Info";
    }

    public GameObject SelectedFireCrew
    {
        get
        {
            return selectedFireCrew;
        }
        set
        {
            selectedFireCrew = value;
            selectedText.text = "Fire Crew " + selectedFireCrew.GetComponent<FireCrew>().CrewID;
            print("This object was selected: Fire Crew " + selectedFireCrew.GetComponent<FireCrew>().CrewID);
        }
    }
    
    // Spawn new Wildfire instance from wildfire prefab
    void lightTile(GameObject spawnLocation, int tileIndex)
    {
        GameObject newWildfire = (GameObject)Instantiate(firePrefab);
        newWildfire.transform.position = spawnLocation.transform.position;
        newWildfire.GetComponent<Wildfire>().hitPoints = 100;
        spawnLocation.GetComponent<TileScript>().setBurning(true);
        wildFires[tileIndex] = newWildfire;
        Debug.Log("Tile " + tileIndex.ToString() + " is on fire!");
        wildfireInstances ++;
    }

    void _spreadfire(GameObject litTile, GameObject adjTile, string windDirection, int index, string adjDirection) 
    {
        // Make sure tile exists (litTile might be on edge)
        if(!GameObject.ReferenceEquals(litTile, adjTile))
        {
            Debug.Log("The " + adjDirection + " adjacent tile exists");
            // Make sure tile isn't burning
            if(!adjTile.GetComponent<TileScript>().getBurning()) 
            {
                Debug.Log("And it's not on fire");
                int chanceToBurn = litTile.GetComponent<TileScript>().getDryness();

                // Check wind direction
                if((windDirection == "North") && (GameObject.ReferenceEquals(adjTile, litTile.GetComponent<TileScript>().GetSouth()))) chanceToBurn *= 2;
                if((windDirection == "South") && (GameObject.ReferenceEquals(adjTile, litTile.GetComponent<TileScript>().GetNorth()))) chanceToBurn *= 2;
                if((windDirection == "East") && (GameObject.ReferenceEquals(adjTile, litTile.GetComponent<TileScript>().GetWest()))) chanceToBurn *= 2;
                if((windDirection == "West") && (GameObject.ReferenceEquals(adjTile, litTile.GetComponent<TileScript>().GetEast()))) chanceToBurn *= 2;

                int multiplier = 0;

                // Check tiles adjacent to north adjacent tile
                if(adjTile.GetComponent<TileScript>().GetNorth().GetComponent<TileScript>().getBurning()) multiplier++;
                if(adjTile.GetComponent<TileScript>().GetSouth().GetComponent<TileScript>().getBurning()) multiplier++;
                if(adjTile.GetComponent<TileScript>().GetEast().GetComponent<TileScript>().getBurning()) multiplier++;
                if(adjTile.GetComponent<TileScript>().GetWest().GetComponent<TileScript>().getBurning()) multiplier++;

                chanceToBurn += 10 * multiplier;
                Debug.Log("Chance to burn is: " + chanceToBurn.ToString());

                int roll = Random.Range(0, 100);
                Debug.Log("Dice roll is: " + roll.ToString());

                // Roll die to see if fire spreads
                if (roll < chanceToBurn) {                    
                    lightTile(adjTile, index);
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
        if(litTile.Equals(northTile))
        {
            Debug.Log("Lit and north are the same!");
        } 
        else 
        {
            _spreadfire(litTile, northTile, windDirection, index - 18, "North");
        }
        if(litTile.Equals(southTile)) 
        {
            Debug.Log("Lit and south are the same!");
        }
        else
        {
            _spreadfire(litTile, southTile, windDirection, index + 18, "South");
        }
        if(litTile.Equals(eastTile)) 
        {
            Debug.Log("Lit and east are the same!");
        }
        else
        {
            _spreadfire(litTile, eastTile, windDirection, index + 1, "East");
        }
        if(litTile.Equals(westTile)) 
        {
            Debug.Log("Lit and west are the same!");
        }
        else 
        {
            _spreadfire(litTile, westTile, windDirection, index - 1, "West");
        }    
    }

    void wildfireBehavior() 
    {
        StartCoroutine(sendNotification("The fire is spreading!", 2));

        for(int i = 1; i < allTiles.Length; i++) 
        {
            Debug.Log("Checking tile #" + i.ToString());

            if(allTiles[i].GetComponent<TileScript>().getBurning())
            {
                Debug.Log("This tile is on fire!");
                spreadFire(allTiles[i], windDirection, i);
            }
        }
    }

    void putOutFire(int tileNumber) 
    {
        if(allTiles[tileNumber].GetComponent<TileScript>().getBurning()) 
        {
            allTiles[tileNumber].GetComponent<TileScript>().setBurning(false);
            Destroy(wildFires[tileNumber]);
        }

        Debug.Log("Put out fire at tile: " + tileNumber.ToString());
        StartCoroutine(sendNotification("Fire has been put out! HUZZAH!", 2));
    }

    void pickEvent() 
    {
        int dice = Random.Range(0, 110);
        //int dice = 105;
        Debug.Log("Game Event Dice Roll is: " + dice.ToString());

        //Nothing
        if(dice <= 40) 
        {
            // Display alert message
            Debug.Log("Nothing event triggered!");
        }

        // Calendar Sale
        if((dice > 40) && (dice <= 55)) 
        {
            // Add money to player resources
            money += 1000;

            // Display alert message
            StartCoroutine(sendNotification("The calendar sale was a success! You've added $1000!", 3));
            Debug.Log("Calendar event triggered!");
        }

        // Lightning Strikes
        if((dice > 55) && (dice <= 60)) 
        {
            int unluckyTile = Random.Range(1,180);
            
            // Make sure tile isn't already on fire
            while(allTiles[unluckyTile].GetComponent<TileScript>().getBurning()) 
            {
                unluckyTile++;
            }

            lightTile(allTiles[unluckyTile], unluckyTile);

            // Display alert message
            StartCoroutine(sendNotification("Oh no! Lightning struck and a wildfire started! Put it out!", 3));
            Debug.Log("Lightning event triggered!");
        }

        //UNFINISHED
        // Career fair
        if((dice > 60) && (dice <= 75)) 
        {
            int bonus = Random.Range(1, 3);
            
            // Add fire crew
            fireCrewInstances += bonus;

            for(int i = 0; i < bonus; i++) 
            {
                //ADD FIRE CREW AND INSTANTIATE AT FIRESTATION
                //AddFireCrew(fireStations[Random.Range(0, fireStations.Length)]);
            }

            string alert = "The career fair worked, we've added " + bonus.ToString() + " recruits!";

            // Display alert message
            StartCoroutine(sendNotification(alert, 3));
            Debug.Log("Career fair event triggered!");
        }

        //UNFINISHED
        // Charitable donation
        if((dice > 75) && (dice <= 80)) 
        {
            // Add money
            money += 10000;

            // Add helicopter
            helicopterInstances++;
            //INSTANTIATE HELICOPTER HERE

            // Display alert message
            StartCoroutine(sendNotification("Generous donor alert! $10,000 added as well as your very own helicopter!", 3));
            Debug.Log("Donation event triggered!");
        }

        //UNFINISHED
        // Retirement
        if((dice > 80) && (dice <= 90)) 
        {
            int penalty = Random.Range(1, 2);

            // Subtract firecrew instances
            fireCrewInstances -=  penalty;

            for(int i = 0; i < penalty; i++) 
            {
                // DESTROY INSTANCE CODE HERE
            }

            string alert = "It looks like " + penalty.ToString() + " of our own are retiring, they've put in their time. Well deserved!";

            // Display alert message
            StartCoroutine(sendNotification(alert, 3));
            Debug.Log("Retirement event triggered!");
        }

        //Gender reveal party
        if((dice > 90) && (dice <= 100)) 
        {
            int unluckyTile = Random.Range(1,180);
            
            // Make sure tile isn't already on fire
            while(allTiles[unluckyTile].GetComponent<TileScript>().getBurning())
            {
                unluckyTile++;
            }

            lightTile(allTiles[unluckyTile], unluckyTile);

            // Display alert message
            StartCoroutine(sendNotification("A new wildfire has appeared! It looks like some nearby fireworks may be the cause.", 3));
            Debug.Log("Reveal party event triggered!");
        }

        //Heavy rain
        if((dice > 100) && (dice <= 110)) 
        {
            List<int> litTileIndex = new List<int>();

            //Grab every lit tile
            for(int i = 0; i < allTiles.Length; i++)
            {
                if(allTiles[i].GetComponent<TileScript>().getBurning())
                {
                    litTileIndex.Add(i);
                }
            }

            //Put out 3 random fires
            putOutFire(litTileIndex[Random.Range(0, litTileIndex.Count)]);
            putOutFire(litTileIndex[Random.Range(0, litTileIndex.Count)]);
            putOutFire(litTileIndex[Random.Range(0, litTileIndex.Count)]);

            // Display alert message
            StartCoroutine(sendNotification("Sweet rain! It's putting out a few fires!", 3));
            Debug.Log("Heavy rain event triggered!");
        }
    }

    IEnumerator sendNotification(string text, int time)
    {
        notificationText.text = text;
        yield return new WaitForSeconds(time);
        notificationText.text = "";
    }

    string pickWindDirection() 
    {
        switch ((int)Random.Range(1, 4)) {
            case 1:
                return "North";

            case 2:
                return "South";

            case 3:
                return "East";

            case 4:
                return "West";
        }

        return "East";
    }
}
