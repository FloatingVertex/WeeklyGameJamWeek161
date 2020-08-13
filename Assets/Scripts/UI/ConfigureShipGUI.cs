using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigureShipGUI : MonoBehaviour
{
    public GlobalConfigurations config;
    public WeaponSelector weapon1;
    public WeaponSelector weapon2;

    public void SetWeaponOptions(string[] options)
    {
        var optionsData = new List<Dropdown.OptionData>();
        foreach (var option in options)
        {
            optionsData.Add(new Dropdown.OptionData(option,config.GetWeaponData(option).weaponSelectionSprite));
        }
        foreach (var selector in new WeaponSelector[]{ weapon1,weapon2 })
        {
            selector.SetOptions(optionsData);
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


