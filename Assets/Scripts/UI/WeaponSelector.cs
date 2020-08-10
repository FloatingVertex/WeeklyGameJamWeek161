using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelector : MonoBehaviour
{
    public string[] options;

    public string WeaponSelected()
    {
        return options[GetComponent<UnityEngine.UI.Dropdown>().value];
    }
}
