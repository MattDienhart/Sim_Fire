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
        Invoke("NextScene", 1.0f);
    }

    void NextScene()
    {
        SceneManager.LoadScene(1);
    }
}
