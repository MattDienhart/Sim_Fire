using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerBehavior : MonoBehaviour
{
    private GameObject[] allTiles;
    private int fireCrewInstances;

    public GameObject fireCrewPrefab;

    private GameObject selectedFireCrew;
    public GameObject SelectedFireCrew
    {
        get 
        {
            return selectedFireCrew;
        }
        set 
        {
            selectedFireCrew = value;
            print("This object was selected: Fire Crew " + selectedFireCrew.GetComponent<FireCrew>().CrewID);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        allTiles = GameObject.FindGameObjectsWithTag("Tile");

        // instantiate the first set of fire crews at the start of the game
        fireCrewInstances = 0;
        AddFireCrew(allTiles[45]);
        AddFireCrew(allTiles[110]);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Spawn new fireCrew instances from the FireCrew prefab
    void AddFireCrew(GameObject spawnLocation)
    {
        GameObject newFireCrew = (GameObject)Instantiate(fireCrewPrefab);
        newFireCrew.transform.position = spawnLocation.transform.position;
        newFireCrew.GetComponent<FireCrew>().CrewID = fireCrewInstances + 1;
        newFireCrew.GetComponent<FireCrew>().waterLevel = 100;
        newFireCrew.GetComponent<FireCrew>().energyLevel = 100;
        fireCrewInstances ++;
    }
}
