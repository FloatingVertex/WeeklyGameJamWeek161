using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class AI : MonoBehaviour
{
    private List<Actor> targets = new List<Actor>();

    private Actor actor;

    private void Start()
    {
        actor = GetComponent<Actor>();
    }

    private void FixedUpdate()
    {
        foreach(var target in targets)
        {
            if (CheckLOS(target))
            {
                if (!target.spotting.Contains(actor))
                {
                    target.spotting.Add(actor);
                }
            }
            else
            {
                target.spotting.Remove(actor);
            }
        }
    }

    bool CheckLOS(Actor target)
    {
        
        var range = ((Vector2)(target.transform.position - transform.position)).magnitude;
        var lineOfSightHit = Physics2D.Raycast(
            transform.position, target.transform.position - transform.position,
            Vector3.Distance(transform.position, target.transform.position),
            Utility.aiVisionRaycastMask);
        return (lineOfSightHit.collider && lineOfSightHit.collider.transform.IsChildOf(target.transform));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var otherActor = other.GetComponentInParent<Actor>();
        if (otherActor && otherActor.affiliation != GetComponent<Actor>().affiliation && !targets.Contains(otherActor))
        {
            Debug.Log(otherActor.gameObject.name + " enter the trigger of " + gameObject.name);
            targets.Add(otherActor);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var otherActor = other.GetComponentInParent<Actor>();
        if (otherActor)
        {
            Debug.Log(otherActor.gameObject.name + " exited the trigger of " + gameObject.name);
            targets.Remove(otherActor);
        }
    }

    public Actor GetSpottedTarget()
    {
        foreach(var actor in targets)
        {
            if(actor.spotting.Count > 0)
            {
                return actor;
            }
        }
        return null;
    }

    public Actor GetTargetInLOS()
    {
        for(int i = 0; i < targets.Count; i++)
        {
            var target = targets[i];
            if (target == null)
            {
                i--;
                targets.Remove(target);
                continue;
            }
            if(CheckLOS(target))
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
