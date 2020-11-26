using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter : MonoBehaviour
{
    public int waterLevel;
    public float movementSpeed = 2.0f;
    private GameManager gameManager;

    private WaterBar waterBar;
    private int totalWaterSprayed;
    private int tileIndex;

    private int helicopterID;
    public int HelicopterID 
    {
        get
        {
            return helicopterID;
        }
        set 
        {
            helicopterID = value;
        }
    }

    public GameObject currentTile;
    public GameObject destinationTile;
    public GameObject DestinationMarker;
    public GameObject targetTile;
    public GameObject TargetMarker;
    public GameObject SelectionBox;
    private GameObject liftoffTile;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        waterBar = gameObject.GetComponentInChildren<WaterBar>();
        SelectionBox.SetActive(false);
        DestinationMarker.SetActive(false);
        TargetMarker.SetActive(false);
        destinationTile = null;
        liftoffTile = null;

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

            // store the tile that we lifted off from in "liftoffTile" so that we can mark it unoccupied later
            liftoffTile = currentTile;
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
                totalWaterSprayed = 0;
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
            StartCoroutine(gameObject.GetComponent<FlyToDest>().Fly(currentTile, destinationTile, movementSpeed));

            // This keeps the destination marker from moving along with the other objects in the FireTruck prefab
            DestinationMarker.transform.position = destinationTile.transform.position;

            // if we just lifted off, mark the liftoffTile as "unoccupied"
            if (liftoffTile == currentTile)
            {
                liftoffTile.GetComponent<TileScript>().SetOccupied(false);
            }
        }
        if (destinationTile == currentTile)
        {
            StopCoroutine(gameObject.GetComponent<FlyToDest>().Fly(currentTile, destinationTile, movementSpeed));
            destinationTile = null;
            DestinationMarker.SetActive(false);
            currentTile.GetComponent<TileScript>().SetOccupied(true); // current tile is now occupied
            liftoffTile = null; // reset the liftoffTile for later use
        }

        // spray water on the target tile until the fire is extinguished
        if ((targetTile != null) && (destinationTile == null))
        {
            StartCoroutine("SprayWater");
        }
        else
        {
            StopCoroutine("SprayWater");
            TargetMarker.SetActive(false);
            totalWaterSprayed = 0;
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

    // Handle spraying water on the fire
    IEnumerator SprayWater()
    {
        if (targetTile.GetComponent<TileScript>().GetBurning() == true && waterLevel >= 1)
        {
            waterLevel -= 1;
            totalWaterSprayed += 1;
        }
        else
        {
            targetTile = null;
        }

        if (totalWaterSprayed >= 10)
        {
            // Grab tile index
            for(int i = 0; i < gameManager.AllTiles.Length; i++ ) 
            {
                if(GameObject.ReferenceEquals(targetTile, gameManager.AllTiles[i])) tileIndex = i;
            }
            Debug.Log("Spraying water on tile:" + (tileIndex + 1).ToString());
            StartCoroutine(gameManager.GetComponent<GameManager>().PutOutFire(tileIndex));
            targetTile = null;
        }

        yield return new WaitForSeconds(1.0f);
    }


}
