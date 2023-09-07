using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public static int timeLimit = 120;

    static int timeLeft;

    public static int TimeLeft { get => timeLeft; set => timeLeft = value; }

    // Start is called before the first frame update
    void Start()
    {
        TimeLeft = timeLimit;
        setTimerText();
        InvokeRepeating("ClockDown", 0, 1);
    }

    void ClockDown()
    {
        if(TimeLeft <= 0)
        {
            TempestController.tc.Win = true;
            TempestController.tc.EndGame();
            return;
        }
        TimeLeft--;
        setTimerText();
    }

    void setTimerText()
    {
        int minute = TimeLeft / 60;
        int second = TimeLeft % 60;
        gameObject.GetComponent<TextMeshProUGUI>().text = string.Format("{0:00}:{1:00}", minute, second);
    }

    public static void Restart()
    {
        TimeLeft = timeLimit; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
