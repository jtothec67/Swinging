using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeTrialManager : MonoBehaviour
{
    public List<GameObject> hoops;
    private List<TTHoop> hoopScripts = new List<TTHoop>();

    private int activeHoop = 0;

    public float timer = 0f;

    public TextMeshProUGUI timerText;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < hoops.Count; i++)
        {
            TTHoop script = hoops[i].GetComponent<TTHoop>();
            if (script != null) hoopScripts.Add(script);

            if (i == 0)
            {
                hoops[i].SetActive(true);
                hoopScripts[i].isActive = true;
            }
            if (i == 1)
            {
                hoops[i].SetActive(true);
                hoopScripts[i].isNext = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (activeHoop != 0)
        {
            timer += Time.deltaTime;
            string time = timer.ToString("0.000");
            timerText.text = time;
        }
    }

    public void ThroughCurrentHoop()
    {
        activeHoop++;

        if (activeHoop == hoops.Count)
        {
            // Finished
            activeHoop = 0;
            timer = 0f;

            hoops[activeHoop].SetActive(true);
            hoopScripts[activeHoop].isActive = true;

            hoops[activeHoop + 1].SetActive(true);
            hoopScripts[activeHoop + 1].isNext = true;

            return;
        }

        hoops[activeHoop].SetActive(true);
        hoopScripts[activeHoop].isActive = true;

        if (activeHoop == hoops.Count - 1)
        {
            // Don't try and do next hoop
            return;
        }

        hoops[activeHoop+1].SetActive(true);
        hoopScripts[activeHoop+1].isNext = true;
    }
}
