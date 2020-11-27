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
    private TileManager tileManager;
    private GameObject borderPrefab;
    private GameObject firePrefab;
    protected List<GameObject> obstacles = new List<GameObject>();

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        tileManager = GameObject.Find("TileManager").GetComponent<TileManager>();
        borderPrefab = tileManager.GetBorderPrefab();
        firePrefab = tileManager.GetFirePrefab();
        GetNeighbors();
    }
    
    public bool GetBurning()
    {
        return burning;
    }

    public virtual void SetBurning(bool change)
    {
        burning = change;
        if(burning)
        {
            SpawnFire();
        }
        else
        {
            Transform fire = transform.Find("Fire");
            if (fire != null) Destroy(fire.gameObject);
        }
    }

    public void DestroyObstacle()
    {
        if(obstacles.Count > 0)
        {
            Destroy(obstacles[0]);
            obstacles.RemoveAt(0);
        }
    }

    public int GetDryness()
    {
        return dryness;
    }

    private void SpawnFire()
    {
        tileManager = GameObject.Find("TileManager").GetComponent<TileManager>();
        firePrefab = tileManager.GetFirePrefab();
        GameObject newFire = Instantiate(
               firePrefab,
               this.transform.position,
               this.transform.rotation,
               this.transform.parent);
        newFire.transform.parent = this.transform;
        newFire.name = "Fire";
    }

    protected virtual void BuildFireLine()
    {
        tileManager = GameObject.Find("TileManager").GetComponent<TileManager>();
        GameObject fireLine = tileManager.GetFireLinePrefab();
        GameObject newLine = Instantiate(
               fireLine,
               this.transform.position,
               this.transform.rotation,
               this.transform.parent);
        newLine.transform.parent = this.transform;
        newLine.name = "FireLine";
        dryness -= 20;
    }

    public void RotateFireLine()
    {
        Transform fireLine = transform.Find("FireLine");
        if (fireLine != null)
            fireLine.Rotate(Vector3.forward * 90);
    }

    private void GetNeighbors()
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
        foreach (Transform child in this.transform)
        {
            if (!child.gameObject.CompareTag("TileBorder"))
            {
                Destroy(child.gameObject);
            }
        }
        borderPrefab = GameObject.Find("BorderPrefab");
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

    public virtual int GetSpeed()
    {
        return 2;
    }

    public void TileNotificationText(string msg)
    {
       // gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
       // gameManager.SetNotificationText(msg);
    }
}

