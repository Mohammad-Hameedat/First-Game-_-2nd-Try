using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseFollowerController : MonoBehaviour
{
    #region Target Objects Management
    protected static List<GameObject> foodTargetObjectsList = new List<GameObject>();


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
    [SerializeField] protected float timeBeforeDying = 15f;
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

        foreach (GameObject targetObject in foodTargetObjectsList)
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
        timeBeforeDying = hungerConfigs.destructionTime;

        RemoveTargetObjectFromList(lastNearestObject);
        Destroy(lastNearestObject);
    }

    #endregion

    #region Target Objects Management

    public virtual int GetNumberOfTargetObjects()
    {
        return foodTargetObjectsList.Count;
    }

    public virtual void AddTargetObjectToList(GameObject targetObject)
    {
        foodTargetObjectsList.Add(targetObject);
    }

    public static void RemoveTargetObjectFromList(GameObject targetObject)
    {
        foodTargetObjectsList.Remove(targetObject);
    }
    #endregion

    #region Hunger System

    protected virtual void HungerHandler()
    {
        timeBeforeGettingHungry += Time.deltaTime;

        if (timeBeforeGettingHungry >= timeBeforeDying)
        {
            Destroy(gameObject);
        }
    }

    public virtual bool IsHungry()
    {
        return timeBeforeGettingHungry >= hungerStartingTime;
    }
    #endregion

    #region Money System


    #endregion
}
