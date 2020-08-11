using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AI))]
public class MoveTowardsTarget : MonoBehaviour
{
    void FixedUpdate()
    {
        if(GetComponent<AI>().GetATarget())
        {
            GetComponent<PolyNav.PolyNavAgent>().SetDestination(GetComponent<AI>().GetATarget().transform.position);
        }
    }
}
