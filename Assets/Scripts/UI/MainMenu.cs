using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject creditsPanel;
    
    private void Start()
    {
        Scores.UploadStart(Scores.username, "MainMenu");
    }

    private void Update()
    {
        
    }

    public void StartButton()
    {
        SceneManager.LoadScene("Level1");
        Debug.Log("Loading next level");
    }

    public void ExitButton()
    {
        Application.Quit();
        Debug.Log("Exit");
    }

    public void CreditsButton()
    {
        creditsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        Debug.Log("Credits working");
    }
    public void BackButton()
    {
        creditsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
