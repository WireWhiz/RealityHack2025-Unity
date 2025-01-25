using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sinebob : MonoBehaviour
{
    float localYStart;

    [Range(0, 1)]
    [SerializeField] float yScale = 1;

    [Range(0, 10)]
    [SerializeField] float speed = 1;

    // Start is called before the first frame update
    void Start()
    {
        localYStart = transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = new Vector3(transform.localPosition.x,
            localYStart + (Mathf.Sin(Time.time * speed) * yScale),
            transform.localPosition.z); 
    }
}
