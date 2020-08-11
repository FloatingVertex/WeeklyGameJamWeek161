using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void Damage(float damageTaken,float terrainDamageRadius, DamageType type, Vector2 point, Vector2 damageDirection, Vector2 surfaceNormal);
}

public enum DamageType
{
    Impact,
    Explosive,
    Other
}
