using System.Collections.Generic;
using UnityEngine;

public class ChildEnemyController : BaseFollowerController
{

    public EnemyProperties enemyProperties;

    // <<<<<<Will be deleted later>>>>>
    // <<<<<<Will be deleted later>>>>>
    // <<<<<<Will be deleted later>>>>>
    public List<GameObject> showListContent;

    protected int numberOfObjectsToEat;
    protected int nextNumberOfObjectsToEat;


    [SerializeField] protected int health;


    protected override void Start()
    {
        health = enemyProperties.health;
        numberOfObjectsToEat = enemyProperties.numberOfObjectsToEat;
        nextNumberOfObjectsToEat = enemyProperties.nextNumberOfObjectsToEat;

        // Right now, the money index is set to 2 and later it must be implemented in different ways
        currentMoneyIndex = 0;

        targetObjectsList = GameManager.currentActiveMainFishObjectsList; // May be overridden in the child classes

        hungerStartingTime = Random.Range(2f, 5f);
    }


    protected override void Update()
    {

        // <<<<<<Will be deleted later>>>>>
        // <<<<<<Will be deleted later>>>>>
        // <<<<<<Will be deleted later>>>>>
        showListContent = targetObjectsList;

        HungerHandler();
    }

    protected virtual void LateUpdate()
    {
        DetectAndDestroyNearestObjects();
    }

    public override Vector3 CheckTargetDirection()
    {
        return lastNearestTargetObject != null ? lastNearestTargetObject.transform.position : Vector3.zero;
    }


    protected virtual void DetectAndDestroyNearestObjects()
    {
        Vector3 nearestTargetPosition = GetNearestObjectPosition();
        float distanceSqr = (nearestTargetPosition - transform.position).sqrMagnitude;
        if (distanceSqr <= followerProperties.nearestDistanceToEatATarget * followerProperties.nearestDistanceToEatATarget)
        {
            HandleTargetObjectInteraction(lastNearestTargetObject);
        }
    }

    protected override void HandleTargetObjectInteraction(GameObject targetObject)
    {
        currentNumberOfEatenObjects++;

        Destroy(targetObject);

        if (currentNumberOfEatenObjects >= health || health <= 0)
        {
            Destroy(gameObject);
        }
        else if (currentNumberOfEatenObjects >= numberOfObjectsToEat)
        {
            hungerTimeCounter = 0f;
            numberOfObjectsToEat += nextNumberOfObjectsToEat;
            hungerStartingTime = Random.Range(enemyProperties.minRandomHungerTime, enemyProperties.maxRandomHungerTime); // Hunger starting duration is set by the enemy properties
        }
    }

    public virtual void TakeDamage(int damage)
    {
        health -= Mathf.Abs(damage); // Damage is always positive so no once can cheat the system

        if (health <= 0)
        {
            Destroy(gameObject);
        }

        Debug.Log("Enemy Health: " + health);
    }

    public virtual int GetHealth()
    {
        return health;
    }


    protected override Vector3 GetNearestObjectPosition()
    {
        float nearestDistance = Mathf.Infinity;
        GameObject nearestTargetObject = null;

        Vector3 currentPosition = transform.position;

        float inRangeThresholdSqr = followerProperties.nearestDistanceToEatATarget * followerProperties.nearestDistanceToEatATarget;

        foreach (GameObject targetObject in targetObjectsList)
        {
            Vector3 directionToTarget = targetObject.transform.position - currentPosition;
            float distanceSqr = directionToTarget.sqrMagnitude;

            if (distanceSqr < nearestDistance)
            {
                nearestDistance = distanceSqr;
                nearestTargetObject = targetObject;


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

    public override int GetNumberOfTargetObjectsInList()
    {
        return targetObjectsList.Count;
    }

    public override bool IsHungry()
    {
        return base.IsHungry();
    }

    protected override void HungerHandler()
    {
        hungerTimeCounter += Time.deltaTime;
    }

    protected override void OnDisable()
    {
        if (!this.gameObject.scene.isLoaded) return;

        GameObject collectableInstance = Instantiate(collectablePrefab, transform.position, Quaternion.identity);
        collectableInstance.GetComponent<Collectable>().collectableConfig = followerProperties.collectableConfigs[currentMoneyIndex];

        GameManager.currentActiveEnemyObjectsList.Remove(gameObject);
    }
}