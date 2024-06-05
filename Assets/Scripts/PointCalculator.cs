using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCalculator : MonoBehaviour
{
    private bool freeFalling = false;
    private bool once = false;
    public float airTime = 0f;

    public int flips = 0;
    public int spins = 0;

    public float previousZRot = 0f;
    public float totalZRot = 0f;

    public float previousYRot = 0f;
    public float totalYRot = 0f;

    public GameObject leftWeb;
    public GameObject rightWeb;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (leftWeb.active || rightWeb.active) freeFalling = false;
        else freeFalling = true;
    }

    void FixedUpdate()
    {
        if (!freeFalling)
        {
            flips = 0;
            spins = 0;
            totalYRot = 0f;
            totalZRot = 0f;
            once = true;
            airTime = 0f;
            return;
        }

        if (once)
        {
            once = false;

            previousZRot = transform.eulerAngles.z;
            previousYRot = transform.eulerAngles.y;

            airTime += Time.deltaTime;

            return;
        }

        float rotationChangeZ = transform.eulerAngles.z - previousZRot;
        float rotationChangeY = transform.eulerAngles.y - previousYRot;

        if (rotationChangeZ > 180) rotationChangeZ -= 360;
        else if (rotationChangeZ < -180) rotationChangeZ += 360;

        if (rotationChangeY > 180) rotationChangeY -= 360;
        else if (rotationChangeY < -180) rotationChangeY += 360;

        totalZRot += rotationChangeZ;
        totalYRot += rotationChangeY;

        airTime += Time.deltaTime;

        if (totalZRot > 360)
        {
            flips += 1;
            totalZRot = 0f;
        }
        else if (totalZRot < -360)
        {
            flips += 1;
            totalZRot = 0f;
        }

        // Y axis rotation doesn't correlate to spins (??)
        if (totalYRot > 360)
        {
            spins += 1;
            totalYRot = 0f;
        }
        else if (totalYRot < -360)
        {
            spins += 1;
            totalYRot = 0f;
        }

        previousZRot = transform.eulerAngles.z;
        previousYRot = transform.eulerAngles.y;
    }
}
