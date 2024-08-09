using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FollowerController : MonoBehaviour
{
    #region Target Objects Management
    private static List<GameObject> targetObjects = new List<GameObject>();
    #endregion

    #region Nearest Object Tracking
    [SerializeField] private GameObject lastNearestObject = null;
    private Vector3 lastPosition;
    private float positionChangeThreshold = 1.5f; // Set a threshold for significant position change
    private float inRangeThreshold = 1.5f; // Set a threshold for the distance between the follower and the target object
    #endregion

    #region Hunger Situation Variables

    [SerializeField] float timeBeforeGettingHungry = 0f;
    float hungerStartingTime = 5f;
    //float hungerEndingTime = 8f;
    float timeBeforeDying = 15f;
    #endregion

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        HungerHandler();

    }


    #region Nearest Object Tracking System

    public Vector3 CheckTargetDirection()
    {
        // Check if the follower has moved significantly since the last check
        if (lastNearestObject != null && (transform.position - lastPosition).sqrMagnitude <= positionChangeThreshold)
        {
            return lastNearestObject.transform.position;
        }
        // Check if the last nearest target object is in follower's range and if the follower is hungry or not
        else if (lastNearestObject != null && (transform.position - lastNearestObject.transform.position).sqrMagnitude <= inRangeThreshold && IsHungry())
        {
            timeBeforeGettingHungry = 0f;
            RemoveTargetObjectFromList(lastNearestObject);
            Destroy(lastNearestObject);
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
    #endregion


    #region Target Objects Management

    public int GetNumberOfTargetObjects()
    {
        return targetObjects.Count;
    }

    public static void AddTargetObjectToList(GameObject targetObject)
    {
        targetObjects.Add(targetObject);
        Debug.Log("Number of Target Object Added = " + targetObjects.Count);
    }

    public static void RemoveTargetObjectFromList(GameObject targetObject)
    {
        targetObjects.Remove(targetObject);
        Debug.Log("Number of Target Object Removed = " + targetObjects.Count);
    }
    #endregion


    #region Hunger System

    private void HungerHandler()
    {
        timeBeforeGettingHungry += Time.deltaTime;

        // Change the color of the follower based on the follower level
        //float changingColorDuration = Mathf.Clamp01((timeBeforeGettingHungry - hungerStartingTime) / (hungerEndingTime / hungerStartingTime));

        if (timeBeforeGettingHungry >= timeBeforeDying)
        {
            Destroy(gameObject);
        }
    }


    public bool IsHungry()
    {
        return timeBeforeGettingHungry >= hungerStartingTime;
    }
    #endregion

}