using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    //checks if any other ui then gameUICanvas is overlaying game screen so ai wont move in this state
    //is checked by AIcontroller to move AI only if this variable is false (then no UI overlay is done exept the gameUI)
    public static bool uiPlaneIsOverlaying;
    public Canvas gameUICanvas;
    public Canvas blackScreenCanvas;
    public Canvas teethBrushingCanvas;
    public Canvas sleepTimeCanvas;

    public InputField sleepTimeMinutesUI;
    public InputField sleepTimeHoursUI;

    private int sleepTimeMinutes;
    private int sleepTimeHours;
    private bool magoIsSleeping;

    // Start is called before the first frame update
    void Start()
    {
        blackScreenCanvas.enabled = false;
        teethBrushingCanvas.enabled = false;
        sleepTimeCanvas.enabled = false;

        enableGameUI();
        uiPlaneIsOverlaying = false;
        magoIsSleeping = false;
        AIController.onAIPlacedIntoScene += AIWasPlaced;
        TeethBrushingController.onTimerFinished += OnTeethBrushingTimerFinished;
    }

    private void OnTeethBrushingTimerFinished()
    {
        //enableGameUI();
        teethBrushingCanvas.enabled = false;
        blackScreenCanvas.enabled = true;
        magoIsSleeping = true;
    }

    private void AIWasPlaced()
    {
        StartCoroutine(waitThenEnableSleepTime(15));
    }

    IEnumerator waitThenEnableSleepTime(int waitInSeconds)
    {
        uiPlaneIsOverlaying = true;
        Debug.Log("Update Method Game Logic4");
        yield return new WaitForSeconds(waitInSeconds);
        Debug.Log("Update Method Game Logic5");
        sleepTimeMinutesUI.text = "";
        sleepTimeHoursUI.text = "";
        disableGameUI();
        sleepTimeCanvas.enabled = true;
        AIController.onAIPlacedIntoScene -= AIWasPlaced;
        Debug.Log("Update Method Game Logic6");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Update Method Game Logic1: "+ uiPlaneIsOverlaying);
        if (!sleepTimeMinutesUI.text.Equals("")&&(System.DateTime.Now.Hour == sleepTimeHours && System.DateTime.Now.Minute == sleepTimeMinutes))
        {
            disableGameUI();
            teethBrushingCanvas.enabled = true;
        }
        if(magoIsSleeping && System.DateTime.Now.Hour == (sleepTimeHours+9%23))
        {
            blackScreenCanvas.enabled = false;
            magoIsSleeping = false;
            enableGameUI();
        }
    }

    public void sleepTimeOKButton()
    {
        if (checkIfTimeWasEntered())
        {
            if (sleepTimeHoursUI.text.Equals(""))
                sleepTimeHours = 0;
            else
                sleepTimeHours = int.Parse(sleepTimeHoursUI.text);

            if (sleepTimeMinutesUI.text.Equals(""))
                sleepTimeMinutes = 0;
            else
                sleepTimeMinutes = int.Parse(sleepTimeMinutesUI.text);

            sleepTimeCanvas.enabled = false;
            enableGameUI();
        }
        
    }

    private bool checkIfTimeWasEntered()
    {
        if (!sleepTimeHoursUI.text.Equals("") || !sleepTimeMinutesUI.text.Equals(""))
            return true;
        else
            return false;
    }

    private void disableGameUI()
    {
        Debug.Log("Update Method Game Logic2");
        gameUICanvas.enabled = false;
        uiPlaneIsOverlaying = true;
    }

    private void enableGameUI()
    {
        Debug.Log("Update Method Game Logic3");
        gameUICanvas.enabled = true;
        uiPlaneIsOverlaying = false;
    }
}
