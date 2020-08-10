using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public GlobalConfigurations config;
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

    public void SetConfiguration(AircraftConfiguration aircraftConfig)
    {
        GetComponent<AircraftManager>().SetWeapons(aircraftConfig.weapons);
    }
}

public struct AircraftConfiguration
{
    public string[] weapons;
}
