using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class NavMeshBuilder : MonoBehaviour
{
    //public GameObject planeToAddNavMesh;
    public ARPlaneManager aRPlaneManager;

    // Start is called before the first frame update
    void Start()
    {
        //NavMeshSurface nms = planeToAddNavMesh.AddComponent(typeof(NavMeshSurface)) as NavMeshSurface;
        //nms.BuildNavMesh();
        
        aRPlaneManager.planesChanged += updateNavMesh;
    }

    private void OnDisable()
    {
        aRPlaneManager.planesChanged -= updateNavMesh;
    }
    private void updateNavMesh(ARPlanesChangedEventArgs args)
    {
        //If new floor, seat or table plane was added, add a navmesh surface to it and bake it.
        foreach(ARPlane arPlane in args.added)
        {   
            if(arPlane.classification.Equals(PlaneClassification.Floor) || arPlane.classification.Equals(PlaneClassification.Seat) || arPlane.classification.Equals(PlaneClassification.Table))
            {
                createAndBackeNavMeshSurface(arPlane);
            }
        }

        //If a floor, seat or table plane was modified then rebake its NavMeshSurface 
        foreach (ARPlane arPlane in args.updated)
        {
            NavMeshSurface nms = arPlane.gameObject.GetComponent<NavMeshSurface>();
            if (nms != null && (arPlane.classification.Equals(PlaneClassification.Floor) || arPlane.classification.Equals(PlaneClassification.Seat) || arPlane.classification.Equals(PlaneClassification.Table)))
            {
                nms.BuildNavMesh();
            }
        }
        
        
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    private void createAndBackeNavMeshSurface(ARPlane arPlane)
    {
        NavMeshSurface nms = arPlane.gameObject.AddComponent(typeof(NavMeshSurface)) as NavMeshSurface;
        nms.BuildNavMesh();
    }
}
