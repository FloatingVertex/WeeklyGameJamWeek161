using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10.0f;
    public float damage = 40f;
    public DamageType dmgType;
    public float timeToLive = 5f;
    public ContactFilter2D filter;

    private void FixedUpdate()
    {
        Vector2 velocity = transform.right * speed;
        Vector3 newPosition = transform.position + (Vector3)velocity * Time.fixedDeltaTime;
        var hits = new List<RaycastHit2D>();
        if(0 != Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y),velocity, filter, hits,velocity.magnitude * Time.fixedDeltaTime))
        {
            RaycastHit2D nearestHit = hits[0];
            float currentClosest = float.PositiveInfinity;
            foreach (var hit in hits){
                if((hit.point - (Vector2)transform.position).magnitude < currentClosest)
                {
                    nearestHit = hit;
                }
            }
            var damageable = nearestHit.collider.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.Damage(damage, dmgType, nearestHit.point, transform.right, nearestHit.normal);
                Destroy(gameObject);
            }
            transform.position = nearestHit.point;
        }
        else {
            transform.position = newPosition;
        }
    }
}
