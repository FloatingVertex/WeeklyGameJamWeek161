using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public GlobalConfigurations config;

    public void LoadMainMenu()
    {
        config.LoadMainMenu();
    }

    public void ReloadCurrent()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name,LoadSceneMode.Single);
    }

    public void LoadNextLevel()
    {
        config.LoadNextLevel(SceneManager.GetActiveScene().name);
    }

    public void LoadLevel(int index)
    {
        SceneManager.LoadScene(config.levels[index]);
    }
}
