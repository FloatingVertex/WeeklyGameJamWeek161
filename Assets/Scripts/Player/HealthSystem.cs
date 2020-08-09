using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if(currentHealth < 1)
        {
            Destroy(this.gameObject);
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }

    
}
