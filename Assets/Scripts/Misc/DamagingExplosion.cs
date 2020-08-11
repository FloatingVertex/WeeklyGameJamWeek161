using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingExplosion : MonoBehaviour
{
    public float radius;
    public float damage;

    // Start is called before the first frame update
    void Start()
    {
        Explosion(transform.position, radius, damage);
    }

    public static void Explosion(Vector2 position, float radius, float damage)
    {
        var overlaps = Physics2D.OverlapCircleAll(position, radius);
        foreach(var collider in overlaps)
        {
            if (!collider.isTrigger && collider.GetComponentInParent<IDamageable>() != null)
            {
                collider.GetComponentInParent<IDamageable>().Damage(damage, radius, DamageType.Explosive, position, Vector2.zero, Vector2.zero);
            }
        }
    }
}
