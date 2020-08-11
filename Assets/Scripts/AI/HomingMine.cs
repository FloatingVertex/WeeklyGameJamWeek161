using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AI))]
[RequireComponent(typeof(IDamageable))]
[RequireComponent(typeof(Navigator))]
public class HomingMine : MonoBehaviour
{
    public float detonationRange = 2f;

    void FixedUpdate()
    {
        if (GetComponent<AI>().GetSpottedTarget())
        {
            var target = (Vector2)GetComponent<AI>().GetSpottedTarget().transform.position;
            GetComponent<Navigator>().target = target;
            GetComponent<Navigator>().active = true;
            if ((target - (Vector2)transform.position).magnitude < detonationRange)
            {
                GetComponent<IDamageable>().Damage(1000f, 0f, DamageType.Other, transform.position, Vector2.zero, Vector2.zero);
            }
        }
        else
        {

            GetComponent<Navigator>().active = false;
        }
    }
}
