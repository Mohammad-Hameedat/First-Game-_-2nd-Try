using System.Collections.Generic;
using UnityEngine;

public class TargetingSystem : MonoBehaviour
{
    public ITargetingStrategy targetingStrategy;

    private IEnumerable<GameObject> targetObjectsList;
    private Transform lastNearestTarget;


    // Set the type of target objects to detect and interact with
    public void SetEatableTargetsList(IEnumerable<GameObject> targets)
    {
        targetObjectsList = targets;
    }

    public Transform GetNearestTarget()
    {
        if (targetObjectsList == null)
            return null;

        Vector3 currentPosition = transform.position;
        lastNearestTarget = targetingStrategy.GetNearestTarget(targetObjectsList, lastNearestTarget, currentPosition);

        return lastNearestTarget;
    }

    // Get the last nearest target object
    public Transform GetlastNearestTarget()
    {
        return lastNearestTarget;
    }
}