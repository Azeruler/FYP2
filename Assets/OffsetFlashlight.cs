using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetFlashlight : MonoBehaviour
{
    private Vector3 localOffset;
    private Transform cameraTransform;
    [SerializeField] private float speed = 3.0f;
    private Vector3 initialOffset;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        initialOffset = transform.position - cameraTransform.position;
    }

    void Update()
    {
        // Maintain the initial offset based on the camera's position
        Vector3 desiredPosition = cameraTransform.position + cameraTransform.TransformDirection(initialOffset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, speed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, cameraTransform.rotation, speed * Time.deltaTime);
    }
}
