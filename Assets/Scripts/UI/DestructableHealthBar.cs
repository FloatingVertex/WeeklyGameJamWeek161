using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestructableHealthBar : MonoBehaviour
{
    public bool showWhenFull = false;
    public Vector2 offset = Vector2.zero;
    public Slider healthBar;

    private Destructable totrack;

    // Start is called before the first frame update
    void Start()
    {
        totrack = GetComponentInParent<Destructable>();
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.value = totrack.health / totrack.GetStartingHealth();
        var newPosition = (Vector2)transform.parent.position + offset;
        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
        transform.rotation = Quaternion.identity;
        if (!showWhenFull)
        {
            if (totrack.health == totrack.GetStartingHealth())
            {
                healthBar.gameObject.SetActive(false);
            }
            else
            {
                healthBar.gameObject.SetActive(true);
            }
        }
    }
}
