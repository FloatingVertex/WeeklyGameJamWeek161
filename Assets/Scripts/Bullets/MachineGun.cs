using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : MonoBehaviour
{
    public GameObject bulletPrefsb;
    public float shotsPerSecond = 10f;
    public float bulletSpeed = 10f;
    public AnimationCurve spreadOverTime;
    public float noiseRate = 10f;
    [Tooltip("1=>1 sec recovers 1 sec of firing, 4=>1 sec recovers from 4 sec of firing")]
    public float recoilRecoveryRate = 1f;

    private bool firing = false;

    private float recoilProgress = 0f;
    private float deltaToFireAgain = 0f; // negative or zero, number of seconds we have to wait to fire again

    public void SetFiring(bool firing)
    {
        this.firing = firing;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        deltaToFireAgain -= Time.fixedDeltaTime;
        if (firing)
        {
            if (deltaToFireAgain <= 0.0f)
            {
                float timeBetweenShots = 1.0f / shotsPerSecond;
                deltaToFireAgain += timeBetweenShots;
                recoilProgress += timeBetweenShots;
                Fire();
            }
        }
        else
        {
            recoilProgress -= Time.fixedDeltaTime * recoilRecoveryRate;
        }

        deltaToFireAgain = Mathf.Max(deltaToFireAgain, 0.0f);

    }

    void Fire()
    {
        float spread = spreadOverTime.Evaluate(recoilProgress);
        float perlin = Mathf.PerlinNoise(Time.timeSinceLevelLoad * noiseRate/2, Time.timeSinceLevelLoad * noiseRate) - 0.5f;
        float rotateBy = perlin * spread * 10.0f;
        var bulletRotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + rotateBy);
        var bullet = Instantiate(bulletPrefsb, transform.position, bulletRotation).GetComponent<Bullet>();
        bullet.speed = bulletSpeed;
    }


}
