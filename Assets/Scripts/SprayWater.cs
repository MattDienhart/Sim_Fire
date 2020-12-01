using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayWater : MonoBehaviour
{
    private GameManager gameManager;
    private int totalWaterSprayed;
    private int waterLevel;
    private int tileIndex;

    public int SprayRate;           // % of unit's water bar per second
    public int ExtinguishAmount;    // % of unit's water bar needed to extinguish any fire


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Douse(GameObject targetTile)
    {
        totalWaterSprayed = 0;

        // attempt to spray water on the fire, and put it out if totalWaterSprayed >= ExtinguishAmount
        while ((totalWaterSprayed < ExtinguishAmount) && (targetTile != null))
        {
            // fetch the water level from the calling unit
            if (gameObject.CompareTag("FireCrew"))
            {
                waterLevel = gameObject.GetComponent<FireCrew>().waterLevel;
            }
            else if (gameObject.CompareTag("FireTruck"))
            {
                waterLevel = gameObject.GetComponent<FireTruck>().waterLevel;
            }
            else if (gameObject.CompareTag("Helicopter"))
            {
                waterLevel = gameObject.GetComponent<Helicopter>().waterLevel;
            }
            
            // spray water on the target if it's still burning and we have enough water left
            if (targetTile.GetComponent<TileScript>().GetBurning() == true && waterLevel >= SprayRate)
            {
                waterLevel -= SprayRate;
                totalWaterSprayed += SprayRate;
            }
            else
            {
                targetTile = null;

                // update the target tile on the calling unit to indicate that we are done spraying water
                if (gameObject.CompareTag("FireCrew"))
                {
                    gameObject.GetComponent<FireCrew>().targetTile = null;
                }
                else if (gameObject.CompareTag("FireTruck"))
                {
                    gameObject.GetComponent<FireTruck>().targetTile = null;
                }
                else if (gameObject.CompareTag("Helicopter"))
                {
                    gameObject.GetComponent<Helicopter>().targetTile = null;
                }
            }

            // determine if the fire is extinguished yet
            if (totalWaterSprayed >= ExtinguishAmount)
            {
                // Grab tile index
                for(int i = 0; i < gameManager.AllTiles.Length; i++ ) 
                {
                    if(GameObject.ReferenceEquals(targetTile, gameManager.AllTiles[i])) tileIndex = i;
                }
                Debug.Log("Put out fire on tile:" + (tileIndex + 1).ToString());
                StartCoroutine(gameManager.GetComponent<GameManager>().PutOutFire(tileIndex));
                targetTile = null;

                // update the target tile on the calling unit to indicate that we are done spraying water
                if (gameObject.CompareTag("FireCrew"))
                {
                    gameObject.GetComponent<FireCrew>().targetTile = null;
                }
                else if (gameObject.CompareTag("FireTruck"))
                {
                    gameObject.GetComponent<FireTruck>().targetTile = null;
                }
                else if (gameObject.CompareTag("Helicopter"))
                {
                    gameObject.GetComponent<Helicopter>().targetTile = null;
                }
            }

            // update the water level of the calling unit
            if (gameObject.CompareTag("FireCrew"))
            {
                gameObject.GetComponent<FireCrew>().waterLevel = waterLevel;
            }
            else if (gameObject.CompareTag("FireTruck"))
            {
                gameObject.GetComponent<FireTruck>().waterLevel = waterLevel;
            }
            else if (gameObject.CompareTag("Helicopter"))
            {
                gameObject.GetComponent<Helicopter>().waterLevel = waterLevel;
            }

            yield return new WaitForSeconds(1.0f);

        }
    }
}
