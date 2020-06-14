using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMnu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void GetHelp()
    {
        SceneManager.LoadScene("RenderTexture");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu 3D");
    }

    public void goToSite()
    {
        Application.OpenURL("http://www.robotstudiosimulator.com/");
    }

    public void goToDoc()
    {
        Application.OpenURL("https://eashan-vytla.gitbook.io/debuggerappmaster/");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
