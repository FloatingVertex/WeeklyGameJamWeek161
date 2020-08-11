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

    public HitEffectConfig[] hitEffects;

    private void FixedUpdate()
    {
        FixedUpdateStep();
    }

    public void FixedUpdateStep()
    {
        Vector2 velocity = transform.right * speed;
        Vector3 newPosition = transform.position + (Vector3)velocity * Time.fixedDeltaTime;
        var hits = new List<RaycastHit2D>();
        if (0 != Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), velocity, filter, hits, velocity.magnitude * Time.fixedDeltaTime))
        {
            RaycastHit2D nearestHit = hits[0];
            float currentClosest = float.PositiveInfinity;
            foreach (var hit in hits)
            {
                if ((hit.point - (Vector2)transform.position).magnitude < currentClosest)
                {
                    nearestHit = hit;
                }
            }
            foreach (var hitEffect in hitEffects)
            {
                if (nearestHit.collider.sharedMaterial == hitEffect.materialHit)
                {
                    var effect = Instantiate(hitEffect.prefabToSpawn, nearestHit.point, Quaternion.identity);
                    Utility.RotateTowardsTarget(effect.transform, nearestHit.point + nearestHit.normal);
                }
            }
            var damageable = nearestHit.collider.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.Damage(damage, dmgType, nearestHit.point, transform.right, nearestHit.normal);
            }
            Destroy(gameObject);
            transform.position = nearestHit.point - (velocity.normalized * 0.5f);
        }
        else
        {
            transform.position = newPosition;
        }
        timeToLive -= Time.fixedDeltaTime;
        if (timeToLive < 0.0f)
        {
            Destroy(gameObject);
        }
    }
}

[System.Serializable]
public struct HitEffectConfig
{
    public PhysicsMaterial2D materialHit;
    public GameObject prefabToSpawn;
}
