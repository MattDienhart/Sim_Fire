using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBar : MonoBehaviour
{
    public float maxWater = 100;
    public float currentWater = 100;
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
        tempScale.x = (currentWater / maxWater) * originalScale;
        gameObject.transform.localScale = tempScale;
    }
}
