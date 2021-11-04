using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class AIController : MonoBehaviour
{
    public ARPlaneManager aRPlaneManager;

    // Start is called before the first frame update
    void Start()
    {
        aRPlaneManager.planesChanged += initializingAgent;
    }



    // Update is called once per frame
    void Update()
    {
        
    }


    private void initializingAgent(ARPlanesChangedEventArgs args)
    {
        foreach (ARPlane arPlane in args.added)
        {
            if (arPlane.classification.Equals(PlaneClassification.Floor))
            {
                transform.position = arPlane.transform.position;
                enabled = true;
                aRPlaneManager.planesChanged -= initializingAgent;
                aRPlaneManager.planesChanged += checkForNewSeatTableFloor;
            }
        }
    }

    private void checkForNewSeatTableFloor(ARPlanesChangedEventArgs args)
    {
        foreach (ARPlane arPlane in args.added)
        {
            //if (arPlane.classification.Equals(PlaneClassification.Floor) || arPlane.classification.Equals(PlaneClassification.Seat) || arPlane.classification.Equals(PlaneClassification.Table))
            //{
            //    transform.position = arPlane.transform.position;
            //}
        }
    }
}
