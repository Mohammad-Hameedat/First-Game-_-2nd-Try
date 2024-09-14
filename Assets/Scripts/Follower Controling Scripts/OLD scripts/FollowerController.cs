//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class FollowerController : MonoBehaviour
//{
//    #region Target Objects Management
//    private static List<GameObject> targetObjects = new List<GameObject>();

//    #region Nearest Object Tracking
//    [Header("Nearest Object Tracking")]

//    [SerializeField] private GameObject lastNearestObject = null;
//    private Vector3 lastPosition;
//    private float positionChangeThreshold = 1.5f; // Set a threshold for significant position change
//    private float inRangeThreshold = 1.5f; // Set a threshold for the distance between the follower and the target object

//    [SerializeField]
//    private int numberOfEatenObjects = 0;
//    #endregion
//    #endregion

//    #region Hunger Situation Variables
//    [Header("Hunger Situation Variables")]

//    [SerializeField] float timeBeforeGettingHungry = 0f;
//    [SerializeField] float hungerStartingTime = 5f;
//    [SerializeField] float timeBeforeDying = 15f;
//    #endregion

//    #region Money Properties

//    [Header("Money configurations")]
//    public GameObject moneyPrefab;
//    public MoneyProperties[] moneyTypes;
//    private int currentMoneyIndex = 0;

//    #endregion


//    private void Start()
//    {
//        lastPosition = transform.position;

//        StartCoroutine(SpawnMoney());

//        //var t = GetComponent<MainFishCHILDScript>();

//        //var r = t.IsHungry();

//    }

//    private void Update()
//    {
//        HungerHandler();

//    }


//    #region Nearest Object Tracking System

//    public Vector3 CheckTargetDirection()
//    {
//        #region Current target object tracking system

//        // Check if the follower has moved significantly since the last check
//        if (lastNearestObject != null && (transform.position - lastPosition).sqrMagnitude <= positionChangeThreshold)
//        {
//            return lastNearestObject.transform.position;
//        }
//        // Check if the last nearest target object is in follower's range and if the follower is hungry or not
//        else if (lastNearestObject != null && (transform.position - lastNearestObject.transform.position).sqrMagnitude <= inRangeThreshold && IsHungry())
//        {
//            HandleTargetObjectInteraction(lastNearestObject);
//        }
//        #endregion


//        // Update last position
//        lastPosition = transform.position;

//        float nearestDistance = Mathf.Infinity;
//        GameObject nearestObject = null;

//        foreach (GameObject targetObject in targetObjects)
//        {
//            float distance = (transform.position - targetObject.transform.position).sqrMagnitude;

//            if (distance < nearestDistance)
//            {
//                nearestDistance = distance;
//                nearestObject = targetObject;
//            }
//        }

//        lastNearestObject = nearestObject;


//        // Check if nearestObject is null before accessing its transform OR Return some default position or handle the case where there is no nearest object
//        return nearestObject != null ? nearestObject.transform.position : Vector3.zero;
//    }


//    void HandleTargetObjectInteraction(GameObject targetObject)
//    {
//        // Eat the target object
//        numberOfEatenObjects++;

//        timeBeforeGettingHungry = 0f;

//        FoodProperties hungerConfigs = lastNearestObject.GetComponent<Target>().foodConfig;
//        hungerStartingTime = hungerConfigs.staminaTime;
//        timeBeforeDying = hungerConfigs.destructionTime;

//        RemoveTargetObjectFromList(lastNearestObject);
//        Destroy(lastNearestObject);
//    }

//    #endregion


//    #region Target Objects Management

//    public int GetNumberOfTargetObjects()
//    {
//        return targetObjects.Count;
//    }

//    public static void AddTargetObjectToList(GameObject targetObject)
//    {
//        targetObjects.Add(targetObject);
//    }

//    public static void RemoveTargetObjectFromList(GameObject targetObject)
//    {
//        targetObjects.Remove(targetObject);
//    }
//    #endregion


//    #region Hunger System

//    private void HungerHandler()
//    {
//        timeBeforeGettingHungry += Time.deltaTime;

//        if (timeBeforeGettingHungry >= timeBeforeDying)
//        {
//            Destroy(gameObject);
//        }
//    }


//    public bool IsHungry()
//    {
//        return timeBeforeGettingHungry >= hungerStartingTime;
//    }
//    #endregion



//    #region Money System

//    IEnumerator SpawnMoney()
//    {
//        while (true)
//        {
//            if (numberOfEatenObjects >= 3)
//            {
//                float randomTime = Random.Range(moneyTypes[currentMoneyIndex].defaultTimeToInitiate, moneyTypes[currentMoneyIndex].defaultTimeToInitiate + 3f);
//                yield return new WaitForSeconds(randomTime);
//                Instantiate(moneyPrefab, transform.position, Quaternion.identity);
//                moneyPrefab.GetComponent<Collectable>().moneyConfig = moneyTypes[currentMoneyIndex];
//            }
//            yield return null;
//        }
//    }


//    #endregion

//}