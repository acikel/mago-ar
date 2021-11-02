using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacement : MonoBehaviour
{
    public GameObject arObjectToSpawn;
    public GameObject placementIndicator;

    private Pose placementPose;
    private ARRaycastManager aRRaycastManager;
    private bool placementPoseValid = false;
    private GameObject spawnedObject;

    void Start()
    {
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();
    }

    void Update()
    {
        if (spawnedObject == null && placementPoseValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            arPlaceObject();
        }

        updatePlacementPose();
        updatePlacementIndicator();
    }

    void updatePlacementIndicator()
    {
        if(spawnedObject==null && placementPoseValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    void updatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        aRRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseValid = hits.Count > 0;
        if(placementPoseValid)
        {
            placementPose = hits[0].pose;
        }
    }

    void arPlaceObject()
    {
        spawnedObject = Instantiate(arObjectToSpawn, placementPose.position, placementPose.rotation);
    }
}
