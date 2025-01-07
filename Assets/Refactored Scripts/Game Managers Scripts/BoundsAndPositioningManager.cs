using System.Collections.Generic;
using UnityEngine;

public class BoundsAndPositioningManager : MonoBehaviour
{
    #region Movement Area Variables
    readonly float xMin = 0.05f, xMax = 0.95f, yMin = 0.05f, yMax = 0.80f;
    readonly float threshold = 0.01f;
    #endregion

    // Corner adjacency: 3x3 grid with diagonals or partial diagonals
    private Dictionary<ScreenCorner, ScreenCorner[]> adjacency;

    private void Awake()
    {
        // Paths from each corner to the other corners
        adjacency = new Dictionary<ScreenCorner, ScreenCorner[]>
        {
            { ScreenCorner.TopLeft, new ScreenCorner[]
            {
                ScreenCorner.TopCenter,
                ScreenCorner.MidLeft
            }},

            { ScreenCorner.TopCenter, new ScreenCorner[]
            {
                ScreenCorner.TopLeft,
                ScreenCorner.TopRight,
                ScreenCorner.MidLeft,
                ScreenCorner.Center,
                ScreenCorner.MidRight
            }},

            { ScreenCorner.TopRight, new ScreenCorner[]
            {
                ScreenCorner.TopCenter,
                ScreenCorner.MidRight
            }},

            { ScreenCorner.MidLeft, new ScreenCorner[]
            {
                ScreenCorner.TopLeft,
                ScreenCorner.TopCenter,
                ScreenCorner.BottomLeft,
                ScreenCorner.BottomCenter,
            }},

            { ScreenCorner.Center, new ScreenCorner[]
            {
                ScreenCorner.TopLeft,
                ScreenCorner.TopCenter,
                ScreenCorner.TopRight,
                ScreenCorner.MidLeft,
                ScreenCorner.MidRight,
                ScreenCorner.BottomLeft,
                ScreenCorner.BottomCenter,
                ScreenCorner.BottomRight
            }},

            { ScreenCorner.MidRight, new ScreenCorner[]
            {
                ScreenCorner.TopRight,
                ScreenCorner.TopCenter,
                ScreenCorner.BottomCenter,
                ScreenCorner.BottomRight
            }},

            { ScreenCorner.BottomLeft, new ScreenCorner[]
            {
                ScreenCorner.TopLeft,
                ScreenCorner.BottomRight
            }},

            { ScreenCorner.BottomCenter, new ScreenCorner[]
            {
                ScreenCorner.BottomLeft,
                ScreenCorner.MidLeft,
                ScreenCorner.Center,
                ScreenCorner.MidRight,
                ScreenCorner.BottomRight
            }},

            { ScreenCorner.BottomRight, new ScreenCorner[]
            {
                ScreenCorner.TopRight,
                ScreenCorner.BottomLeft
            }}
        };
    }

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
    public Vector3 GenerateRandomClampedPosition()
    {
        // Get the new random position
        Vector3 newPosition = new(
            Random.Range(xMin, xMax),
            Random.Range(yMin, yMax),
            0
            );

        Vector3 clampedRandomPosition = Camera.main.ViewportToWorldPoint(newPosition);
        clampedRandomPosition.z = -2;

        // Set the new position of the object
        return clampedRandomPosition;
    }

