using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{
    private void Update()
    {
        IWeapon[] weapon = GetComponentsInChildren<IWeapon>();
        foreach(IWeapon weapon1 in weapon)
        {
            weapon1.SetFiring(true);
        }
    }
}
