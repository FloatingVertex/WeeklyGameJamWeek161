using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    public GameObject panel;
    public GlobalConfigurations config;

    public void Retry()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void CloseGame()
    {
        Application.Quit();
        Debug.Log("Game is closing");
    }

    public void BackToMenu()
    {
        config.LoadMainMenu();
    }


    public void OpenPanel()
    {
        Animator animator = GetComponent<Animator>();

        if(panel != null)
        {
            bool isOpen = animator.GetBool("Open");

            animator.SetBool("Open", isOpen);
        }
    }
}
