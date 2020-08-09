using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailGun : MonoBehaviour, IWeapon
{
    public GameObject bulletPrefab;
    public float secondsBetweenShots = 2f;
    public float bulletSpeed = 100f;

    private bool firing = false;
    private float deltaToFireAgain = 0f;

    public void SetFiring(bool firing)
    {
        this.firing = firing;
    }

    public void FixedUpdate()
    {
        deltaToFireAgain -= Time.fixedDeltaTime;
        if (firing)
        {
            if (deltaToFireAgain <= 0.0f)
            {
                deltaToFireAgain += secondsBetweenShots;
                Fire();
            }
        }

        deltaToFireAgain = Mathf.Max(deltaToFireAgain, 0.0f);
    }

    private void Fire()
    {
        var bullet = Instantiate(bulletPrefab, transform.position, transform.rotation).GetComponent<Bullet>();
        bullet.speed = bulletSpeed;
    }

    public float BulletSpeed()
    {
        return bulletSpeed;
    }

    public bool GetFiring()
    {
        return firing;
    }
}
