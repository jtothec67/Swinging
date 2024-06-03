using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmenyBehaviour : MonoBehaviour
{
    public GameObject bullet;
    private GameObject player;
    public ParticleSystem explosion;

    public AudioSource audioSource;
    public AudioClip audioHit;
    public AudioClip audioShoot;
    public AudioClip audioScan;
    public AudioClip[] audioIdle;
    public float maxMakeNoiseTime = 6f;
    public float minMakeNoiseTime = 3f;
    public AudioClip audioExplode;

    public float strafeMovementSpeed = 2.5f;
    public float strafeRange = 10f;
    public float searchTime = 6f;
    public float movementSpeed = 5;
    public int lives = 2;
    public float shootDistance = 10f;
    public float shootCoolDown = 2f;
    public Transform[] waypoints;

    private int currentClip = 0;
    private float makeNoise;
    private int currentWaypointIndex = 0;
    private bool isDead = false;
    private float makeNoiseTimer = 0f;
    private float searchTimer;
    private float shootTimer = 0f;
    private float getHitCooldown = 0f;
    private bool canSeePlayer = false;

    private Vector3 strafeTarget;

    // Start is called before the first frame update
    void Start()
    {
        makeNoise = Random.Range(minMakeNoiseTime, maxMakeNoiseTime);
        searchTimer = searchTime + 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            transform.rotation = Quaternion.Euler(90, transform.localEulerAngles.y, transform.localEulerAngles.z);
            return;
        }

        makeNoiseTimer += Time.deltaTime;
        getHitCooldown += Time.deltaTime;
        searchTimer += Time.deltaTime;
        shootTimer += Time.deltaTime;

        canSeePlayer = false;

        Collider[] collider = Physics.OverlapSphere(transform.position, shootDistance);
        foreach (Collider col in collider)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                player = col.gameObject;

                RaycastHit toPlayer;
                Physics.Raycast(transform.position, col.gameObject.transform.position - transform.position, out toPlayer, Mathf.Infinity);

                if (toPlayer.collider.CompareTag("Player"))
                {
                    canSeePlayer = true;
                    
                }
            }
        }


        if(canSeePlayer)
        {
            transform.LookAt(player.transform.position);

            AIStrafe();

            searchTimer = 0f;

            if (shootTimer > shootCoolDown)
            {
                ShootBullet();
                audioSource.PlayOneShot(audioShoot);
                shootTimer = 0;
            }
        }
        else if(searchTimer < searchTime)
        {
            if(!audioSource.isPlaying) audioSource.PlayOneShot(audioScan);
            // Search
        }
        else
        {
            if (makeNoiseTimer > makeNoise)
            {
                audioSource.PlayOneShot(audioIdle[currentClip]);
                currentClip++;

                if (currentClip >= audioIdle.Length) currentClip = 0;

                makeNoise = Random.Range(minMakeNoiseTime, maxMakeNoiseTime);
                makeNoiseTimer = 0;
            }

            strafeTarget = new Vector3(0f, 0f, 0f);
            AIMove();
        }
    }

    void AIStrafe()
    {
        if(strafeTarget == new Vector3 (0f, 0f, 0f))
        {
            bool loop = true;
            while(loop)
            {
                strafeTarget = transform.position + Random.insideUnitSphere * strafeRange;
                Collider[] colliders = Physics.OverlapSphere(strafeTarget, 1);
                if (colliders.Length == 0) loop = false;
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, strafeTarget, strafeMovementSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, strafeTarget) < 0.1f)
        {
            bool loop = true;
            while (loop)
            {
                strafeTarget = transform.position + Random.insideUnitSphere * strafeRange;
                Collider[] colliders = Physics.OverlapSphere(strafeTarget, 1);
                if (colliders.Length == 0) loop = false;
            }
        }
    }

    void AIMove()
    {
        if (waypoints.Length == 0) return;

        transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypointIndex].position, movementSpeed * Time.deltaTime);
        transform.LookAt(waypoints[currentWaypointIndex].position);
        transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, transform.eulerAngles.z);

        if (Vector3.Distance (transform.position, waypoints[currentWaypointIndex].position) < 0.1f)
        {
            if(currentWaypointIndex >= waypoints.Length-1)
            {
                currentWaypointIndex = 0;
            }
            else
            {
                currentWaypointIndex++;
            }
        }
    }

    void ShootBullet()
    {
        RaycastHit bulletTravel;
        Physics.Raycast(transform.position, player.transform.position - transform.position + new Vector3 ( 0f, 1f, 0f), out bulletTravel, Mathf.Infinity);

        Vector3 relativePos = bulletTravel.point - transform.position;
        var newBullet = Instantiate(bullet, transform.position, Quaternion.LookRotation(relativePos, Vector3.up));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, shootDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, strafeRange);
    }

    void OnCollisionEnter(Collision hit)
    {
        if (hit.gameObject.CompareTag("Bullet"))
        {
            if (getHitCooldown > 0.1f)
            {
                if (lives <= 1)
                {
                    audioSource.PlayOneShot(audioExplode);
                    explosion.Play();
                    isDead = true;
                    Destroy(gameObject, 2f);
                }
                else
                {
                    audioSource.PlayOneShot(audioHit);
                    getHitCooldown = 0;
                    lives--;
                }
            }
        }
    }
}
