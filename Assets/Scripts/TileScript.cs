using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public abstract  class TileScript : MonoBehaviour
{
    protected int dryness;
    protected int speed;
    private int amountBurned = 0;
    public bool burning = false;
    public Sprite sprite;
    public string terrain = "terrain tile";

    // Neighboring Tiles
    protected List<GameObject> neighborTiles = new List<GameObject>();
    public GameObject northTile;
    public GameObject southTile;
    public GameObject eastTile;
    public GameObject westTile;

    public bool occupied = false;
    protected int borderCount = 0;

    int columnCount = 18;
    public string color = "red";
    private GameManager gameManager;

    private GameObject borderPrefab;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        borderPrefab = GameObject.Find("BorderPrefab");
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

    public bool GetBurning()
    {
        return burning;
    }

    public virtual void SetBurning(bool change)
    {
        burning = change;
    }

    public int getDryness()
    {
        return dryness;
    }

    private void getNeighbors()
    {
        string temp = "";
        int tileNum = System.Int32.Parse(Regex.Match(this.name, @"\d+").Value);
        if (tileNum - 1 > 0 && (tileNum / columnCount) == ((tileNum - 1) / columnCount))
        {
            westTile = GameObject.Find("Tile (" + (tileNum - 1) + ")");
            neighborTiles.Add(westTile);
            temp += " west: " + westTile.name;
        }
        if (tileNum + 1 < 180 && (tileNum / columnCount) == ((tileNum+1) / columnCount))
        {
            eastTile = GameObject.Find("Tile (" + (tileNum + 1) + ")");
            neighborTiles.Add(eastTile);
            temp += " eastTile: " + eastTile.name;
        }
        if (tileNum - 18 > 0)
        {
            northTile = GameObject.Find("Tile (" + (tileNum - 18) + ")");

            neighborTiles.Add(northTile);
            temp += " northTile: " + northTile.name;
        }
        if (tileNum + 18 < 180)
        {
            southTile = GameObject.Find("Tile (" + (tileNum + 18) + ")");

            neighborTiles.Add(southTile);
            temp += " southTile: " + southTile.name;
        }
    }

    public void  CheckNeighbors()
    {
        int fireCount = 0;
        foreach (GameObject tile in neighborTiles)
        {
            if (tile.GetComponent<TileScript>().GetBurning()) fireCount++;
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


    public int GetAmountBurned()
    {
        return amountBurned;
    }


    // Handle the selection of this object
    public void Selected()
    {
        Debug.Log("Tile " + gameObject.GetInstanceID() + " has been clicked");
        // If we are not in any selection mode, deselect all fire crews
        if(gameManager.DestSelectModeOn == false && gameManager.TargetSelectModeOn == false)
        {
            gameManager.SelectedUnit = null;
            gameManager.SelectedTile = null;
        }
        else if ((gameManager.DestSelectModeOn == true || gameManager.TargetSelectModeOn == true) && gameManager.SelectedUnit != null)
        {
            gameManager.SelectedTile = gameObject;
        }
    }
    public bool GetOccupied()
    {
        return occupied;
    }

    public void SetOccupied(bool value)
    {
        occupied = value;
    }

    public string GetTerrain()
    {
        return terrain;
    }
    public virtual void SetBorderSprite(Sprite sprite, float rotation)
    {
        //string x = "dirtCorner";
        Debug.Log("SetBorderSprite name: " + sprite.name.Length + " t: " + this.name);

        foreach (Transform child in this.transform)
        {
            if (!child.gameObject.CompareTag("TileBorder"))
            {
                Destroy(child.gameObject);
            }
        }
        borderPrefab = GameObject.Find("BorderPrefab");
        //  Debug.Log("set border" + borderPrefab.name);
        GameObject newBorder = Instantiate(
               borderPrefab,
               this.transform.position,
               this.transform.rotation,
               this.transform.parent);
        newBorder.transform.localScale = this.transform.localScale;
        // If corner adjust order layer on top of border
        if (sprite.name.IndexOf("Corner") != -1) newBorder.GetComponent<SpriteRenderer>().sortingOrder = 2;
        newBorder.GetComponent<SpriteRenderer>().sprite = sprite;
        newBorder.name = "Border";
        float current = transform.localRotation.eulerAngles.z;
        // Rotate child
        newBorder.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, current + rotation));
        newBorder.transform.parent = gameObject.transform;
        // Grass border should be placed on top of all others
        if (sprite.name == "grassEdge")
        {
            newBorder.GetComponent<SpriteRenderer>().sortingOrder = 2;
        }
        borderCount++;
    }
    public virtual void SetCornerSprite(Sprite sprite, float rotation)
    {
        SetBorderSprite(sprite, rotation);
    }
}

