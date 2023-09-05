using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public static int timeLimit = 120;

    static int timer;
    // Start is called before the first frame update
    void Start()
    {
        timer = timeLimit;
        setTimerText();
        InvokeRepeating("ClockDown", 0, 1);
    }

    void ClockDown()
    {
        if(timer <= 0)
        {
            TempestController.tc.Win = true;
            TempestController.tc.EndGame();
            return;
        }
        timer--;
        setTimerText();
    }

    void setTimerText()
    {
        int minute = timer / 60;
        int second = timer % 60;
        gameObject.GetComponent<TextMeshProUGUI>().text = string.Format("{0:00}:{1:00}", minute, second);
    }

    public static void Restart()
    {
        timer = timeLimit; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
