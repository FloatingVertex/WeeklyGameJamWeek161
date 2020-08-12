using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelector : MonoBehaviour
{
    public string[] options;

    public void SetOptions(string[] newOptions)
    {
        var optionsData = new List<Dropdown.OptionData>();
        foreach(var option in newOptions)
        {
            optionsData.Add(new Dropdown.OptionData(option));
        }
        options = newOptions;
        GetComponent<UnityEngine.UI.Dropdown>().options = optionsData;
        GetComponent<UnityEngine.UI.Dropdown>().value = 0;
    }

    public string WeaponSelected()
    {
        return options[GetComponent<UnityEngine.UI.Dropdown>().value];
    }
}
