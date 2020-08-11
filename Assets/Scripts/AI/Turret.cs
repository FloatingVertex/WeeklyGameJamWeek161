using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AI))]
public class Turret : MonoBehaviour
{
    public float turnRate = 30f;
    [Tooltip("How long bursts of fire last for")]
    public float burstTime = 0.5f;
    [Tooltip("Time between burts of fire")]
    public float burstDelay = 2f;
    
    private float burstTimeLeft = 0f;
    private float burstDelayLeft = 0f;

    private AI ai;

    private void Start()
    {
        ai = GetComponent<AI>();
    }

    private void FixedUpdate()
    {
        var weapons = GetComponentsInChildren<IWeapon>();
        if (!weapons[0].GetFiring())
        {
            var target = ai.GetTargetInLOS()?.transform;
            if (burstDelayLeft <= 0f && target)
            {
                var range = ((Vector2)(target.position - transform.position)).magnitude;
                var targetPosition = target.position;
                if (target.GetComponent<Rigidbody2D>())
                {
                    // lead target
                    targetPosition += (Vector3)(target.GetComponent<Rigidbody2D>().velocity * (range / weapons[0].BulletSpeed()));
                }
                if (Utility.RotateTowardsTarget(transform, targetPosition, turnRate * Time.fixedDeltaTime))
                {
                    foreach (var weapon in weapons)
                    {
                        weapon.SetFiring(true);
                    }
                    burstTimeLeft = burstTime;
                }
            }
        }
        else
        {
            burstTimeLeft -= Time.fixedDeltaTime;
            if(burstTimeLeft <= 0.0f)
            {
                foreach (var weapon in weapons)
                {
                    weapon.SetFiring(false);
                }
                burstDelayLeft = burstDelay;
            }
        }
        burstDelayLeft -= Time.fixedDeltaTime;
    }
    
}
