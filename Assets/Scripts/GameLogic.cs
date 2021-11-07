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

    private IEnumerator waitThenEnableSleepTime(int waitInSeconds)
    {
        yield return new WaitForSeconds(waitInSeconds);
        sleepTimeMinutesUI.text = "";
        sleepTimeHoursUI.text = "";
        disableGameUI();
        sleepTimeCanvas.enabled = true;
        AIController.onAIPlacedIntoScene -= AIWasPlaced;
    }

    // Update is called once per frame
    void Update()
    {
        if (System.DateTime.Now.Hour == sleepTimeHours && System.DateTime.Now.Minute == sleepTimeMinutes)
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
        gameUICanvas.enabled = false;
        uiPlaneIsOverlaying = true;
    }

    private void enableGameUI()
    {
        gameUICanvas.enabled = true;
        uiPlaneIsOverlaying = false;
    }
}
