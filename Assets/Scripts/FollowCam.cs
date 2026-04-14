
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform camTransform;
    private Vector3 relativeOffset;

    void Start()
    {
        if (camTransform == null) camTransform = Camera.main.transform;
        // Record where this player is relative to the camera at the start
        relativeOffset = transform.position - camTransform.position;
    }

    void Update()
    {
        // Move the player to stay with the camera, 
        // but because it's not a child, it won't scale!
        transform.position = camTransform.position + relativeOffset;
    }
}