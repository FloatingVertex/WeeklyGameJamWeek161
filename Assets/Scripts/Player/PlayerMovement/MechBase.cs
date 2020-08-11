using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MechBase : MonoBehaviour
{
    public float walkSpeed = 2f;

    public void Move(InputAction.CallbackContext context)
    {
        GetComponent<Rigidbody2D>().velocity = context.ReadValue<Vector2>() * walkSpeed;
    }

}
