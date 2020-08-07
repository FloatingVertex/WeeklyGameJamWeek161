using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour, IDamageable
{
    public float health = 100;

    public void Damage(float damageTaken, Vector2 point)
    {
        health -= damageTaken;
        if(health < 0)
        {
            Destroy(gameObject);
        }
    }
}
