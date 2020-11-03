using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class StartScreenScript : MonoBehaviour
{
    public Button startButton;
    public GameObject[] fires;

    void Start()
    {
        startButton.onClick.AddListener(() => StartClicked());
    }

    void StartClicked()
    {
        fires[0].GetComponent<SpriteRenderer>().enabled = true;
        fires[1].GetComponent<SpriteRenderer>().enabled = true;
        fires[2].GetComponent<SpriteRenderer>().enabled = true;
        Invoke("NextScene", 1.0f);
    }

    void NextScene()
    {
        SceneManager.LoadScene(1);
    }
}
