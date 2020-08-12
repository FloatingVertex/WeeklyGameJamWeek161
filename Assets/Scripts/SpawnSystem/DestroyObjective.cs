using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjective : MonoBehaviour, IObjective
{
    public GameObject toDestroy;

    public bool IsComplete()
    {
        return toDestroy == null;
    }

    private void Start()
    {
        if(!GetComponentInParent<LevelManager>())
        {
            Debug.LogError("Objectives must be child of the level manager");
        }
        transform.position = toDestroy.transform.position;
    }
}
