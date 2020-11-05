using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCrew : MonoBehaviour
{
    public int waterLevel;
    public int energyLevel;
    public float movementSpeed = 1.0f;
    private float lastWaypointTime;
    private GameManager gameManager;

    public Sprite unselected;
    public Sprite selected;
    private SpriteRenderer crewSpriteRenderer;

    private WaterBar waterBar;
    private int totalWaterSprayed;
    private int tileIndex;
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
    public GameObject nextTile;
    public GameObject DestinationMarker;
    public GameObject targetTile;
    public GameObject TargetMarker;

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
        nextTile = null;
    }

    // Update is called once per frame
    void Update()
    {
        // disable the box collider for the currently occupied tile to avoid "OnMouseUp" interference
        currentTile.GetComponent<Collider2D>().enabled = false;

        // If this is not the currently selected object, show the "unselected" sprite and remove the destination marker
        if (gameManager.SelectedFireCrew != gameObject)
        {
            crewSpriteRenderer.sprite = unselected;
            DestinationMarker.SetActive(false);
            TargetMarker.SetActive(false);
        }

        // If a destination has been chosen for this crew, update the variable
        if (gameManager.SelectedFireCrew == gameObject && gameManager.SelectedTile != null && gameManager.DestSelectModeOn == true)
        {
            destinationTile = gameManager.SelectedTile;
            DestinationMarker.SetActive(true);
            DestinationMarker.transform.position = destinationTile.transform.position;
            gameManager.SelectedTile = null;
            gameManager.DestSelectModeOn = false;
        }

        // If a target has been chosen for the crew to spray water on, update the variable
        if (gameManager.SelectedFireCrew == gameObject && gameManager.SelectedTile != null && gameManager.TargetSelectModeOn == true)
        {
            // We can only mark a tile as a target if it is currently on fire and it is adjacent to this fire crew
            // We also cannot mark a tile as a target if the fire crew is still moving
            if ((gameManager.SelectedTile == currentTile.GetComponent<TileScript>().GetNorth() ||
                gameManager.SelectedTile == currentTile.GetComponent<TileScript>().GetEast() ||
                gameManager.SelectedTile == currentTile.GetComponent<TileScript>().GetSouth() ||
                gameManager.SelectedTile == currentTile.GetComponent<TileScript>().GetWest()) &&
                destinationTile == null && gameManager.SelectedTile.GetComponent<TileScript>().getBurning() == true)
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

        // update status bars for energy and water
        waterBar.currentWater = waterLevel;
        energyBar.currentEnergy = energyLevel;

        // move the crew to the next tile if not at the destination
        if (destinationTile != null && currentTile != null)
        {
            StartCoroutine("MoveToDest");
        }
        if (destinationTile == currentTile)
        {
            StopCoroutine("MoveToDest");
            destinationTile = null;
            DestinationMarker.SetActive(false);
        }

        // spray water on the target tile until the fire is extinguished
        if (targetTile != null && destinationTile == null)
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
    void OnMouseUp()
    {
        // If this is the currently selected game object, update the sprite
        crewSpriteRenderer.sprite = selected;
        gameManager.SelectedFireCrew = gameObject;

        if (destinationTile != null)
        {
            DestinationMarker.SetActive(true);
        }
    }

    // Handle movmenet to the destination
    IEnumerator MoveToDest()
    {
        Vector3 currentTilePosition = currentTile.transform.position;
        Vector3 destPosition = destinationTile.transform.position;
        bool destUnreachable= false;

        //print("Attempting to move to the destination");
        // Choose the next tile to move to, if we haven't already
        if (nextTile == null)
        {
            GameObject[] adjacentTiles = new GameObject[4];
            int i = 0;
            // check how many tiles are adjacent to the current one
            if (currentTile.GetComponent<TileScript>().GetNorth() != null)
            {
                adjacentTiles[i] = currentTile.GetComponent<TileScript>().GetNorth();
                i++;
            }
            if (currentTile.GetComponent<TileScript>().GetEast() != null)
            {
                adjacentTiles[i] = currentTile.GetComponent<TileScript>().GetEast();
                i++;
            }
            if (currentTile.GetComponent<TileScript>().GetSouth() != null)
            {
                adjacentTiles[i] = currentTile.GetComponent<TileScript>().GetSouth();
                i++;
            }
            if (currentTile.GetComponent<TileScript>().GetWest() != null)
            {
                adjacentTiles[i] = currentTile.GetComponent<TileScript>().GetWest();
                i++;
            }

            // find the adjacent tile that will bring us the closest to the destination tile
            float minDistance = Vector3.Distance(currentTilePosition, destPosition);
            int minDistanceIndex = 0;
            for (int j = 0; j<i; j++)
            {
                Vector3 nextPosition = adjacentTiles[j].transform.position;
                float distance = Vector3.Distance(nextPosition, destPosition);

                // update the minimum distance
                if (distance < minDistance)
                {
                    if (adjacentTiles[j].GetComponent<TileScript>().getBurning() == false)
                    {
                        minDistance = distance;
                        minDistanceIndex = j;
                    }
                    else if (adjacentTiles[j].GetComponent<TileScript>().getBurning() == true && adjacentTiles[j] == destinationTile)
                    {
                        destUnreachable = true;
                    }
                }
            }

            // the next tile is the one with the shortest distance to the destination
            if (!destUnreachable)
            {
                nextTile = adjacentTiles[minDistanceIndex];
                lastWaypointTime = Time.time;
            }
            else
            {
                // can't reach destination tile, so stop here
                nextTile = null;
                destinationTile = currentTile;
            }
        }
        else if (gameObject.transform.position.Equals(nextTile.transform.position))
        {
            // we have arrived at the next tile
            currentTile.GetComponent<Collider2D>().enabled = true;
            currentTile = nextTile;
            nextTile = null;
        }
        else
        {
            // keep moving to the next tile
            Vector3 nextTilePosition = nextTile.transform.position;
            float segmentLength = Vector3.Distance(currentTilePosition, nextTilePosition);
            float totalSegmentTime = segmentLength / movementSpeed;
            float timeOnSegment = Time.time - lastWaypointTime;
            if (timeOnSegment < totalSegmentTime - 0.05)
            {
                // we are far away from the next tile, so keep moving
                gameObject.transform.position = Vector2.Lerp(currentTilePosition, nextTilePosition, (timeOnSegment / totalSegmentTime));
            }
            else
            {
                // we are close enough to the next tile, so match position
                gameObject.transform.position = nextTile.transform.position;
            }

            // This keeps the destination marker from moving along with the other objects in the FireCrew prefab
            DestinationMarker.transform.position = destinationTile.transform.position;

        }

        yield return null;
    }

    // Handle spraying water on the fire
    IEnumerator SprayWater()
    {
        if (targetTile.GetComponent<TileScript>().getBurning() == true && waterLevel >= 1)
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
