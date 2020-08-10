using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AircraftMovement : MonoBehaviour
{
    public float speed = 1f;
    [Tooltip("Degees per second")]
    public float turnRate = 180f;

    private Vector2 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        targetPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());// TODO Camera.main is slow
    }

    private void FixedUpdate()
    {
        TurnTowardTarget(targetPosition, turnRate * Time.fixedDeltaTime);
        GetComponent<Rigidbody2D>().velocity = transform.right * speed;
    }

    void TurnTowardTarget(Vector2 target, float maxAngleDeg)
    {
        Utility.RotateTowardsTarget(transform, target, maxAngleDeg);
    }
}
