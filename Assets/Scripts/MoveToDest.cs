using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToDest : MonoBehaviour
{
    public GameObject nextTile;
    private float lastWaypointTime;
    private List<GameObject> adjacentTiles = new List<GameObject>();
    private List<GameObject> visitedTiles = new List<GameObject>();

    private GameObject northTile;
    private GameObject eastTile;
    private GameObject southTile;
    private GameObject westTile;

    // Start is called before the first frame update
    void Start()
    {
        nextTile = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Handle unit's movmenet to the destination
    public IEnumerator Move(GameObject currentTile, GameObject destinationTile, float movementSpeed)
    {
        Vector3 destPosition = destinationTile.transform.position;
        bool destUnreachable= false;
        adjacentTiles.Clear();
        visitedTiles.Clear();

        while(destUnreachable == false && currentTile != destinationTile)
        {
            Vector3 currentTilePosition = currentTile.transform.position;
            adjacentTiles.Clear();

            // keep the calling unit's destination marker from moving as the unit moves
            if (gameObject.CompareTag("FireCrew"))
            {
                gameObject.GetComponent<FireCrew>().DestinationMarker.transform.position = destinationTile.transform.position;
            }
            else if (gameObject.CompareTag("FireTruck"))
            {
                gameObject.GetComponent<FireTruck>().DestinationMarker.transform.position = destinationTile.transform.position;
            }

            // Choose the next tile to move to, if we haven't already
            if (nextTile == null)
            {
                northTile = currentTile.GetComponent<TileScript>().GetNorth();
                eastTile = currentTile.GetComponent<TileScript>().GetEast();
                southTile = currentTile.GetComponent<TileScript>().GetSouth();
                westTile = currentTile.GetComponent<TileScript>().GetWest();

                // populate "adjacentTiles", a list of all the unoccupied neighboring tiles
                if ((northTile != null) && (northTile != currentTile))
                {
                    // omit burning or occupied tiles
                    if (northTile.GetComponent<TileScript>().GetBurning() == true || northTile.GetComponent<TileScript>().GetOccupied() == true || visitedTiles.Contains(northTile))
                    {
                        if (northTile == destinationTile)
                        {
                            // set this flag if we are next to the destination tile, but can't reach it
                            destUnreachable = true;
                        }
                    }
                    else 
                    {
                        adjacentTiles.Add(northTile);
                    }
                }
                if ((eastTile != null) && (eastTile != currentTile))
                {
                    // omit burning or occupied tiles
                    if (eastTile.GetComponent<TileScript>().GetBurning() == true || eastTile.GetComponent<TileScript>().GetOccupied() == true || visitedTiles.Contains(eastTile))
                    {
                        if (eastTile == destinationTile)
                        {
                            // set this flag if we are next to the destination tile, but can't reach it
                            destUnreachable = true;
                        }
                    }
                    else 
                    {
                        adjacentTiles.Add(eastTile);
                    }
                }
                if ((southTile != null) && (southTile != currentTile))
                {               
                    // omit burning or occupied tiles
                    if (southTile.GetComponent<TileScript>().GetBurning() == true || southTile.GetComponent<TileScript>().GetOccupied() == true || visitedTiles.Contains(southTile))
                    {
                        if (southTile == destinationTile)
                        {
                            // set this flag if we are next to the destination tile, but can't reach it
                            destUnreachable = true;
                        }
                    }
                    else 
                    {
                        adjacentTiles.Add(southTile);
                    }
                }
                if ((westTile != null) && (westTile != currentTile))
                {
                    // omit burning or occupied tiles
                    if (westTile.GetComponent<TileScript>().GetBurning() == true || westTile.GetComponent<TileScript>().GetOccupied() == true || visitedTiles.Contains(westTile))
                    {
                        if (westTile == destinationTile)
                        {
                            // set this flag if we are next to the destination tile, but can't reach it
                            destUnreachable = true;
                        }
                    }
                    else 
                    {
                        adjacentTiles.Add(westTile);
                    }
                }

                // find the adjacent tile that will bring us the closest to the destination tile
                float minDistance = 9999.0f;
                int minDistanceIndex = 0;
                foreach (GameObject neighbor in adjacentTiles)
                {
                    Vector3 nextPosition = neighbor.transform.position;
                    float distance = Vector3.Distance(nextPosition, destPosition);

                    // find the minimum distance
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        minDistanceIndex = adjacentTiles.IndexOf(neighbor);
                    }
                }

                // the next tile is the one with the shortest distance to the destination
                if ((destUnreachable == false) && (adjacentTiles.Count > 0))
                {
                    nextTile = adjacentTiles[minDistanceIndex];
                    lastWaypointTime = Time.time;
                }
                else
                {
                    // can't reach destination tile, so stop here
                    nextTile = null;
                    destinationTile = currentTile;
                    Debug.Log("Destination cannot be reached!");

                    // set the calling unit's "destinationTile" parameter to the current tile
                    if (gameObject.CompareTag("FireCrew"))
                    {
                        gameObject.GetComponent<FireCrew>().destinationTile = currentTile;
                    }
                    else if (gameObject.CompareTag("FireTruck"))
                    {
                        gameObject.GetComponent<FireTruck>().destinationTile = currentTile;
                    }
                }
            }
            else if (gameObject.transform.position.Equals(nextTile.transform.position))
            {
                // we have arrived at the next tile
                currentTile.GetComponent<TileScript>().SetOccupied(false); // current tile is now unoccupied
                nextTile.GetComponent<TileScript>().SetOccupied(true); // next tile is now occupied
                visitedTiles.Add(currentTile);
                currentTile = nextTile;
                nextTile = null;

                // update the calling unit's "currentTile" parameter
                if (gameObject.CompareTag("FireCrew"))
                {
                    gameObject.GetComponent<FireCrew>().currentTile = currentTile;
                }
                else if (gameObject.CompareTag("FireTruck"))
                {
                    gameObject.GetComponent<FireTruck>().currentTile = currentTile;
                }

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

                    // keep the calling unit's destination marker from moving
                    if (gameObject.CompareTag("FireCrew"))
                    {
                        gameObject.GetComponent<FireCrew>().DestinationMarker.transform.position = destinationTile.transform.position;
                    }
                    else if (gameObject.CompareTag("FireTruck"))
                    {
                        gameObject.GetComponent<FireTruck>().DestinationMarker.transform.position = destinationTile.transform.position;
                    }
                }

            }
            yield return null;
        }
    }
}
