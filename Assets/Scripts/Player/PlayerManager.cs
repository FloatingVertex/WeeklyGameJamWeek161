using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public GlobalConfigurations config;
    public GameObject aircraftPrefab;
    
    private AircraftConfiguration aircraftConfig;

    private void SpawnConfigruationUI()
    {
        var canvas = GetComponentInChildren<Canvas>();
        var configuration = Instantiate(config.prefabs.aircraftConfigureUI, canvas.transform);
        var spawnButton = Instantiate(config.prefabs.spawnButton, canvas.transform);
        spawnButton.GetComponent<Button>().onClick.AddListener(()=> {
            ClearUI();
            SpawnAircraft(configuration.GetComponent<ConfigureShipGUI>().getConfig());
        });
    }

    private void ClearUI()
    {
        Utility.DeleteAllChildren(GetComponentInChildren<Canvas>().transform);
    }

    void SpawnAircraft(AircraftConfiguration config)
    {
        var aircraft = Instantiate(aircraftPrefab, transform.position, transform.rotation, transform);
        SetConfiguration(config);
        if (aircraftConfig.weapons != null)
        {
            aircraft.GetComponent<AircraftManager>().SetWeapons(aircraftConfig.weapons);
        }
        if (GetComponentInChildren<FollowCam>())
        {
            GetComponentInChildren<FollowCam>().target = aircraft.transform;
        }
        if (TerrainManager.singleton)
        {
            TerrainManager.singleton.playModeTransformToFollow = aircraft.transform;
        }

        // spawn ui
        var canvas = GetComponentInChildren<Canvas>();
        var gameUI = Instantiate(this.config.prefabs.gameUI, canvas.transform);
        gameUI.GetComponent<GameUI>().manager = aircraft.GetComponent<AircraftManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnConfigruationUI();
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
