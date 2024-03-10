using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] audioClips;
    private int currentClip = 0;

    public Transform cameraTransform;
    public GameObject leftHand;
    public GameObject rightHand;
    public GameObject bullet;

    public GameObject[] bullsLeftUI;

    public int bullsTotal = 6;
    private int bullsLeft = 6;
    private float timer = 0f;
    private bool currentHandL = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0;  i < bullsLeftUI.Length; i++)
        {
            if (bullsLeft > i)
            {
                bullsLeftUI[i].SetActive(true);
            }
            else
            {
                bullsLeftUI[i].SetActive(false);
            }
        }

        if (bullsLeft < bullsTotal)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
        }
        
        
        if (bullsLeft > 0 && Input.GetKeyDown(KeyCode.LeftShift))
        {
            ShootBullet();
            bullsLeft--;
        }

        if(timer > 1.5f && bullsLeft < bullsTotal)
        {
            bullsLeft++;
            timer = 0;
        }
    }

    void ShootBullet()
    {
        RaycastHit bulletTravel;
        Physics.Raycast(cameraTransform.position, cameraTransform.forward, out bulletTravel, Mathf.Infinity);

        if (currentHandL)
        {
            Vector3 relativePos = bulletTravel.point - leftHand.transform.position;
            Instantiate(bullet, leftHand.transform.position, Quaternion.LookRotation(relativePos, Vector3.up));
            currentHandL = false;
        }
        else
        {
            Vector3 relativePos = bulletTravel.point - rightHand.transform.position;
            Instantiate(bullet, rightHand.transform.position, Quaternion.LookRotation(relativePos, Vector3.up));
            currentHandL = true;
        }

        PlaySound();
    }

    void PlaySound()
    {
        audioSource.PlayOneShot(audioClips[currentClip]);
        currentClip++;

        if (currentClip >= audioClips.Length) currentClip = 0;
    }
}
