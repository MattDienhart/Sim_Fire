using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCrew : MonoBehaviour
{
    public int waterLevel;
    public int energyLevel;
    public float movementSpeed = 1.0f;
    private GameManager gameManager;

    public Sprite unselected;
    public Sprite selected;
    private SpriteRenderer crewSpriteRenderer;

    private WaterBar waterBar;
    private EnergyBar energyBar;

    private int crewID;
    public int CrewID
    {
        get
        {
            return crewID;
        }
        set
        {
            crewID = value;
        }
    }

    public GameObject currentTile;
    public GameObject destinationTile;
    public GameObject DestinationMarker;
    public GameObject targetTile;
    public GameObject TargetMarker;

    private bool startMoving;
    private bool startDousing;
    private bool startClearing;
    private bool startBuilding;

    // Start is called before the first frame update
    void Start()
    {
        crewSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        waterBar = gameObject.GetComponentInChildren<WaterBar>();
        energyBar = gameObject.GetComponentInChildren<EnergyBar>();
        DestinationMarker.SetActive(false);
        TargetMarker.SetActive(false);
        destinationTile = null;
        startMoving = false;
        startDousing = false;
        startClearing = false;
        startBuilding = false;

        // mark the current tile as "occupied" so that other units & fires can't overlap
        currentTile.GetComponent<TileScript>().SetOccupied(true);
    }

    // Update is called once per frame
    void Update()
    {
        // If we are near a firehouse, refill water and energy levels
        if (currentTile.GetComponent<TileScript>().nearFireHouse == true)
        {
            waterLevel = 100;
            energyLevel = 100;
        }

        // If this is not the currently selected object, show the "unselected" sprite and remove the destination marker
        if (gameManager.SelectedUnit != gameObject)
        {
            crewSpriteRenderer.sprite = unselected;
            DestinationMarker.SetActive(false);
            TargetMarker.SetActive(false);
        }

        // If a destination has been chosen for this crew, update the variable
        if (gameManager.SelectedUnit == gameObject && gameManager.SelectedTile != null && gameManager.DestSelectModeOn == true)
        {
            destinationTile = gameManager.SelectedTile;
            DestinationMarker.SetActive(true);
            gameManager.SelectedTile = null;
            gameManager.DestSelectModeOn = false;
            startMoving = true;
        }

        // If a target has been chosen for the crew to spray water on, update the variable
        if (gameManager.SelectedUnit == gameObject && gameManager.SelectedTile != null && gameManager.TargetSelectModeOn == true)
        {
            // We can only mark a tile as a target if it is currently on fire and it is adjacent to this fire crew
            // We also cannot mark a tile as a target if the fire crew is still moving
            if ((gameManager.SelectedTile == currentTile.GetComponent<TileScript>().GetNorth() ||
                gameManager.SelectedTile == currentTile.GetComponent<TileScript>().GetEast() ||
                gameManager.SelectedTile == currentTile.GetComponent<TileScript>().GetSouth() ||
                gameManager.SelectedTile == currentTile.GetComponent<TileScript>().GetWest()) &&
                destinationTile == null)
            {
                if ((gameManager.SelectedTile.GetComponent<TileScript>().GetBurning() == true) && (gameManager.SprayWaterMode == true))
                {
                    targetTile = gameManager.SelectedTile;
                    TargetMarker.SetActive(true);
                    TargetMarker.transform.position = targetTile.transform.position;
                    startDousing = true;
                }
                else if ((gameManager.SelectedTile.GetComponent<TileScript>().GetBurning() == false) && (gameManager.ClearVegMode == true) &&
                    (gameManager.SelectedTile.GetComponent<TileScript>().GetOccupied() == false))
                {
                    targetTile = gameManager.SelectedTile;
                    TargetMarker.SetActive(true);
                    TargetMarker.transform.position = targetTile.transform.position;
                    startClearing = true;
                }        
                else if ((gameManager.SelectedTile.GetComponent<TileScript>().GetBurning() == false) && (gameManager.FireLineMode == true) && 
                    (gameManager.SelectedTile.GetComponent<TileScript>().GetOccupied() == false)) 
                {
                    targetTile = gameManager.SelectedTile;
                    TargetMarker.SetActive(true);
                    TargetMarker.transform.position = targetTile.transform.position;
                    startBuilding = true;
                }
            }

            // reset the selection parameters
            gameManager.SelectedTile = null;
            gameManager.TargetSelectModeOn = false;
            gameManager.SprayWaterMode = false;
            gameManager.ClearVegMode = false;
            gameManager.FireLineMode = false;
        }

        // update status bars for energy and water
        waterBar.currentWater = waterLevel;
        energyBar.currentEnergy = energyLevel;

        // move the crew to the next tile if not at the destination
        if ((destinationTile != null) && (currentTile != null) && (destinationTile != currentTile))
        {
            if (startMoving == true)
            {
                StartCoroutine(gameObject.GetComponent<MoveToDest>().Move(currentTile, destinationTile, movementSpeed));
                startMoving = false;
            }

            // This keeps the destination marker from moving along with the other objects in the FireCrew prefab
            DestinationMarker.transform.position = destinationTile.transform.position;

            // If the unit was in the middle of a behavior, stop when movement begins
            targetTile = null;
        }
        if (destinationTile == currentTile)
        {
            StopCoroutine(gameObject.GetComponent<MoveToDest>().Move(currentTile, destinationTile, movementSpeed));
            destinationTile = null;
            DestinationMarker.SetActive(false);
        }

        // perform an action on the target tile
        if ((targetTile != null) && (destinationTile == null))
        {
            if (startDousing == true)
            {
                // spray water on the target tile
                StartCoroutine(gameObject.GetComponent<SprayWater>().Douse(targetTile));
                startDousing = false;
            }
            else if (startClearing == true)
            {
                // clear vegetation away from the target tile
                StartCoroutine(gameObject.GetComponent<ClearVegetation>().Clear(targetTile));
                startClearing = false;
            }
            else if (startBuilding == true)
            {
                // build a fire line on the target tile
                StartCoroutine(gameObject.GetComponent<BuildFireLine>().Build(targetTile));
                startBuilding = false;
            }
        }
        else
        {
            StopCoroutine(gameObject.GetComponent<SprayWater>().Douse(targetTile));
            StopCoroutine(gameObject.GetComponent<ClearVegetation>().Clear(targetTile));
            StopCoroutine(gameObject.GetComponent<BuildFireLine>().Build(targetTile));
            targetTile = null;
            TargetMarker.SetActive(false);
            startDousing = false;
            startClearing = false;
            startBuilding = false;
        }
    }

    // Handle selection of this object
    public void Selected()
    {
        // If this is the currently selected game object, update the sprite
        crewSpriteRenderer.sprite = selected;
        gameManager.SelectedUnit = gameObject;

        if (destinationTile != null)
        {
            DestinationMarker.SetActive(true);
        }
    }
}
