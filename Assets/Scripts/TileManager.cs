using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class TileManager : MonoBehaviour
{
    public GameObject forestPrefab;
    public GameObject sandPrefab;
    public Transform tileLocation1;
    private GameObject[] emptyTiles;
    public GameObject[] tilePrefabs;
    public int columnCount = 18;
    public int rowCount = 10;
    private int[] usedValues;
    public GameObject BorderPrefab;
    
    List<int> values = new List<int>();
    string[] terrainTypes = { "Sand", "Forest", "Road", "SideRoad" };

    public Sprite[] borderSprites;

    // Start is called before the first frame update
    void Start()
    {
        emptyTiles = GameObject.FindGameObjectsWithTag("EmptyTile");
        //values = new int[60];
        usedValues = Enumerable.Repeat(0, 180).ToArray();
        int startIndex = Random.Range(0, 179);

        int currentIndex = startIndex;
        values.Add(currentIndex);
        int[] oneEight = { 1, 1 };
        oneEight[Random.Range(0, 2)] += 17;
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
            if (count > 500) { Debug.Log("ISSUE"); break; }
            currentIndex = values[Random.Range(0, values.Count)];
        }
        // ROAD select random column, if not sand try again
        int roadColumn = Random.Range(1, columnCount-1);
        if(usedValues[roadColumn] != 0) roadColumn = Random.Range(1, columnCount - 1);
        int sideStreet1 = Random.Range(1, rowCount - 1);
        int side1NegPos = Random.Range(0, 2) * 2 - 1;
        int side1Len = Random.Range(columnCount / 4, columnCount / 4 + 2);
        //Debug.Log("r: " + roadColumn + " side1Len: " + side1Len + " n: " + side1NegPos + " res: " + (roadColumn + side1Len * side1NegPos));
        if ((roadColumn + side1Len * side1NegPos) < 1 || 
            ((roadColumn + side1Len * side1NegPos) > columnCount))
        {
            side1NegPos *= -1;
        }        

        int sideStreet2 = Random.Range(1, rowCount - 2);
        int side2NegPos = Random.Range(0, 2) * 2 - 1;
        int side2Len = Random.Range(columnCount / 4 - 1, columnCount / 4 + 2);
        if ((roadColumn + side2Len * side2NegPos) < 1 || (roadColumn + side2Len * side2NegPos) > columnCount) side2NegPos *= -1;

        while (side1NegPos == side2NegPos && CheckSideStreets(sideStreet1, sideStreet2))
        {
           // Debug.Log("c--2: " + count2++);
            sideStreet2 = Random.Range(2, rowCount - 2);
        }
    //    Debug.Log("s2: " + sideStreet2 * side2NegPos + " s1: " + sideStreet1 * side1NegPos);

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
            if (i % 18 == 0) mygrid += "\r\n" + (i / 18 + 1) + ":  ";
            mygrid += usedValues[i];
        }
        Debug.Log("-grid: \n" + mygrid);
        // Choose the water column
        int waterColumn = 1 + (columnCount - 1) * Random.Range(0, 2);
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
         //  Debug.Log("j-> " + j + " and: " +  usedValues[j]);
            GameObject tempTile = Instantiate(
               tilePrefabs[usedValues[j]],
               emptyTiles[j].transform.position,
               emptyTiles[j].transform.rotation,
               forestPrefab.transform.parent);
            tempTile.transform.localScale = emptyTiles[j].transform.localScale;
            tempTile.name = "Tile (" + j + ")";
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
                   //Debug.Log("border count: " + borderSprites + "  uv: " + (j + next - columnCount));
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
            else if(usedValues[j] == 0)
            {
                SetBorders(j, tempTile);
            }
            //terrainTypes[usedValues[j]] + " (" + j + ")";
            Destroy(emptyTiles[j]);
        }
        // Clear empty array and repopulate with tiles remaining
        System.Array.Clear(emptyTiles, 0, emptyTiles.Length);
        emptyTiles = GameObject.FindGameObjectsWithTag("EmptyTile");
    }

    private bool ValidIndex(int currentIndex, int newIndex)
    {
        bool temp = true;
        if( newIndex < 0 || newIndex > 179 ||
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
        int east = tileNum+1 < 179 && (tileNum+1)/columnCount 
            == tileNum/columnCount ? usedValues[tileNum + 1] : -1;
        int west = tileNum - 1 > 0 && tileNum / columnCount == (tileNum - 1) / columnCount
            ? usedValues[tileNum - 1] : -1;
        int north = tileNum + columnCount < 179 ? usedValues[tileNum + columnCount] : -1;
        int south = tileNum - columnCount > 0 ? usedValues[tileNum - columnCount] : 0;
        // if surrounded by sand tiles do nothing
        if (east == 0 || west == 0 || north == 0 || south == 0)
        {
            return;
        } else if (east == 1 || west == 1 || north == 1 || south == 1)
        {
            // if surrounded by forest convert to forest tile, index 1
            usedValues[tileNum] = 1;
        }
    }

    private void SetBorders(int tileNum, GameObject currentTile)
    {
        int east = tileNum + 1 < 179 && (tileNum + 1) / columnCount
            == tileNum / columnCount ? usedValues[tileNum + 1] : -1;
        int west = tileNum - 1 > 0 && tileNum / columnCount == (tileNum - 1) / columnCount
            ? usedValues[tileNum - 1] : -1;
        int north = tileNum + columnCount < 179 ? usedValues[tileNum + columnCount] : -1;
        int south = tileNum - columnCount > 0 ? usedValues[tileNum - columnCount] : 0;
        // if surrounded by sand tiles do nothing
        if (east != usedValues[tileNum] && east != -1)
        {
            Debug.Log("east");
            currentTile.GetComponent<TileScript>().SetBorderSprite(borderSprites[east], 180);
        }
        if (west != usedValues[tileNum] && west != -1)
        {
            Debug.Log("west");
            currentTile.GetComponent<TileScript>().SetBorderSprite(borderSprites[west], 0);
        }
        if (north != usedValues[tileNum] && north != -1)
        {
            currentTile.GetComponent<TileScript>().SetBorderSprite(borderSprites[north], 90);
        }
        if (south != usedValues[tileNum] && south != -1)
        {
            Debug.Log("west");
            currentTile.GetComponent<TileScript>().SetBorderSprite(borderSprites[south], -90);
        }
    }

}
