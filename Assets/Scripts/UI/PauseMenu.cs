using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;

    public void Back()
    {
        pauseMenu.SetActive(false);
    }

    public void Exit()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
