using System;
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
    private List<GameObject> renderersOfMago;
    private IEnumerator coroutineNewAIPosition;
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    public delegate void OnAIPlacedIntoScene();
    public static OnAIPlacedIntoScene onAIPlacedIntoScene;

    private void Start()
    {
        //Debug.Log("Start Method");
        renderersOfMago = new List<GameObject>();

        //renderersOfMago.AddRange(transform.GetChild(0).gameObject.GetComponentsInChildren<Renderer>());
        //Debug.Log("Start Method child name1: " + transform.GetChild(0).gameObject.name);
        Transform parentTransform = transform.GetChild(0).gameObject.transform;
        for(int i =0; i< parentTransform.childCount; i++)
        {
            //Debug.Log("Start Method child name2: " + parentTransform.GetChild(i).gameObject.name);
            parentTransform.GetChild(i).gameObject.SetActive(false);
            renderersOfMago.Add(parentTransform.GetChild(i).gameObject);
        }

        /*
        foreach (Transform childTransform in transform.GetChild(0).gameObject.transform)
        {
            Debug.Log("Start Method child name3: "+ childTransform.name);
            childTransform.gameObject.SetActive(false);
            renderersOfMago.Add(childTransform.gameObject);
        }
        */
        GameLogic.onSleepTimeOKButton += OnSleepTimeOkButtonPressed;
        nexPositionFound = false;
        //aRPlaneManager.planesChanged += InitializingAgent;
        //enabled = false;
        listOfARPlanesWithNavmesh = new List<ARPlane>();
        agent = GetComponent<NavMeshAgent>();
        //searchNewLocation = false;
        currentPlaneMagoIsPositioned = null;
        StartCoroutine(WaitBeforeSearchingNewPositionCoroutine(10));
        //Debug.Log("Start Method1");
        StartCoroutine(WaitThenInitializeMago(10));
        //Debug.Log("Start Method2");
    }

    private void OnSleepTimeOkButtonPressed()
    {
        searchNewLocation = true;
    }


    // Update is called once per frame
    void Update()
    {
        //Debug.Log("update called: "+searchNewLocation +" uPlane: "+ GameLogic.uiPlaneIsOverlaying);
        if (searchNewLocation && !GameLogic.uiPlaneIsOverlaying) 
        {
            //Debug.Log("WaitBeforeSearchingNewPositionCoroutine3");
            //Debug.Log("Update Method1");
            coroutineNewAIPosition =GetNewNavMeshPositionCoroutine(10);
            StartCoroutine(coroutineNewAIPosition);
        }
        if (GameLogic.uiPlaneIsOverlaying)
        {
            if (coroutineNewAIPosition != null)
            {
                StopCoroutine(coroutineNewAIPosition);
                //Debug.Log("Update Method2");
            }
            searchNewLocation = false;
            //Debug.Log("Update Method3");
        }
    }

    
    IEnumerator WaitBeforeSearchingNewPositionCoroutine(int waitInSeconds)
    {
        //Debug.Log("WaitBeforeSearchingNewPositionCoroutine1");
        yield return new WaitForSeconds(waitInSeconds);
        searchNewLocation = true;
        //Debug.Log("WaitBeforeSearchingNewPositionCoroutine2");
    }

    IEnumerator GetNewNavMeshPositionCoroutine(int waitInSeconds)
    {
        //Print the time of when the function is first called.
        //Debug.Log("Update Method4");
        //Debug.Log("GetNewNavMeshPositionCoroutine1");
        //Debug.Log("WaitBeforeSearchingNewPositionCoroutine4"+ currentPlaneMagoIsPositioned);
        searchNewLocation = false;
        nexPositionFound = false;
        if (currentPlaneMagoIsPositioned!=null)
            generateRandomDestination();
        else
        {
            nexPositionFound = true;
        }
        
        //yield on a new YieldInstruction that waits for 5 seconds.
        //yield return new WaitUntil(() => nexPositionFound == true);
        while (!nexPositionFound)
        {
            yield return null;
        }
        //Debug.Log("GetNewNavMeshPositionCoroutine3");
        yield return new WaitForSeconds(waitInSeconds);
        searchNewLocation = true;
        //After we have waited 5 seconds print the time again.
        //Debug.Log("Finished Coroutine at timestamp : " + Time.time);
        //Debug.Log("GetNewNavMeshPositionCoroutine4");
    }

    private void generateRandomDestination()
    {
        //Get random walkable plane
        //int randomIndex = Random.Range(0, listOfARPlanesWithNavmesh.Count);
        //currentPlaneMagoIsPositioned = listOfARPlanesWithNavmesh[randomIndex];

        float maxDistance = GetFarestBoundry(currentPlaneMagoIsPositioned);
        int counter = 0;
        while (!nexPositionFound)
        {
            if (counter == 25)
            {
                counter = 0;
                maxDistance = GetClosestBoundry(currentPlaneMagoIsPositioned);
            }
            // Get Random Point inside Sphere which position is center, radius is maxDistance
            Vector3 randomPos = UnityEngine.Random.insideUnitSphere * maxDistance + currentPlaneMagoIsPositioned.center;

            NavMeshHit hit; // NavMesh Sampling Info Container
            //Debug.Log("GetNewNavMeshPositionCoroutine2" + randomPos +" maxDistance: "+ maxDistance + " center: "+ currentPlaneMagoIsPositioned.center);
            // from randomPos find a nearest point on NavMesh surface in range of maxDistance
            if (NavMesh.SamplePosition(randomPos, out hit, maxDistance, NavMesh.AllAreas) && hit.position != agent.gameObject.transform.position)
            {
                //Debug.Log("GetNewNavMeshPositionCoroutine2" + hit.position);
                agent.SetDestination(hit.position);
                nexPositionFound = true;
                //Debug.Log("GetNewNavMeshPositionCoroutine2.5 "+ nexPositionFound);
            }
            counter++;
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

    private float GetClosestBoundry(ARPlane arPlane)
    {
        float minRadius = float.PositiveInfinity;
        foreach (Vector2 boundary in arPlane.boundary)
        {
            if (boundary.magnitude < minRadius)
                minRadius = boundary.magnitude;
        }
        return minRadius;
    }

    /*private void InitializingAgent(ARPlanesChangedEventArgs args)
    {
        StartCoroutine(WaitThenInitializeMago(30));
    }*/

    private void checkForNewSeatTableFloor(ARPlanesChangedEventArgs args)
    {
        foreach (ARPlane arPlane in args.removed)
        {
            checkForFloorSeatTableAndRemoveFromList(arPlane);
        }

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
            if (currentPlaneMagoIsPositioned!=null && currentPlaneMagoIsPositioned.Equals(arPlane))
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
                if (renderersOfMago!=null)
                {
                    foreach (GameObject gameObject in renderersOfMago)
                    {
                        gameObject.SetActive(true);
                    }
                    renderersOfMago = null;
                    StartCoroutine(waitThenInvokeAIPlacedIntoSceneEvent(15));
                    //onAIPlacedIntoScene?.Invoke();
                }
                currentPlaneMagoIsPositioned = arPlane;
                transform.position = currentPlaneMagoIsPositioned.transform.position;
                //Debug.Log("Initialized plane: " + arPlane.name);
            }
        }
    }

    IEnumerator waitThenInvokeAIPlacedIntoSceneEvent(int waitInSeconds)
    {
        yield return new WaitForSeconds(waitInSeconds);
        onAIPlacedIntoScene?.Invoke();
    }
}
