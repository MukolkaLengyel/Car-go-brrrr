using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 1.5f, -4f);
    public float smoothSpeed = 0.125f;

    void LateUpdate()
    {
        // Smoothly follow position
        Vector3 desiredPosition = target.position + target.TransformDirection(offset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Look at target, but lock X and Z rotation
        transform.LookAt(target);
        Vector3 euler = transform.eulerAngles;
        transform.eulerAngles = new Vector3(0, euler.y, 0); // Only keep yaw (Y rotation)
    }
}
