using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagoController : MonoBehaviour
{
    public Animator myAnimatorController;

    public GameObject apple;

    public GameObject brush;

    public GameObject sleepyMask;

    public float moveSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        if (apple.activeSelf) apple.SetActive(false);
        if (brush.activeSelf) brush.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        transform.parent.transform.Translate(Vector3.forward * moveSpeed);

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartOrStopRunning();
        }
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ReadyForFood();
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            EatFood();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            BrushTeeth();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Sleep();
        }
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            SitDown();
        }

        
    }

    public void StartOrStopRunning()
    {
        if (apple.activeSelf) apple.SetActive(false);
        if (brush.activeSelf) brush.SetActive(false);
        myAnimatorController.SetTrigger("triggerRun");
    }

    public void ReadyForFood()
    {
        if (apple.activeSelf) apple.SetActive(false);
        if (brush.activeSelf) brush.SetActive(false);
        myAnimatorController.SetTrigger("triggerReadyForFood");
    }
    public void EatFood()
    {
        apple.SetActive(true);
        if (brush.activeSelf) brush.SetActive(false);
        apple.GetComponent<Animator>().Play("Apple|AppleAction", -1, 0f);
        myAnimatorController.SetTrigger("triggerEat");
    }

    public void BrushTeeth()
    {
        if (apple.activeSelf) apple.SetActive(false);
        if (!brush.activeSelf)
        {
            brush.SetActive(true);
            myAnimatorController.SetTrigger("triggerBrushTeeth");
        }
        else if (brush.activeSelf)
        {
            brush.SetActive(false);
            myAnimatorController.SetTrigger("triggerBrushTeeth");
        }
    }

    public void Sleep()
    {
        if (brush.activeSelf) brush.SetActive(false);
        if (apple.activeSelf) apple.SetActive(false);

        sleepyMask.SetActive(!sleepyMask.activeSelf);
        myAnimatorController.SetTrigger("triggerSleep");
    }

    public void SitDown()
    {
        if (apple.activeSelf) apple.SetActive(false);
        if (brush.activeSelf) brush.SetActive(false);
        myAnimatorController.SetTrigger("triggerSit");
    }
}
