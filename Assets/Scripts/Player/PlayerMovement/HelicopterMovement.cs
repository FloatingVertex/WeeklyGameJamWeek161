using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HelicopterMovement : MonoBehaviour
{
    public float speed = 2f;

    private Vector2 inputDirection = Vector2.zero;

    public void Move(InputAction.CallbackContext context)
    {
        inputDirection = context.ReadValue<Vector2>();
        GetComponent<Rigidbody2D>().velocity = context.ReadValue<Vector2>() * speed;
    }

    private void FixedUpdate()
    {
        GetComponent<Rigidbody2D>().velocity = inputDirection * speed;
    }
}
