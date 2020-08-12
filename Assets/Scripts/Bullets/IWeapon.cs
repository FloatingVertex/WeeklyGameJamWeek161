using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    float BulletSpeed();
    void SetFiring(bool firing);
    bool GetFiring();

    float ReloadStatus();
}
