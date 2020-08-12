using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AircraftManager : MonoBehaviour
{
    public GlobalConfigurations config;
    public Transform[] weaponHardpoints;

    private IWeapon[] weapons;

    private void Start()
    {
    }

    public void SetWeapons(string[] weapons)
    {
        this.weapons = new IWeapon[weapons.Length];
        for (int i = 0; i < weapons.Length && i < weaponHardpoints.Length; i++)
        {
            Utility.DeleteAllChildren(weaponHardpoints[i]);
            this.weapons[i] = Instantiate(config.GetWeaponData(weapons[i]), weaponHardpoints[i].position, weaponHardpoints[i].rotation, weaponHardpoints[i]).GetComponent<IWeapon>();
        }
    }

    public float[] GetWeaponReload()
    {
        var val = new float[weapons.Length];
        for(int i = 0; i < weapons.Length;i++)
        {
            val[i] = weapons[i].ReloadStatus();
        }
        return val;
    }

    private void SetFireing(int weaponIndex, bool setFiring)
    {
        if (weapons != null && weapons.Length > weaponIndex && weapons[weaponIndex] != null)
        {
            weapons[weaponIndex].SetFiring(setFiring);
        }
    }

    public void Fire1(InputAction.CallbackContext context)
    {
        SetFireing(0, !context.canceled);
    }

    public void Fire2(InputAction.CallbackContext context)
    {
        SetFireing(1, !context.canceled);
    }
}
