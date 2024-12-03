using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainFishTargetingStrategy : ITargetingStrategy
{
    private int frameInterval = 5; // Interval to distribute nearest object checks
    private int currentObjectIndex = 0; // Tracks the current object index being processed across frames

    public Transform GetNearestTarget(IEnumerable<GameObject> targetObjectsList, Transform lastNearestObject, Vector3 currentPosition)
    {
        if (Time.frameCount % frameInterval != 0 && lastNearestObject != null)
        {
            return lastNearestObject;
        }
        else
        {
            Transform nearestTarget = null;
            float minDistance = Mathf.Infinity;


            int index = 0;

            foreach (GameObject targetObject in targetObjectsList)
            {
                if (index % frameInterval == currentObjectIndex)
                {
                    Vector3 directionToTarget = targetObject.transform.position - currentPosition;
                    float distanceSqr = directionToTarget.sqrMagnitude;

                    if (distanceSqr < minDistance)
                    {
                        minDistance = distanceSqr;
                        nearestTarget = targetObject.transform;
                    }
                }
                index++;
            }

            currentObjectIndex = (currentObjectIndex + 1) % frameInterval;
            return nearestTarget;
        }
    }
}
