using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfacePlacer : MonoBehaviour
{
    public float maxReach = 10f;
    public GameObject prefab;
    public PlacementAllignTo alignTo;
    public float offsetAlongNormal = 0.1f;
    [Tooltip("Object new spawns will be parented to")]
    public Transform parent;
    public LayerMask mask = ~0;
    public bool placeOnEdge = true;
}

public enum PlacementAllignTo
{
    PointToMouse,
    SurfaceNormal,
    None
}