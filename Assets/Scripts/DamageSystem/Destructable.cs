using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour, IDamageable
{
    public float health = 100;
    public GameObject destroyedPrefab;

    private float startingHealth;

    void Start()
    {
        startingHealth = health;
    }

    public float GetStartingHealth()
    {
        return startingHealth;
    }

    public void Damage(float damageTaken, float terrainDamageRadius, DamageType type, Vector2 point, Vector2 damageDirection, Vector2 surfaceNormal)
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
        else
        {
            FlashRed();
        }
    }

    public void FlashRed()
    {
        foreach (var obj in GetComponentsInChildren<SpriteRenderer>())
        {
            obj.color = Color.red;
        }
        StartCoroutine(ResetColor());
    }

    IEnumerator ResetColor()
    {
        yield return new WaitForSeconds(0.2f);
        foreach (var obj in GetComponentsInChildren<SpriteRenderer>()) {
            obj.color = Color.white;
        }
    }
}
