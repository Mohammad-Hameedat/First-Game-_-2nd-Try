using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ChildEnemyFoodEaterController : ChildEnemyController
{

    public GameObject foodPrefab;

    [SerializeField] private List<GameObject> foodsList = new();

    private IEnumerable<GameObject> combinedEatableTargetsList = new List<GameObject>();


    protected override void Start()
    {
        base.Start();

        foodsList = GameManager.foodTargetObjectsList;

        combinedEatableTargetsList = targetObjectsList.Concat(foodsList);


        health = 30;
    }


    protected override void Update()
    {
        base.Update();

        //showListContent = combinedList.ToList();
    }


    // Get the nearest object position
    protected override Vector3 GetNearestObjectPosition()
    {
        float nearestDistance = Mathf.Infinity;
        GameObject nearestTargetObject = null;

        Vector3 currentPosition = transform.position;

        foreach (GameObject targetObject in combinedEatableTargetsList) // The loop iterates through the combined list of main fishies and food objects to find the nearest object to the enemy instead of just the main fishies list
        {
            Vector3 directionToTarget = targetObject.transform.position - currentPosition;
            float distanceSqr = directionToTarget.sqrMagnitude;

            if (distanceSqr < nearestDistance)
            {
                nearestDistance = distanceSqr;
                nearestTargetObject = targetObject;

                float inRangeThresholdSqr = nearestDistanceToTargetToEat * nearestDistanceToTargetToEat;

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


    protected override void HandleTargetObjectInteraction(GameObject targetObject)
    {
        if (targetObject.GetComponent<Food>() != null)
        {
            Food foodInstance = targetObject.GetComponent<Food>();
            health -= foodInstance.foodConfig.damage;
        }
        else
        {
            numberOfEatenObjects++;
        }

        Destroy(targetObject);

        if (numberOfEatenObjects >= health || health <= 0)
        {
            Destroy(gameObject);
        }
        else if (numberOfEatenObjects >= numberOfObjectsToEat)
        {
            timeBeforeGettingHungry = 0f;
            numberOfObjectsToEat += nextNumberOfObjectsToEat;
            hungerStartingTime = Random.Range(3f, 10f); // Hunger starting duration is between 6 and 15 seconds
        }
    }


    public override int GetNumberOfTargetObjectsInList()
    {
        return combinedEatableTargetsList.Count();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }


}
