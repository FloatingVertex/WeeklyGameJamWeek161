using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnappingBackground : MonoBehaviour
{
    public Transform target;
    public float snapToNearest = 0.5f;
    public Vector2 offset;

    void Update()
    {
        var t = target.position / snapToNearest;
        transform.position = new Vector3(Mathf.RoundToInt(t.x) * snapToNearest + offset.x, Mathf.RoundToInt(t.y) * snapToNearest + offset.y, transform.position.z);
    }
}
