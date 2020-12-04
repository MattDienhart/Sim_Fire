using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraScript : MonoBehaviour
{
    int speed = 5;
    bool xMoreZero = true;
    bool yLessZero = true;
    bool yMore19 = true;
    bool xLess35 = true;
    float yMin = -19.7f;
    float xMax = 35.67f;

    private void Start()
    {
        if (GameObject.Find("Tile (718)")  && !GameObject.Find("Tile (1608)"))
        {
            yMin = -9.9f;
            xMax = 17.82f;
        }
        else if (!GameObject.Find("Tile (1608)"))
        {
            Destroy(GetComponent<CameraScript>());
        }
        
    }

    void Update()
    {
        xMoreZero = transform.position.x > 0;
        yLessZero = transform.position.y < 0;
        yMore19 = transform.position.y > yMin;
        xLess35 = transform.position.x < xMax;
        if (Input.GetKey(KeyCode.RightArrow) && xLess35)
        {
            transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.LeftArrow) && xMoreZero)
        {
            transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.DownArrow) && yMore19)
        {
            transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.UpArrow) && yLessZero)
        {
            transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
        }
    }
}
