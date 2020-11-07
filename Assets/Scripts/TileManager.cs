using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TileManager : MonoBehaviour
{
    public GameObject forestPrefab;
    public GameObject sandPrefab;
    public Transform tileLocation1;
    public GameObject[] emptyTiles;
    public int columnCount = 18;
    public int rowCount = 10;
  //  public int[] values;
    public int[] usedValues;
    List<int> values = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        emptyTiles = GameObject.FindGameObjectsWithTag("EmptyTile");
        //values = new int[60];
        usedValues = Enumerable.Range(0, 181).ToArray();
        int startIndex = Random.Range(0, 179);

        int currentIndex = startIndex;
        values.Add(currentIndex);
        int[] oneEight = { 1, 1 };
        oneEight[Random.Range(0, 2)] += 17;
        int negPos = Random.Range(0, 1) * 2 - 1;
        int count = 0;
        while(values.Count < 59)
        {
          //  Debug.Log("--------------");
         //   Debug.Log(currentIndex + oneEight[0] * negPos);
            //values[i++] = currentIndex;
            if (ValidIndex(currentIndex, (currentIndex + oneEight[0] * negPos)))
            {
                values.Add(currentIndex + oneEight[0] * negPos);
            }
                
            if (ValidIndex(currentIndex, (currentIndex - oneEight[0] * negPos)))
            {
                values.Add(currentIndex - oneEight[0] * negPos);
            }
                
            if (ValidIndex(currentIndex, (currentIndex + oneEight[1] * negPos)))
            {
                values.Add(currentIndex + oneEight[1] * negPos);
            }
                
            if (ValidIndex(currentIndex, (currentIndex - oneEight[1] * negPos)))
            {
                values.Add(currentIndex - oneEight[1] * negPos);
            }
            count++;
            if(count > 500) { Debug.Log("ISSUE"); break; }
            currentIndex = values[Random.Range(0, values.Count)];

        }
        Debug.Log("------------len: " + values.Count);
        for (int j = 0; j < values.Count; j++)
        {
            GameObject go = Instantiate(
               forestPrefab,
               emptyTiles[values[j]].transform.position,
               emptyTiles[values[j]].transform.rotation,
               forestPrefab.transform.parent);
            go.transform.localScale = emptyTiles[values[j]].transform.localScale;
            go.name = "Forest " + values[j];
            Destroy(emptyTiles[values[j]]);
        }

        // Clear empty array and repopulate with tiles remaining
        System.Array.Clear(emptyTiles, 0, emptyTiles.Length);
        emptyTiles = GameObject.FindGameObjectsWithTag("EmptyTile");
        /*
        while (prefabCount < emptyTiles.Length / 3)
        {
            List<int> change = new List<int> { 1, -1, columnCount, -columnCount };
            int changeIndex = Random.Range(0, change.Count);
            int nextIndex = currentIndex + change[changeIndex];
            //Debug.Log("cur: " + currentIndex + " nextIndex: " + nextIndex + " changeIndex: " + changeIndex);
            while (nextIndex < 0 || nextIndex > emptyTiles.Length - 1 || values[nextIndex] == -1 || nextIndex%startIndex > 9 ||
                Mathf.Abs(change[changeIndex]) == 1 && nextIndex / columnCount != currentIndex / columnCount)
            {
                change.RemoveAt(changeIndex);
                if (change.Count < 1)
                {
                    currentIndex = Random.Range(0, 179);
                    while (values[currentIndex] != -1) currentIndex = Random.Range(0, 179);
                    change = new List<int> { 1, -1, columnCount, -columnCount };
                }
                changeIndex = Random.Range(0, change.Count);
                nextIndex = currentIndex + change[changeIndex];
                bug++;
                if (bug > 500)
                {
                    //Debug.Log("BREAKKKKKK");
                    break;
                }
                stuff += nextIndex + ", ";
            }
            //Debug.Log("stuff: " + stuff);
            currentIndex = nextIndex;
            prefabCount++;
            GameObject go = Instantiate(
                forestPrefab,
                emptyTiles[currentIndex].transform.position,
                emptyTiles[currentIndex].transform.rotation,
                forestPrefab.transform.parent);
            go.transform.localScale = emptyTiles[currentIndex].transform.localScale;
            go.name = "Forest " + currentIndex;
            Destroy(emptyTiles[currentIndex]);
            values[currentIndex] = -1;

            bug++;
            if (bug > 1000) { Debug.Log("bugggggg"); break; }
        }
        */
    }

    private void GenerateRoads()
    {
        int negPos = Random.Range(0, 1);
    }

    private bool ValidIndex(int currentIndex, int newIndex)
    {
        /*
        bool lessMore = newIndex < 0 || newIndex > 179;
        bool indof = values.IndexOf(newIndex) != -1;
        bool absol = Mathf.Abs(currentIndex - newIndex) == 1;
        bool samerow = newIndex / columnCount != currentIndex / columnCount;
        Debug.Log("c: " + currentIndex + " n: " + newIndex + " l: " + lessMore + " indof: " + indof +
            " absol: " + absol + " same: " + samerow);
        */
        bool temp = true;
        if( newIndex < 0 || newIndex > 179 ||
            values.IndexOf(newIndex) != -1 ||
            Mathf.Abs(currentIndex - newIndex) == 1 &&
                newIndex / columnCount != currentIndex / columnCount
            ) temp = false;
      //  Debug.Log("temp: " + temp);
        return temp;
    }
}
