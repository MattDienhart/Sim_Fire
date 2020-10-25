using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerBehavior : MonoBehaviour
{
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
            print("This object was selected: Fire Crew " + selectedFireCrew.GetInstanceID());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
