using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeethBrushingController : MonoBehaviour
{
    public delegate void OnTimerFinished();
    public static OnTimerFinished onTimerFinished;

    private const int timeForCountdownInSeconds = 180;
    public float timeRemainingInSeconds = timeForCountdownInSeconds;
    private bool timerIsRunning=false;
    public Text timeText;
    

    // Start is called before the first frame update
    void Start()
    {
        //timerIsRunning = false;
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemainingInSeconds > 0)
            {
                timeRemainingInSeconds -= Time.deltaTime;
                DisplayTime(timeRemainingInSeconds);
            }
            else
            {
                //Debug.Log("Time has run out!");
                timerIsRunning = false;
                timeRemainingInSeconds = timeForCountdownInSeconds;
                onTimerFinished?.Invoke();
            }
        }
    }

    private void OnEnable()
    {
        //Starts timer.
        timeRemainingInSeconds = timeForCountdownInSeconds;
        timerIsRunning = true;
        DisplayTime(timeRemainingInSeconds);
    }

    private void OnDisable()
    {
        timeRemainingInSeconds = timeForCountdownInSeconds;
        timerIsRunning = false;
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
