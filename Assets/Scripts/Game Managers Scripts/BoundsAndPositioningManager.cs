using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class BoundsAndPositioningManager : MonoBehaviour
{

    #region Movement Area Variables
    float xMin = 0.05f, xMax = 0.95f, yMin = 0.05f, yMax = 0.80f;
    float threshold = 0.01f;
    #endregion


    #region Movement Area Methods
    // Clamp the position of the object to the camera view
    public Vector3 ClampPositionWithInView(Vector3 objectPosition)
    {
        // Get the position of the object in the camera view
        Vector3 viewPos = Camera.main.WorldToViewportPoint(objectPosition);

        // Clamp the position of the object to the camera view
        viewPos.x = Mathf.Clamp(viewPos.x, xMin, xMax);
        viewPos.y = Mathf.Clamp(viewPos.y, yMin, yMax);

        // Get the new position of the object
        Vector3 clampedPosition = Camera.main.ViewportToWorldPoint(viewPos);
        clampedPosition.z = -2;

        // Set the new position of the object
        return clampedPosition;
    }


    // Get a new random position for the object
    public Vector3 GetNewRandomPosition()
    {
        // Get the new random position
        Vector3 newPosition = new Vector3(Random.Range(xMin, xMax), Random.Range(yMin, yMax), 0);
        Vector3 clampedRandomPosition = Camera.main.ViewportToWorldPoint(newPosition);
        clampedRandomPosition.z = -2;

        // Set the new position of the object
        return clampedRandomPosition;
    }

    // Check if the object is close to the edge of the movement-area
    public bool PositionCheck()
    {
        Vector3 positionInCameraViewport = Camera.main.WorldToViewportPoint(transform.position);

        if (positionInCameraViewport.x <= xMin + threshold || positionInCameraViewport.x >= xMax - threshold || positionInCameraViewport.y <= yMin + threshold || positionInCameraViewport.y >= yMax - threshold)
        {
            return true;

        }
        else
        {
            return false;
        }
    }

    #endregion

}
