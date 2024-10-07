using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChildEnemyFoodEaterController : ChildEnemyController
{

    public GameObject foodPrefab;


    [SerializeField] private List<GameObject> foodsList = new();
    private IEnumerable<GameObject> combinedEatableTargetsList = new List<GameObject>();


    protected override void Start()
    {
        base.Start();
        foodsList = GameManager.currentActiveFoodTargetObjectsList;
        combinedEatableTargetsList = targetObjectsList.Concat(foodsList);
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }


    // Get the nearest object position
    protected override Vector3 GetNearestObjectPosition()
    {
        float nearestDistance = Mathf.Infinity;
        GameObject nearestTargetObject = null;

        Vector3 currentPosition = transform.position;

        foreach (GameObject targetObject in combinedEatableTargetsList) // The loop iterates through the combined lists (main fish and food lists) to find the nearest object to the enemy instead of just the main fishies list
        {
            Vector3 directionToTarget = targetObject.transform.position - currentPosition;
            float distanceSqr = directionToTarget.sqrMagnitude;

            if (distanceSqr < nearestDistance)
            {
                nearestDistance = distanceSqr;
                nearestTargetObject = targetObject;

                float inRangeThresholdSqr = followerProperties.nearestDistanceToEatATarget * followerProperties.nearestDistanceToEatATarget;

                // Early exit if within interaction range
                if (distanceSqr <= inRangeThresholdSqr)
                {
                    break;
                }
            }
        }

        lastNearestTargetObject = nearestTargetObject;
        return nearestTargetObject != null ? nearestTargetObject.transform.position : Vector3.zero;
    }

    // Determine the type of the target object and handle the interaction accordingly
    protected override void HandleTargetObjectInteraction(GameObject targetObject)
    {
        if (targetObject.GetComponent<Food>() != null)
        {
            Food foodInstance = targetObject.GetComponent<Food>();
            health -= foodInstance.foodConfig.damage;
        }
        else
        {
            currentNumberOfEatenObjects++;
        }

        Destroy(targetObject);

        if (currentNumberOfEatenObjects >= health || health <= 0)
        {
            Destroy(gameObject);
        }
        else if (currentNumberOfEatenObjects >= numberOfObjectsToEat)
        {
            hungerTimeCounter = 0f;
            numberOfObjectsToEat += nextNumberOfObjectsToEat;
            hungerStartingTime = Random.Range(enemyProperties.minRandomHungerTime, enemyProperties.maxRandomHungerTime); // Hunger starting duration is between 3 and 10 seconds
        }
    }


    public override int GetNumberOfTargetObjectsInList()
    {
        return combinedEatableTargetsList.Count();
    }

    public override Vector3 CheckTargetDirection()
    {
        return base.CheckTargetDirection();
    }

    protected override void DetectAndDestroyNearestObjects()
    {
        base.DetectAndDestroyNearestObjects();
    }

    public override void TakeDamage(int damage)
    {
        throw new System.NotImplementedException();
    }

    protected override void HungerHandler()
    {
        base.HungerHandler();
    }

    public override bool IsHungry()
    {
        return base.IsHungry();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }
}
