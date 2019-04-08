using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void GotoMainScene()
    {
        SceneManager.LoadScene("Game");
    }

    public void GotoMenuScene()
    {
        SceneManager.LoadScene("Menu");
    }
    public void DoQuit()
    {
        Debug.Log("Has Quit");
        Application.Quit();
    }
}
