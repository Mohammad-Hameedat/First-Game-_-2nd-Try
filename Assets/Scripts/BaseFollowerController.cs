using System.Collections.Generic;
using UnityEngine;

public abstract class BaseFollowerController : MonoBehaviour
{
    protected static List<GameObject> targetObjects = new List<GameObject>();
    public static int numberOfTargetObjects;

    private GameObject lastNearestObject = null;
    private Vector3 lastPosition;
    private float checkThreshold = 2f; // Set a threshold for significant position change

    protected virtual void Start()
    {
        numberOfTargetObjects = targetObjects.Count;
        lastPosition = transform.position;
    }

    public Vector3 CheckNearestTargetObject()
    {
        // Check if the follower has moved significantly since the last check
        if (lastNearestObject != null && (transform.position - lastPosition).sqrMagnitude <= checkThreshold)
        {
            return lastNearestObject.transform.position;
        }

        // Update last position
        lastPosition = transform.position;

        float nearestDistance = Mathf.Infinity;
        GameObject nearestObject = null;

        foreach (GameObject targetObject in targetObjects)
        {
            float distance = (transform.position - targetObject.transform.position).sqrMagnitude;

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestObject = targetObject;
            }
        }

        lastNearestObject = nearestObject;

        // Check if nearestObject is null before accessing its transform OR Return some default position or handle the case where there is no nearest object
        return nearestObject != null ? nearestObject.transform.position : Vector3.zero;
    }

    public static void AddTargetObjectToList(GameObject targetObject)
    {
        targetObjects.Add(targetObject);
        numberOfTargetObjects = targetObjects.Count;
        Debug.Log("Number of Target Object Added = " + numberOfTargetObjects);
    }

    public static void RemoveTargetObjectFromList(GameObject targetObject)
    {
        targetObjects.Remove(targetObject);
        numberOfTargetObjects = targetObjects.Count;
        Debug.Log("Number of Target Object Removed = " + numberOfTargetObjects);
    }
}
