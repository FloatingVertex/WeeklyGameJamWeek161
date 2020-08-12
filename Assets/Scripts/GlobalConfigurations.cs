using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Config", menuName = "Configuration/GlobalConfigs", order = 1)]
public class GlobalConfigurations : ScriptableObject
{
    public WeaponData[] weapons;

    public PrefabPointers prefabs;

    public GameObject GetWeaponData(string name)
    {
        foreach(var weapon in weapons)
        {
            if(weapon.prefab.name == name)
            {
                return weapon.prefab;
            }
        }
        throw new System.Exception("Failed to find weapon with name: "+name);
    }
}

[System.Serializable]
public struct WeaponData
{
    public GameObject prefab;
}

[System.Serializable]
public struct PrefabPointers
{
    public GameObject aircraftConfigureUI;
    public GameObject spawnButton;
    public GameObject gameUI;
}
