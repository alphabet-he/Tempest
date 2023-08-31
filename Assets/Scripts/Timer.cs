using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public int timeLimit = 120;
    // Start is called before the first frame update
    void Start()
    {
        setTimerText();
        InvokeRepeating("ClockDown", 0, 1);
    }

    void ClockDown()
    {
        if(timeLimit <= 0)
        {
            TempestController.tc.EndGame();
            return;
        }
        timeLimit--;
        setTimerText();
    }

    void setTimerText()
    {
        int minute = timeLimit / 60;
        int second = timeLimit % 60;
        gameObject.GetComponent<TextMeshProUGUI>().text = string.Format("{0:00}:{1:00}", minute, second);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
