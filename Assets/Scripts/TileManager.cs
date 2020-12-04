using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TileManager : MonoBehaviour
{
    [Header("Tiles")]
    public GameObject forestPrefab;
    public GameObject sandPrefab;
    public GameObject[] tilePrefabs;
    
    private int[] usedValues;
    private GameObject[] emptyTiles;

    [Header("Prefabs")]
    public GameObject firePrefab;
    public GameObject fireLinePrefab;
    public GameObject borderPrefab;

    List<int> values = new List<int>();
    string[] terrainTypes = { "Sand", "Forest", "Road", "SideRoad" };

    private int columnCount = 54;
    private int rowCount = 30;

    private int waterColumn;

    [Header("Edges")]
    public Sprite[] borderSprites;
    public Sprite[] cornerSprites;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "MediumStart")
        {
            columnCount = 36;
            rowCount = 20;
        } else if (SceneManager.GetActiveScene().name == "SmallStart")
        {
            columnCount = 18;
            rowCount = 10;
        }
        int tileCount = columnCount * rowCount;
        emptyTiles = GameObject.FindGameObjectsWithTag("EmptyTile");
        usedValues = Enumerable.Repeat(0, tileCount).ToArray();
      //  Debug.Log("rowCount: " + rowCount);
      //  Debug.Log("len: " + usedValues.Length);
        int startIndex = Random.Range(0, (columnCount * rowCount - 1));
        int currentIndex = startIndex;
        values.Add(currentIndex);
        int[] oneEight = { 1, 1 };
        oneEight[Random.Range(0, 2)] += columnCount;
        int negPos = Random.Range(0, 2) * 2 - 1;
        int count = 0;

        while (values.Count < usedValues.Length * 0.4f)
        {
            if (ValidIndex(currentIndex, (currentIndex + oneEight[0] * negPos)))
            {
                values.Add(currentIndex + oneEight[0] * negPos);
                usedValues[currentIndex + oneEight[0] * negPos] = 1;
            }

            if (ValidIndex(currentIndex, (currentIndex - oneEight[0] * negPos)))
            {
                values.Add(currentIndex - oneEight[0] * negPos);
                usedValues[currentIndex - oneEight[0] * negPos] = 1;
            }

            if (ValidIndex(currentIndex, (currentIndex + oneEight[1] * negPos)))
            {
                values.Add(currentIndex + oneEight[1] * negPos);
                usedValues[currentIndex + oneEight[1] * negPos] = 1;
            }

            if (ValidIndex(currentIndex, (currentIndex - oneEight[1] * negPos)))
            {
                values.Add(currentIndex - oneEight[1] * negPos);
                usedValues[currentIndex - oneEight[1] * negPos] = 1;
            }

            count++;
            if (count > 12500) { Debug.Log("ISSUE"); break; }
            currentIndex = values[Random.Range(0, values.Count)];
        }
        // ROAD select random column, if not sand try again
        int roadColumn = Random.Range(1, columnCount-1);
        if(usedValues[roadColumn] != 0) roadColumn = Random.Range(1, columnCount - 1);
        int sideStreet1 = Random.Range(1, rowCount - 1);
        int side1NegPos = Random.Range(0, 2) * 2 - 1;
        int side1Len = Random.Range(columnCount / 3 - 2, columnCount / 2 + 2);
        if ((roadColumn + side1Len * side1NegPos) < 2 || 
            ((roadColumn + side1Len * side1NegPos) > columnCount -1 ))
        {
            side1NegPos *= -1;
        }        

        int sideStreet2 = Random.Range(1, rowCount - 2);
        int side2NegPos = Random.Range(0, 2) * 2 - 1;
        int side2Len = Random.Range(columnCount / 3 - 2, columnCount / 2 + 2);
        if ((roadColumn + side2Len * side2NegPos) < 1 || (roadColumn + side2Len * side2NegPos) > columnCount) side2NegPos *= -1;

        while (side1NegPos == side2NegPos && CheckSideStreets(sideStreet1, sideStreet2))
        {
            sideStreet2 = Random.Range(2, rowCount - 2);
        }
        for (int i = 0; i < rowCount; i++)
        {
            usedValues[roadColumn + (columnCount * i)] = 2;
            if (i == sideStreet1) 
            {
                int j;
                int sideCell = roadColumn + (columnCount * i);
                for (j = 1; j < side1Len; j++)
                {
                    usedValues[sideCell + j * side1NegPos] = 3;
                    usedValues[sideCell - columnCount + j * side1NegPos] = 4;
                    usedValues[sideCell + columnCount + j * side1NegPos] = 4;
                }
                // Houses at end of side street
                usedValues[sideCell + j * side1NegPos] = 4;
                usedValues[sideCell - columnCount + j * side1NegPos] = 4;
                usedValues[sideCell + columnCount + j * side1NegPos] = 4;
                // Houses along main road, make sure not to cover other side road
                if(usedValues[sideCell - 1 * side1NegPos] != 3) usedValues[sideCell - 1 * side1NegPos] = 4;
                if (usedValues[sideCell - columnCount - 1 * side1NegPos] != 3) usedValues[sideCell - columnCount - 1 * side1NegPos] = 4;
                if (usedValues[sideCell + columnCount - 1 * side1NegPos] != 3) usedValues[sideCell + columnCount - 1 * side1NegPos] = 4;
            } 
            if (i == sideStreet2)
            {
                for (int j = 1; j < side2Len; j++)
                    usedValues[(roadColumn + (columnCount * i)) + j * side2NegPos] = 3;
            }
        }
        // Debugging to output current board numbers
        string mygrid = "";
        for (int i = 0; i < usedValues.Length; i++)
        {
            if (i % columnCount == 0) mygrid += "\r\n" + (i / columnCount + 1) + ":  ";
            mygrid += usedValues[i];
        }
        Debug.Log("-grid: \n" + mygrid);
        // Choose the water column
        waterColumn = 1 + (columnCount - 1) * Random.Range(0, 2);
        for (int i = 0; i < rowCount; i++)
        { 
            usedValues[waterColumn - 1 + (columnCount * i)] = 5;
        }
        // Check if sand tile island
        for (int j = 0; j < usedValues.Length; j++)
        {
            if (usedValues[j] == 0) SandCheck(j);
        }
        // Instatiate all tile objects forest, road, sand
        for (int j = 0; j < usedValues.Length; j++)
        {
            // Check if sand tile island
            GameObject tempTile = Instantiate(
               tilePrefabs[usedValues[j]],
               emptyTiles[j].transform.position,
               emptyTiles[j].transform.rotation,
               forestPrefab.transform.parent);
            tempTile.transform.localScale = emptyTiles[j].transform.localScale;
            tempTile.name = "Tile (" + j + ")";
            tempTile.tag = "Tile";

            // if water, added land border to it
            if (usedValues[j] == 5)
            {
                int next = -1;
                if (waterColumn < columnCount - 1) next = 1;
                // create border for water, child over water 
                if (usedValues[j + next] != 3)
                {
                    float rotation = 0;
                    if (waterColumn == 1) rotation = 180f;
                    tempTile.GetComponent<WaterTerrain>().SetBorderSprite
                        (borderSprites[usedValues[j + next]], rotation);
                }
                else 
                {
                    // else if a road tile make the board tile what the border above it is
                    float rotation = 0;
                    if (waterColumn == 1) rotation = 180f;
                    int tileIndex = j + next - columnCount;
                    if (tileIndex < 0) tileIndex = 3;
                    tempTile.GetComponent<WaterTerrain>().SetBorderSprite
                        (borderSprites[usedValues[tileIndex]], rotation);
                }
            }
            // If sand or dirt/forest tiles, create needed borders
            if(usedValues[j] == 0 || usedValues[j] == 1 || usedValues[j] == 5)
            {
                SetBorders(j, tempTile);
            }
            // Make sure edge tiles go to edge of actual viewable screen, increase scale slightly
            if (j < columnCount || j > (columnCount * (rowCount - 1))-1)
            {
                Vector3 locScale = tempTile.transform.localScale;
                tempTile.transform.localScale = new Vector3(locScale.x, locScale.y * 1.1f, locScale.z);
            } 
                Destroy(emptyTiles[j]);         
        }
        UpdateAllNeighbors();
    }

    private void UpdateAllNeighbors()
    {
        GameObject[] allNewTiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach(GameObject tile in allNewTiles)
        {
            tile.GetComponent<TileScript>().GetNeighbors();
        }
    }

    private bool ValidIndex(int currentIndex, int newIndex)
    {
        bool temp = true;
        if( newIndex < 0 || newIndex > (columnCount * rowCount - 1) ||
            values.IndexOf(newIndex) != -1 ||
            Mathf.Abs(currentIndex - newIndex) == 1 &&
                newIndex / columnCount != currentIndex / columnCount
            ) temp = false;
        return temp;
    }

    private bool CheckSideStreets(int sideStreet1, int sideStreet2)
    {
        return sideStreet2 == sideStreet1 ||
            sideStreet2 - 1 == sideStreet1 || 
            sideStreet2 + 1 == sideStreet1 ||
            sideStreet2 - 2 == sideStreet1 ||
            sideStreet2 + 2 == sideStreet1;
    }

    private void SandCheck(int tileNum)
    {
        int east = tileNum+1 < (columnCount * rowCount - 1) && (tileNum+1)/columnCount 
            == tileNum/columnCount ? usedValues[tileNum + 1] : -1;
        int west = tileNum - 1 > 0 && tileNum / columnCount == (tileNum - 1) / columnCount
            ? usedValues[tileNum - 1] : -1;
        int south = tileNum + columnCount < (columnCount * rowCount - 1) ? usedValues[tileNum + columnCount] : -1;
        int north = tileNum - columnCount > 0 ? usedValues[tileNum - columnCount] : 0;
        // if surrounded by sand tiles do nothing
        if (east == 0 || west == 0 || south == 0 || north == 0)
        {
            return;
        } else if (east == 1 || west == 1 || south == 1 || north == 1)
        {
            // if surrounded by forest convert to forest tile, index 1
            usedValues[tileNum] = 1;
        }
    }

    private void SetBorders(int tileNum, GameObject currentTile)
    {
        int east = tileNum + 1 < (columnCount * rowCount - 1) && (tileNum + 1) / columnCount
            == tileNum / columnCount ? usedValues[tileNum + 1] : -1;
        int west = tileNum - 1 > 0 && tileNum / columnCount == (tileNum - 1) / columnCount
            ? usedValues[tileNum - 1] : -2;
        int south = tileNum + columnCount < (columnCount * rowCount -1) ? usedValues[tileNum + columnCount] : -3;
        int north = tileNum - columnCount > 0 ? usedValues[tileNum - columnCount] : -4;
        bool eastDiff = east != usedValues[tileNum];
        bool westDiff = west != usedValues[tileNum];
        // if surrounded by sand tiles do nothing
        if (eastDiff && east != -1)
        {
            currentTile.GetComponent<TileScript>().SetBorderSprite(borderSprites[east], 180);
        }
        if (westDiff && west != -2)
        {
        // Debug.Log("west: " + tileNum);
            currentTile.GetComponent<TileScript>().SetBorderSprite(borderSprites[west], 0);
        }
        if (south != usedValues[tileNum] && south != -3)
        {
            currentTile.GetComponent<TileScript>().SetBorderSprite(borderSprites[south], 90);
        }
        if (north != usedValues[tileNum] && north != -4)
        {
          //  Debug.Log("west");
            currentTile.GetComponent<TileScript>().SetBorderSprite(borderSprites[north], -90);
        }

        // Corner borders creation
        if (south > -1)
        {
            if (east > -1) {
                    int southEast = usedValues[tileNum + columnCount + 1];
                    if (usedValues[tileNum] != southEast && southEast != east && southEast != south)
                    {
                        currentTile.GetComponent<TileScript>().SetBorderSprite(cornerSprites[southEast], 90);
                    }
            }
            if (west > -1)
            {
                int southWest = usedValues[tileNum + columnCount - 1];
                if (usedValues[tileNum] != southWest && southWest != west && southWest != south)
                {
                    currentTile.GetComponent<TileScript>().SetBorderSprite(cornerSprites[southWest], 0);
                }
            }
        }

        // Checking north corners
        if (north > -1)
        {
            if (east > -1)
            {
                int northEast = usedValues[tileNum - columnCount + 1];
                if (usedValues[tileNum] != northEast && northEast != east && northEast != north)
                {
                    currentTile.GetComponent<TileScript>().SetBorderSprite(cornerSprites[northEast], 180);
                }
            }
            if (west > -1)
            {
                int northWest = usedValues[tileNum - columnCount - 1];
                if (usedValues[tileNum] != northWest && northWest != west && northWest != north)
                {
                    currentTile.GetComponent<TileScript>().SetBorderSprite(cornerSprites[northWest], 270);
                }
            }
        }
    }

    public GameObject GetFirePrefab()
    {
        return firePrefab;
    }

    public GameObject GetBorderPrefab()
    {
        return borderPrefab;
    }

    public GameObject GetFireLinePrefab()

    {
        return fireLinePrefab;
    }

    
    public int GetRowCount()
    {
        return rowCount;
    }

    public int GetColumnCount()
    {
        return columnCount;
    }

    public int[] GetEntireGrid()
    {
        return usedValues;
    }

    public int GetWaterColumn()
    {
        return waterColumn;
    }

    public void UpdateTileGrid(int[] valuesArray, int waterC)
    {
        for (int j = 0; j < valuesArray.Length; j++)
        {
            // Check if sand tile island
            GameObject tempTile = Instantiate(
               tilePrefabs[valuesArray[j]],
               emptyTiles[j].transform.position,
               emptyTiles[j].transform.rotation,
               forestPrefab.transform.parent);
            tempTile.transform.localScale = emptyTiles[j].transform.localScale;
            tempTile.name = "Tile (" + j + ")";
            tempTile.tag = "Tile";

            // if water, added land border to it
            if (valuesArray[j] == 5)
            {
                int next = -1;
                if (waterC < columnCount - 1) next = 1;
                // create border for water, child over water 
                if (valuesArray[j + next] != 3)
                {
                    float rotation = 0;
                    if (waterC == 1) rotation = 180f;
                    tempTile.GetComponent<WaterTerrain>().SetBorderSprite
                        (borderSprites[valuesArray[j + next]], rotation);
                }
                else
                {
                    // else if a road tile make the board tile what the border above it is
                    float rotation = 0;
                    if (waterC == 1) rotation = 180f;
                    int tileIndex = j + next - columnCount;
                    if (tileIndex < 0) tileIndex = 3;
                    tempTile.GetComponent<WaterTerrain>().SetBorderSprite
                        (borderSprites[valuesArray[tileIndex]], rotation);
                }
            }
            // If sand or dirt/forest tiles, create needed borders
            if (valuesArray[j] == 0 || valuesArray[j] == 1 || valuesArray[j] == 5)
            {
                SetBorders(j, tempTile);
            }
            // Make sure edge tiles go to edge of actual viewable screen, increase scale slightly
            if (j < columnCount || j > (columnCount * (rowCount - 1)) - 1)
            {
                Vector3 locScale = tempTile.transform.localScale;
                tempTile.transform.localScale = new Vector3(locScale.x, locScale.y * 1.1f, locScale.z);
            }
            Destroy(emptyTiles[j]);
        }
        UpdateAllNeighbors();
    }
}
