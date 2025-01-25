using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothedCamera : MonoBehaviour
{
    public float rotateSpeed;
    private Quaternion lastRotation;
    private void Start()
    {
        lastRotation = transform.rotation;
    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.RotateTowards(lastRotation, transform.parent.rotation, Quaternion.Angle(lastRotation, transform.parent.rotation) * rotateSpeed * Time.deltaTime);
        lastRotation = transform.rotation;
    }
}
