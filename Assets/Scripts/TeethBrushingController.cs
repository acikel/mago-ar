using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeethBrushingController : MonoBehaviour
{
    public delegate void OnTimerFinished();
    public static OnTimerFinished onTimerFinished;

    public float timeRemaining = 3;
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
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                //Debug.Log("Time has run out!");
                timeRemaining = 3;
                timerIsRunning = false;
                onTimerFinished?.Invoke();
            }
        }
    }

    private void OnEnable()
    {
        //Starts timer.
        timeRemaining = 3;
        timerIsRunning = true;
        DisplayTime(timeRemaining);
    }

    private void OnDisable()
    {
        timeRemaining = 3;
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
