using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCrew : MonoBehaviour
{
    public int waterLevel;
    public int energyLevel;
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

    // Start is called before the first frame update
    void Start()
    {
        crewSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        waterBar = gameObject.GetComponentInChildren<WaterBar>();
        energyBar = gameObject.GetComponentInChildren<EnergyBar>();
    }

    // Update is called once per frame
    void Update()
    {
        // If this is not the currently selected object, update the sprite
        if (gameManager.SelectedFireCrew != gameObject)
        {
            crewSpriteRenderer.sprite = unselected;
        }

        // update status bars for energy and water
        waterBar.currentWater = waterLevel;
        energyBar.currentEnergy = energyLevel;
    }

    // Handle selection of this object
    void OnMouseUp()
    {
        // If this is the currently selected game object, update the sprite
        crewSpriteRenderer.sprite = selected;
        gameManager.SelectedFireCrew = gameObject;
    }


}
