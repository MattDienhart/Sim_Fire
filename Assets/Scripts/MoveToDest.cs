using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToDest : MonoBehaviour
{
    public GameObject nextTile;
    private float lastWaypointTime;

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
        Vector3 currentTilePosition = currentTile.transform.position;
        Vector3 destPosition = destinationTile.transform.position;
        bool destUnreachable= false;

        // Choose the next tile to move to, if we haven't already
        if (nextTile == null)
        {
            GameObject[] adjacentTiles = new GameObject[4];
            int i = 0;
            // populate "adjacentTiles[]", a list of all the unoccupied neighboring tiles
            if ((currentTile.GetComponent<TileScript>().GetNorth() != null) && (currentTile.GetComponent<TileScript>().GetNorth() != currentTile))
            {
                adjacentTiles[i] = currentTile.GetComponent<TileScript>().GetNorth();

                // omit burning or occupied tiles
                if (adjacentTiles[i].GetComponent<TileScript>().getBurning() == true || adjacentTiles[i].GetComponent<TileScript>().GetOccupied() == true)
                {
                    if (adjacentTiles[i] == destinationTile)
                    {
                        // set this flag if we are next to the destination tile, but can't reach it
                        destUnreachable = true;
                    }
                    else
                    {
                        adjacentTiles[i] = null;
                    }
                }
                else 
                {
                    i++;
                }
            }
            if ((currentTile.GetComponent<TileScript>().GetEast() != null) && (currentTile.GetComponent<TileScript>().GetEast() != currentTile))
            {
                adjacentTiles[i] = currentTile.GetComponent<TileScript>().GetEast();
                
                // omit burning or occupied tiles
                if (adjacentTiles[i].GetComponent<TileScript>().getBurning() == true || adjacentTiles[i].GetComponent<TileScript>().GetOccupied() == true)
                {
                    if (adjacentTiles[i] == destinationTile)
                    {
                        // set this flag if we are next to the destination tile, but can't reach it
                        destUnreachable = true;
                    }
                    else
                    {
                        adjacentTiles[i] = null;
                    }
                }
                else 
                {
                    i++;
                }
            }
            if ((currentTile.GetComponent<TileScript>().GetSouth() != null) && (currentTile.GetComponent<TileScript>().GetSouth() != currentTile))
            {
                adjacentTiles[i] = currentTile.GetComponent<TileScript>().GetSouth();
                
                // omit burning or occupied tiles
                if (adjacentTiles[i].GetComponent<TileScript>().getBurning() == true || adjacentTiles[i].GetComponent<TileScript>().GetOccupied() == true)
                {
                    if (adjacentTiles[i] == destinationTile)
                    {
                        // set this flag if we are next to the destination tile, but can't reach it
                        destUnreachable = true;
                    }
                    else
                    {
                        adjacentTiles[i] = null;
                    }
                }
                else 
                {
                    i++;
                }
            }
            if ((currentTile.GetComponent<TileScript>().GetWest() != null) && (currentTile.GetComponent<TileScript>().GetWest() != currentTile))
            {
                adjacentTiles[i] = currentTile.GetComponent<TileScript>().GetWest();
                
                // omit burning or occupied tiles
                if (adjacentTiles[i].GetComponent<TileScript>().getBurning() == true || adjacentTiles[i].GetComponent<TileScript>().GetOccupied() == true)
                {
                    if (adjacentTiles[i] == destinationTile)
                    {
                        // set this flag if we are next to the destination tile, but can't reach it
                        destUnreachable = true;
                    }
                    else
                    {
                        adjacentTiles[i] = null;
                    }
                }
                else 
                {
                    i++;
                }
            }

            // find the adjacent tile that will bring us the closest to the destination tile
            float minDistance = 9999.0f;
            int minDistanceIndex = 0;
            for (int j = 0; j<i; j++)
            {
                Vector3 nextPosition = adjacentTiles[j].transform.position;
                float distance = Vector3.Distance(nextPosition, destPosition);

                // find the minimum distance
                if (distance < minDistance)
                {
                    minDistance = distance;
                    minDistanceIndex = j;
                }
            }

            // the next tile is the one with the shortest distance to the destination
            if ((destUnreachable == false) && (adjacentTiles[minDistanceIndex] != null))
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
            currentTile.GetComponent<TileScript>().SetOccupied(); // current tile is now unoccupied
            nextTile.GetComponent<TileScript>().SetOccupied(); // next tile is now occupied
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
            }

        }

        yield return null;
    }
}
