using System.Collections.Generic;
using UnityEngine;

public abstract class BaseFollowerController : MonoBehaviour
{
    #region References
    [Header("References")]
    public GameObject collectablePrefab;
    public FollowerProperties followerProperties;
    protected List<GameObject> targetObjectsList = new();
    #endregion


    #region Utility Controllers
    [Header("Utility Controllers")]
    [SerializeField] protected int currentNumberOfEatenObjects = 0; // The number of objects eaten by the follower
    [SerializeField] protected int currentMoneyIndex = 0; // The current money index to select the money type to be spawned by the follower
    #endregion


    #region Target Objects Management
    [Header("Nearest Object Tracking")]
    [SerializeField] protected GameObject lastNearestTargetObject = null; // The last nearest object detected by the follower
    #endregion


    #region Hunger Situation Variables
    [Header("Hunger Situation Handlers")]
    [SerializeField] protected float hungerTimeCounter = 0f; // The time before the object gets hungry
    [SerializeField] protected float hungerStartingTime; // The time before the object gets hungry after eating a target object
    [SerializeField] protected float destructionTime; // The time before the object gets destroyed after getting hungry
    #endregion


    private int frameInterval = 5; // Interval to distribute nearest object checks
    private int currentObjectIndex = 0; // Tracks the current object being processed across frames


    protected abstract void Start();

    protected abstract void Update();


    #region Nearest Object Tracking System

    // Check the nearest object and return the direction of the target object
    public virtual Vector3 CheckTargetDirection()
    {
        // Handle the target object interaction if the object is close enough to eat
        if (lastNearestTargetObject != null && IsCloseEnoughToEat())
        {
            HandleTargetObjectInteraction(lastNearestTargetObject);
        }

        //lastPosition = transform.position;
        return GetNearestObjectPosition();
    }

    // Check if the object is within range of the nearest object
    protected bool IsCloseEnoughToEat()
    {
        float distance = (lastNearestTargetObject.transform.position - transform.position).magnitude;
        return distance <= followerProperties.nearestDistanceToEatATarget; // A range threshold squared to determine if the object is within range
    }


    // Get the nearest object position, but spread the work over multiple frames
    protected virtual Vector3 GetNearestObjectPosition()
    {
        // Only run the check every X frames for performance reasons | X = frameInterval (5)
        if (Time.frameCount % frameInterval != 0)
        {
            return lastNearestTargetObject != null ? lastNearestTargetObject.transform.position : Vector3.zero;
        }

        Vector3 currentPosition = transform.position;
        float closerTargetsDetectionRangeThresholdSqr = followerProperties.closeTargetsRangeThreshold * followerProperties.closeTargetsRangeThreshold;

        // Check if the last nearest target object is still in the closer detection range of the follower
        if (lastNearestTargetObject != null && (lastNearestTargetObject.transform.position - currentPosition).sqrMagnitude <= closerTargetsDetectionRangeThresholdSqr)
        {
            return lastNearestTargetObject.transform.position;
        }
        // If the last nearest object is not in the range of the follower, find the nearest target object
        else
        {
            GameObject nearestObject = null;
            float nearestDistance = Mathf.Infinity;

            // Loop over only a subset of target objects each frame to distribute the load
            /*
            for (int i = 0; i < targetObjectsList.Count; i++)
            {
                // Limit processing each frame to avoid checking all objects at once
                if (i % frameInterval == currentObjectIndex)
                {
                    GameObject targetObject = targetObjectsList[i];

                      foreach (GameObject targetObject in targetObjectsList)
            {
                // Limit processing each frame to avoid checking all objects at once
                if (targetObjectsList.IndexOf(targetObject) % frameInterval == currentObjectIndex)
                {
                    if (targetObject == null) continue;

            */
            for (int i = 0; i < targetObjectsList.Count; i++)
            {
                // Limit processing each frame to avoid checking all objects at once
                if (i % frameInterval == currentObjectIndex)
                {
                    GameObject targetObject = targetObjectsList[i];
                    if (targetObject == null) continue;

                    Vector3 directionToTarget = targetObject.transform.position - currentPosition;
                    float closestTargetDistance = directionToTarget.sqrMagnitude;

                    if (closestTargetDistance < nearestDistance)
                    {
                        nearestDistance = closestTargetDistance;
                        nearestObject = targetObject;
                    }
                }
            }

            // Cycle through objects across frames
            currentObjectIndex = (currentObjectIndex + 1) % frameInterval;

            lastNearestTargetObject = nearestObject;
            return nearestObject != null ? nearestObject.transform.position : Vector3.zero;
        }
    }




    //// Get the nearest object position
    //protected virtual Vector3 GetNearestObjectPosition()
    //{
    //    Vector3 currentPosition = transform.position;
    //    float closerTargetsDetectionRangeThresholdSqr = followerProperties.closeTargetsRangeThreshold * followerProperties.closeTargetsRangeThreshold;

    //    // Check if the last nearest target object is still in the closer detection range of the follower
    //    if (lastNearestTargetObject != null && (lastNearestTargetObject.transform.position - currentPosition).sqrMagnitude <= closerTargetsDetectionRangeThresholdSqr)
    //    {
    //        return lastNearestTargetObject.transform.position;
    //    }
    //    // If the last nearest object is not in the range of the follower, find the nearest target object
    //    else
    //    {
    //        GameObject nearestObject = null;
    //        float nearestDistance = Mathf.Infinity;

    //        foreach (GameObject targetObject in targetObjectsList)
    //        {
    //            if (lastNearestTargetObject.IsDestroyed())
    //            {
    //                break;
    //            }

    //            Vector3 directionToTarget = targetObject.transform.position - currentPosition;
    //            float closestTargetDistance = directionToTarget.sqrMagnitude;

    //            if (closestTargetDistance < nearestDistance)
    //            {
    //                nearestDistance = closestTargetDistance;
    //                nearestObject = targetObject;
    //            }
    //        }

    //        lastNearestTargetObject = nearestObject;
    //        return nearestObject != null ? nearestObject.transform.position : Vector3.zero;

    //    }
    //}



    // Check the nearest object and return the nearest object
    protected virtual void HandleTargetObjectInteraction(GameObject targetObject)
    {
        currentNumberOfEatenObjects++;
        hungerTimeCounter = 0f;

        FoodProperties foodStaminaConfigs = lastNearestTargetObject.GetComponent<Food>().foodConfig;
        hungerStartingTime = foodStaminaConfigs.staminaTime;
        destructionTime = foodStaminaConfigs.destructionTime;

        Destroy(lastNearestTargetObject);
    }
    #endregion


    #region Target Objects Management
    // Get the number of target objects in the list
    public virtual int GetNumberOfTargetObjectsInList()
    {
        return targetObjectsList.Count;
    }
    #endregion


    #region Hunger System
    // Handle the hunger situation of the object
    protected virtual void HungerHandler()
    {
        hungerTimeCounter += Time.deltaTime;

        if (hungerTimeCounter >= destructionTime)
        {
            Destroy(gameObject);
        }
    }

    // Check if the object is hungry
    public virtual bool IsHungry()
    {
        return hungerTimeCounter >= hungerStartingTime;
    }
    #endregion


    protected abstract void OnDisable();
}
