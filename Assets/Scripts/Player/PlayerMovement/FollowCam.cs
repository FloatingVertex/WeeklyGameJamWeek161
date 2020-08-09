using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform target;
    public float maxOffset = 10.0f;

    private void Awake()
    {
        if(Utility.playerShip == null)
        {
            Utility.playerShip = target;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var offset = ((Vector2)Input.mousePosition - new Vector2(Screen.width/2, Screen.height/2))/ Mathf.Min(Screen.height, Screen.width) / 2;
        if(offset.magnitude > 1.0f)
        {
            offset = offset.normalized;
        }
        offset = offset * maxOffset;
        Vector2 newPosition = (Vector2)target.transform.position + offset;
        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
    }
}
