using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public void SpawnPlayerPrefab(GameObject prefab)
    {
        var newObj = Instantiate(prefab, transform.position, transform.rotation);
    }

}