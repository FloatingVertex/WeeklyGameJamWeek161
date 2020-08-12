using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableObjective : MonoBehaviour, IObjective
{
    public bool completed = false;

    public bool IsComplete()
    {
        return completed;
    }
    private void Start()
    {
        if (!GetComponentInParent<LevelManager>())
        {
            Debug.LogError("Objectives must be child of the level manager");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var otherActor = other.GetComponentInParent<Actor>();
        if (otherActor && otherActor.affiliation == Affiliation.Player)
        {
            completed = true;
            gameObject.SetActive(false);
        }
    }
}
