using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigureShipGUI : MonoBehaviour
{
    public WeaponSelector weapon1;
    public WeaponSelector weapon2;

    public void SetWeaponOptions(string[] options)
    {
        foreach(var selector in new WeaponSelector[]{ weapon1,weapon2 })
        {
            selector.SetOptions(options);
        }
    }
    
    public AircraftConfiguration getConfig()
    {
        var config = new AircraftConfiguration();
        config.weapons = new string[2];
        config.weapons[0] = weapon1.WeaponSelected();
        config.weapons[1] = weapon2.WeaponSelected();
        return config;
    }

}


