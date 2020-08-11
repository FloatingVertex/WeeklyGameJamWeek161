using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public static class ShadowCaster
{
    public static void CreateShadowCaster(int pathIndex, Transform parent)
    {
        var go = new GameObject(pathIndex.ToString());
        go.transform.parent = parent;
        go.transform.position = parent.position;
        go.transform.rotation = parent.rotation;
        go.AddComponent<ShadowCaster2D>();
    }
}

