using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FirstSceneScript : MonoBehaviour
{
    public Button easyBtn;
    public Button mediumBtn;
    public Button hardBtn;

    void Start()
    {;
        easyBtn.onClick.AddListener(() => EasyClicked());
        mediumBtn.onClick.AddListener(() => MediumClicked());
        hardBtn.onClick.AddListener(() => HardClicked());
    }

    void EasyClicked()
    {
        ShowFire();
        Invoke("EasyScene", 1.0f);
    }

    void MediumClicked()
    {
        ShowFire();
        Invoke("MediumScene", 1.0f);
    }

    void HardClicked()
    {
        ShowFire();
        Invoke("HardScene", 1.0f);
    }

    void ShowFire()
    {
        GameObject.Find("BigFire1").GetComponent<SpriteRenderer>().enabled = true;
        GameObject.Find("BigFire2").GetComponent<SpriteRenderer>().enabled = true;
        GameObject.Find("BigFire3").GetComponent<SpriteRenderer>().enabled = true;
        GameObject.Find("BigFire3").GetComponent<SpriteRenderer>().enabled = true;
    }


    void EasyScene()
    {
        SceneManager.LoadScene(1);
    }

    void MediumScene()
    {
        SceneManager.LoadScene(2);
    }

    void HardScene()
    {
        SceneManager.LoadScene(3);
    }
}
