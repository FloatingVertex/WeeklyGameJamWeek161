using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HelicopterMovement : MonoBehaviour
{
    public float speed = 2f;

    public void Move(InputAction.CallbackContext context)
    {
        GetComponent<Rigidbody2D>().velocity = context.ReadValue<Vector2>() * speed;
    }

}
