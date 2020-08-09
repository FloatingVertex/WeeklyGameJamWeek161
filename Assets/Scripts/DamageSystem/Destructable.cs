﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour, IDamageable
{
    public float health = 100;
    public GameObject destroyedPrefab;

    public void Damage(float damageTaken, DamageType type, Vector2 point, Vector2 damageDirection, Vector2 surfaceNormal)
    {
        health -= damageTaken;
        if(health < 0)
        {
            if (destroyedPrefab)
            {
                Instantiate(destroyedPrefab, transform.position, transform.rotation);
            }
            Destroy(gameObject);
        }
    }
}
