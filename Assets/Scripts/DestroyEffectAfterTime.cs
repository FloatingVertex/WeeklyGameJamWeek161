using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffectAfterTime : MonoBehaviour
{
    public float timer = 1.0f;
    public float audioVolume = 1.0f;

    void Start()
    {
        StartCoroutine(DestroyAfterTime(timer));
    }

    IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        Destroy(gameObject);
    }
}
