using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public float turnRate = 30f;
    [Tooltip("How long bursts of fire last for")]
    public float burstTime = 0.5f;
    [Tooltip("Time between burts of fire")]
    public float burstDelay = 2f;

    public float range = 4f;

    public Transform target;
    
    private float burstTimeLeft = 0f;
    private float burstDelayLeft = 0f;

    private void FixedUpdate()
    {
        var weapon = GetComponentInChildren<IWeapon>();
        if (!weapon.GetFiring())
        {
            if (burstDelayLeft <= 0f && Vector3.Distance(target.position,transform.position) < range)
            {
                var lineOfSightHit = Physics2D.Raycast(transform.position, target.position - transform.position, Vector3.Distance(transform.position, target.position), Utility.aiVisionRaycastMask);
                if (lineOfSightHit.collider && lineOfSightHit.collider.transform.IsChildOf(target))
                {
                    var targetPosition = target.position;
                    if (target.GetComponent<Rigidbody2D>())
                    {
                        // lead target
                        targetPosition += (Vector3)(target.GetComponent<Rigidbody2D>().velocity * (range / weapon.BulletSpeed()));
                    }
                    if (Utility.RotateTowardsTarget(transform, targetPosition, turnRate * Time.fixedDeltaTime))
                    {
                        weapon.SetFiring(true);
                        burstTimeLeft = burstTime;
                    }
                }
            }
        }
        else
        {
            burstTimeLeft -= Time.fixedDeltaTime;
            if(burstTimeLeft <= 0.0f)
            {
                weapon.SetFiring(false);
                burstDelayLeft = burstDelay;
            }
        }
        burstDelayLeft -= Time.fixedDeltaTime;
    }
    
}
