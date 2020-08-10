using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public GlobalConfigurations config;
    public GameObject aircraftPrefab;

    private AircraftConfiguration aircraftConfig;

    // Start is called before the first frame update
    void Start()
    {
        var aircraft = Instantiate(aircraftPrefab, transform.position, transform.rotation, transform);
        if (aircraftConfig.weapons != null)
        {
            aircraft.GetComponent<AircraftManager>().SetWeapons(aircraftConfig.weapons);
        }
        if (GetComponentInChildren<FollowCam>())
        {
            GetComponentInChildren<FollowCam>().target = aircraft.transform;
        }
        TerrainManager.singleton.playModeTransformToFollow = aircraft.transform;
    }

    public void SetConfiguration(AircraftConfiguration aircraftConfig)
    {
        if (GetComponentInChildren<AircraftManager>())
        {
            GetComponentInChildren<AircraftManager>().SetWeapons(aircraftConfig.weapons);
        }
        this.aircraftConfig = aircraftConfig;
    }
}

[System.Serializable]
public struct AircraftConfiguration
{
    public string[] weapons;
}
