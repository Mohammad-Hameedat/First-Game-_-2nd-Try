using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseFollowerController : MonoBehaviour
{
    #region Target Objects Management
    protected List<GameObject> targetObjectsList = new();


    #region Nearest Object Tracking
    [Header("Nearest Object Tracking")]
    [SerializeField] protected GameObject lastNearestTargetObject = null; // The last nearest object detected by the follower
    protected float nearestDistanceToTargetToEat = 1f; // Set a threshold for the distance between the follower and the target object before eating it
    protected float closeTargetsRangeThreshold = 3f; // Set a threshold for the detection range of the target object

    [SerializeField] protected int numberOfEatenObjects = 0; // The number of objects eaten by the follower
    #endregion
    #endregion

    #region Hunger Situation Variables
    [Header("Hunger Situation Variables")]
    [SerializeField] protected float timeBeforeGettingHungry = 0f; // The time before the object gets hungry
    [SerializeField] protected float hungerStartingTime = 5f; // The time before the object gets hungry after eating a target object
    [SerializeField] protected float timeBeforeDestruction = 15f; // The time before the object gets destroyed after getting hungry
    #endregion

    #region Money Properties
    [Header("Money configurations")]
    public GameObject moneyPrefab; // The money prefab to be spawned by the follower
    public MoneyProperties[] moneyTypes; // The money types that can be spawned by the follower
    [SerializeField] protected int currentMoneyIndex = 0; // The current money index to select the money type to be spawned by the follower
    #endregion

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

        return distance <= nearestDistanceToTargetToEat; // A range threshold squared to determine if the object is within range
    }

    // Get the nearest object position
    protected virtual Vector3 GetNearestObjectPosition()
    {
        Vector3 currentPosition = transform.position;

        if (lastNearestTargetObject != null && (lastNearestTargetObject.transform.position - currentPosition).sqrMagnitude <= closeTargetsRangeThreshold * closeTargetsRangeThreshold)
        {
            return lastNearestTargetObject.transform.position;
        }
        else
        {
            GameObject nearestObject = null;
            float nearestDistance = Mathf.Infinity;

            foreach (GameObject targetObject in targetObjectsList)
            {
                Vector3 directionToTarget = targetObject.transform.position - currentPosition;
                float closestDistanceRangeThreshold = directionToTarget.sqrMagnitude;

                if (closestDistanceRangeThreshold < nearestDistance)
                {
                    nearestDistance = closestDistanceRangeThreshold;
                    nearestObject = targetObject;

                    if (lastNearestTargetObject.IsDestroyed())
                    {
                        break;
                    }
                }
            }

            lastNearestTargetObject = nearestObject;
            return nearestObject != null ? nearestObject.transform.position : Vector3.zero;

        }
    }


    // Check the nearest object and return the nearest object
    protected virtual void HandleTargetObjectInteraction(GameObject targetObject)
    {
        numberOfEatenObjects++;
        timeBeforeGettingHungry = 0f;

        FoodProperties hungerConfigs = lastNearestTargetObject.GetComponent<Food>().foodConfig;
        hungerStartingTime = hungerConfigs.staminaTime;
        timeBeforeDestruction = hungerConfigs.destructionTime;

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
