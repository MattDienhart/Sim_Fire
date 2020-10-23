using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllTilesScript : MonoBehaviour
{
    /// -------------> Kurt likely to delete, testing adjustable game size
    /*
    public Camera mainCamera;
    public GameObject tilePrefab;
    void Start()
    {
        int rowLength = 0;
        Vector3 pos = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0));
        pos = new Vector3(pos.x + tilePrefab.GetComponent<Renderer>().bounds.extents.x,
                          pos.y + tilePrefab.GetComponent<Renderer>().bounds.extents.y,
                          0);
        while(pos.x < 1)
        {
            Instantiate(tilePrefab, pos, Quaternion.identity);
            pos.x++;
            rowLength++;
        }

        pos.x = 0;
        while (pos.y > 0)
        {
            pos.y++;
            for (int i = 0; i < rowLength; i++)
            {
                Instantiate(tilePrefab, pos, Quaternion.identity);
                pos.x++;
            }
            pos.y++;
            pos.x = 0;
        }
        


    }

    // Update is called once per frame
    void Update()
    {
        
    }
    */
}
