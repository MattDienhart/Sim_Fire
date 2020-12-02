using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class StartScreenScript : MonoBehaviour
{
    public Button startBtn;

    void Start()
    {
        //DontDestroyOnLoad(GameObject.Find("TileManager"));
        startBtn = GameObject.Find("StartBtn").GetComponent<Button>();
        startBtn.onClick.AddListener(() => StartClicked());
    }

    void StartClicked()
    {
        GameObject.Find("BigFire1").GetComponent<SpriteRenderer>().enabled = true;
        GameObject.Find("BigFire2").GetComponent<SpriteRenderer>().enabled = true;
        GameObject.Find("BigFire3").GetComponent<SpriteRenderer>().enabled = true;
        GameObject.Find("BigFire3").GetComponent<SpriteRenderer>().enabled = true;
        Invoke("NextScene", 1.0f);
    }

    void NextScene()
    {
        SceneManager.LoadScene(4);
    }
}
