using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwingingMechanics : MonoBehaviour
{
    private Rigidbody MaleDummyRB;
    public Rigidbody leftForearmRB;
    public Rigidbody rightForearmRB;

    public Transform cameraTransform;

    public Graphic crosshair;

    public GameObject leftWeb;
    public GameObject leftHand;
    private Vector3 leftSwingPoint;

    public GameObject rightWeb;
    public GameObject rightHand;
    private Vector3 rightSwingPoint;
    
    private float webDistance;

    public float leftWebDist;
    public float rightWebDist;

    public bool leftOnce;
    public bool rightOnce;
    
    public AudioSource audioSourceSwing;
    public AudioClip audioClipSwing;

    public AudioSource audioSourceAir;

    public float swingForce = 10f;
    public float farSwingForce = 10f;
    public float speedLimit = 15f;
    public float webLimit = 10f;
    public float airMovementForce = 0.5f;
    public float groundMovementForce = 2;
    public float endOfSwingForce = 3f;
    
    public float speed;

    public float minAirSoundSpeed;
    public float maxAirSoundSpeed;

    public float volumeLevel = 0f;
    private Vector3 nullVector = Vector3.zero;

    void Start()
    {
        leftWeb.SetActive(false);
        rightWeb.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        MaleDummyRB = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            // Does maths for what force to apply if holding down mouse click
            swingPhysics(leftSwingPoint, leftHand);
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            swingPhysics(rightSwingPoint, rightHand);
        }

        // Does maths for limiting player speed
       //controlSpeed();
    }

    void Update()
    {
        speed = Vector3.Magnitude(MaleDummyRB.velocity);

        if (speed <= minAirSoundSpeed)
        {
            volumeLevel = 0.0f;
        }
        else if (speed >= maxAirSoundSpeed)
        {
            volumeLevel = 1.0f;
        }
        else
        {
            // Linear interpolation between 0 and 1 based on the range [minAirSoundSpeed, maxAirSoundSpeed]
            volumeLevel = (speed - minAirSoundSpeed) / (maxAirSoundSpeed - minAirSoundSpeed);
        }

        audioSourceAir.volume = volumeLevel;

        // Player always looks forward
        //transform.localRotation = Quaternion.Euler(0f, cameraTransform.eulerAngles.y, 0f);

        // Check if eligable for swing
        RaycastHit aimingAt;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out aimingAt, webLimit))
        {
            // Solid crosshair if can swing
            crosshair.color = new Vector4(1f, 1f, 1f, 1f);

            // Assign the current aiming at point to swing point if can swing and clicks mouse
            if (Input.GetMouseButtonDown(0))
            {
                leftSwingPoint = aimingAt.point;

                audioSourceSwing.pitch = Random.Range(0.9f, 1f);
                audioSourceSwing.PlayOneShot(audioClipSwing);
            }

            if (Input.GetMouseButtonDown(1))
            {
                rightSwingPoint = aimingAt.point;

                audioSourceSwing.pitch = Random.Range(0.9f, 1f);
                audioSourceSwing.PlayOneShot(audioClipSwing);
            }
        }
        else
        {
            // See through crosshair if can't swing
            crosshair.color = new Vector4(1f, 1f, 1f, 0.3f);
        }


        if (Input.GetKey(KeyCode.Mouse0))
        {
            // Do web visuals
            animateWeb(leftSwingPoint, leftWeb, leftHand);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // Stop web swinging
            applyEndOfSwingForce();
            leftSwingPoint = nullVector;
            leftWeb.SetActive(false);
            leftOnce = true;
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            animateWeb(rightSwingPoint, rightWeb, rightHand);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            applyEndOfSwingForce();
            rightSwingPoint = nullVector;
            rightWeb.SetActive(false);
            rightOnce = true;
        }

        
        if (Input.GetKey(KeyCode.Space))
        {
            // Put player in the air
            Vector3 newPos = new Vector3(transform.position.x, 20f, transform.position.z);
            transform.position = newPos;
        }

        // Checks for wsad movement and applies the physics
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f))
        {
            groundMovement(hit);
        }
        else
        {
            airMovement();
        }
    }

    void controlSpeed()
    {
        if (speed > speedLimit)
        {
            float overSpeed = speed - speedLimit;

            float targetSpeed = speedLimit + (overSpeed * 0.7f);

            MaleDummyRB.velocity = MaleDummyRB.velocity.normalized * targetSpeed;
            MaleDummyRB.angularVelocity = MaleDummyRB.angularVelocity;
        }
    }

    void groundMovement(RaycastHit hit)
    {
        // Get input from the player
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Create a movement vector based on the input
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        // Calculate the force direction relative to the camera's direction
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // Ensure the forward vector is always on the horizontal plane
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        // Normalize vectors (important if the camera is tilted)
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate the direction in which to apply the force
        Vector3 forceDirection = (cameraForward * moveVertical + cameraRight * moveHorizontal).normalized;

        MaleDummyRB.AddForce(forceDirection * groundMovementForce * Time.deltaTime, ForceMode.VelocityChange);
    }

    void airMovement()
    {
        // Get input from the player
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Create a movement vector based on the input
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        // Calculate the force direction relative to the camera's direction
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // Ensure the forward vector is always on the horizontal plane
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        // Normalize vectors (important if the camera is tilted)
        //cameraForward.Normalize();
        //cameraRight.Normalize();

        // Calculate the direction in which to apply the force
        Vector3 forceDirection = (cameraForward * moveVertical + cameraRight * moveHorizontal).normalized;

        MaleDummyRB.AddForce(forceDirection * airMovementForce * Time.deltaTime, ForceMode.VelocityChange);
    }

    void animateWeb(Vector3 swingPoint, GameObject web, GameObject hand)
    {
        if (swingPoint == nullVector) return;
        
        web.SetActive(true);
        web.transform.position = (swingPoint + hand.transform.position) / 2;
        web.transform.LookAt(swingPoint);
        web.transform.Rotate(90.0f, 0.0f, 0.0f, Space.Self);
        webDistance = Vector3.Distance(swingPoint, hand.transform.position);
        if (leftOnce && hand.name == "B-palm_02_L")
        {
            leftWebDist = webDistance;
            leftOnce = false;
        }
        else if (rightOnce && hand.name == "B-palm_02_R")
        {
            rightWebDist = webDistance;
            rightOnce = false;
        }
        Vector3 rescale = web.transform.localScale;
        rescale.y = webDistance / 2;
        web.transform.localScale = rescale;
        
    }

    void swingPhysics(Vector3 swingPoint, GameObject hand)
    {
        if (swingPoint == nullVector) return;

        Vector3 playerToSwingPoint = swingPoint - hand.transform.position;
        Vector3 playerDirection = playerToSwingPoint.normalized;

        // Calculate the swing force using dot product
        float dotProduct = Vector3.Dot(playerDirection, playerToSwingPoint.normalized);

        // Apply the swing force to the player's Rigidbody
        //MaleDummyRB.AddForce(swingForceVector, ForceMode.VelocityChange);

        // Left hand and near swing
        if (hand.name == "B-palm_02_L" && leftWebDist < playerToSwingPoint.magnitude)
        {
            Vector3 swingForceVector = playerToSwingPoint.normalized * dotProduct * farSwingForce;
            leftForearmRB.AddForce(swingForceVector, ForceMode.VelocityChange);
        }
        // Left hand and far swing
        else if (hand.name == "B-palm_02_L")
        {
            Vector3 swingForceVector = playerToSwingPoint.normalized * dotProduct * swingForce;
            leftForearmRB.AddForce(swingForceVector, ForceMode.VelocityChange);
        }
        // Right hand and near swing
        else if (rightWebDist < playerToSwingPoint.magnitude)
        {
            Vector3 swingForceVector = playerToSwingPoint.normalized * dotProduct * farSwingForce;
            rightForearmRB.AddForce(swingForceVector, ForceMode.VelocityChange);
        }
        // Right hand and far swing
        else
        {
            Vector3 swingForceVector = playerToSwingPoint.normalized * dotProduct * swingForce;
            rightForearmRB.AddForce(swingForceVector, ForceMode.VelocityChange);
        }
    }

    void applyEndOfSwingForce()
    {
        if (MaleDummyRB.velocity.y < 0) return;

        Vector3 force = MaleDummyRB.velocity.normalized * endOfSwingForce;

        // Apply the swing force to the player's Rigidbody
        MaleDummyRB.AddForce(force, ForceMode.VelocityChange);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(cameraTransform.position, webLimit);
    }
}
