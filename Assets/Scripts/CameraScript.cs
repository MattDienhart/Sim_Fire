using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    int speed = 5;
    bool xMoreZero = true;
    bool yLessZero = true;
    bool yMore19 = true;
    bool xLess35 = true;

    void Update()
    {
        xMoreZero = transform.position.x > 0;
        yLessZero = transform.position.y < 0;
        yMore19 = transform.position.y > -19.7f;
        xLess35 = transform.position.x < 35.67f;
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
