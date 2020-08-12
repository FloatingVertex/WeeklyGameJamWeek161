using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    public GameObject panel;

    private void Start()
    {
        gameObject.SetActive(false);
    }

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
        SceneManager.LoadScene("MainMenu");
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
