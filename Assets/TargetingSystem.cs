using System.Collections.Generic;
using UnityEngine;

public class TargetingSystem : MonoBehaviour
{
    private List<GameObject> targetObjectsList = new();
    private Transform lastNearestTarget;

    private int frameInterval = 5; // Interval to distribute nearest object checks
    private int currentObjectIndex = 0; // Tracks the current object being processed across frames

    private FollowerProperties properties;
    private float sqrLargerDetectionRadius;

    private void Awake()
    {
        properties = GetComponent<MovementController>().properties;
    }

    private void Start()
    {
        sqrLargerDetectionRadius = properties.closeTargetsRangeThreshold * properties.closeTargetsRangeThreshold;
    }

    // Set the type of target objects to detect and interact with
    public void SetEatableTargetsList(List<GameObject> targets)
    {
        targetObjectsList = targets;
    }

    // Get the nearest object from the list of target objects
    public Transform GetNearestTarget()
    {
        //Debug.Log(targetObjectsList.Count);


        //Only run the check every X frames for performance reasons | X = frameInterval(5)
        if (Time.frameCount % frameInterval != 0 || (lastNearestTarget != null && (lastNearestTarget.position - transform.position).sqrMagnitude <= sqrLargerDetectionRadius))
        {
            return lastNearestTarget;
        }
        else
        {
            Transform nearestTarget = null;
            float minDistance = Mathf.Infinity;

            Vector3 currentPosition = transform.position;

            for (int i = 0; i < targetObjectsList.Count; i++)
            {
                if (i % frameInterval == currentObjectIndex)
                {
                    GameObject targetObject = targetObjectsList[i];
                    if (targetObject == null) continue;

                    Vector3 directionToTarget = targetObject.transform.position - currentPosition;
                    float distanceSqr = directionToTarget.sqrMagnitude;

                    if(distanceSqr < minDistance)
                    {
                        minDistance = distanceSqr;
                        nearestTarget = targetObject.transform;
                    }
                }
            }

            // Cycle through the target objects list across frames
            currentObjectIndex = (currentObjectIndex + 1) % frameInterval;

            lastNearestTarget = nearestTarget;
            return nearestTarget;
        }
    }

    // Get the last nearest target object
    public Transform GetlastNearestTarget()
    {
        return lastNearestTarget;
    }
}
