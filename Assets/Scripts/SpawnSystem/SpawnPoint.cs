using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public AircraftConfiguration defaultConfig;

    public void SpawnPlayerPrefab(GameObject prefab)
    {
        var newPlayerPrefab = Instantiate(prefab, transform.position, transform.rotation);
        newPlayerPrefab.GetComponent<PlayerManager>().SetConfiguration(defaultConfig);
    }
}