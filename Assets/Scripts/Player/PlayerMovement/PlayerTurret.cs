using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTurret : MonoBehaviour
{
    [Tooltip("Degees per second")]
    public float turnRate = 180f;

    private Vector2 targetPosition;

    void Update()
    {
        targetPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());// TODO Camera.main is slow
    }

    private void FixedUpdate()
    {
        Utility.RotateTowardsTarget(transform, targetPosition, turnRate * Time.fixedDeltaTime);
    }
}
