using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTHoop : MonoBehaviour
{
    public TimeTrialManager managerScript;

    public bool isActive = false;
    public bool isNext = false;

    public Material activeMat;
    public Material nextMat;

    private MeshRenderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        renderer = gameObject.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive || isNext)
        {
            if (isActive) renderer.material = activeMat;
            else renderer.material = nextMat;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        if (other.gameObject.tag == "Player")
        {
            managerScript.ThroughCurrentHoop();

            isActive = false;
            gameObject.SetActive(false);
        }
    }
}
