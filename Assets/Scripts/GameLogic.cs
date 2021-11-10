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
    public GameObject gameUICanvas;
    public GameObject blackScreenCanvas;
    public GameObject teethBrushingCanvas;
    public GameObject sleepTimeCanvas;

    public InputField sleepTimeMinutesUI;
    public InputField sleepTimeHoursUI;

    private int sleepTimeMinutes;
    private int sleepTimeHours;
    private bool magoIsSleeping;

    public delegate void OnSleepTimeOKButton();
    public static OnSleepTimeOKButton onSleepTimeOKButton;

    public GameObject foodGameObject;
    public GameObject foodGameObjectTest;
    public Button foodButton;
    public GameObject magoRenderer;
    public GameObject mago;

    public Button sleepTimeOkButton;

    public Camera aICamera;
    public static bool magoIsBeeingFeeded;

    private Vector3 touchPos;
    private RaycastHit raycastTouchHit;
    // Start is called before the first frame update
    void Start()
    {
        foodButton.onClick.AddListener(gameUIFoodButton);
        sleepTimeOkButton.onClick.AddListener(sleepTimeOKButton);

        blackScreenCanvas.SetActive(false);
        teethBrushingCanvas.SetActive(false);
        sleepTimeCanvas.SetActive(false);

        enableGameUI();
        uiPlaneIsOverlaying = false;
        magoIsSleeping = false;
        AIController.onAIPlacedIntoScene += AIWasPlaced;
        TeethBrushingController.onTimerFinished += OnTeethBrushingTimerFinished;

        
    }

    private void OnTeethBrushingTimerFinished()
    {
        //enableGameUI();
        teethBrushingCanvas.SetActive(false);
        blackScreenCanvas.SetActive(true);
        magoIsSleeping = true;
    }

    private void AIWasPlaced()
    {
        //StartCoroutine(waitThenEnableSleepTime(15));
        //Debug.Log("Update Method Game Logic4");
        //yield return new WaitForSeconds(waitInSeconds);
        //Debug.Log("Update Method Game Logic5");
        sleepTimeMinutesUI.text = "";
        sleepTimeHoursUI.text = "";
        disableGameUI();
        sleepTimeCanvas.SetActive(true);
        AIController.onAIPlacedIntoScene -= AIWasPlaced;
        //Debug.Log("Update Method Game Logic6");
    }

    /*IEnumerator waitThenEnableSleepTime(int waitInSeconds)
    {
       
        
    }*/

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            if (Input.GetTouch(0).phase == TouchPhase.Began)
                Physics.Raycast(touchPos, Vector3.zero,out raycastTouchHit);
        }

        //Debug.Log("Update Method Game Logic1: "+ uiPlaneIsOverlaying);
        if (!sleepTimeMinutesUI.text.Equals("")&&(System.DateTime.Now.Hour == sleepTimeHours && System.DateTime.Now.Minute == sleepTimeMinutes))
        {
            disableGameUI();
            teethBrushingCanvas.SetActive(true);
        }
        if(magoIsSleeping && System.DateTime.Now.Hour == (sleepTimeHours+9%23))
        {
            blackScreenCanvas.SetActive(false);
            magoIsSleeping = false;
            enableGameUI();
        }
    }

    public void sleepTimeOKButton()
    {
        //Debug.Log("OK button clicked");
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

            sleepTimeCanvas.SetActive(false);
            enableGameUI();
            onSleepTimeOKButton?.Invoke();
        }
        
    }


    public void gameUIFoodButton()
    {
        magoIsBeeingFeeded = true;
        Debug.Log("seeing mago1: "+ magoRenderer + " renderer: "+ magoRenderer.GetComponent<Renderer>() + " is visible? " + magoRenderer.GetComponent<Renderer>().isVisible + " uiPlaneIsOverlaying: "+ uiPlaneIsOverlaying);
        if (!uiPlaneIsOverlaying && magoRenderer.GetComponent<Renderer>().isVisible)
        {
            Debug.Log("seeing mago2");

            //mago.transform.LookAt(aICamera.transform);
            
            //Vector3 position3D = Camera.main.WorldToScreenPoint(new Vector3(foodButton.transform.position.x, foodButton.transform.position.y - 4, foodButton.transform.position.z));
            Vector3 position3D = aICamera.WorldToScreenPoint(new Vector3(foodButton.transform.position.x, foodButton.transform.position.y + 1, foodButton.transform.position.z));
            Debug.Log("seeing mago3 touch position: "+ touchPos);
            Debug.Log("seeing mago3 button position: " + position3D);
            
            Vector3 magoPos = new Vector3(mago.transform.position.x, mago.transform.position.y+1, mago.transform.position.z);
            Debug.Log("seeing mago3 magoPos position: " + magoPos);
            //Instantiate(foodGameObject, position3D, Quaternion.identity);
            //Instantiate(foodGameObject, touchPos, Quaternion.identity);
            Instantiate(foodGameObject, magoPos, Quaternion.identity);
            Instantiate(foodGameObjectTest, position3D, Quaternion.identity);

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
        //Debug.Log("Update Method Game Logic2");
        gameUICanvas.SetActive(false);
        uiPlaneIsOverlaying = true;
    }

    private void enableGameUI()
    {
        //Debug.Log("Update Method Game Logic3");
        gameUICanvas.SetActive(true);
        uiPlaneIsOverlaying = false;
    }
}
