using System.Collections.Generic;
using UnityEngine;

public class TargetingSystem : MonoBehaviour
{
    public ITargetingStrategy targetingStrategy;

    private IEnumerable<GameObject> targetObjectsList;
    private Transform lastNearestTarget;


    // Set the type of target objects to detect and interact with
    public void SetTargetObjectsList(IEnumerable<GameObject> targetsList)
    {
        targetObjectsList = targetsList;
    }


    // Get the last nearest target object detected but if no target object is found, check for the nearest target object
    public Transform GetlastNearestTarget()
    {
        return lastNearestTarget;
    }


    public Transform GetNearestTarget()
    {
        //if (targetedEnemyObjectsList as List<GameObject> == null) // NOTE: -> For future testing

        if (targetObjectsList == null)
            return null;

        Vector3 currentPosition = transform.position;
        lastNearestTarget = targetingStrategy.GetNearestTarget(targetObjectsList, lastNearestTarget, currentPosition);

        return lastNearestTarget;
    }
}