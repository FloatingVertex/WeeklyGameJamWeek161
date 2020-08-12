using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AI))]
[RequireComponent(typeof(Rigidbody2D))]
public class Navigator : MonoBehaviour
{
    public float waypointTriggerDistance = 0.3f;
    public float speed = 1f;
    private Vector2[] path;
    private Vector2 pathTarget;
    private int currentWaypointIndex;
    public float rotationRate = 0f;
    public Vector2 target;
    public bool active;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        if (path != null && path.Length > 1) {
            for (int i = 1; i < path.Length; i++)
            {
                Gizmos.DrawLine(path[i-1], path[i]);
            }
        }
    }

    void FixedUpdate()
    {
        if (active)
        {
            if (path == null || (target - pathTarget).magnitude / (target - (Vector2)transform.position).magnitude > 0.1f)
            {
                // Need to recalculate path
                path = GridAStar.singleton.CalculatePath(transform.position, target);
                if (path == null)
                {
                    path = new Vector2[0];
                }
                pathTarget = target;
                currentWaypointIndex = 0;
            }
            if (path.Length > currentWaypointIndex && (path[currentWaypointIndex] - (Vector2)transform.position).magnitude < waypointTriggerDistance)
            {
                currentWaypointIndex++;
            }
            if (path.Length > currentWaypointIndex)
            {
                // move towards next waypoint
                var velocity = (path[currentWaypointIndex] - (Vector2)transform.position) * speed;
                GetComponent<Rigidbody2D>().velocity = velocity;
                Utility.RotateTowardsTarget(transform,(Vector2)transform.position + velocity, rotationRate * Time.fixedDeltaTime);
            }
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }
}
