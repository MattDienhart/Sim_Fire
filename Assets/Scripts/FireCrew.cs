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

    // Start is called before the first frame update
    void Start()
    {
        crewSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        waterBar = gameObject.GetComponentInChildren<WaterBar>();
        energyBar = gameObject.GetComponentInChildren<EnergyBar>();
        DestinationMarker.SetActive(false);
        destinationTile = null;
        nextTile = null;
    }

    // Update is called once per frame
    void Update()
    {
        // If this is not the currently selected object, show the "unselected" sprite and remove the destination marker
        if (gameManager.SelectedFireCrew != gameObject)
        {
            crewSpriteRenderer.sprite = unselected;
            DestinationMarker.SetActive(false);
        }

        // If a destination has been chosen for this crew, update the variable
        if (gameManager.SelectedFireCrew == gameObject && gameManager.SelectedTile != null)
        {
            destinationTile = gameManager.SelectedTile;
            DestinationMarker.SetActive(true);
            DestinationMarker.transform.position = gameManager.SelectedTile.transform.position;
            gameManager.SelectedTile = null;
            gameManager.DestSelectModeOn = false;
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


}
