using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChildEnemyFoodEaterController : ChildEnemyController
{

    public GameObject targetPrefab_Food;

    [SerializeField] private List<GameObject> foodsList = new List<GameObject>();



    public IEnumerable<GameObject> combinedList = new List<GameObject>();


    protected override void Start()
    {
        base.Start();

        foodsList = GameManager.foodTargetObjectsList;

        combinedList = targetObjectsList.Concat(foodsList);


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
        GameObject nearestObject = null;

        Vector3 currentPosition = transform.position;

        foreach (GameObject targetObject in combinedList) // The loop iterates through the combined list of main fishies and food objects to find the nearest object to the enemy instead of just the main fishies list
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


    protected override void HandleTargetObjectInteraction(GameObject targetObject)
    {
        if (targetObject.GetType() == targetPrefab_Food.GetType())
        {
            health -= targetObject.GetComponent<Food>().foodConfig.damage;
        }
        else
        {
            numberOfEatenObjects++;
        }

        Destroy(targetObject);

        if (numberOfEatenObjects >= health)
        {
            Destroy(gameObject);
        }
        else if (health <= 0)
        {
            Destroy(gameObject);
        }
        else if (numberOfEatenObjects >= numberOfObjectsToEat)
        {
            timeBeforeGettingHungry = 0f;
            numberOfObjectsToEat += nextNumberOfObjectsToEat;
            hungerStartingTime = Random.Range(6f, 15f); // Hunger starting duration is between 6 and 15 seconds
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }


}
