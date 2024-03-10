using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public float bulletSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * bulletSpeed);
    }

    void OnCollisionEnter(Collision hit)
    {
        if (hit.gameObject.CompareTag("Landscape"))
        {
            Destroy(gameObject);
        }
        else if (hit.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
        else if (hit.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
