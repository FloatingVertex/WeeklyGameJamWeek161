using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Navigator))]
public class RangedAI : MonoBehaviour
{
    public float engagementRange = 10f;
    public float ghostFollowTime = 5f;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GetComponent<AI>().GetTargetInLOS())
        {
            var target = (Vector2)GetComponent<AI>().GetTargetInLOS().transform.position;

            if ((target - (Vector2)transform.position).magnitude < engagementRange)
            {
                GetComponent<Navigator>().active = false;
            }
            else
            {
                GetComponent<Navigator>().target = target;
                GetComponent<Navigator>().active = true;
            }
        }
        else if (GetComponent<AI>().GetSpottedTarget())
        {
            var target = (Vector2)GetComponent<AI>().GetSpottedTarget().transform.position;
            GetComponent<Navigator>().target = target;
            GetComponent<Navigator>().active = true;
        }
        else if(Time.time - Actor.timeLastSpoted < ghostFollowTime && (Actor.lastKnownPlayerLocation - (Vector2)transform.position).magnitude < GetComponent<CircleCollider2D>().radius)
        {
            var target = Actor.lastKnownPlayerLocation;
            GetComponent<Navigator>().target = target;
            GetComponent<Navigator>().active = true;
        }
        else
        {
            GetComponent<Navigator>().active = false;
        }
    }
}
