using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class TileScript : MonoBehaviour
{
    private int dryness = 50;
    private int amountBurned = 0;
    public bool burning = false;
    public Sprite sprite;
    public string type = "terrain tile";
    List<GameObject> neighborTiles = new List<GameObject>();
    public GameObject northTile;
    public GameObject southTile;
    public GameObject eastTile;
    public GameObject westTile;

    public string color = "red";
    private GameManager gameManager;

    private string terrian; // tile dependant? forest, hill, water, grass, road
    // private int elevation = 0; Maybe a feature for later

    void Start()
    {

//        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        getNeighbors();
    }

    // Update is called once per frame
    void Update()
    {
        if(burning) {
            GetComponent<SpriteRenderer>().enabled = true;
        } else {
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public bool getBurning()
    {
        return burning;
    }

    public void setBurning(bool change)
    {
        burning = change;
    }

    public int getDryness()
    {
        return dryness;
    }

    private void getNeighbors()
    {
        int tileNum = System.Int32.Parse(Regex.Match(this.name, @"\d+").Value);
        if (tileNum - 1 > 0)
        {
            westTile = GameObject.Find("Tile (" + (tileNum - 1) + ")");
            neighborTiles.Add(westTile);
        }
        if (tileNum + 1 < 180)
        {
            eastTile = GameObject.Find("Tile (" + (tileNum + 1) + ")");
            neighborTiles.Add(eastTile);
        }
        if (tileNum - 18 > 0)
        {

            southTile = GameObject.Find("Tile (" + (tileNum - 18) + ")");

            southTile = GameObject.Find("Tile (" + (tileNum + 18) + ")");

            neighborTiles.Add(southTile);
        }
        if (tileNum + 18 < 180)
        {

            northTile = GameObject.Find("Tile (" + (tileNum + 18) + ")");

            northTile = GameObject.Find("Tile (" + (tileNum - 18) + ")");

            neighborTiles.Add(northTile);
        }
    }

    public void  checkNeighbors()
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

    public GameObject GetNorth()
    {
        if(neighborTiles.Contains(northTile)) 
        {
            return northTile;
        }
        else
        {
            return null;
        }
    }

    public GameObject GetSouth()
    {
        if(neighborTiles.Contains(southTile)) 
        {
            return southTile;
        }
        else
        {
            return null;
        }
    }

    public GameObject GetEast()
    {
        if(neighborTiles.Contains(eastTile)) 
        {
            return eastTile;
        }
        else
        {
            return null;
        }
    }

    public GameObject GetWest()
    {
        if(neighborTiles.Contains(westTile)) 
        {
            return westTile;
        }
        else
        {
            return null;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("Collided");
    }


    public int getAmountBurned()
    {
        return amountBurned;
    }


    // Handle the selection of this object
    void OnMouseUp()
    {
        // If we are not in destination select mode, deselect all fire crews
        if(gameManager.DestSelectModeOn == false)
        {
            gameManager.SelectedFireCrew = null;
            gameManager.SelectedTile = null;
        }
        else if (gameManager.DestSelectModeOn == true && gameManager.SelectedFireCrew != null)
        {
            gameManager.SelectedTile = gameObject;
        }
    }

}
