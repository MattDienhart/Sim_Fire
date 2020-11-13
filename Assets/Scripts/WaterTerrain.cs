using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTerrain : TileScript
{
    public Sprite[] sprites;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetBorderSprite(int index)
    {
        if (index >= sprites.Length) { Debug.Log("~~~~~~~index issue: " + index);  return; }

        gameObject.transform.GetChild(0)
            .GetComponent<SpriteRenderer>().sprite = sprites[index];
    }

    public void rotateBorder()
    {
        float curRot = transform.localRotation.eulerAngles.z;
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, curRot + 180));
    }


}
