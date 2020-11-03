
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    private GameObject[] allTiles;
    private GameObject[] fireHouses;
    private GameObject[] wildFires;
    List<int> litTiles = new List<int>();
    private string windDirection;
    public int money;
    public int happiness;
    public int difficulty;
    private int fireCrewInstances;
    private int helicopterInstances;
    private int wildfireInstances;

    public GameObject fireCrewPrefab;
    public GameObject firePrefab;
    
    private GameObject selectedFireCrew;
    private GameObject selectedTile;

    public bool DestSelectModeOn;
    

    [Header("HUD")]
    public Text moneyText;
    public Text selectedText;
<<<<<<< HEAD
    public Text happinessText;
    public Text moneyText;
=======
    public Text notificationText;
    public Text windDirectionText;
>>>>>>> 776b0976db7b3b58db827f9891a131ae477a3c9e
    public Button crewBtn;
    public Button dispatchBtn;
    public Button infoBtn;
    


    // Start is called before the first frame update
    void Start()
    {
        allTiles = GameObject.FindGameObjectsWithTag("Tile");
        fireHouses = GameObject.FindGameObjectsWithTag("Firehouse");
        wildFires =  new GameObject[181];
        windDirection = pickWindDirection();
        difficulty = 2;
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
<<<<<<< HEAD
        lightTile(allTiles[1], 1);
        lightTile(allTiles[3], 3);
        lightTile(allTiles[34], 34);
        Debug.Log("here: " + System.Int32.Parse(Regex.Replace(moneyText.text, "[^.0-9]", "")));
=======
        StartCoroutine(lightTile(allTiles[29], 29));
        StartCoroutine(lightTile(allTiles[138], 138));
        StartCoroutine(sendNotification("Oh no, there are two wildfires! Put them out!", 3));

>>>>>>> 776b0976db7b3b58db827f9891a131ae477a3c9e
        // Start wildfire behavior
        InvokeRepeating("wildfireBehavior", 10, 40);
        InvokeRepeating("pickEvent", 60, 120);

        DestSelectModeOn = false;
    }

    // Update is called once per frame
    void Update()
    {
<<<<<<< HEAD
        if (happiness != System.Convert.ToInt32(happinessText.text)) { }
            happinessText.text = happiness.ToString();
        if (money !=  System.Int32.Parse(Regex.Replace(moneyText.text, "[^.0-9]", "")))
        {
            moneyText.text = "$" + money.ToString();
        }   
=======
        moneyText.text = "$" + money.ToString();
>>>>>>> 776b0976db7b3b58db827f9891a131ae477a3c9e
    }

    // Spawn new fireCrew instances from the FireCrew prefab
    void AddFireCrew(GameObject spawnLocation)
    {
        GameObject newFireCrew = (GameObject)Instantiate(fireCrewPrefab);
        newFireCrew.transform.position = spawnLocation.transform.position;
        newFireCrew.GetComponent<FireCrew>().CrewID = fireCrewInstances + 1;
        newFireCrew.GetComponent<FireCrew>().waterLevel = 100;
        newFireCrew.GetComponent<FireCrew>().energyLevel = 100;
        newFireCrew.GetComponent<FireCrew>().currentTile = spawnLocation;
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

        // Toggle between destination select mode OFF and ON
        // Must have selected a fire crew before trying to dispatch
        if (!DestSelectModeOn && SelectedFireCrew != null)
        {
            DestSelectModeOn = true;
            selectedText.text = "Dispatch";
        }
        else
        {
            DestSelectModeOn = false;
            selectedText.text = "";
        }
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
            if (selectedFireCrew != null)
            {
                selectedText.text = "Fire Crew " + selectedFireCrew.GetComponent<FireCrew>().CrewID;
                print("This object was selected: Fire Crew " + selectedFireCrew.GetComponent<FireCrew>().CrewID);
            }
        }
    }

    public GameObject SelectedTile
    {
        get 
        {
            return selectedTile;
        }
        set 
        {
            selectedTile = value;
            if (selectedTile != null)
            {
                print("This tile was selected: " + selectedTile.GetInstanceID());
            }
        }
    }
    
    // Spawn new Wildfire instance from wildfire prefab
    IEnumerator lightTile(GameObject spawnLocation, int tileIndex)
    {
        GameObject newWildfire = (GameObject)Instantiate(firePrefab);
        newWildfire.transform.position = spawnLocation.transform.position;
        newWildfire.GetComponent<Wildfire>().hitPoints = 100;
        spawnLocation.GetComponent<TileScript>().setBurning(true);
        wildFires[tileIndex] = newWildfire;
        Debug.Log("Tile " + tileIndex.ToString() + " is on fire!");
        litTiles.Add(tileIndex);
        wildfireInstances ++;
        yield return null;
    }

    IEnumerator _spreadFire(GameObject litTile, GameObject inspectTile, string windDirection, int index, string adjDirection, Action<bool> callback) 
    {
        //Find tiles surrounding tile adjacent to lit tile (Yes I know a little complicated, if I figure out an easier way I'll change it)
        GameObject northTile = inspectTile.GetComponent<TileScript>().northTile;
        GameObject southTile = inspectTile.GetComponent<TileScript>().southTile;
        GameObject eastTile = inspectTile.GetComponent<TileScript>().eastTile;
        GameObject westTile = inspectTile.GetComponent<TileScript>().westTile;

        // Make sure tile exists (litTile might be on edge)
        if(!GameObject.ReferenceEquals(litTile, inspectTile))
        {
            Debug.Log("The " + adjDirection + " adjacent tile exists");
            // Make sure tile isn't burning
            if(!inspectTile.GetComponent<TileScript>().getBurning()) 
            {
                Debug.Log("And it's not on fire");
                int chanceToBurn = inspectTile.GetComponent<TileScript>().getDryness();
                Debug.Log("Tile dryness is " + inspectTile.GetComponent<TileScript>().getDryness());

                // Check wind direction
                if (southTile) if((windDirection == "North") && (southTile.GetComponent<TileScript>().getBurning())) chanceToBurn *= 2;
                if (northTile) if((windDirection == "South") && (northTile.GetComponent<TileScript>().getBurning())) chanceToBurn *= 2;
                if (westTile) if((windDirection == "East") && (westTile.GetComponent<TileScript>().getBurning())) chanceToBurn *= 2;
                if (eastTile) if((windDirection == "West") && (eastTile.GetComponent<TileScript>().getBurning())) chanceToBurn *= 2;

                int multiplier = 0;

                // Check which surrounding tiles are on fire
                if (northTile) if(northTile.GetComponent<TileScript>().getBurning()) multiplier++;
                if (southTile) if(southTile.GetComponent<TileScript>().getBurning()) multiplier++;
                if (eastTile) if(eastTile.GetComponent<TileScript>().getBurning()) multiplier++;
                if (westTile) if(westTile.GetComponent<TileScript>().getBurning()) multiplier++;
                Debug.Log("Multiplier is: " + multiplier.ToString());

                chanceToBurn += 10 * multiplier;
                Debug.Log("Chance to burn is: " + chanceToBurn.ToString());

                int roll = UnityEngine.Random.Range(0, 100);
                Debug.Log("Dice roll is: " + roll.ToString());

                // Roll die to see if fire spreads
                if (roll < chanceToBurn) {                    
                    StartCoroutine(lightTile(inspectTile, index));
                    callback(true);
                }
            }
        } else {
            Debug.Log("Lit and " + adjDirection + " are the same!");
        }

        yield return null; 
    }

    IEnumerator spreadFire(GameObject litTile, string windDirection, int index) 
    {
        Debug.Log("Spreadfire function run with wind: " + windDirection + " at tile: " + index.ToString());

        //Grab surrounding tiles to lit tile
        GameObject northTile = litTile.GetComponent<TileScript>().GetNorth();
        GameObject southTile = litTile.GetComponent<TileScript>().GetSouth();
        GameObject eastTile = litTile.GetComponent<TileScript>().GetEast();
        GameObject westTile = litTile.GetComponent<TileScript>().GetWest();

        bool hasLitOne = false;
        yield return new WaitForSeconds(1); 

        //Execute _spreadfire if tile exists and none have been newly lit on fire
        if ((northTile) && (!hasLitOne)) StartCoroutine(_spreadFire(litTile, northTile, windDirection, index - 18, "North",  (i) =>
        {
            hasLitOne = i;
        })); 
        
        Debug.Log("hasLitOne is " + hasLitOne.ToString());

        if ((southTile) && (!hasLitOne)) StartCoroutine(_spreadFire(litTile, southTile, windDirection, index + 18, "South",  (i) =>
        {
            hasLitOne = i;
        })); 
        
        Debug.Log("hasLitOne is " + hasLitOne.ToString());

        if ((eastTile) && (!hasLitOne)) StartCoroutine(_spreadFire(litTile, eastTile, windDirection, index + 1, "East",  (i) =>
        {
            hasLitOne = i;
        })); 
        
        Debug.Log("hasLitOne is " + hasLitOne.ToString());

        if ((westTile) && (!hasLitOne)) StartCoroutine(_spreadFire(litTile, westTile, windDirection, index - 1, "West",  (i) =>
        {
            hasLitOne = i;
        })); 
        
        Debug.Log("hasLitOne is " + hasLitOne.ToString());

        yield return null;    
    }

    void wildfireBehavior() 
    {
        StartCoroutine(sendNotification("The fire is spreading!", 2));

        //Spread fire as much as difficulty allows
        for(int i = 0; i < difficulty; i++) {
            Debug.Log("Executing spreadFire " + (i + 1).ToString() + " time");
            int randomIndex = (int)UnityEngine.Random.Range(0, litTiles.Count);
            StartCoroutine(spreadFire(allTiles[litTiles[randomIndex]], windDirection, litTiles[randomIndex]));
        }
    }

    IEnumerator putOutFire(int tileNumber) 
    {
        if(allTiles[tileNumber].GetComponent<TileScript>().getBurning()) 
        {
           // allTiles[tileNumber].GetComponent<TileScript>().setBurning(false);
            Destroy(wildFires[tileNumber]);
            wildfireInstances--;
            litTiles.Remove(tileNumber);
        }

        Debug.Log("Put out fire at tile: " + tileNumber.ToString());
        StartCoroutine(sendNotification("Fire has been put out! HUZZAH!", 2));
        yield return null;
    }

    void pickEvent() 
    {
        int dice = UnityEngine.Random.Range(0, 110);
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
            int unluckyTile = UnityEngine.Random.Range(1,180);
            
            // Make sure tile isn't already on fire
            while(allTiles[unluckyTile].GetComponent<TileScript>().getBurning()) 
            {
                unluckyTile++;
            }

            StartCoroutine(lightTile(allTiles[unluckyTile], unluckyTile));

            // Display alert message
            StartCoroutine(sendNotification("Oh no! Lightning struck and a wildfire started! Put it out!", 3));
            Debug.Log("Lightning event triggered!");
            Debug.Log("Unlucky tile:" + unluckyTile.ToString());
        }

        // NEEDS TO INSTANTIATE AT FIREHOUSE LOCATION
        // FOR NOW INSTANTIATES AT TILE 40
        // Career fair
        if((dice > 60) && (dice <= 75)) 
        {
            int bonus = UnityEngine.Random.Range(1, 3);
            
            // Add fire crew
            fireCrewInstances += bonus;

            for(int i = 0; i < bonus; i++) 
            {
                //ADD FIRE CREW AND INSTANTIATE AT FIRESTATION
                AddFireCrew(allTiles[40]);
            }

            string alert = "The career fair worked, we've added " + bonus.ToString() + " recruits!";

            // Display alert message
            StartCoroutine(sendNotification(alert, 3));
            Debug.Log("Career fair event triggered!");
        }

        // NEEDS HELICOPTER IMPLEMENTATION
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
            int penalty = UnityEngine.Random.Range(1, 2);

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
            int unluckyTile = UnityEngine.Random.Range(1,180);
            
            // Make sure tile isn't already on fire
            while(allTiles[unluckyTile].GetComponent<TileScript>().getBurning())
            {
                unluckyTile++;
            }

            StartCoroutine(lightTile(allTiles[unluckyTile], unluckyTile));

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
            for(int i = 0 ; i < 3; i++) 
            {
                if(litTileIndex.Count > 0) 
                {
                    int index = UnityEngine.Random.Range(0, litTileIndex.Count);
                    StartCoroutine(putOutFire(litTileIndex[index]));
                    litTileIndex.RemoveAt(index);
                }
            }

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
        switch ((int)UnityEngine.Random.Range(1, 4)) {
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
