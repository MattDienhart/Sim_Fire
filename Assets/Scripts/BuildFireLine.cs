using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildFireLine : MonoBehaviour
{
    private GameManager gameManager;
    private int totalEnergyUsed;
    private int energyLevel;
    private int tileIndex;

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

    public IEnumerator Build(GameObject targetTile)
    {
        totalEnergyUsed = 0;

        // attempt to build a fireline on the target tile, and succeed if totalEnergyUsed >= FinishAmount
        while ((totalEnergyUsed < FinishAmount) && (targetTile != null))
        {
            // fetch the energy level from the calling unit
            energyLevel = gameObject.GetComponent<FireCrew>().energyLevel;
            
            // expend energy to dig the fire line
            if (energyLevel >= WorkRate)
            {
                energyLevel -= WorkRate;
                totalEnergyUsed += WorkRate;
            }
            else
            {
                targetTile = null;
            }

            // required energy has been expended, so call BuildFireLine()
            if (totalEnergyUsed >= FinishAmount)
            {
                // Grab tile index
                for(int i = 0; i < gameManager.AllTiles.Length; i++ ) 
                {
                    if(GameObject.ReferenceEquals(targetTile, gameManager.AllTiles[i])) tileIndex = i;
                }
                Debug.Log("Cleared vegetation on tile: " + (tileIndex + 1).ToString());
                //targetTile.GetComponent<TileScript>().BuildFireLine();
                targetTile = null;
            }

            // update the energy level of the calling unit
            gameObject.GetComponent<FireCrew>().energyLevel = energyLevel;

            yield return new WaitForSeconds(1.0f);

        }
    }
}
