
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
    private TileManager tileManager;
    public int[] entireGrid;
    public int waterColumn;
    public List<int> litTiles = new List<int>();
    public List<int> loadLitTiles = new List<int>();
    public List<string> crewTileLocations = new List<string>();
    public List<string> truckTileLocations = new List<string>();
    public List<string> helicopterTileLocations = new List<string>();
    public int baseSpawnLocation;
    public string windDirection;
    public int money;
    public int happiness;
    private int columnCount;
    private int rowCount;
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
    public int spreadFactor;
    public int spreadFrequency;
    public int crewCost;
    public int truckCost;
    public int helicopterCost;
    public int startingFires;
    public int startingUnits;
    public int reward;
    private int fireCrewInstances;
    private int fireTruckInstances;
    private int helicopterInstances;
    private int wildfireInstances;

    public GameObject fireCrewPrefab;
    public GameObject fireTruckPrefab;
    public GameObject helicopterPrefab;
    
    private GameObject selectedUnit;
    private GameObject selectedTile;

    public bool DestSelectModeOn;
    public bool TargetSelectModeOn;

    enum easy
    {
        startingFires = 5,
        startingUnits = 2,
        startingMoney = 100,
        spreadFactor = 3,
        spreadFrequency = 40,
        crewCost = 100,
        truckCost = 1000,
        helicopterCost = 2000,
        reward = 100
    }
    enum medium
    {
        startingFires = 10,
        startingUnits = 3,
        startingMoney = 100,
        spreadFactor = 5,
        spreadFrequency = 35,
        crewCost = 200,
        truckCost = 2000,
        helicopterCost = 4000,
        reward = 75
    }
    enum hard
    {
        startingFires = 20,
        startingUnits = 5,
        startingMoney = 100,
        spreadFactor = 8,
        spreadFrequency = 30,
        crewCost = 300,
        truckCost = 3000,
        helicopterCost = 6000,
        reward = 50
    }

    [Header("HUD")]
    public Text moneyText;
    public Text selectedText;

    public Text happinessText;
    public GameObject notificationBox;
    public Text notificationText;
    public Text windDirectionText;

    public Button crewBtn;
    public Button dispatchBtn;
    public Button purchaseCrewBtn;
    public Button purchaseTruckBtn;
    public Button purchaseHelicopterBtn;
    public Button clearVegBtn;
    public Button fireLineBtn;
    public GameObject pauseMenu;
    public Button pauseBtn;
    public Button closePauseBtn;
    public Text pauseText;
    public Text saveText;
    public Text loadText;
    public Text quitText;
    public Button saveBtn;
    public Button loadBtn;
    public Button quitBtn;

    public bool SprayWaterMode;
    public bool ClearVegMode;
    public bool FireLineMode;

    // Start is called before the first frame update
    void Start()
    {
        allTiles = GameObject.FindGameObjectsWithTag("Tile");
        tileManager = GameObject.Find("TileManager").GetComponent<TileManager>();

        // Set difficulty settings
        if (tileManager.easyDifficulty)
        {
            startingFires = (int)easy.startingFires;
            startingUnits = (int)easy.startingUnits;
            money = (int)easy.startingMoney;
            spreadFactor = (int)easy.spreadFactor;
            spreadFrequency = (int)easy.spreadFrequency;
            crewCost = (int)easy.crewCost;
            truckCost = (int)easy.truckCost;
            helicopterCost = (int)easy.helicopterCost;
            reward = (int)easy.reward;
        }
        else if (tileManager.mediumDifficulty)
        {
            startingFires = (int)medium.startingFires;
            startingUnits = (int)medium.startingUnits;
            money = (int)medium.startingMoney;
            spreadFactor = (int)medium.spreadFactor;
            spreadFrequency = (int)medium.spreadFrequency;
            crewCost = (int)medium.crewCost;
            truckCost = (int)medium.truckCost;
            helicopterCost = (int)medium.helicopterCost;
            reward = (int)medium.reward;
        }
        else
        {
            startingFires = (int)hard.startingFires;
            startingUnits = (int)hard.startingUnits;
            money = (int)hard.startingMoney;
            spreadFactor = (int)hard.spreadFactor;
            spreadFrequency = (int)hard.spreadFrequency;
            crewCost = (int)hard.crewCost;
            truckCost = (int)hard.truckCost;
            helicopterCost = (int)hard.helicopterCost;
            reward = (int)hard.reward;
        }
        
        baseSpawnLocation = PlaceFirehouse();
        columnCount = GameObject.Find("TileManager").GetComponent<TileManager>().GetColumnCount();
        rowCount = GameObject.Find("TileManager").GetComponent<TileManager>().GetRowCount();
        Debug.Log("columnCount is: " + columnCount.ToString() + " rowCount is: " + rowCount.ToString());

        // Set on click listeners
        crewBtn.onClick.AddListener(() => CrewClicked());
        dispatchBtn.onClick.AddListener(() => DispatchClicked());
        purchaseCrewBtn.onClick.AddListener(() => PurchaseCrewClicked());
        purchaseTruckBtn.onClick.AddListener(() => PurchaseTruckClicked());
        purchaseHelicopterBtn.onClick.AddListener(() => PurchaseHelicopterClicked());
        clearVegBtn.onClick.AddListener(() => ClearVegClicked());
        fireLineBtn.onClick.AddListener(() => FireLineClicked());
        pauseBtn.onClick.AddListener(() => PauseClicked());
        closePauseBtn.onClick.AddListener(() => ClosePauseClicked());
        saveBtn.onClick.AddListener(() => SaveGame());
        loadBtn.onClick.AddListener(() => LoadGame());
        quitBtn.onClick.AddListener(Application.Quit);

        // initialize button modes
        SprayWaterMode = false;
        ClearVegMode = false;
        FireLineMode = false;

        // instantiate the first set of fire crews at the start of the game
        fireCrewInstances = 0;
        fireTruckInstances = 0;
        helicopterInstances = 0;
        for(int i = 0; i < startingUnits; i++)
        {
            int spawnHere = generateSpawnLocation();
            AddFireCrew(AllTiles[spawnHere]);
            allTiles[spawnHere].GetComponent<TileScript>().SetOccupied(true);
        }

        // Instantiate wildfire
        wildfireInstances = 0;
        Debug.Log("here: " + System.Int32.Parse(Regex.Replace(moneyText.text, "[^.0-9]", "")));

        //Randomly start fires in the map
        for(int i = 0; i < startingFires; i++)
        {
            int fireLocation = (int)UnityEngine.Random.Range(0, allTiles.Length - 1);
            while(allTiles[fireLocation].GetComponent<TileScript>().GetOccupied())
            {
                fireLocation++;
                if(fireLocation >= allTiles.Length)
                {
                    fireLocation = 0;
                }
            }
            StartCoroutine(LightTile(allTiles[fireLocation], fireLocation));
        }
        
        StartCoroutine(SendNotification("Oh no, there are " + wildfireInstances.ToString() + " wildfires! Put them out!", 3));

        // Start game behavior
        InvokeRepeating("WildFireBehavior", 10, spreadFrequency);
        InvokeRepeating("PickEvent", 60, 120);
        InvokeRepeating("CalcHappy", 0, 5);
        InvokeRepeating("PickWindDirection", 0, 120);

        DestSelectModeOn = false;
        TargetSelectModeOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        // if (happiness != System.Convert.ToInt32(happinessText.text)) { }
        //     happinessText.text = happiness.ToString();

        if (wildfireInstances < 0)
        {
            wildfireInstances = 0;
        }

        if (fireCrewInstances < 0)
        {
            fireCrewInstances = 0;
        }

        if (fireTruckInstances < 0)
        {
            fireTruckInstances = 0;
        }

        if (helicopterInstances < 0)
        {
            helicopterInstances = 0;
        }

        if (money < 0)
        {
            money = 0;
        }

        if (happiness < 0)
        {
            happiness = 0;
        }

        if (money !=  System.Int32.Parse(Regex.Replace(moneyText.text, "[^.0-9]", "")))
        {
            moneyText.text = "$" + money.ToString();
        }   

        moneyText.text = "$" + money.ToString();
        happinessText.text = happiness.ToString() + "/100";
        windDirectionText.text = "The wind blows: \n" + windDirection;

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

    void PauseClicked()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    void ClosePauseClicked()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    // Spawn new fireCrew instances from the FireCrew prefab
    void AddFireCrew(GameObject baseSpawnLocation)
    {
        GameObject newFireCrew = (GameObject)Instantiate(fireCrewPrefab);
        newFireCrew.transform.position = baseSpawnLocation.transform.position;
        newFireCrew.GetComponent<FireCrew>().CrewID = fireCrewInstances + 1;
        newFireCrew.GetComponent<FireCrew>().waterLevel = 100;
        newFireCrew.GetComponent<FireCrew>().energyLevel = 100;
        newFireCrew.GetComponent<FireCrew>().currentTile = baseSpawnLocation;
        fireCrew.Add(newFireCrew);
        fireCrewInstances ++;
    }

    // Spawn new Fire Truck instances from the FireTruck prefab
    void AddFireTruck(GameObject baseSpawnLocation)
    {
        GameObject newFireTruck = (GameObject)Instantiate(fireTruckPrefab);
        newFireTruck.transform.position = baseSpawnLocation.transform.position;
        newFireTruck.GetComponent<FireTruck>().TruckID = fireTruckInstances + 1;
        newFireTruck.GetComponent<FireTruck>().waterLevel = 100;
        newFireTruck.GetComponent<FireTruck>().currentTile = baseSpawnLocation;
        fireTruck.Add(newFireTruck);
        fireTruckInstances ++;
    }

    void AddHelicopter(GameObject baseSpawnLocation)
    {
        GameObject newHelicopter = (GameObject)Instantiate(helicopterPrefab);
        newHelicopter.transform.position = baseSpawnLocation.transform.position;
        newHelicopter.GetComponent<Helicopter>().HelicopterID = helicopterInstances + 1;
        newHelicopter.GetComponent<Helicopter>().waterLevel = 100;
        newHelicopter.GetComponent<Helicopter>().currentTile = baseSpawnLocation;
        helicopter.Add(newHelicopter);
        helicopterInstances ++;
    }

    void CrewClicked()
    {
        Debug.Log("Crew button has been clicked.");
        
        // Toggle between target select mode OFF and ON
        // Must have selected a fire crew, fire truck, or helicopter before trying to spray water
        if ((!TargetSelectModeOn) && (SelectedUnit != null))
        {
            TargetSelectModeOn = true;
            SprayWaterMode = true;
            DestSelectModeOn = false;  // don't want to have two selection modes active at the same time
            selectedText.text = "Extinguish";
            StartCoroutine(SendNotification("Select a fire to extinguish", 2));
        }
        else
        {
            TargetSelectModeOn = false;
            SprayWaterMode = false;
            selectedText.text = "";
            StartCoroutine(SendNotification("ERROR: Select a unit first", 2));
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
            StartCoroutine(SendNotification("Select a tile to move to", 2));
        }
        else
        {
            DestSelectModeOn = false;
            selectedText.text = "";
            StartCoroutine(SendNotification("ERROR: Select a unit first", 2));
        }
    }

    void InfoClicked()
    {
        Debug.Log("Info button has been clicked.");
        selectedText.text = "Info";
    }

    // Generates spawn location closest to fire house
    int generateSpawnLocation()
    {
        int spawnLocation = baseSpawnLocation;
        Debug.Log("Base spawn location is: " + baseSpawnLocation.ToString());

        while(allTiles[spawnLocation].GetComponent<TileScript>().GetOccupied())
        {
            // Move spawn location to random direction
            int dice = UnityEngine.Random.Range(0, 4);
            switch(dice)
            {
                // Shift spawn location up
                case 0: 
                // Is spawn location on top edge of map?
                if((spawnLocation >= 0) && (spawnLocation < columnCount))
                {
                    // Flip a coin
                    if((int)UnityEngine.Random.Range(0, 2) == 0)
                    {
                        // Top left corner
                        if(spawnLocation == 0)
                        {
                            // Flip a coin to either move right or down
                            int addThis = ((int)UnityEngine.Random.Range(0, 2) == 0) ? 1 : columnCount;
                            spawnLocation += addThis;
                        }
                        else
                        {
                            spawnLocation --; // Move left
                        }
                    }
                    else
                    {
                        // Top right corner
                        if(spawnLocation == columnCount - 1)
                        {
                            // Flip a coin to either move left or down
                            int addThis = ((int)UnityEngine.Random.Range(0, 2) == 0) ? -1 : columnCount;
                            spawnLocation += addThis;
                        }
                        else
                        {
                            spawnLocation ++; // Move right
                        }
                    }
                }
                // Not on top edge of map
                else
                {
                    spawnLocation -= columnCount; // Move up
                }
                break;

                // Shift spawn location down
                case 1: 
                // Is spawn location on bottom edge of map?
                if((spawnLocation > (allTiles.Length - columnCount)) && (spawnLocation < allTiles.Length))
                {
                    // Flip a coin
                    if((int)UnityEngine.Random.Range(0, 2) == 0)
                    {
                        // Bottom left corner
                        if(spawnLocation == (allTiles.Length - columnCount))
                        {
                            // Flip a coin to either move right or up
                            int addThis = ((int)UnityEngine.Random.Range(0, 2) == 0) ? 1 : - columnCount;
                            spawnLocation += addThis;
                        }
                        else
                        {
                            spawnLocation --; // Move left
                        }
                    }
                    else
                    {
                        // Bottom right corner
                        if(spawnLocation == allTiles.Length - 1)
                        {
                            // Flip a coin to either move left or up
                            int addThis = ((int)UnityEngine.Random.Range(0, 2) == 0) ? -1 : - columnCount;
                            spawnLocation += addThis;
                        }
                        else
                        {
                            spawnLocation ++; // Move right
                        }
                    }
                }
                // Not on top edge of map
                else
                {
                    spawnLocation += columnCount; // Move down
                }
                break;

                // Shift spawn location right
                case 2:
                // Is spawn location on right edge of map?
                if(spawnLocation % columnCount == columnCount - 1)
                {
                    // Flip a coin
                    if((int)UnityEngine.Random.Range(0, 2) == 0)
                    {
                        // Top right corner
                        if(spawnLocation == columnCount - 1)
                        {
                            // Flip a coin to either move left or down
                            int addThis = ((int)UnityEngine.Random.Range(0, 2) == 0) ? -1 : columnCount;
                            spawnLocation += addThis;
                        }
                        else
                        {
                            spawnLocation -= columnCount; // Move up
                        }
                    }
                    else
                    {
                        // Bottom right corner
                        if(spawnLocation == allTiles.Length - 1)
                        {
                            // Flip a coin to either move left or up
                            int addThis = ((int)UnityEngine.Random.Range(0, 2) == 0) ? -1 : -columnCount;
                            spawnLocation += addThis;
                        }
                        else
                        {
                            spawnLocation += columnCount; // Move down
                        }
                    }
                }
                else
                {
                    spawnLocation ++; // Move right
                }
                break;

                // Shift spawn location left
                case 3:
                // Is spawn location on left edge of map?
                if(spawnLocation % columnCount == 0)
                {
                    // Flip a coin
                    if((int)UnityEngine.Random.Range(0, 2) == 0)
                    {
                        // Top left corner
                        if(spawnLocation == 0)
                        {
                            // Flip a coin to either move right or down
                            int addThis = ((int)UnityEngine.Random.Range(0, 2) == 0) ? 1 : columnCount;
                            spawnLocation += addThis;
                        }
                        else
                        {
                            spawnLocation -= columnCount; // Move up
                        }
                    }
                    else
                    {
                        // Bottom left corner
                        if(spawnLocation == (allTiles.Length - columnCount))
                        {
                            // Flip a coin to either move right or up
                            int addThis = ((int)UnityEngine.Random.Range(0, 2) == 0) ? 1 : -columnCount;
                            spawnLocation += addThis;
                        }
                        else
                        {
                            spawnLocation += columnCount; // Move down
                        }
                    }
                }
                else
                {
                    spawnLocation --; // Move left
                }
                break;
            }

            // Edge cases
            if(spawnLocation >= allTiles.Length)
            {
                spawnLocation = allTiles.Length - 1;
            }
            else if(spawnLocation < 0)
            {
                spawnLocation = 0;
            }
        }

        Debug.Log("Spawn location is: " + spawnLocation.ToString());
        return spawnLocation;
    }

    void PurchaseCrewClicked()
    {
        Debug.Log("Purchase Crew button has been clicked.");

        if(money >= crewCost)
        {
            AddFireCrew(AllTiles[generateSpawnLocation()]);
            money -= crewCost;
            StartCoroutine(SendNotification("You have just added a new crew member!", 2));
        }
        else
        {
            StartCoroutine(SendNotification("You lack the funds to add a new member!", 2));
        }
    }

    void PurchaseTruckClicked()
    {
        Debug.Log("Purchase Truck button has been clicked.");

        if(money >= truckCost)
        {
            money -= truckCost;
            AddFireTruck(AllTiles[generateSpawnLocation()]);
            StartCoroutine(SendNotification("You have just bought a new fire truck!", 2));
        }
        else
        {
            StartCoroutine(SendNotification("You lack the funds to buy a new fire truck!", 2));
        }
    }

    void PurchaseHelicopterClicked()
    {
        Debug.Log("Purchase Helicopter button has been clicked.");

        if(money >= helicopterCost)
        {
            money -= helicopterCost;
            AddHelicopter(AllTiles[generateSpawnLocation()]);
            StartCoroutine(SendNotification("You have just bought a new helicopter!", 2));
        }
        else
        {
            StartCoroutine(SendNotification("You lack the funds to buy a new helicopter!", 2));
        }
    }

    void ClearVegClicked()
    {
        Debug.Log("Clear Vegetation has been clicked.");

        // Toggle between target select mode OFF and ON
        // Must have selected a fire crew before trying to clear vegetation
        if ((!TargetSelectModeOn) && (SelectedUnit != null) && (SelectedUnit.CompareTag("FireCrew")))
        {
            TargetSelectModeOn = true;
            ClearVegMode = true;
            DestSelectModeOn = false;  // don't want to have two selection modes active at the same time
            selectedText.text = "Clear Vegetation";
            StartCoroutine(SendNotification("Select a tile to clear vegetation", 2));
        }
        else
        {
            TargetSelectModeOn = false;
            ClearVegMode = false;
            selectedText.text = "";
            StartCoroutine(SendNotification("ERROR: Select a unit first", 2));
        }
    }

    void FireLineClicked()
    {
        Debug.Log("Build Fire Line has been clicked.");

        // Toggle between target select mode OFF and ON
        // Must have selected a fire crew before trying to build a fire line
        if ((!TargetSelectModeOn) && (SelectedUnit != null) && (SelectedUnit.CompareTag("FireCrew")))
        {
            TargetSelectModeOn = true;
            FireLineMode = true;
            DestSelectModeOn = false;  // don't want to have two selection modes active at the same time
            selectedText.text = "Build Fire Line";
            StartCoroutine(SendNotification("Select a tile to dig a fire line", 2));
        }
        else
        {
            TargetSelectModeOn = false;
            FireLineMode = false;
            selectedText.text = "";
            StartCoroutine(SendNotification("ERROR: Select a unit first", 2));
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
    
    IEnumerator LightTile(GameObject baseSpawnLocation, int tileIndex)
    {
        baseSpawnLocation.GetComponent<TileScript>().SetBurning(true);
        Debug.Log("Tile " + tileIndex.ToString() + " is on fire!");
        litTiles.Add(tileIndex);
        wildfireInstances++;
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
            if(!inspectTile.GetComponent<TileScript>().GetBurning()) 
            {
                Debug.Log("And it's not on fire");
                int chanceToBurn = inspectTile.GetComponent<TileScript>().GetDryness();
                Debug.Log("Tile dryness is " + inspectTile.GetComponent<TileScript>().GetDryness());

                // Check wind direction
                if (southTile) if((windDirection == "North") && (southTile.GetComponent<TileScript>().GetBurning())) chanceToBurn *= 2;
                if (northTile) if((windDirection == "South") && (northTile.GetComponent<TileScript>().GetBurning())) chanceToBurn *= 2;
                if (westTile) if((windDirection == "East") && (westTile.GetComponent<TileScript>().GetBurning())) chanceToBurn *= 2;
                if (eastTile) if((windDirection == "West") && (eastTile.GetComponent<TileScript>().GetBurning())) chanceToBurn *= 2;

                int multiplier = 0;

                // Check which surrounding tiles are on fire
                if (northTile) if(northTile.GetComponent<TileScript>().GetBurning()) multiplier++;
                if (southTile) if(southTile.GetComponent<TileScript>().GetBurning()) multiplier++;
                if (eastTile) if(eastTile.GetComponent<TileScript>().GetBurning()) multiplier++;
                if (westTile) if(westTile.GetComponent<TileScript>().GetBurning()) multiplier++;
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
            if (northTile)
            {
                if (northTile.GetComponent<TileScript>().GetOccupied())
                {
                    goto case 2;
                }

                if (!hasLitOne) 
                    StartCoroutine(_SpreadFire(litTile, northTile, windDirection, index - 18, "North",  (i) =>
                    {
                        hasLitOne = i;
                    }));
            }

            Debug.Log("hasLitOne is " + hasLitOne.ToString());
            break;

            case 2:
            //Check if tile is occupied
            if (southTile)
            {
                if (southTile.GetComponent<TileScript>().GetOccupied())
                {
                    goto case 3;
                }

                if (!hasLitOne)
                    StartCoroutine(_SpreadFire(litTile, southTile, windDirection, index + columnCount, "South",  (i) =>
                    {
                        hasLitOne = i;
                    }));
            }

            Debug.Log("hasLitOne is " + hasLitOne.ToString());
            break;

            case 3:
            //Check if tile is occupied
            if (eastTile)
            {
                if (eastTile.GetComponent<TileScript>().GetOccupied())
                {
                    goto case 4;
                }

                if (!hasLitOne) 
                StartCoroutine(_SpreadFire(litTile, eastTile, windDirection, index + 1, "East",  (i) =>
                    {
                        hasLitOne = i;
                    }));
            }

            Debug.Log("hasLitOne is " + hasLitOne.ToString());
            break;

            case 4:
            //Check if tile is occupied
            if (westTile)
            {
                if (westTile.GetComponent<TileScript>().GetOccupied())
                {
                    goto NoFires;
                }

                if (!hasLitOne) 
                StartCoroutine(_SpreadFire(litTile, westTile, windDirection, index - 1, "West",  (i) =>
                    {
                        hasLitOne = i;
                    }));
            }

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

        //Spread fire as much as spreadFactor allows
        for(int i = 0; i < spreadFactor; i++) {
            Debug.Log("Executing SpreadFire " + (i + 1).ToString() + " time");
            int randomIndex = (int)UnityEngine.Random.Range(0, litTiles.Count);
            StartCoroutine(SpreadFire(allTiles[litTiles[randomIndex]], windDirection, litTiles[randomIndex]));
        }
    }

    public IEnumerator PutOutFire(int tileNumber) 
    {
        if(allTiles[tileNumber].GetComponent<TileScript>().GetBurning()) 
        {
            allTiles[tileNumber].GetComponent<TileScript>().SetBurning(false);
            wildfireInstances--;
            litTiles.Remove(tileNumber);
            money += reward;
            StartCoroutine(SendNotification("Fire has been put out! HUZZAH!", 2));
            Debug.Log("Put out fire at tile: " + tileNumber.ToString());
        }
        
        yield return null;
    }

    void PickEvent() 
    {
        int dice = UnityEngine.Random.Range(0, 120);
        // dice = 120;
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
            int unluckyTile = UnityEngine.Random.Range(1, columnCount * rowCount);
            
            // Make sure tile isn't already on fire or occupied
            while((allTiles[unluckyTile].GetComponent<TileScript>().GetBurning()) 
            || (allTiles[unluckyTile].GetComponent<TileScript>().GetOccupied()))
            {
                unluckyTile++;
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
                AddFireCrew(AllTiles[generateSpawnLocation()]);
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
            AddHelicopter(AllTiles[generateSpawnLocation()]);

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
            int unluckyTile = UnityEngine.Random.Range(1, columnCount * rowCount);
            
            // Make sure tile isn't already on fire or occupied
            while((allTiles[unluckyTile].GetComponent<TileScript>().GetBurning()) 
            || (allTiles[unluckyTile].GetComponent<TileScript>().GetOccupied()))
            {
                unluckyTile++;
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
                if(allTiles[i].GetComponent<TileScript>().GetBurning())
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

        //Destroy fire lines
        if((dice > 110) && (dice <= 120)) 
        {
            bool displayMessage = false;
            
            for(int i = 0; i < allTiles.Length; i++)
            {
                if(allTiles[i].GetComponent<TileScript>().GetFireLineBoolean())
                {
                    allTiles[i].GetComponent<TileScript>().DestroyFireLine();
                    displayMessage = true;
                }
            }

            // Display alert message
            if (displayMessage)
            {
                StartCoroutine(SendNotification("Oh no! A tiny earthquake filled your firelines with dirt!", 3));
            }
            
            Debug.Log("Destroy fire lines triggered!");
        }
    }

    public IEnumerator SendNotification(string text, int time)
    {
        notificationBox.SetActive(true);
        notificationText.text = text;
        yield return new WaitForSeconds(time);
        notificationText.text = "";
        notificationBox.SetActive(false);
    }

    void PickWindDirection()
    {
        switch ((int)UnityEngine.Random.Range(1, 4)) {
            case 1:
                windDirection = "North";
                break;

            case 2:
                windDirection = "South";
                break;

            case 3:
                windDirection = "East";
                break;

            case 4:
                windDirection = "West";
                break;
        }
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

    public void SetNotificationText(string msg)
    {
        notificationText.text = msg;
    }
    
    void SaveGame()
    {
        // Save game data
        for(int i = 0; i < fireCrew.Count; i++)
        {
            crewTileLocations.Add(fireCrew[i].GetComponent<FireCrew>().currentTile.name);
            Debug.Log("Added " + crewTileLocations[i] + " to crewTileLocations");
        }

        for(int i = 0; i < fireTruck.Count; i++)
        {
            truckTileLocations.Add(fireTruck[i].GetComponent<FireTruck>().currentTile.name);
            Debug.Log("Added " + truckTileLocations[i] + " to truckTileLocations");
        }

        for(int i = 0; i < helicopter.Count; i++)
        {
            helicopterTileLocations.Add(helicopter[i].GetComponent<Helicopter>().currentTile.name);
            Debug.Log("Added " + helicopterTileLocations[i] + " to helicopterTileLocations");
        }

        // entireGrid = tileManager.GetEntireGrid();
        // waterColumn = tileManager.GetWaterColumn();
        SaveLoadSystem.SaveGame(this);

        // UI
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        StartCoroutine(SendNotification("Game has been saved!", 2));
    }

    void LoadGame()
    {
        GameData data = SaveLoadSystem.LoadGame();

        if(data != null)
        {
            // Put out all fires
            for(int i = 0; i < allTiles.Length; i++)
            {
                StartCoroutine(PutOutFire(i));
            }

            // Destroy fireCrew, trucks, and helicopters
            for(int i = fireCrew.Count - 1; i >= 0; i--)
            {
                Destroy(fireCrew[i]);
                fireCrew.RemoveAt(i);
            }

            for(int i = fireTruck.Count - 1; i >= 0; i--)
            {
                Destroy(fireTruck[i]);
                fireTruck.RemoveAt(i);
            }

            for(int i = helicopter.Count - 1; i >= 0; i--)
            {
                Destroy(helicopter[i]);
                helicopter.RemoveAt(i);
            }

            fireCrewInstances = 0;
            fireTruckInstances = 0;
            helicopterInstances = 0;

            if((fireCrew.Count != 0) || (fireTruck.Count != 0) || (helicopter.Count != 0))
            {
                Debug.LogError("Objects were not wiped out");
            }

            money = data.money;
            happiness = data.happiness;
            windDirection = data.windDirection;
            windDirectionText.text = "The wind blows: \n" + windDirection;
            loadLitTiles = data.litTiles;
            crewTileLocations = data.crewTileLocations;
            truckTileLocations = data.truckTileLocations;
            helicopterTileLocations = data.helicopterTileLocations;
            // entireGrid = data.entireGrid;
            // waterColumn = data.waterColumn;

            // Regenerate tiles
            // tileManager.UpdateTileGrid(entireGrid, waterColumn);

            for(int i = 0; i < loadLitTiles.Count; i++)
            {
                StartCoroutine(LightTile(allTiles[loadLitTiles[i]], loadLitTiles[i]));
            }

            for(int i = 0; i < crewTileLocations.Count; i++)
            {
                AddFireCrew(GameObject.Find(crewTileLocations[i]));
            }

            for(int i = 0; i < truckTileLocations.Count; i++)
            {
                AddFireTruck(GameObject.Find(truckTileLocations[i]));
            }

            for(int i = 0; i < helicopterTileLocations.Count; i++)
            {
                AddHelicopter(GameObject.Find(helicopterTileLocations[i]));
            }

            // UI
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
            StartCoroutine(SendNotification("Game has been loaded!", 2));
        }
        else
        {
            // UI
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
            StartCoroutine(SendNotification("ERROR: Unable to locate save file", 2));
        }
    } 

    // positions firehouse next to road
    int PlaceFirehouse()
    {
        int count = 0;
        GameObject fireHouse = GameObject.Find("Firehouse");
        int rnd = UnityEngine.Random.Range(0, allTiles.Length);
        bool nearRoad = false;
        bool goodTerrain = false;
        while(!goodTerrain || !nearRoad)
        {
            rnd = UnityEngine.Random.Range(1, allTiles.Length - 1);
            nearRoad = allTiles[rnd - 1].GetComponent<TileScript>().GetTerrain() == "Road";
            nearRoad = nearRoad || allTiles[rnd + 1].GetComponent<TileScript>().GetTerrain() == "Road";
            goodTerrain = allTiles[rnd].GetComponent<TileScript>().GetTerrain() == "Sand";
            goodTerrain = goodTerrain || allTiles[rnd].GetComponent<TileScript>().GetTerrain() == "forest";
            count++;
            if (count > 150) break;
        }
        fireHouse.transform.position = allTiles[rnd].transform.position;
        allTiles[rnd].GetComponent<TileScript>().SetFirehouseNeighbors();
        allTiles[rnd].GetComponent<TileScript>().DestroyObstacle();
        allTiles[rnd].GetComponent<TileScript>().DestroyObstacle();
        allTiles[rnd].GetComponent<TileScript>().DestroyObstacle();
        allTiles[rnd].GetComponent<TileScript>().DestroyObstacle();
        allTiles[rnd].GetComponent<TileScript>().SetOccupied(true);
        return rnd;
    }
}
