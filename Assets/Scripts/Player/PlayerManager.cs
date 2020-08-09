using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public GameObject aircraftPrefab;

    // Start is called before the first frame update
    void Start()
    {
        var aircraft = Instantiate(aircraftPrefab, transform.position, transform.rotation, transform);
        if (GetComponentInChildren<FollowCam>())
        {
            GetComponentInChildren<FollowCam>().target = aircraft.transform;
        }
        TerrainManager.singleton.playModeTransformToFollow = aircraft.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetWeapon1(string name)
    {

    }
}

public struct AircraftConfiguration
{

}

public enum AircraftType
{
    Normal,
}

public enum AircraftWeapon
{
    MachineGun,
    RailGun,
}

public enum Raycaster
{

}
