using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class BoostSystem : MonoBehaviour
{
    
    public float cooldown = 20f;
    public float duration = 10f;
    public float speedBoost = 8f;
    public float speedNormal;
    public bool hasCooldown = false;

    public SpriteRenderer boostRenderer;
    public SpriteRenderer normalRenderer;


    private void Start()
    {
        
        StartCoroutine(ActivateCooldown());

        speedNormal = GetComponent<HelicopterMovement>().speed;
        
    }

    private void Update()
    {
    }
    
    public void Boost()
    {
        if (!hasCooldown)
        {
            hasCooldown = true;
            StartCoroutine(ActivateCooldown());
            GetComponent<HelicopterMovement>().speed = speedBoost;
            normalRenderer.enabled = false;
            boostRenderer.enabled = true;
            StartCoroutine(ResetMovement());
        }
    }

    IEnumerator ResetMovement()
    {
        yield return new WaitForSeconds(duration);
        GetComponent<HelicopterMovement>().speed = speedNormal;
        normalRenderer.enabled = true;
        boostRenderer.enabled = false;
    }

    IEnumerator ActivateCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        hasCooldown = false;
    }
    
}
