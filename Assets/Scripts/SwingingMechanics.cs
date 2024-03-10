using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwingingMechanics : MonoBehaviour
{
    private Rigidbody MaleDummyRB;
    public Transform cameraTransform;

    public Graphic crosshair;

    public GameObject leftWeb;
    public GameObject leftHand;
    private Vector3 leftSwingPoint;

    public GameObject rightWeb;
    public GameObject rightHand;
    private Vector3 rightSwingPoint;
    
    private float webDistance;
    
    public AudioSource audioSourceSwing;
    public AudioClip audioClipSwing;

    public AudioSource audioSourceAir;

    public float swingForce = 10f;
    public float speedLimit = 15f;
    public float webLimit = 10f;
    public float airMovementForce = 0.5f;
    public float groundMovementForce = 2;
    
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
        MaleDummyRB = this.GetComponent<Rigidbody>();
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
        controlSpeed();
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
        transform.localRotation = Quaternion.Euler(0f, cameraTransform.eulerAngles.y, 0f);

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
            leftSwingPoint = nullVector;
            leftWeb.SetActive(false);
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            animateWeb(rightSwingPoint, rightWeb, rightHand);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            rightSwingPoint = nullVector;
            rightWeb.SetActive(false);
        }

        
        if (Input.GetKey(KeyCode.Space))
        {
            // Put player in the air
            Vector3 newPos = new Vector3(transform.position.x, 20f, transform.position.z);
            transform.position = newPos;
        }

        // Checks for wsad movement and applies the physics
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.1f))
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

            float targetSpeed = speedLimit + (overSpeed / 1.1f);

            MaleDummyRB.velocity = MaleDummyRB.velocity.normalized * targetSpeed;
        }
    }

    void groundMovement(RaycastHit hit)
    {
        //if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
        //{
        //    transform.Translate(0, 0, (groundMovementForce * Time.deltaTime) / 2);
        //    transform.Translate((-groundMovementForce * Time.deltaTime) / 2, 0, 0);
        //}
        //else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        //{
        //    transform.Translate(0, 0, (groundMovementForce * Time.deltaTime) / 2);
        //    transform.Translate((groundMovementForce * Time.deltaTime) / 2, 0, 0);
        //}
        //else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        //{
        //    transform.Translate(0, 0, (-groundMovementForce * Time.deltaTime) / 2);
        //    transform.Translate((-groundMovementForce * Time.deltaTime) / 2, 0, 0);
        //}
        //else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
        //{
        //    transform.Translate(0, 0, (-groundMovementForce * Time.deltaTime) / 2);
        //    transform.Translate((groundMovementForce * Time.deltaTime) / 2, 0, 0);
        //}
        //else if (Input.GetKey(KeyCode.W))
        //{
        //    transform.Translate(0, 0, groundMovementForce * Time.deltaTime);
        //}
        //else if (Input.GetKey(KeyCode.A))
        //{
        //    transform.Translate(-groundMovementForce * Time.deltaTime, 0, 0);
        //}
        //else if (Input.GetKey(KeyCode.D))
        //{
        //    transform.Translate(groundMovementForce * Time.deltaTime, 0, 0);
        //}
        //else if (Input.GetKey(KeyCode.S))
        //{
        //    transform.Translate(0, 0, -groundMovementForce * Time.deltaTime);
        //}

        //Vector3 newPos = new Vector3 (transform.position.x, hit.point.y + 0.05f, transform.position.z);
        //transform.position = newPos;

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
        {
            MaleDummyRB.AddForce((transform.forward * groundMovementForce * Time.deltaTime) / 2, ForceMode.VelocityChange);
            MaleDummyRB.AddForce((-transform.right * groundMovementForce * Time.deltaTime) / 2, ForceMode.VelocityChange);
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            MaleDummyRB.AddForce((transform.forward * groundMovementForce * Time.deltaTime) / 2, ForceMode.VelocityChange);
            MaleDummyRB.AddForce((transform.right * groundMovementForce * Time.deltaTime) / 2, ForceMode.VelocityChange);
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        {
            MaleDummyRB.AddForce((-transform.forward * groundMovementForce * Time.deltaTime) / 2, ForceMode.VelocityChange);
            MaleDummyRB.AddForce((-transform.right * groundMovementForce * Time.deltaTime) / 2, ForceMode.VelocityChange);
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
        {
            MaleDummyRB.AddForce((-transform.forward * groundMovementForce * Time.deltaTime) / 2, ForceMode.VelocityChange);
            MaleDummyRB.AddForce((transform.right * groundMovementForce * Time.deltaTime) / 2, ForceMode.VelocityChange);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            MaleDummyRB.AddForce(transform.forward * groundMovementForce * Time.deltaTime, ForceMode.VelocityChange);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            MaleDummyRB.AddForce(-transform.right * groundMovementForce * Time.deltaTime, ForceMode.VelocityChange);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            MaleDummyRB.AddForce(transform.right * groundMovementForce * Time.deltaTime, ForceMode.VelocityChange);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            MaleDummyRB.AddForce(-transform.forward * groundMovementForce * Time.deltaTime, ForceMode.VelocityChange);
        }
    }

    void airMovement()
    {
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
        {
            MaleDummyRB.AddForce((transform.forward * airMovementForce * Time.deltaTime) / 2, ForceMode.VelocityChange);
            MaleDummyRB.AddForce((-transform.right * airMovementForce * Time.deltaTime) / 2, ForceMode.VelocityChange);
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            MaleDummyRB.AddForce((transform.forward * airMovementForce * Time.deltaTime) / 2, ForceMode.VelocityChange);
            MaleDummyRB.AddForce((transform.right * airMovementForce * Time.deltaTime) / 2, ForceMode.VelocityChange);
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        {
            MaleDummyRB.AddForce((-transform.forward * airMovementForce * Time.deltaTime) / 2, ForceMode.VelocityChange);
            MaleDummyRB.AddForce((-transform.right * airMovementForce * Time.deltaTime) / 2, ForceMode.VelocityChange);
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
        {
            MaleDummyRB.AddForce((-transform.forward * airMovementForce * Time.deltaTime) / 2, ForceMode.VelocityChange);
            MaleDummyRB.AddForce((transform.right * airMovementForce * Time.deltaTime) / 2, ForceMode.VelocityChange);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            MaleDummyRB.AddForce(transform.forward * airMovementForce * Time.deltaTime, ForceMode.VelocityChange);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            MaleDummyRB.AddForce(-transform.right * airMovementForce * Time.deltaTime, ForceMode.VelocityChange);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            MaleDummyRB.AddForce(transform.right * airMovementForce * Time.deltaTime, ForceMode.VelocityChange);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            MaleDummyRB.AddForce(-transform.forward * airMovementForce * Time.deltaTime, ForceMode.VelocityChange);
        }
    }

    void animateWeb(Vector3 swingPoint, GameObject web, GameObject hand)
    {
        if (swingPoint == nullVector) return;
        
        web.SetActive(true);
        web.transform.position = (swingPoint + hand.transform.position) / 2;
        web.transform.LookAt(swingPoint);
        web.transform.Rotate(90.0f, 0.0f, 0.0f, Space.Self);
        webDistance = Vector3.Distance(swingPoint, hand.transform.position);
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
        Vector3 swingForceVector = playerToSwingPoint.normalized * dotProduct * swingForce;

        // Apply the swing force to the player's Rigidbody
        MaleDummyRB.AddForce(swingForceVector, ForceMode.VelocityChange);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(cameraTransform.position, webLimit);
    }
}
