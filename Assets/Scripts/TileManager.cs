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
    public GameObject[] emptyTiles;
    public GameObject[] tilePrefabs;
    public int columnCount = 18;
    public int rowCount = 10;
    private int[] usedValues;
    //public char[] tileTypes;
    List<int> values = new List<int>();
    string[] terrainTypes = { "Sand", "Forest", "Road", "SideRoad" };

    // Start is called before the first frame update
    void Start()
    {
        emptyTiles = GameObject.FindGameObjectsWithTag("EmptyTile");
        //values = new int[60];
        usedValues = Enumerable.Repeat(0, 181).ToArray();
        int startIndex = Random.Range(0, 179);

        int currentIndex = startIndex;
        values.Add(currentIndex);
        int[] oneEight = { 1, 1 };
        oneEight[Random.Range(0, 2)] += 17;
        int negPos = Random.Range(0, 2) * 2 - 1;
        int count = 0;

        while(values.Count < usedValues.Length/3)
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
            if(count > 500) { Debug.Log("ISSUE"); break; }
            currentIndex = values[Random.Range(0, values.Count)];
        }
        Debug.Log("------------len: " + values.Count);

        // ROAD
        int roadColumn = Random.Range(1, columnCount-1);

        int sideStreet1 = Random.Range(1, rowCount - 1);
        int side1NegPos = Random.Range(0, 2) * 2 - 1;
        int side1Len = Random.Range(columnCount / 4, columnCount / 4 + 2);
        //Debug.Log("r: " + roadColumn + " side1Len: " + side1Len + " n: " + side1NegPos + " res: " + (roadColumn + side1Len * side1NegPos));
        if ((roadColumn + side1Len * side1NegPos) < 1 || 
            ((roadColumn + side1Len * side1NegPos) > columnCount))
        {
            side1NegPos *= -1;
        }
            

        //Debug.Log("--r: " + roadColumn + " side1Len: " + side1Len + " n: " + side1NegPos + " res: " + (roadColumn + side1Len * side1NegPos));

        int sideStreet2 = Random.Range(1, rowCount - 2);
        int side2NegPos = Random.Range(0, 2) * 2 - 1;
        int side2Len = Random.Range(columnCount / 4 - 1, columnCount / 4 + 2);
        if ((roadColumn + side2Len * side2NegPos) < 1 || (roadColumn + side2Len * side2NegPos) > columnCount) side2NegPos *= -1;
        Debug.Log("s2: " + sideStreet2*side2NegPos + " s1: " + sideStreet1*side1NegPos);
        int count2 = 0;
        while (side1NegPos == side2NegPos && CheckSideStreets(sideStreet1, sideStreet2))
        {
            Debug.Log("c--2: " + count2++);
            sideStreet2 = Random.Range(2, rowCount - 2);
        }
        Debug.Log("s2: " + sideStreet2 * side2NegPos + " s1: " + sideStreet1 * side1NegPos);

        for (int i = 0; i < rowCount; i++)
        {
            usedValues[roadColumn + (columnCount * i)] = 2;
            if (i == sideStreet1) 
            {
                for (int j = 1; j < side1Len; j++)
                    usedValues[(roadColumn + (columnCount * i)) + j * side1NegPos] = 3;
            } 
            if (i == sideStreet2)
            {
                for (int j = 1; j < side2Len; j++)
                    usedValues[(roadColumn + (columnCount * i)) + j * side2NegPos] = 3;
            }
        }
        for(int n = 0; n < usedValues.Length; n++)
        {
           // Debug.Log("n-> " + n + " val: " + usedValues[n]);
        }

        
        // Instatiate all tile objects forest, road, sand
        for (int j = 0; j < usedValues.Length; j++)
        {
            // Check if sand tile island
            if (usedValues[j] == 0) SandCheck(j);
      //      Debug.Log("j-> " + j + " and: " +  usedValues[j]);
            GameObject tempTile = Instantiate(
               tilePrefabs[usedValues[j]],
               emptyTiles[j].transform.position,
               emptyTiles[j].transform.rotation,
               forestPrefab.transform.parent);
            tempTile.transform.localScale = emptyTiles[j].transform.localScale;
            tempTile.name = "Tile (" + j + ")";//terrainTypes[usedValues[j]] + " (" + j + ")";
            Destroy(emptyTiles[j]);
        }
        




        /*
        GameObject[] fest = GameObject.FindGameObjectsWithTag("Tile");
        Debug.Log("North: " + fest[5].GetComponent<ForestTerrain>().GetNorth().name);
        Debug.Log("South: " + fest[5].GetComponent<ForestTerrain>().GetSouth().name);
        Debug.Log("East: " + fest[5].GetComponent<ForestTerrain>().GetEast().name);
        Debug.Log("West: " + fest[5].GetComponent<ForestTerrain>().GetWest().name);
        */
        // Clear empty array and repopulate with tiles remaining
        System.Array.Clear(emptyTiles, 0, emptyTiles.Length);
        emptyTiles = GameObject.FindGameObjectsWithTag("EmptyTile");
    }

    private void GenerateRoads()
    {
        int negPos = Random.Range(0, 1);
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
        bool one = (tileNum + 1 < 179 && usedValues[tileNum + 1] == 0);
        bool two = (tileNum - 1 > 0 && usedValues[tileNum - 1] == 0);
        bool three = tileNum + columnCount < 179 && usedValues[tileNum + columnCount] == 0;
        bool four = tileNum - columnCount > 0 && usedValues[tileNum - columnCount] == 0;
        char z = ' ';
        if (!(one || two || three || four)) z = 'z';
        Debug.Log(z + " t: " + tileNum + " 1: " + one + " 2: " + two + " 3: " + three + " 4: " + four);
        if (tileNum + 1 < 179 && usedValues[tileNum + 1] == 0 ||
            tileNum - 1 > 0 && usedValues[tileNum - 1] == 0 ||
             tileNum + columnCount < 179 && usedValues[tileNum + columnCount] == 0 ||
             tileNum - columnCount > 0 && usedValues[tileNum - columnCount] == 0)
            return;
        usedValues[tileNum] = 1;
    }
}
