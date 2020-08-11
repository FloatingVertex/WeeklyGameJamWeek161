using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class ExplodingBarrel : MonoBehaviour
{
    public Collider2D nonTriggerCollider;

    void OnTriggerEnter2D(Collider2D other)
    {
        var otherActor = other.GetComponentInParent<Actor>();
        if (otherActor && otherActor.affiliation == Affiliation.Player)
        {
            nonTriggerCollider.enabled = false;
            GetComponent<Actor>().affiliation = Affiliation.Player;
            nonTriggerCollider.enabled = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var otherActor = other.GetComponentInParent<Actor>();
        if (otherActor && otherActor.affiliation == Affiliation.Player)
        {
            nonTriggerCollider.enabled = false;
            GetComponent<Actor>().affiliation = Affiliation.Enemy;
            nonTriggerCollider.enabled = true;
        }
    }
}
