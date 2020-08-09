using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    private void Update()
    {
        IWeapon[] weapon = GetComponentsInChildren<IWeapon>();

        if(Input.GetMouseButton(0))
        {
            foreach(IWeapon weapon1 in weapon)
            {
                weapon1.SetFiring(true);
            }
        }

        if(Input.GetMouseButtonUp(0))
        {
            foreach (IWeapon weapon1 in weapon)
            {
                weapon1.SetFiring(false);
            }
        }
    }
}
