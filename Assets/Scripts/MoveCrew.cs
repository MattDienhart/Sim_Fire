using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCrew : MonoBehaviour
{
    [HideInInspector]
    public GameObject[] waypoints;

    private int currentWaypoint = 0;
    private float lastWaypointSwitchTime;
    public float speed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        lastWaypointSwitchTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
