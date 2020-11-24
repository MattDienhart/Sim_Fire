
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    private GameObject[] allTiles;
    public GameObject[] AllTiles 
    {
        get
        {
            return allTiles;
        }
        set 
        {
            allTiles = value;
        }
    }
    private List<GameObject> fireCrew = new List<GameObject>();
    private List<GameObject> fireTruck = new List<GameObject>();
    private List<GameObject> helicopter = new List<GameObject>();
    private GameObject[] wildFires;
    private List<int> litTiles = new List<int>();
    private string windDirection;
    public int money;
    public int happiness;
    public int Happiness 
    {
        get
        {
            return happiness;
        }
        set 
        {
            happiness = value;
        }
    }
    public int difficulty;
    private int fireCrewInstances;
    private int fireTruckInstances;
    private int helicopterInstances;
    private int wildfireInstances;

    public GameObject fireCrewPrefab;
    public GameObject fireTruckPrefab;
    public GameObject helicopterPrefab;
    public GameObject firePrefab;
    
    private GameObject selectedUnit;
    private GameObject selectedTile;

    public bool DestSelectModeOn;
    public bool TargetSelectModeOn;
    

    [Header("HUD")]
    public Text moneyText;
    public Text selectedText;

    public Text happinessText;

    public Text notificationText;
    public Text windDirectionText;

    public Button crewBtn;
    public Button dispatchBtn;
    public Button infoBtn;
    public Button purchaseCrewBtn;
    public Button purchaseTruckBtn;
    

    // Start is called before the first frame update
    void Start()
    {
        allTiles = GameObject.FindGameObjectsWithTag("Tile");
        wildFires =  new GameObject[181];
        windDirection = PickWindDirection();
        difficulty = 2;
        windDirectionText.text = "The wind blows: \n" + windDirection;

        // Set on click listeners
        crewBtn.onClick.AddListener(() => CrewClicked());
        dispatchBtn.onClick.AddListener(() => DispatchClicked());
        infoBtn.onClick.AddListener(() => InfoClicked());
        purchaseCrewBtn.onClick.AddListener(() => PurchaseCrewClicked());
        purchaseTruckBtn.onClick.AddListener(() => PurchaseTruckClicked());
        // instantiate the first set of fire crews at the start of the game
        fireCrewInstances = 0;
        fireTruckInstances = 0;
        helicopterInstances = 0;
        AddFireCrew(allTiles[45]);
        AddFireCrew(allTiles[110]);
        AddFireTruck(allTiles[111]);
        AddHelicopter(allTiles[112]);

        // Instantiate wildfire
        wildfireInstances = 0;
        Debug.Log("here: " + System.Int32.Parse(Regex.Replace(moneyText.text, "[^.0-9]", "")));

        StartCoroutine(LightTile(allTiles[29], 29));
        StartCoroutine(LightTile(allTiles[138], 138));
        StartCoroutine(SendNotification("Oh no, there are two wildfires! Put them out!", 3));

        // Start wildfire behavior
        InvokeRepeating("WildFireBehavior", 10, 40);
        InvokeRepeating("PickEvent", 60, 120);
        InvokeRepeating("CalcHappy", 0, 5);

        DestSelectModeOn = false;
        TargetSelectModeOn = false;
    }

    // Update is called once per frame
    void Update()
    {

        // if (happiness != System.Convert.ToInt32(happinessText.text)) { }
        //     happinessText.text = happiness.ToString();
        if (money !=  System.Int32.Parse(Regex.Replace(moneyText.text, "[^.0-9]", "")))
        {
            moneyText.text = "$" + money.ToString();
        }   

        moneyText.text = "$" + money.ToString();
        happinessText.text = happiness.ToString() + "/100";

        if ((selectedUnit != null) && (!DestSelectModeOn) && (!TargetSelectModeOn))
        {
            if (selectedUnit.CompareTag("FireCrew"))
            {
                selectedText.text = "Fire Crew " + selectedUnit.GetComponent<FireCrew>().CrewID;
            }
            else if (selectedUnit.CompareTag("FireTruck"))
            {
                selectedText.text = "Fire Truck " + selectedUnit.GetComponent<FireTruck>().TruckID;
            }
            else if (selectedUnit.CompareTag("Helicopter"))
            {
                selectedText.text = "Helicopter " + selectedUnit.GetComponent<Helicopter>().HelicopterID;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            LayerMask tileSelectMask = LayerMask.GetMask("Tile Select");
            LayerMask defaultMask = LayerMask.GetMask("Default") | LayerMask.GetMask("UI");
            LayerMask mask;

            if (DestSelectModeOn == true || TargetSelectModeOn == true)
            {
                mask = tileSelectMask;
            }
            else
            {
                mask = defaultMask;
            }

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, 20.0f, mask);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("FireCrew"))
                {
                    hit.collider.gameObject.GetComponent<FireCrew>().Selected();
                }
                else if (hit.collider.gameObject.CompareTag("FireTruck"))
                {
                    hit.collider.gameObject.GetComponent<FireTruck>().Selected();
                }
                else if (hit.collider.gameObject.CompareTag("Helicopter"))
                {
                    hit.collider.gameObject.GetComponent<Helicopter>().Selected();
                }
                else if (hit.collider.gameObject.CompareTag("Tile"))
                {
                    hit.collider.gameObject.GetComponent<TileScript>().Selected();
                }
            }
            else
            {
                selectedUnit = null;
                selectedTile = null;
                selectedText.text = "";
            }
        }
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
        fireCrew.Add(newFireCrew);
        fireCrewInstances ++;
    }

    // Spawn new Fire Truck instances from the FireTruck prefab
    void AddFireTruck(GameObject spawnLocation)
    {
        GameObject newFireTruck = (GameObject)Instantiate(fireTruckPrefab);
        newFireTruck.transform.position = spawnLocation.transform.position;
        newFireTruck.GetComponent<FireTruck>().TruckID = fireTruckInstances + 1;
        newFireTruck.GetComponent<FireTruck>().waterLevel = 100;
        newFireTruck.GetComponent<FireTruck>().currentTile = spawnLocation;
        fireTruck.Add(newFireTruck);
        fireTruckInstances ++;
    }

    void AddHelicopter(GameObject spawnLocation)
    {
        GameObject newHelicopter = (GameObject)Instantiate(helicopterPrefab);
        newHelicopter.transform.position = spawnLocation.transform.position;
        newHelicopter.GetComponent<Helicopter>().HelicopterID = helicopterInstances + 1;
        newHelicopter.GetComponent<Helicopter>().waterLevel = 100;
        newHelicopter.GetComponent<Helicopter>().currentTile = spawnLocation;
        helicopter.Add(newHelicopter);
        helicopterInstances ++;
    }

    void CrewClicked()
    {
        Debug.Log("Crew button has been clicked.");
        
        // Toggle between target select mode OFF and ON
        // Must have selected a fire crew before trying to spray water
        if ((!TargetSelectModeOn) && (SelectedUnit != null))
        {
            TargetSelectModeOn = true;
            DestSelectModeOn = false;  // don't want to have two selection modes active at the same time
            selectedText.text = "Extinguish";
        }
        else
        {
            TargetSelectModeOn = false;
            selectedText.text = "";
        }
    }

    void DispatchClicked()
    {
        Debug.Log("Dispatch button has been clicked.");

        // Toggle between destination select mode OFF and ON
        // Must have selected a fire crew before trying to dispatch
        if ((!DestSelectModeOn) && (SelectedUnit != null))
        {
            DestSelectModeOn = true;
            TargetSelectModeOn = false;  // don't want to have two selection modes active at the same time
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

    void PurchaseCrewClicked()
    {
        Debug.Log("Purchase Crew button has been clicked.");
        int crewCost = 100;

        if(money >= crewCost)
        {
            money -= crewCost;
            //AddFireCrew(GameObject.FindGameObjectWithTag("Firehouse"));
            AddFireCrew(AllTiles[39]);
        }
    }

    void PurchaseTruckClicked()
    {
        Debug.Log("Purchase Truck button has been clicked.");
        int truckCost = 1000;

        if(money >= truckCost)
        {
            money -= truckCost;
            //AddFireTruck(GameObject.FindGameObjectWithTag("Firehouse"));
            AddFireTruck(AllTiles[39]);
        }
    }

    public GameObject SelectedUnit
    {
        get
        {
            return selectedUnit;
        }
        set
        {
            selectedUnit = value;
            if (selectedUnit != null)
            {
                if (selectedUnit.CompareTag("FireCrew"))
                {
                    selectedText.text = "Fire Crew " + selectedUnit.GetComponent<FireCrew>().CrewID;
                }
                else if (selectedUnit.CompareTag("FireTruck"))
                {
                    selectedText.text = "Fire Truck " + selectedUnit.GetComponent<FireTruck>().TruckID;
                }
                else if (selectedUnit.CompareTag("Helicopter"))
                {
                    selectedText.text = "Helicopter " + selectedUnit.GetComponent<Helicopter>().HelicopterID;
                }
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
                Debug.Log("This tile was selected: " + selectedTile.GetInstanceID());
            }
        }
    }
    
    // Spawn new Wildfire instance from wildfire prefab
    IEnumerator LightTile(GameObject spawnLocation, int tileIndex)
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

    IEnumerator _SpreadFire(GameObject litTile, GameObject inspectTile, string windDirection, int index, string adjDirection, Action<bool> callback) 
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
                    StartCoroutine(LightTile(inspectTile, index));
                    callback(true);
                }
            }
        } else {
            Debug.Log("Lit and " + adjDirection + " are the same!");
        }

        yield return null; 
    }

    IEnumerator SpreadFire(GameObject litTile, string windDirection, int index) 
    {
        Debug.Log("SpreadFire function run with wind: " + windDirection + " at tile: " + index.ToString());

        //Grab surrounding tiles to lit tile
        GameObject northTile = litTile.GetComponent<TileScript>().GetNorth();
        GameObject southTile = litTile.GetComponent<TileScript>().GetSouth();
        GameObject eastTile = litTile.GetComponent<TileScript>().GetEast();
        GameObject westTile = litTile.GetComponent<TileScript>().GetWest();

        bool hasLitOne = false;
        yield return new WaitForSeconds(1);

        // Pick a random direction (North, south, east, or west)
        switch((int)UnityEngine.Random.Range(1, 4))
        {
            case 1:
            //Check if tile is occupied
            for(int i = 0; i < fireCrew.Count; i++)
            {
                if(northTile == fireCrew[i].GetComponent<FireCrew>().currentTile)
                {
                    Debug.Log("northTile is occupied");
                    goto case 2;
                }
            }

            for(int i = 0; i < fireTruck.Count; i++)
            {
                if(northTile == fireTruck[i].GetComponent<FireTruck>().currentTile)
                {
                    Debug.Log("northTile is occupied");
                    goto case 2;
                }
            }

            if ((northTile) && (!hasLitOne)) 
                StartCoroutine(_SpreadFire(litTile, northTile, windDirection, index - 18, "North",  (i) =>
                {
                    hasLitOne = i;
                }));
            Debug.Log("hasLitOne is " + hasLitOne.ToString());
            break;

            case 2:
            //Check if tile is occupied
            for(int i = 0; i < fireCrew.Count; i++)
            {
                if(southTile == fireCrew[i].GetComponent<FireCrew>().currentTile)
                {
                    Debug.Log("southTile is occupied");
                    goto case 3;
                }
            }

            for(int i = 0; i < fireTruck.Count; i++)
            {
                if(southTile == fireTruck[i].GetComponent<FireTruck>().currentTile)
                {
                    Debug.Log("southTile is occupied");
                    goto case 3;
                }
            }

            if ((southTile) && (!hasLitOne)) 
                StartCoroutine(_SpreadFire(litTile, southTile, windDirection, index + 18, "South",  (i) =>
                {
                    hasLitOne = i;
                }));
            Debug.Log("hasLitOne is " + hasLitOne.ToString());
            break;

            case 3:
            //Check if tile is occupied
            for(int i = 0; i < fireCrew.Count; i++)
            {
                if(eastTile == fireCrew[i].GetComponent<FireCrew>().currentTile)
                {
                    Debug.Log("eastTile is occupied");
                    goto case 4;
                }
            }

            for(int i = 0; i < fireTruck.Count; i++)
            {
                if(eastTile == fireTruck[i].GetComponent<FireTruck>().currentTile)
                {
                    Debug.Log("eastTile is occupied");
                    goto case 4;
                }
            }

            if ((eastTile) && (!hasLitOne)) 
            StartCoroutine(_SpreadFire(litTile, eastTile, windDirection, index + 1, "East",  (i) =>
                {
                    hasLitOne = i;
                }));
            Debug.Log("hasLitOne is " + hasLitOne.ToString());
            break;

            case 4:
            //Check if tile is occupied
            for(int i = 0; i < fireCrew.Count; i++)
            {
                if(westTile == fireCrew[i].GetComponent<FireCrew>().currentTile)
                {
                    Debug.Log("WestTile is occupied");
                    goto NoFires;
                }
            }

            for(int i = 0; i < fireTruck.Count; i++)
            {
                if(westTile == fireTruck[i].GetComponent<FireTruck>().currentTile)
                {
                    Debug.Log("westTile is occupied");
                    goto NoFires;
                }
            }

            if ((westTile) && (!hasLitOne)) 
            StartCoroutine(_SpreadFire(litTile, westTile, windDirection, index - 1, "West",  (i) =>
                {
                    hasLitOne = i;
                }));
            Debug.Log("hasLitOne is " + hasLitOne.ToString());
            break;
        }

        NoFires:
            Debug.Log("No fires have been lit");
        yield return null;    
    }

    void WildFireBehavior() 
    {
        StartCoroutine(SendNotification("The fire is spreading!", 2));

        //Spread fire as much as difficulty allows
        for(int i = 0; i < difficulty; i++) {
            Debug.Log("Executing SpreadFire " + (i + 1).ToString() + " time");
            int randomIndex = (int)UnityEngine.Random.Range(0, litTiles.Count);
            StartCoroutine(SpreadFire(allTiles[litTiles[randomIndex]], windDirection, litTiles[randomIndex]));
        }
    }

    public IEnumerator PutOutFire(int tileNumber) 
    {
        if(allTiles[tileNumber].GetComponent<TileScript>().getBurning()) 
        {
            allTiles[tileNumber].GetComponent<TileScript>().setBurning(false);
            Destroy(wildFires[tileNumber]);
            wildfireInstances--;
            litTiles.Remove(tileNumber);
            money += 100;
        }

        Debug.Log("Put out fire at tile: " + tileNumber.ToString());
        StartCoroutine(SendNotification("Fire has been put out! HUZZAH!", 2));
        yield return null;
    }

    void PickEvent() 
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
            StartCoroutine(SendNotification("The calendar sale was a success! You've added $1000!", 3));
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

            for(int i = 0; i < fireCrew.Count; i++)
            {
                if(allTiles[unluckyTile] == fireCrew[i].GetComponent<FireCrew>().currentTile)
                {
                    unluckyTile++;
                }
            }

            for(int i = 0; i < fireTruck.Count; i++)
            {
                if(allTiles[unluckyTile] == fireTruck[i].GetComponent<FireTruck>().currentTile)
                {
                    unluckyTile++;
                }
            }

            StartCoroutine(LightTile(allTiles[unluckyTile], unluckyTile));

            // Display alert message
            StartCoroutine(SendNotification("Oh no! Lightning struck and a wildfire started! Put it out!", 3));
            Debug.Log("Lightning event triggered!");
            Debug.Log("Unlucky tile:" + unluckyTile.ToString());
        }

        // Career fair
        if((dice > 60) && (dice <= 75)) 
        {
            int bonus = UnityEngine.Random.Range(1, 2);
            
            // Add fire crew
            for(int i = 0; i < bonus; i++) 
            {
                //AddFireCrew(GameObject.FindGameObjectWithTag("Firehouse"));
                AddFireCrew(AllTiles[39]);
            }

            string alert = "The career fair worked, we've added " + bonus.ToString() + " recruits!";

            // Display alert message
            StartCoroutine(SendNotification(alert, 3));
            Debug.Log("Career fair event triggered!");
        }

        // Charitable donation
        if((dice > 75) && (dice <= 80)) 
        {
            int donation = 10000;
            
            // Add money
            money += donation;

            // Add Fire Truck
            //AddFireTruck(GameObject.FindGameObjectWithTag("Firehouse"));
            AddFireTruck(AllTiles[39]);

            // Display alert message
            StartCoroutine(SendNotification("Generous donor alert! $" + donation.ToString() + " added as well as your very own helicopter!", 3));
            Debug.Log("Donation event triggered!");
        }

        // Retirement
        if((dice > 80) && (dice <= 90) && (fireCrew.Count > 0)) 
        {
            int penalty = (int)UnityEngine.Random.Range(1, 2);
            if(penalty > fireCrew.Count)
            {
                penalty = fireCrew.Count;
            }

            // Subtract firecrew instances
            for(int i = 0; i < penalty; i++) 
            {
                int random = (int)UnityEngine.Random.Range(0, fireCrew.Count);
                Destroy(fireCrew[random]);
                fireCrew.RemoveAt(random);
                fireCrewInstances--;
            }

            string alert = "It looks like " + penalty.ToString() + " of our own are retiring, they've put in their time. Well deserved!";

            // Display alert message
            StartCoroutine(SendNotification(alert, 3));
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

            for(int i = 0; i < fireCrew.Count; i++)
            {
                if(allTiles[unluckyTile] == fireCrew[i].GetComponent<FireCrew>().currentTile)
                {
                    unluckyTile++;
                }
            }

            for(int i = 0; i < fireTruck.Count; i++)
            {
                if(allTiles[unluckyTile] == fireTruck[i].GetComponent<FireTruck>().currentTile)
                {
                    unluckyTile++;
                }
            }

            StartCoroutine(LightTile(allTiles[unluckyTile], unluckyTile));

            // Display alert message
            StartCoroutine(SendNotification("A new wildfire has appeared! It looks like some nearby fireworks may be the cause.", 3));
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
                    StartCoroutine(PutOutFire(litTileIndex[index]));
                    litTileIndex.RemoveAt(index);
                }
            }

            // Display alert message
            StartCoroutine(SendNotification("Sweet rain! It's putting out a few fires!", 3));
            Debug.Log("Heavy rain event triggered!");
        }
    }

    IEnumerator SendNotification(string text, int time)
    {
        notificationText.text = text;
        yield return new WaitForSeconds(time);
        notificationText.text = "";
    }

    string PickWindDirection()
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

    void CalcHappy()
    {
        // Start with a perfect rating
        double result = 100;

        // If any fires exist
        if(wildfireInstances > 0) 
        {
            result -= 10;
        }

        // Multiply remaining happiness with the percentage of normal tiles
        result *= (((double)allTiles.Length - (double)wildfireInstances) / (double)allTiles.Length);
        happiness = (int)result;
    }
}
