using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform target;
    public void Update()
    {
        transform.rotation = Quaternion.LookRotation(target.position - transform.position);
    }
}
