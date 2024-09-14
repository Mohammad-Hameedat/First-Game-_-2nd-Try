using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseFollowerController : MonoBehaviour
{
    #region Target Objects Management
    protected List<GameObject> targetObjectsList = new List<GameObject>();


    #region Nearest Object Tracking
    [Header("Nearest Object Tracking")]
    [SerializeField] protected GameObject lastNearestObject = null;
    protected Vector3 lastPosition;
    protected float positionChangeThreshold { get; set; } = 1.5f;  // Set a threshold for significant position change
    protected float inRangeThreshold { get; set; } = 1.5f; // Set a threshold for the distance between the follower and the target object
    [SerializeField] protected int numberOfEatenObjects = 0;
    #endregion
    #endregion

    #region Hunger Situation Variables
    [Header("Hunger Situation Variables")]
    [SerializeField] protected float timeBeforeGettingHungry = 0f;
    [SerializeField] protected float hungerStartingTime = 5f;
    [SerializeField] protected float timeBeforeDestruction = 15f;
    #endregion

    #region Money Properties
    [Header("Money configurations")]
    public GameObject moneyPrefab;
    public MoneyProperties[] moneyTypes;
    [SerializeField] protected int currentMoneyIndex = 0;
    #endregion

    protected virtual void Start()
    {
        lastPosition = transform.position;
    }



    //protected virtual void Update()
    //{
    //    HungerHandler();
    //}



    #region Nearest Object Tracking System

    // Check the nearest object and return the direction of the target object
    public virtual Vector3 CheckTargetDirection()
    {
        if (lastNearestObject != null)
        {
            float positionDeltaSqr = (transform.position - lastPosition).sqrMagnitude;

            if (positionDeltaSqr <= positionChangeThreshold)
            {
                // If the object hasn't moved significantly, keep the same target
                return lastNearestObject.transform.position;
            }
            else if (IsInRange())
            {
                // If in range, handle interaction
                HandleTargetObjectInteraction(lastNearestObject);
            }
        }

        lastPosition = transform.position;
        return GetNearestObjectPosition();
    }

    // Check if the object is within range of the nearest object
    protected bool IsInRange()
    {
        if (lastNearestObject == null)
            return false;

        return (transform.position - lastNearestObject.transform.position).sqrMagnitude <= inRangeThreshold;
    }

    // Get the nearest object position
    protected virtual Vector3 GetNearestObjectPosition()
    {
        float nearestDistance = Mathf.Infinity;
        GameObject nearestObject = null;

        Vector3 currentPosition = transform.position;

        foreach (GameObject targetObject in targetObjectsList)
        {
            if (targetObject == null)
            {
                continue;
            }
            else
            {
                Vector3 directionToTarget = targetObject.transform.position - currentPosition;
                float distanceSqr = directionToTarget.sqrMagnitude;

                if (distanceSqr < nearestDistance)
                {
                    nearestDistance = distanceSqr;
                    nearestObject = targetObject;

                    float inRangeThresholdSqr = inRangeThreshold * inRangeThreshold;

                    // Early exit if within interaction range
                    if (distanceSqr <= inRangeThresholdSqr)
                    {
                        break;
                    }
                }
            }
        }

        lastNearestObject = nearestObject;
        return nearestObject != null ? nearestObject.transform.position : Vector3.zero;
    }


    // Check the nearest object and return the nearest object
    protected virtual void HandleTargetObjectInteraction(GameObject targetObject)
    {
        numberOfEatenObjects++;
        timeBeforeGettingHungry = 0f;

        FoodProperties hungerConfigs = lastNearestObject.GetComponent<Food>().foodConfig;
        hungerStartingTime = hungerConfigs.staminaTime;
        timeBeforeDestruction = hungerConfigs.destructionTime;

        Destroy(lastNearestObject);
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
        timeBeforeGettingHungry += Time.deltaTime;

        if (timeBeforeGettingHungry >= timeBeforeDestruction)
        {
            Destroy(gameObject);
        }
    }

    // Check if the object is hungry
    public virtual bool IsHungry()
    {
        return timeBeforeGettingHungry >= hungerStartingTime;
    }
    #endregion


    protected abstract void OnDestroy();


}
