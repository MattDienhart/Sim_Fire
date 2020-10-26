using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBar : MonoBehaviour
{
    public float maxEnergy = 100;
    public float currentEnergy = 100;
    private float originalScale;

    // Start is called before the first frame update
    void Start()
    {
        originalScale = gameObject.transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 tempScale = gameObject.transform.localScale;
        tempScale.x = (currentEnergy / maxEnergy) * originalScale;
        gameObject.transform.localScale = tempScale;
    }
}
