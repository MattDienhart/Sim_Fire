using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public abstract class TileScript : MonoBehaviour
{
    protected int dryness;
    protected int speed;
    private int amountBurned = 0;
    public bool burning = false;
    public string terrain = "terrain tile";

    // Neighboring Tiles
    protected List<GameObject> neighborTiles = new List<GameObject>();
    public GameObject northTile;
    public GameObject southTile;
    public GameObject eastTile;
    public GameObject westTile;

    public bool occupied = false;
    protected int borderCount = 0;


    private GameManager gameManager;
    private TileManager tileManager;
    private GameObject borderPrefab;
    private GameObject firePrefab;
    protected List<GameObject> obstacles = new List<GameObject>();


    public bool fireLinePresent = false;
    private GameObject fireLineObject = null;

    public bool nearFireHouse = false;

    private int columnCount = 18 * 3;
    private int rowCount = 10 * 3;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        // gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        tileManager = GameObject.Find("TileManager").GetComponent<TileManager>();
        borderPrefab = tileManager.GetBorderPrefab();
        firePrefab = tileManager.GetFirePrefab();

        columnCount = tileManager.GetColumnCount();
        rowCount = tileManager.GetRowCount();
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
            if(dryness - 10 > 0)dryness -= 10;
        }
    }

    public int GetObstacles()
    {
        return obstacles.Count;
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

    public virtual void BuildFireLine()
    {
        tileManager = GameObject.Find("TileManager").GetComponent<TileManager>();
        GameObject fireLine = tileManager.GetFireLinePrefab();
        fireLineObject = Instantiate(
               fireLine,
               this.transform.position,
               this.transform.rotation,
               this.transform.parent);
        fireLineObject.transform.parent = this.transform;
        fireLineObject.name = "FireLine";
        if (dryness - 20 > 0) dryness -= 20;
        fireLinePresent = true;
    }

    public bool GetFireLineBoolean()
    {
        return fireLinePresent;
    }

    public void DestroyFireLine()
    {
        fireLinePresent = false;
        Destroy(fireLineObject);
    }

    public void RotateFireLine()
    {
        Transform fireLine = transform.Find("FireLine");
        if (fireLine != null)
            fireLine.Rotate(Vector3.forward * 90);
    }

    public void GetNeighbors()
    {
        string temp = "";
        int tileNum = System.Int32.Parse(Regex.Match(this.name, @"\d+").Value);

        if (tileNum - 1 > 0 && (tileNum / columnCount) == ((tileNum - 1) / columnCount))
        {
            westTile = GameObject.Find("Tile (" + (tileNum - 1) + ")");
            neighborTiles.Add(westTile);

        }
        if (tileNum + 1 < rowCount * columnCount && (tileNum / columnCount) == ((tileNum + 1) / columnCount))
        {
            eastTile = GameObject.Find("Tile (" + (tileNum + 1) + ")");
            neighborTiles.Add(eastTile);

        }
        if (tileNum - columnCount > 0)
        {
            northTile = GameObject.Find("Tile (" + (tileNum - columnCount) + ")");

            neighborTiles.Add(northTile);

        }
        if ((tileNum + columnCount) < (rowCount * columnCount))
        {
           // Debug.Log("south: " + (tileNum + columnCount));
            southTile = GameObject.Find("Tile (" + (tileNum + columnCount) + ")");

            neighborTiles.Add(southTile);

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

    public int GetAmountBurned()
    {
        return amountBurned;
    }

    // Handle the selection of this object
    public void Selected()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            Debug.Log("Tile " + gameObject.GetInstanceID() + " has been clicked");
            // If we are not in any selection mode, deselect all fire crews
            if (gameManager.DestSelectModeOn == false && gameManager.TargetSelectModeOn == false)
            {
                gameManager.SelectedUnit = null;
                gameManager.SelectedTile = null;
            }
            else if ((gameManager.DestSelectModeOn == true ||
                gameManager.TargetSelectModeOn == true) && gameManager.SelectedUnit != null)
            {
                gameManager.SelectedTile = gameObject;
            }
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
        borderPrefab = GameObject.Find("TileManager").GetComponent<TileManager>().GetBorderPrefab();
        foreach (Transform child in this.transform)
        {
            if (!child.gameObject.CompareTag("TileBorder"))
            {
                Destroy(child.gameObject);
            }
        }
        GameObject newBorder = Instantiate(
               borderPrefab,
               this.transform.position,
               this.transform.rotation,
               this.transform.parent);
        newBorder.transform.localScale = this.transform.localScale;
        // If corner adjust order layer on top of border
        if (sprite.name == "grassEdge")
        {
            newBorder.GetComponent<SpriteRenderer>().sortingOrder = 4;
        } else if (sprite.name == "grassCorner")
        {
            newBorder.GetComponent<SpriteRenderer>().sortingOrder = 3;
        }
        // just grass 
        newBorder.GetComponent<SpriteRenderer>().sprite = sprite;
        newBorder.name = "Border";
        float current = transform.localRotation.eulerAngles.z;
        // Rotate child
        newBorder.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, current + rotation));
        newBorder.transform.parent = gameObject.transform;
        // Grass border should be placed on top of all others
        if (sprite.name == "grassEdge")
        {
            newBorder.GetComponent<SpriteRenderer>().sortingOrder = 5;
        }
        else if(sprite.name == "grassCorner")
        {
            newBorder.GetComponent<SpriteRenderer>().sortingOrder = 4;
        }
        else if (sprite.name == "dirtCorner")
        {
            newBorder.GetComponent<SpriteRenderer>().sortingOrder = 3;
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
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.SetNotificationText(msg);
    }

    public void SetFirehouseTile()
    {
        nearFireHouse = true;
    }

    public void SetFirehouseNeighbors()
    {
        nearFireHouse = true;
        foreach (GameObject neighbor in neighborTiles)
        {
            neighbor.GetComponent<TileScript>().SetFirehouseTile();
        }
    }

    public bool GetNearFirehouse()
    {
        return nearFireHouse;
    }
}

