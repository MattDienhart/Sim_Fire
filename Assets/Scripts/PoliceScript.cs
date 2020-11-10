using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceScript : MonoBehaviour
{

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            if (hit.collider != null)
            {
                Vector2 pos = hit.collider.gameObject.GetComponent<Transform>().position;
                Debug.Log("Clicked: " + hit.collider.gameObject.name + "  x: " + pos.x + " y: " + pos.y);
                transform.position = (pos);

                Debug.Log(hit.collider.name);
            }
        }
    }
}
