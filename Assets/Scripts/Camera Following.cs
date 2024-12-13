using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;
    public bool followPlayer = true; // New flag to enable/disable following
    private Vector3 initialPosition; // To store the initial position of the camera
    private Quaternion initialRotation;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - player.transform.position;
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (followPlayer) // Check if following is enabled
        {
            transform.position = player.transform.position + offset;
        }
    }
    public void ResetCamera() // Method to reset camera position and rotation
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
    }
}
