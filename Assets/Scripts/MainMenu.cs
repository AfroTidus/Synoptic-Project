using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Menu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Lvl1()
    {
        SceneManager.LoadScene("Level1");
    }

    public void Lvl2()
    {
        SceneManager.LoadScene("Level2");
    }

    public void Test()
    {
        SceneManager.LoadScene("Testing");
    }

    public void Explain()
    {
        SceneManager.LoadScene("Controls");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
