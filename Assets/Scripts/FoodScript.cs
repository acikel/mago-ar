using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        // if left-mouse-button is being held OR there is at least one touch
        if (Input.GetMouseButton(0) || Input.touchCount > 0)
        {
            // get touch position in screen space
            // (if touch, gets average of all touches)
            Vector3 screenPos = Input.GetTouch(0).position;
            // set a distance from the camera
            screenPos.z = 10.0f;
            // convert touch position to world space
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

            // get current position of this GameObject
            Vector3 newPos = transform.position;
            // set x position to mouse world-space x position
            newPos.x = worldPos.x;
            // apply new position
            transform.position = newPos;
        }

    }
}
