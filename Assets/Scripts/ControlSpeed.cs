using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSpeed : MonoBehaviour
{
    private Rigidbody rb;
    public float speedLimit = 13;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float speed = Vector3.Magnitude(rb.velocity);
        if (speed > speedLimit)
        {
            float overSpeed = speed - speedLimit;

            float targetSpeed = speedLimit + (overSpeed * 0.7f);

            rb.velocity = rb.velocity.normalized * targetSpeed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
