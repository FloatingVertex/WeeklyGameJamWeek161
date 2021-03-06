﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : MonoBehaviour, IWeapon
{
    public GameObject bulletPrefab;
    public float shotsPerSecond = 10f;
    public float bulletSpeed = 10f;
    public AnimationCurve spreadOverTime;
    public float noiseRate = 10f;
    [Tooltip("1=>1 sec recovers 1 sec of firing, 4=>1 sec recovers from 4 sec of firing")]
    public float recoilRecoveryRate = 0.5f;

    private bool firing = false;

    public float recoilProgress = 0f;
    private float deltaToFireAgain = 0f; // negative or zero, number of seconds we have to wait to fire again

    public void SetFiring(bool firing)
    {
        this.firing = firing;
    }

    private float TimeBetweenShots()
    {
        return 1.0f / shotsPerSecond;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        deltaToFireAgain -= Time.fixedDeltaTime;
        if (firing)
        {
            if (deltaToFireAgain <= 0.0f)
            {
                deltaToFireAgain += TimeBetweenShots();
                recoilProgress += TimeBetweenShots();
                Fire();
            }
        }
        else
        {
            recoilProgress = Mathf.Max(recoilProgress - Time.fixedDeltaTime * recoilRecoveryRate,0f);
        }

        deltaToFireAgain = Mathf.Max(deltaToFireAgain, 0.0f);

    }

    void Fire()
    {
        float spread = spreadOverTime.Evaluate(recoilProgress);
        float perlin = Mathf.PerlinNoise(Time.timeSinceLevelLoad * noiseRate/2, Time.timeSinceLevelLoad * noiseRate) - 0.5f;
        float rotateBy = perlin * spread * 10.0f;
        var bulletRotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + rotateBy);
        var bullet = Instantiate(bulletPrefab, transform.position, bulletRotation).GetComponent<Bullet>();
        bullet.speed = bulletSpeed;
        bullet.FixedUpdateStep();
    }

    public float BulletSpeed()
    {
        return bulletSpeed;
    }
    public bool GetFiring()
    {
        return firing;
    }

    public float ReloadStatus()
    {
        return (1f-Mathf.Max(0f,deltaToFireAgain/ TimeBetweenShots()));
    }
}
