using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public GameObject[] aircraft;
    public GameObject[] weapons;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnAircraftPrefab(GameObject prefab)
    {
        var newObj = Instantiate(prefab,transform.position,transform.rotation);
    }
}
