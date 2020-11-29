using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTruck : MonoBehaviour
{
    public int waterLevel;
    public float movementSpeed = 2.0f;
    private GameManager gameManager;

    private WaterBar waterBar;

    private int truckID;
    public int TruckID 
    {
        get
        {
            return truckID;
        }
        set 
        {
            truckID = value;
        }
    }

    public GameObject currentTile;
    public GameObject destinationTile;
    public GameObject DestinationMarker;
    public GameObject targetTile;
    public GameObject TargetMarker;
    public GameObject SelectionBox;

    private bool startDousing;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        waterBar = gameObject.GetComponentInChildren<WaterBar>();
        SelectionBox.SetActive(false);
        DestinationMarker.SetActive(false);
        TargetMarker.SetActive(false);
        destinationTile = null;
        startDousing = false;

        // mark the current tile as "occupied" so that other units & fires can't overlap
        currentTile.GetComponent<TileScript>().SetOccupied(true);
    }

    // Update is called once per frame
    void Update()
    {
        // If this is not the currently selected object, show the "unselected" sprite and remove the destination marker
        if (gameManager.SelectedUnit != gameObject)
        {
            SelectionBox.SetActive(false);
            DestinationMarker.SetActive(false);
            TargetMarker.SetActive(false);
        }

        // If a destination has been chosen for this unit, update the variable
        if (gameManager.SelectedUnit == gameObject && gameManager.SelectedTile != null && gameManager.DestSelectModeOn == true)
        {
            destinationTile = gameManager.SelectedTile;
            DestinationMarker.SetActive(true);
            DestinationMarker.transform.position = destinationTile.transform.position;
            gameManager.SelectedTile = null;
            gameManager.DestSelectModeOn = false;
        }

        // If a target has been chosen for the unit to spray water on, update the variable
        if (gameManager.SelectedUnit == gameObject && gameManager.SelectedTile != null && gameManager.TargetSelectModeOn == true)
        {
            // We can only mark a tile as a target if it is currently on fire and it is adjacent to this fire truck
            // We also cannot mark a tile as a target if the fire truck is still moving
            if ((gameManager.SelectedTile == currentTile.GetComponent<TileScript>().GetNorth() ||
                gameManager.SelectedTile == currentTile.GetComponent<TileScript>().GetEast() ||
                gameManager.SelectedTile == currentTile.GetComponent<TileScript>().GetSouth() ||
                gameManager.SelectedTile == currentTile.GetComponent<TileScript>().GetWest()) &&
                destinationTile == null && gameManager.SelectedTile.GetComponent<TileScript>().GetBurning() == true)
            {
                targetTile = gameManager.SelectedTile;
                TargetMarker.SetActive(true);
                TargetMarker.transform.position = targetTile.transform.position;
                startDousing = false;
            }

            // reset the selection parameters
            gameManager.SelectedTile = null;
            gameManager.TargetSelectModeOn = false;
        }

        // update water status bar
        waterBar.currentWater = waterLevel;

        // move the unit to the next tile if not at the destination
        if ((destinationTile != null) && (currentTile != null) && (destinationTile != currentTile))
        {
            StartCoroutine(gameObject.GetComponent<MoveToDest>().Move(currentTile, destinationTile, movementSpeed));

            // This keeps the destination marker from moving along with the other objects in the FireTruck prefab
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

        // spray water on the target tile until the fire is extinguished or we move away
        if ((targetTile != null) && (destinationTile == null))
        {
            if (startDousing == true)
            {
                StartCoroutine(gameObject.GetComponent<SprayWater>().Douse(targetTile));
                startDousing = false;
            }
        }
        else
        {
            StopCoroutine(gameObject.GetComponent<SprayWater>().Douse(targetTile));
            TargetMarker.SetActive(false);
            targetTile = null;
            startDousing = false;
        }
    }

    // Handle selection of this object
    public void Selected()
    {
        // If this is the currently selected game object, update the sprite
        SelectionBox.SetActive(true);
        gameManager.SelectedUnit = gameObject;

        if (destinationTile != null)
        {
            DestinationMarker.SetActive(true);
        }
    }

}
