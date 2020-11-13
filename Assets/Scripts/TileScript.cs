using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public abstract class TileScript : MonoBehaviour
{
    protected int dryness;
    protected int speed;
    private int amountBurned = 0;
    public bool burning = false;
    public Sprite sprite;
    public string terrian = "terrain tile";

    protected List<GameObject> neighborTiles = new List<GameObject>();
    private GameObject northTile;
    private GameObject southTile;
    private GameObject eastTile;
    private GameObject westTile;

    void Start()
    {
//        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
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

    public virtual void setBurning(bool change)
    {
        burning = change;
    }

    public int getDryness()
    {
        return dryness;
    }

    public void getNeighbors()
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
            neighborTiles.Add(southTile);
        }
        if (tileNum + 18 < 180)
        {
            northTile = GameObject.Find("Tile (" + (tileNum + 18) + ")");
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
        if(northTile) return northTile;
        return this.gameObject;
    }

    public GameObject GetSouth()
    {
        if (northTile) return southTile;
        return this.gameObject;
    }

    public GameObject GetEast()
    {
        if (northTile) return eastTile;
        return this.gameObject;
    }

    public GameObject GetWest()
    {
        if (northTile) return westTile;
        return this.gameObject;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("Collided");
    }

    public int GetAmountBurned()
    {
        return amountBurned;
    }

    public int GetSpeed()
    {
        return speed;
    }

    public string GetTerrian()
    {
        return terrian;
    }

    public void SetSprite(Sprite newSprite)
    {
        GetComponent<SpriteRenderer>().sprite = newSprite;
    }
}
