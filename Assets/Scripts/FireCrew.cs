﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCrew : MonoBehaviour
{
    public int WaterLevel;
    public int EnergyLevel;
    private GameManagerBehavior gameManager;

    public Sprite unselected;
    public Sprite selected;
    private SpriteRenderer crewSpriteRenderer;

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
        crewSpriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManagerBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        // If this is not the currently selected object, update the sprite
        if (gameManager.SelectedFireCrew != gameObject)
        {
            crewSpriteRenderer.sprite = unselected;
        }
    }

    // Handle selection of this object
    void OnMouseUp()
    {
        // If this is the currently selected game object, update the sprite
        crewSpriteRenderer.sprite = selected;
        gameManager.SelectedFireCrew = gameObject;
    }


}