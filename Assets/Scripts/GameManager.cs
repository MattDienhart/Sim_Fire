using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private GameObject[] allTiles;
    private int fireCrewInstances;

    public GameObject fireCrewPrefab;
    
    private GameObject selectedFireCrew;
    

    [Header("HUD")]
    public Text selectedText;
    public Button crewBtn;
    public Button dispatchBtn;
    public Button infoBtn;


    // Start is called before the first frame update
    void Start()
    {
        allTiles = GameObject.FindGameObjectsWithTag("Tile");

        // Set on click listeners
        crewBtn.onClick.AddListener(() => CrewClicked());
        dispatchBtn.onClick.AddListener(() => DispatchClicked());
        infoBtn.onClick.AddListener(() => InfoClicked());
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

    void CrewClicked()
    {
        Debug.Log("Crew button has been clicked.");
        selectedText.text = "Crew";
    }

    void DispatchClicked()
    {
        Debug.Log("Dispatch button has been clicked.");
        selectedText.text = "Dispatch";
    }

    void InfoClicked()
    {
        Debug.Log("Info button has been clicked.");
        selectedText.text = "Info";
    }

    public GameObject SelectedFireCrew
    {
        get
        {
            return selectedFireCrew;
        }
        set
        {
            selectedFireCrew = value;
            selectedText.text = "Fire Crew " + selectedFireCrew.GetComponent<FireCrew>().CrewID;
            print("This object was selected: Fire Crew " + selectedFireCrew.GetComponent<FireCrew>().CrewID);
        }
    }
}
