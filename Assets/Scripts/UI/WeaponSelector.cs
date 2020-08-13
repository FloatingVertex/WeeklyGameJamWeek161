using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelector : MonoBehaviour
{
    public string[] options;

    public void SetOptions(List<Dropdown.OptionData> newOptions)
    {
        var optionsStr = new string[newOptions.Count];
        for(int i = 0; i < newOptions.Count; i++)
        {
            optionsStr[i] = newOptions[i].text;
        }
        options = optionsStr;
        GetComponent<UnityEngine.UI.Dropdown>().options = newOptions;
        GetComponent<UnityEngine.UI.Dropdown>().value = 0;
    }

    public string WeaponSelected()
    {
        return options[GetComponent<UnityEngine.UI.Dropdown>().value];
    }
}
