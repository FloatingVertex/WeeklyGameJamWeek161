using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static Transform playerShip;

    public static LayerMask aiVisionRaycastMask = LayerMask.GetMask("Default");

    // Rotates transform to point towards target (x axis treated as forward)
    // returns if transform points at target at the end of it
    public static bool RotateTowardsTarget(Transform transform, Vector2 target, float maxAngle = 180f)
    {
        float angle = Mathf.Atan2(target.y - transform.position.y, target.x - transform.position.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxAngle);
        return transform.rotation == targetRotation;
    }

    public static void DeleteAllChildren(Transform transform)
    {
        var toDelete = new List<Transform>();
        foreach(Transform child in transform)
        {
            toDelete.Add(child);
        }
        foreach(var child in toDelete)
        {
            if (Application.isPlaying)
            {
                GameObject.Destroy(child.gameObject);
            }
            else
            {
                GameObject.DestroyImmediate(child.gameObject);
            }
        }
    }
}
