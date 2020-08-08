using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    //READ THIS
      // PLEASE ADD THIS SCRIPT TO THE CAMERA IN THE SCENE
    //READ THIS
    public Transform player;
    public Vector3 offset;

    private void Update()
    {
        transform.position = new Vector3(player.position.x + offset.x, player.position.y + offset.y, offset.z);
    }
}
