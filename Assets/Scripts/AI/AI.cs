using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class AI : MonoBehaviour
{
    private List<Actor> targets = new List<Actor>();

    void OnTriggerEnter2D(Collider2D other)
    {
        var otherActor = other.GetComponentInParent<Actor>();
        if (otherActor && otherActor.affiliation != GetComponent<Actor>().affiliation)
        {
            targets.Add(otherActor);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var otherActor = other.GetComponentInParent<Actor>();
        if (otherActor)
        {
            targets.Remove(otherActor);
        }
    }

    public Actor GetTargetInLOS()
    {
        for(int i = 0; i < targets.Count; i++)
        {
            var target = targets[i];
            if(target == null)
            {
                i--;
                targets.Remove(target);
                continue;
            }
            var range = ((Vector2)(target.transform.position - transform.position)).magnitude;
            var lineOfSightHit = Physics2D.Raycast(transform.position, target.transform.position - transform.position, Vector3.Distance(transform.position, target.transform.position), Utility.aiVisionRaycastMask);
            if (lineOfSightHit.collider && lineOfSightHit.collider.transform.IsChildOf(target.transform))
            {
                return target;
            }
        }
        return null;
    }

    public Actor GetATarget()
    {
        if(targets.Count == 0)
        {
            return null;
        }
        return targets[0];
    }
}
