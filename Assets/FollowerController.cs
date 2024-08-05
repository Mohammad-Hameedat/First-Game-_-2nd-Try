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
    private float checkThreshold = 2f; // Set a threshold for significant position change
    #endregion




    float timeBeforeGettingHungry = 0f;
    float hungerStartingTime = 5f;
    float hungerEndingTime = 8f;
    float timeBeforeDying = 15f;



    public MeshRenderer meshRenderer { get; private set; }



    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        lastPosition = transform.position;
    }

    private void Update()
    {
        HungerHandler();
    }


    #region Nearest Object Tracking System
    public Vector3 CheckNearestTargetObject()
    {
        // Check if the follower has moved significantly since the last check
        if (lastNearestObject != null && (transform.position - lastPosition).sqrMagnitude <= checkThreshold)
        {
            return lastNearestObject.transform.position;
        }
        // Check if the last nearest target object is so close to the follower
        else if (lastNearestObject != null && (transform.position - lastNearestObject.transform.position).magnitude <= 1f)
        {
            RemoveTargetObjectFromList(lastNearestObject);
            Destroy(lastNearestObject);
            lastNearestObject = null;
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

    public static int GetNumberOfTargetObjects()
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



    private void HungerHandler()
    {
        timeBeforeGettingHungry += Time.deltaTime;

        float changingColorDuration = Mathf.Clamp01((timeBeforeGettingHungry - hungerStartingTime) / (hungerEndingTime / hungerStartingTime));
    }


    public bool IsHungry()
    {
        return timeBeforeGettingHungry >= hungerStartingTime;
    }


}