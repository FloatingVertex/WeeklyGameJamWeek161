using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftManager : MonoBehaviour
{
    public GlobalConfigurations config;
    public Transform[] weaponHardpoints;


    public void SetWeapons(string[] weapons)
    {
        for(int i = 0; i < weapons.Length; i++)
        {
            Utility.DeleteAllChildren(weaponHardpoints[i]);
            Instantiate(config.GetWeaponData(weapons[i]), weaponHardpoints[i].position, weaponHardpoints[i].rotation, weaponHardpoints[i]);
        }
    }
}
