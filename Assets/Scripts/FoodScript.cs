using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FoodScript : MonoBehaviour, IEndDragHandler, IDragHandler
{
    private Vector3 startPosition;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        startPosition.y = startPosition.y + 100;
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        transform.position = startPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
        // if left-mouse-button is being held OR there is at least one touch
        if (isActiveAndEnabled)
        {
            // get touch position in screen space
            // (if touch, gets average of all touches)
            Vector2 screenPos = Input.GetTouch(0).position;
            // set a distance from the camera
            //screenPos.z = 10.0f;
            // convert touch position to world space
            //Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

            // get current position of this GameObject
            //Vector3 newPos = transform.position;
            // set x position to mouse world-space x position
            //newPos.x = worldPos.x;
            // apply new position
            //transform.position = newPos;
            //screenPos.y = screenPos.y + 100;
            //transform.position = screenPos;
        }

    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        gameObject.SetActive(false);
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        Vector2 screenPos = Input.GetTouch(0).position;
        // set a distance from the camera
        //screenPos.z = 10.0f;
        // convert touch position to world space
        //Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        // get current position of this GameObject
        //Vector3 newPos = transform.position;
        // set x position to mouse world-space x position
        //newPos.x = worldPos.x;
        // apply new position
        //transform.position = newPos;
        screenPos.y = screenPos.y + 100;
        transform.position = screenPos;
    }
}
