﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class BoostSystem : MonoBehaviour
{
    
    public float cooldown = 20f;
    public float duration = 10f;
    private float speedBoost = 8f;
    public float speedNormal;
    private bool hasCooldown;


    private void Start()
    {
        
        StartCoroutine(ActivateCooldown());

        speedNormal = GetComponent<HelicopterMovement>().speed;
        
    }

    private void Update()
    {
        Debug.Log(speedNormal);
        Debug.Log(hasCooldown);
    }
    
    public void Boost()
    {
            GetComponent<HelicopterMovement>().speed = speedBoost;
            StartCoroutine(ActivateCooldown());
            StartCoroutine(ResetMovement());    
    }

    IEnumerator ResetMovement()
    {
        yield return new WaitForSeconds(duration);
        GetComponent<HelicopterMovement>().speed = speedNormal;
        Debug.Log("Reset called");
    }

    IEnumerator ActivateCooldown()
    {
        hasCooldown = true;
        yield return new WaitForSeconds(cooldown);
        hasCooldown = false;
        Debug.Log("Cooldown called");
    }
    
}