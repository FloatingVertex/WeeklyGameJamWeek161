using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Config", menuName = "Configuration/GlobalConfigs", order = 1)]
public class GlobalConfigurations : ScriptableObject
{
    public WeaponData[] weapons;

    public PrefabPointers prefabs;

    public List<string> levels = new List<string>();

    public string mainMenuLevelName = "MainMenu";

    public GameObject GetWeaponPrefab(string name)
    {
        return GetWeaponData(name).prefab;
    }

    public WeaponData GetWeaponData(string name)
    {
        foreach (var weapon in weapons)
        {
            if (weapon.prefab.name == name)
            {
                return weapon;
            }
        }
        throw new System.Exception("Failed to find weapon with name: " + name);
    }

    public void LoadNextLevel(string currentLevel)
    {
        var currentIndex = levels.IndexOf(currentLevel);
        if(currentIndex < 0)
        {
            Debug.LogError("Failed to find current level in list");
            return;
        }
        if(currentIndex < levels.Count)
        {
            SceneManager.LoadScene(levels[currentIndex + 1], LoadSceneMode.Single);
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuLevelName);
    }
}

[System.Serializable]
public struct WeaponData
{
    public GameObject prefab;
    public Sprite weaponSelectionSprite;
}

[System.Serializable]
public struct PrefabPointers
{
    public GameObject aircraftConfigureUI;
    public GameObject spawnButton;
    public GameObject gameUI;
    public GameObject pauseMenuUI;
    public GameObject levelFinishedScreen;
}
