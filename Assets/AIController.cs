using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class AIController : MonoBehaviour
{
    public ARPlaneManager aRPlaneManager;
    private List<ARPlane> listOfARPlanesWithNavmesh;
    private NavMeshAgent agent;
    private bool searchNewLocation;
    private ARPlane currentPlaneMagoIsPositioned;
    private bool nexPositionFound;
    // Start is called before the first frame update
    void Awake()
    {
        nexPositionFound = false;
        //aRPlaneManager.planesChanged += InitializingAgent;
        enabled = false;
        listOfARPlanesWithNavmesh = new List<ARPlane>();
        agent = GetComponent<NavMeshAgent>();
        searchNewLocation = false;
        currentPlaneMagoIsPositioned = null;
    }

    private void Start()
    {
        StartCoroutine(WaitBeforeSearchingNewPositionCoroutine(30));
        StartCoroutine(WaitThenInitializeMago(30));
    }


    // Update is called once per frame
    void Update()
    {
        if (searchNewLocation) 
        { 
            StartCoroutine(GetNewNavMeshPositionCoroutine(10));
        }
    }

    IEnumerator WaitBeforeSearchingNewPositionCoroutine(int waitInSeconds)
    {
        yield return new WaitForSeconds(waitInSeconds);
        searchNewLocation = true;
    }

    IEnumerator GetNewNavMeshPositionCoroutine(int waitInSeconds)
    {
        //Print the time of when the function is first called.
        //Debug.Log("Started Coroutine at timestamp : " + Time.time);
        searchNewLocation = false;
        if (currentPlaneMagoIsPositioned!=null)
            generateRandomDestination();
        nexPositionFound = false;
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitUntil(() => nexPositionFound == true);
        yield return new WaitForSeconds(waitInSeconds);
        searchNewLocation = true;
        //After we have waited 5 seconds print the time again.
        //Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }

    private void generateRandomDestination()
    {
        //Get random walkable plane
        int randomIndex = Random.Range(0, listOfARPlanesWithNavmesh.Count);
        currentPlaneMagoIsPositioned = listOfARPlanesWithNavmesh[randomIndex];

        float maxDistance = GetFarestBoundry(currentPlaneMagoIsPositioned);
        while (!nexPositionFound)
        {
            // Get Random Point inside Sphere which position is center, radius is maxDistance
            Vector3 randomPos = Random.insideUnitSphere * maxDistance + currentPlaneMagoIsPositioned.center;

            NavMeshHit hit; // NavMesh Sampling Info Container

            // from randomPos find a nearest point on NavMesh surface in range of maxDistance
            if (NavMesh.SamplePosition(randomPos, out hit, maxDistance, NavMesh.AllAreas) && hit.position != agent.gameObject.transform.position)
            {
                agent.SetDestination(hit.position);
                nexPositionFound = true;
            }
        }
        
        /*
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Bounds bounds = mesh.bounds;
        float radius = bounds.extents.magnitude;
        //oder:
        float diameter = bounds.extents.x * 2;
        */
    }

    IEnumerator WaitThenInitializeMago(int waitInSeconds)
    {

        yield return new WaitForSeconds(waitInSeconds);
        InitializeAgent();
    }
    private void InitializeAgent()
    {
        
        foreach (ARPlane arPlane in aRPlaneManager.trackables)
        {
            initializeStartPlaneForMago(arPlane);
            checkForFloorSeatTableAndAddToList(arPlane);
        }
        /*
        ARPlane firstARFloor = null;
        foreach (ARPlane arPlane in added)
        {
            setFirstFloor(arPlane, firstARFloor);
            checkForFloorSeatTableAndAddToList(arPlane);
        }

        foreach (ARPlane arPlane in updated)
        {
            setFirstFloor(arPlane, firstARFloor);
            checkForFloorSeatTableAndAddToList(arPlane);
            checkIfClassificationChanged(arPlane);
        }

        foreach (ARPlane arPlane in removed)
        {
            checkForFloorSeatTableAndRemoveFromList(arPlane);
        }

        transform.position = firstARFloor.transform.position;
        enabled = true;
        */
        //aRPlaneManager.planesChanged -= InitializingAgent;
        aRPlaneManager.planesChanged += checkForNewSeatTableFloor;
    }

    private float GetFarestBoundry(ARPlane arPlane)
    {
        float maxRadius = 0;
        foreach (Vector2 boundary in arPlane.boundary)
        {
            if (boundary.magnitude > maxRadius)
                maxRadius = boundary.magnitude;
        }
        return maxRadius;
    }

    /*private void InitializingAgent(ARPlanesChangedEventArgs args)
    {
        StartCoroutine(WaitThenInitializeMago(30));
    }*/

    private void checkForNewSeatTableFloor(ARPlanesChangedEventArgs args)
    {
        foreach (ARPlane arPlane in args.added)
        {
            if (currentPlaneMagoIsPositioned == null)
                initializeStartPlaneForMago(arPlane);
            checkForFloorSeatTableAndAddToList(arPlane);
        }

        foreach (ARPlane arPlane in args.updated)
        {
            if (currentPlaneMagoIsPositioned == null)
                initializeStartPlaneForMago(arPlane);
            checkForFloorSeatTableAndAddToList(arPlane);
            checkIfClassificationChanged(arPlane);
        }

        foreach (ARPlane arPlane in args.removed)
        {
            checkForFloorSeatTableAndRemoveFromList(arPlane);
        }
    }





    private void checkForFloorSeatTableAndAddToList(ARPlane arPlane)
    {
        if (!listOfARPlanesWithNavmesh.Contains(arPlane) && (arPlane.classification.Equals(PlaneClassification.Seat) || arPlane.classification.Equals(PlaneClassification.Table) || arPlane.classification.Equals(PlaneClassification.Floor)))
            listOfARPlanesWithNavmesh.Add(arPlane);
    }

    private void checkForFloorSeatTableAndRemoveFromList(ARPlane arPlane)
    {
        if (listOfARPlanesWithNavmesh.Contains(arPlane))
        {
            listOfARPlanesWithNavmesh.Remove(arPlane);
            if (currentPlaneMagoIsPositioned.Equals(arPlane))
                currentPlaneMagoIsPositioned = null;
        }
    }
    private void checkIfClassificationChanged(ARPlane arPlane)
    {
        if (listOfARPlanesWithNavmesh.Contains(arPlane) && !(arPlane.classification.Equals(PlaneClassification.Seat) || arPlane.classification.Equals(PlaneClassification.Table) || arPlane.classification.Equals(PlaneClassification.Floor)))
            listOfARPlanesWithNavmesh.Remove(arPlane);
    }

    private void initializeStartPlaneForMago(ARPlane arPlane)
    {
        if (arPlane.classification.Equals(PlaneClassification.Floor))
        {
            if (currentPlaneMagoIsPositioned == null)
            {
                if (!enabled)
                    enabled = true;
                currentPlaneMagoIsPositioned = arPlane;
                transform.position = currentPlaneMagoIsPositioned.transform.position;
            }
        }
    }

    
}
