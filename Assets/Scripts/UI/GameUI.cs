using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public AircraftManager manager;
    public Slider healthSlider;
    public Slider[] weaponSliders;
    public Text objectCountLebel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!manager)
        {
            manager = Utility.playerShip?.GetComponent<AircraftManager>();
        }
        if(manager)
        {
            healthSlider.value = manager.GetComponent<Destructable>().health / manager.GetComponent<Destructable>().GetStartingHealth();
            var weaponStatus = manager.GetComponentInChildren<AircraftManager>().GetWeaponReload();
            for(int i =0; i < weaponSliders.Length && i < weaponStatus.Length;i++)
            {
                weaponSliders[i].value = weaponStatus[i];
            }
            (int done, int total) = LevelManager.singleton.GetObjectivesInfo();
            objectCountLebel.text = done + "/" + total+" "+LevelManager.singleton.levelObjectiveString;
        }
    }
}
