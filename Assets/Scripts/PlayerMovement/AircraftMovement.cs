using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);// TODO Camera.main is slow
    }

    private void FixedUpdate()
    {
        TurnTowardTarget(targetPosition, turnRate * Time.fixedDeltaTime);
        GetComponent<Rigidbody2D>().velocity = transform.right * speed;
    }

    void TurnTowardTarget(Vector2 target, float maxAngleDeg)
    {
        float angle = Mathf.Atan2(target.y - transform.position.y, target.x - transform.position.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxAngleDeg);
    }
}