    // Check if the object is close to the edge of the movement-area
    public bool PositionCheck()
    {
        Vector3 positionInCameraViewport = Camera.main.WorldToViewportPoint(transform.position);

        if (positionInCameraViewport.x <= xMin + threshold ||
            positionInCameraViewport.x >= xMax - threshold ||
            positionInCameraViewport.y <= yMin + threshold ||
            positionInCameraViewport.y >= yMax - threshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion


    #region Screen Corner Methods

    public Vector3 CornerToWorldPosition(ScreenCorner corner)
    {
        // Compute midpoints in viewport space
        float xMid = ( xMin + xMax ) * 0.5f;
        float yMid = ( yMin + yMax ) * 0.5f;

        Vector2 viewportPosition = Vector2.zero;
        switch (corner)
        {
            case ScreenCorner.TopLeft:
                viewportPosition = new Vector2(xMin + threshold, yMax - threshold);
                break;
            case ScreenCorner.TopCenter:
                viewportPosition = new Vector2(xMid, yMax - threshold);
                break;
            case ScreenCorner.TopRight:
                viewportPosition = new Vector2(xMax - threshold, yMax - threshold);
                break;


            case ScreenCorner.MidLeft:
                viewportPosition = new Vector2(xMin + threshold, yMid);
                break;
            case ScreenCorner.Center:
                viewportPosition = new Vector2(xMid, yMid);
                break;
            case ScreenCorner.MidRight:
                viewportPosition = new Vector2(xMax - threshold, yMid);
                break;


            case ScreenCorner.BottomLeft:
                viewportPosition = new Vector2(xMin + threshold, yMin + threshold);
                break;
            case ScreenCorner.BottomCenter:
                viewportPosition = new Vector2(xMid, yMin + threshold);
                break;
            case ScreenCorner.BottomRight:
                viewportPosition = new Vector2(xMax - threshold, yMin + threshold);
                break;
        }

        // Convert viewport -> world
        Vector3 worldPos = Camera.main.ViewportToWorldPoint(new Vector3(viewportPosition.x, viewportPosition.y, 0f));
        worldPos.z = -2f;
        return worldPos;
    }


    public ScreenCorner FindBestNextCorner(ScreenCorner currentCorner, Vector3 targetPosition)
    {
        float currentDistance = Vector3.Distance(CornerToWorldPosition(currentCorner), targetPosition);


        float bestDistance = currentDistance;
        ScreenCorner bestCorner = currentCorner;

        if (!adjacency.ContainsKey(currentCorner))
        {
            return currentCorner;
        }

        // Check the distance to each adjacent corner from the current corner
        foreach (ScreenCorner adjacentCorner in adjacency[currentCorner])
        {
            float dist1 = Vector3.Distance(CornerToWorldPosition(adjacentCorner), targetPosition);

            float bestSubDistance = dist1;
            ScreenCorner bestSubCorner = adjacentCorner;


            if (adjacency.ContainsKey(adjacentCorner))
            {
                foreach (ScreenCorner secondHop in adjacency[adjacentCorner])
                {
                    float dist2 = Vector3.Distance(CornerToWorldPosition(secondHop), targetPosition);
                    if (dist2 > bestSubDistance)
                    {
                        bestSubDistance = dist2;
                        bestSubCorner = secondHop;
                    }
                }
            }

            if (bestSubDistance > bestDistance)
            {
                bestDistance = bestSubDistance;
                bestCorner = bestSubCorner;
            }
        }

        // If the best corner is the same as the current corner, try to find a fallback corner
        if (bestCorner == currentCorner)
        {
            float fallbckDistance = Mathf.Infinity;
            ScreenCorner fallbackCorner = currentCorner;

            foreach (ScreenCorner adjacentCorner in adjacency[currentCorner])
            {
                float dist1 = Vector3.Distance(CornerToWorldPosition(adjacentCorner), targetPosition);
                if (dist1 > fallbckDistance)
                {
                    fallbckDistance = dist1;
                    fallbackCorner = adjacentCorner;
                }
            }

            if (fallbackCorner != currentCorner)
            {
                bestCorner = fallbackCorner;
            }
        }


        //Debug.Log($"Target Position: {nextCornerWorldPosition}");

        return bestCorner;
    }


    public ScreenCorner GetNearestCorner(Vector3 position)
    {
        float closestDistance = Mathf.Infinity;
        ScreenCorner nearestCorner = ScreenCorner.Center;

        // Check the distance to each corner
        foreach (ScreenCorner corner in adjacency.Keys)
        {
            float distance = Vector3.Distance(CornerToWorldPosition(corner), position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestCorner = corner;
            }
        }

        return nearestCorner;
    }


    #endregion
}

public enum ScreenCorner
{
    TopLeft,
    TopCenter,
    TopRight,
    MidLeft,
    Center,
    MidRight,
    BottomLeft,
    BottomCenter,
    BottomRight,
}