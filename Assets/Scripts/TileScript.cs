using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class TileScript : MonoBehaviour
{
    private int dryness = 50;
    private int damage = 0;
    private bool burning = false;
    public Sprite[] sprites;
    List<GameObject> neighborTiles = new List<GameObject>();

    private string terrian; // tile dependant? forest, hill, water, grass, road
    // private int elevation = 0; Maybe a feature for later


    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
        getNeighbors();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool getBurning()
    {
        return burning;
    }

    public void setBurning(bool change)
    {
        burning = change;
    }

    private void getNeighbors()
    {
        int tileNum = System.Int32.Parse(Regex.Match(this.name, @"\d+").Value);
        if(tileNum - 1 > 0) neighborTiles.Add(GameObject.Find("Tile (" + (tileNum - 1) + ")"));
        if (tileNum + 1 < 180) neighborTiles.Add(GameObject.Find("Tile (" + (tileNum - 1) + ")"));
        if (tileNum - 10 > 0) neighborTiles.Add(GameObject.Find("Tile (" + (tileNum - 1) + ")"));
        if (tileNum + 10 < 180) neighborTiles.Add(GameObject.Find("Tile (" + (tileNum - 1) + ")"));
    }

    private void checkNeighbors()
    {
        int fireCount = 0;
        foreach (GameObject tile in neighborTiles)
        {
            if (tile.GetComponent<TileScript>().getBurning()) fireCount++;
        }
        if(fireCount > 3)
        {
            dryness = 100;
            burning = true;
        }
    }
}
