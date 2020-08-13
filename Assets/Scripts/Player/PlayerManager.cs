using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public GlobalConfigurations config;
    public GameObject aircraftPrefab;
    
    private AircraftConfiguration aircraftConfig;

    private void SpawnConfigruationUI()
    {
        var canvas = GetComponentInChildren<Canvas>();
        var configuration = Instantiate(config.prefabs.aircraftConfigureUI, canvas.transform);
        configuration.GetComponent<ConfigureShipGUI>().SetWeaponOptions(LevelManager.singleton.availableWeapons);
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

        SpawnGameUI();
    }

    public void SpawnGameUI()
    {
        ClearUI();
        var aircraft = GetComponentInChildren<AircraftManager>();
        var canvas = GetComponentInChildren<Canvas>();
        var gameUI = Instantiate(this.config.prefabs.gameUI, canvas.transform);
        gameUI.GetComponent<GameUI>().manager = aircraft;
    }

    public void Pause()
    {
        if (GetComponentInChildren<PauseMenu>() == null)
        {
            ClearUI();
            var canvas = GetComponentInChildren<Canvas>();
            Instantiate(config.prefabs.pauseMenuUI, canvas.transform);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnConfigruationUI();
    }

    private void Update()
    {
        if(Keyboard.current.escapeKey.isPressed)
        {
            Pause();
        }
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
