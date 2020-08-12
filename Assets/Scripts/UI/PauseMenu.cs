using ICSharpCode.NRefactory.Ast;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public static bool isPaused = false;

    private void Update()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Paused();   
        }

       

        
        
    }

    public void Paused()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
    }

    public void Exit()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
