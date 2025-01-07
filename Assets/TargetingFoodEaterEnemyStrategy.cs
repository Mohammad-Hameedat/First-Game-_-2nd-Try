using System.Collections.Generic;
using UnityEngine;

public class TargetingFoodEaterEnemyStrategy : ITargetingStrategy
{
    public Transform GetNearestTarget(IEnumerable<GameObject> targetObjectsList, Transform lastNearestTarget, Vector3 currentPosition)
    {
        Transform nearestTarget = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject targetObject in targetObjectsList)
        {
            if (targetObject == null)
                continue;

            Vector3 directionToTarget = targetObject.transform.position - currentPosition;
            float distanceSqr = directionToTarget.sqrMagnitude;

            if (distanceSqr < minDistance)
            {
                minDistance = distanceSqr;
                nearestTarget = targetObject.transform;
            }
        }

        return nearestTarget;
    }
}
