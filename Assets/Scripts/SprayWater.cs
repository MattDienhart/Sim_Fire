using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayWater : MonoBehaviour
{
    private FireCrewData fireCrewData;
    public List<GameObject> firesInRange;

    // Start is called before the first frame update
    void Start()
    {
        firesInRange = new List<GameObject>();
        fireCrewData = gameObject.GetComponentInChildren<FireCrewData>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
