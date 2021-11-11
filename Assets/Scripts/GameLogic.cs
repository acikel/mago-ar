using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    //checks if any other ui then gameUICanvas is overlaying game screen so ai wont move in this state
    //is checked by AIcontroller to move AI only if this variable is false (then no UI overlay is done exept the gameUI)
    public static bool uiPlaneIsOverlaying;
    public GameObject gameUICanvas;
    public GameObject blackScreenCanvas;
    public GameObject teethBrushingCanvas;
    public GameObject timeToSleepCanvas;
    public GameObject sleepTimeCanvas;

    public InputField sleepTimeMinutesUI;
    public InputField sleepTimeHoursUI;

    private int sleepTimeMinutes;
    private int sleepTimeHours;
    private bool magoIsSleeping;

    public delegate void OnSleepTimeOKButton();
    public static OnSleepTimeOKButton onSleepTimeOKButton;

    public GameObject foodGameObjectUI;
    public GameObject foodGameObject3D;
    public Button foodButton;
    public GameObject magoRenderer;
    public GameObject mago;

    public Button sleepTimeOkButton;
    public Button timeToSleepOkButton;
    public Button settingsButton;
    public Text timeToSleepText;
    public Text timeToSleepText2;

    public Camera aICamera;
    public Camera secondMagoCamera;
    public static bool magoIsBeeingFeeded;

    public Transform foodPosition;

    public Animator magoAnimator;
    public NavMeshAgent agent;
    public MagoController magoController;
    private Vector3 touchPos;
    private RaycastHit raycastTouchHit;
    private Color magoCameraColor;

    public GameObject appleAnimation;
    public GameObject brushAnimation;

    private bool timeToSleepOkButtonClicked;
    private bool magoIsSitting;
    // Start is called before the first frame update
    void Start()
    {
        magoIsSitting = false;
        secondMagoCamera.enabled = false;
        foodButton.onClick.AddListener(gameUIFoodButton);
        sleepTimeOkButton.onClick.AddListener(sleepTimeOKButton);
        timeToSleepOkButton.onClick.AddListener(timeToSleepOKButton);
        settingsButton.onClick.AddListener(SettingsWasClicked);

        blackScreenCanvas.SetActive(false);
        teethBrushingCanvas.SetActive(false);
        timeToSleepCanvas.SetActive(false);
        sleepTimeCanvas.SetActive(false);
        magoCameraColor = secondMagoCamera.backgroundColor;
        timeToSleepOkButtonClicked = false;

        enableGameUI();
        uiPlaneIsOverlaying = false;
        magoIsSleeping = false;
        AIController.onAIPlacedIntoScene += AIWasPlaced;
        TeethBrushingController.onTimerFinished += OnTeethBrushingTimerFinished;
        FoodScript.onFoodGiven += OnFoodGiven;

        
    }

    
    private void OnTeethBrushingTimerFinished()
    {
        //enableGameUI();
        teethBrushingCanvas.SetActive(false);
        blackScreenCanvas.SetActive(true);
        secondMagoCamera.backgroundColor = Color.black;
        magoIsSleeping = true;
        timeToSleepOkButtonClicked = false;
        //magoController.BrushTeeth();
        ResetAllTriggers();
        magoController.Sleep();
    }

    private void AIWasPlaced()
    {
        //StartCoroutine(waitThenEnableSleepTime(15));
        //Debug.Log("Update Method Game Logic4");
        //yield return new WaitForSeconds(waitInSeconds);
        //Debug.Log("Update Method Game Logic5");
        if (!sleepTimeMinutesUI.text.Equals(""))
            return;
        sleepTimeMinutesUI.text = "";
        sleepTimeHoursUI.text = "";
        disableGameUI();
        sleepTimeCanvas.SetActive(true);
        AIController.onAIPlacedIntoScene -= AIWasPlaced;
        //Debug.Log("Update Method Game Logic6");
    }

    private void SettingsWasClicked()
    {
        disableGameUI();
        sleepTimeCanvas.SetActive(true);
    }

    /*IEnumerator waitThenEnableSleepTime(int waitInSeconds)
    {
       
        
    }*/

    private IEnumerator waitThenGetBackToIdleFromEating(int waitForSeconds)
    {
        yield return new WaitForSeconds(waitForSeconds);
        ResetAllTriggers();
        agent.enabled = true;
        //magoController.EatFood();

    }
    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                //touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                //touchPos = Input.GetTouch(0).position;
                touchPos = Input.GetTouch(0).rawPosition;
                Physics.Raycast(touchPos, aICamera.transform.forward, out raycastTouchHit);
                if (raycastTouchHit.collider.name.Contains("Mago") || raycastTouchHit.collider.name.Equals("AI"))
                {
                    if (agent.isActiveAndEnabled)
                    {
                        agent.enabled = false;
                        mago.transform.LookAt(aICamera.transform);
                        ResetAllTriggers();
                        magoController.SitDown();
                    }
                    else
                    {
                        mago.transform.LookAt(aICamera.transform);
                        agent.enabled = true;
                        ResetAllTriggers();

                    }
                    
                }
            }
            //if (Input.GetTouch(0).phase == TouchPhase.Ended && foodGameObjectUI.activeSelf)
            //{
                
            //}
        }

        //Debug.Log("Update Method Game Logic1: "+ sleepTimeMinutes + " sleepTimeHours: "+ sleepTimeHours + " timeToSleepOkButtonClicked: " + timeToSleepOkButtonClicked + " sleepTimeMinutesUI.text: "+ sleepTimeMinutesUI.text);
        if (!timeToSleepOkButtonClicked && !sleepTimeMinutesUI.text.Equals("")&&(System.DateTime.Now.Hour == sleepTimeHours && System.DateTime.Now.Minute == sleepTimeMinutes))
        //if (!sleepTimeMinutesUI.text.Equals("") && (System.DateTime.Now.Hour == sleepTimeHours && System.DateTime.Now.Minute == sleepTimeMinutes))
        {
            disableGameUI();
            timeToSleepCanvas.SetActive(true);
            //Debug.Log("Animation name: "+magoAnimator.GetCurrentAnimatorStateInfo(0).IsName("Sitting"));
            if (!magoIsSitting)
            {
                magoIsSitting = true;
                ResetAllTriggers();
                magoController.SitDown();
            }
        }
        if(magoIsSleeping && System.DateTime.Now.Hour == ((sleepTimeHours+9)%23))
        {
            blackScreenCanvas.SetActive(false);
            secondMagoCamera.backgroundColor = magoCameraColor;
            magoIsSleeping = false;

            //magoController.Sleep();
            ResetAllTriggers();
            enableGameUI();
        }
    }


    private void OnFoodGiven()
    {
        //foodGameObjectUI.SetActive(false);
        ResetAllTriggers();
        magoController.EatFood();
        StartCoroutine(waitThenGetBackToIdleFromEating(3));
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

            DisplayTime(timeToSleepText, (sleepTimeHours + 9) % 23, sleepTimeMinutes);
            DisplayTime(timeToSleepText2, (sleepTimeHours + 9) % 23, sleepTimeMinutes);
            sleepTimeCanvas.SetActive(false);
            enableGameUI();
            onSleepTimeOKButton?.Invoke();
        }
        
    }
    public void timeToSleepOKButton()
    {
        //Debug.Log("OK button clicked");
        timeToSleepOkButtonClicked = true;
        magoIsSitting = false;
        timeToSleepCanvas.SetActive(false);
        teethBrushingCanvas.SetActive(true);

        //magoController.SitDown();
        ResetAllTriggers();
        magoController.BrushTeeth();

    }

    public void gameUIFoodButton()
    {
        magoIsBeeingFeeded = true;
        //Debug.Log("seeing mago1: "+ magoRenderer + " renderer: "+ magoRenderer.GetComponent<Renderer>() + " is visible? " + magoRenderer.GetComponent<Renderer>().isVisible + " uiPlaneIsOverlaying: "+ uiPlaneIsOverlaying);
        if (!uiPlaneIsOverlaying && magoRenderer.GetComponent<Renderer>().isVisible)
        {
            //Debug.Log("seeing mago2");

            agent.enabled = false;
            mago.transform.LookAt(aICamera.transform);
            ResetAllTriggers();
            magoController.ReadyForFood();
            foodGameObjectUI.SetActive(true);

            /*
            //Vector3 position3D = Camera.main.WorldToScreenPoint(new Vector3(foodButton.transform.position.x, foodButton.transform.position.y - 4, foodButton.transform.position.z));
            Vector3 position3D = aICamera.WorldToScreenPoint(new Vector3(foodButton.transform.position.x, foodButton.transform.position.y + 1, foodButton.transform.position.z+10));
            Debug.Log("seeing mago3 touch position: "+ touchPos);
            Debug.Log("seeing mago3 button position: " + position3D);
            Debug.Log("seeing mago3 raycastTouchHit position: " + raycastTouchHit.point);

            Vector3 magoPos = new Vector3(mago.transform.position.x, mago.transform.position.y+1, mago.transform.position.z);
            Debug.Log("seeing mago3 magoPos position: " + magoPos);
            touchPos.y = touchPos.y + 1;
            touchPos.z = touchPos.z + 2;
            magoPos.z = magoPos.z + 10;
            //Instantiate(foodGameObject, position3D, Quaternion.identity);
            //Instantiate(foodGameObjectTest, touchPos, Quaternion.identity);
            Instantiate(foodGameObject, magoPos, Quaternion.identity);
            Instantiate(foodGameObjectTest, raycastTouchHit.point, Quaternion.identity);
            */


            //Instantiate(foodGameObject3D, foodPosition.position, Quaternion.identity);
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
        secondMagoCamera.enabled = true;
    }

    private void enableGameUI()
    {
        //Debug.Log("Update Method Game Logic3");
        gameUICanvas.SetActive(true);
        uiPlaneIsOverlaying = false;
        secondMagoCamera.enabled = false;
    }


    void DisplayTime(Text timeText,float timeHours, float timeMins)
    {
        //timeHours=timeHours-1;
        timeText.text = string.Format("{0:00}:{1:00}", timeHours, timeMins);
    }

    private void ResetAllTriggers()
    {
        
        foreach (var param in magoAnimator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                magoAnimator.ResetTrigger(param.name);
            }
        }
        appleAnimation.SetActive(false);
        brushAnimation.SetActive(false);
    }

}
