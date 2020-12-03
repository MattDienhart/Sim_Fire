using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildFireLine : MonoBehaviour
{
    private GameManager gameManager;
    private int totalEnergyUsed;
    private int energyLevel;
    private int tileIndex;
    private GameObject build;
    public GameObject buildNorth;
    public GameObject buildSouth;
    public GameObject buildEast;
    public GameObject buildWest;

    public int WorkRate;        // % of unit's energy bar per second
    public int FinishAmount;    // % of unit's energy bar needed to finish building the fire line

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Build(GameObject targetTile, string direction)
    {
        totalEnergyUsed = 0;

        switch(direction)
        {
            case "North": build = buildNorth; break;
            case "South": build = buildSouth; break;
            case "East": build = buildEast; break;
            case "West": build = buildWest; break;
        }

        // attempt to build a fireline on the target tile, and succeed if totalEnergyUsed >= FinishAmount
        while ((totalEnergyUsed < FinishAmount) && (targetTile != null))
        {
            // fetch the energy level from the calling unit
            energyLevel = gameObject.GetComponent<FireCrew>().energyLevel;
            
            // expend energy to dig the fire line
            if (targetTile.GetComponent<TileScript>().fireLinePresent == false && energyLevel >= WorkRate)
            {
                energyLevel -= WorkRate;
                totalEnergyUsed += WorkRate;
                build.SetActive(true);
            }
            else
            {
                targetTile = null;
                gameObject.GetComponent<FireCrew>().targetTile = null;
                StartCoroutine(gameManager.SendNotification("ERROR: There's already a fireline there", 2));
            }

            // required energy has been expended, so call BuildFireLine()
            if (totalEnergyUsed >= FinishAmount && targetTile.GetComponent<TileScript>().fireLinePresent == false)
            {
                // Grab tile index
                for(int i = 0; i < gameManager.AllTiles.Length; i++ ) 
                {
                    if(GameObject.ReferenceEquals(targetTile, gameManager.AllTiles[i])) tileIndex = i;
                }
                Debug.Log("Cleared vegetation on tile: " + (tileIndex + 1).ToString());
                targetTile.GetComponent<TileScript>().BuildFireLine();
                targetTile = null;
                gameObject.GetComponent<FireCrew>().targetTile = null;
                build.SetActive(false);
            }

            // update the energy level of the calling unit
            gameObject.GetComponent<FireCrew>().energyLevel = energyLevel;

            yield return new WaitForSeconds(1.0f);

        }
    }
}
