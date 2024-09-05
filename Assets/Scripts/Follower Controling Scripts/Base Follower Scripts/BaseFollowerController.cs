using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseFollowerController : MonoBehaviour
{
    #region Target Objects Management
    protected static List<GameObject> targetObjectsList = new List<GameObject>();


    #region Nearest Object Tracking
    [Header("Nearest Object Tracking")]
    [SerializeField] protected GameObject lastNearestObject = null;
    protected Vector3 lastPosition;
    protected float positionChangeThreshold = 1.5f; // Set a threshold for significant position change
    protected float inRangeThreshold = 1.5f; // Set a threshold for the distance between the follower and the target object
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

    public virtual Vector3 CheckTargetDirection()
    {
        if (lastNearestObject != null && (transform.position - lastPosition).sqrMagnitude <= positionChangeThreshold)
        {
            return lastNearestObject.transform.position;
        }
        else if (lastNearestObject != null && (transform.position - lastNearestObject.transform.position).sqrMagnitude <= inRangeThreshold && IsHungry())
        {
            HandleTargetObjectInteraction(lastNearestObject);
        }

        lastPosition = transform.position;

        float nearestDistance = Mathf.Infinity;
        GameObject nearestObject = null;

        foreach (GameObject targetObject in targetObjectsList)
        {
            float distance = (transform.position - targetObject.transform.position).sqrMagnitude;

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestObject = targetObject;
            }
        }

        lastNearestObject = nearestObject;
        return nearestObject != null ? nearestObject.transform.position : Vector3.zero;
    }

    protected virtual void HandleTargetObjectInteraction(GameObject targetObject)
    {
        numberOfEatenObjects++;
        timeBeforeGettingHungry = 0f;

        FoodProperties hungerConfigs = lastNearestObject.GetComponent<Target>().foodConfig;
        hungerStartingTime = hungerConfigs.staminaTime;
        timeBeforeDestruction = hungerConfigs.destructionTime;

        Destroy(lastNearestObject);
    }

    #endregion

    #region Target Objects Management
    public virtual int GetNumberOfTargetObjects()
    {
        return targetObjectsList.Count;
    }
    #endregion

    #region Hunger System

    protected virtual void HungerHandler()
    {
        timeBeforeGettingHungry += Time.deltaTime;

        if (timeBeforeGettingHungry >= timeBeforeDestruction)
        {
            Destroy(gameObject);
        }
    }

    public virtual bool IsHungry()
    {
        return timeBeforeGettingHungry >= hungerStartingTime;
    }
    #endregion


}
