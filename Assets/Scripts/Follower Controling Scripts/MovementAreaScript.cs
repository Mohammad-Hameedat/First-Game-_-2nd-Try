using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAreaScript : MonoBehaviour
{
    private Camera mainCamera;

    float xMin = 0.05f, xMax = 0.95f, yMin = 0.05f, yMax = 0.80f;


    private void Start()
    {
        mainCamera = Camera.main;

    }


    public void ClampPositionWithInView(Transform objectTransform)
    {
        // Get the current position of the object
        Vector3 newPosition = objectTransform.position;

        // Get the position of the object in the camera view
        Vector3 viewPos = mainCamera.WorldToViewportPoint(newPosition);

        // Clamp the position of the object to the camera view
        viewPos.x = Mathf.Clamp(viewPos.x, xMin, xMax);
        viewPos.y = Mathf.Clamp(viewPos.y, yMin, yMax);

        // Get the new position of the object
        newPosition = mainCamera.ViewportToWorldPoint(viewPos);

        // Set the new position of the object
        objectTransform.position = newPosition;
    }

}
