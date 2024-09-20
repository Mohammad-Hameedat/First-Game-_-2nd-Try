using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChildEnemyController : BaseFollowerController
{
    // <<<<<<Will be deleted later>>>>>
    // <<<<<<Will be deleted later>>>>>
    // <<<<<<Will be deleted later>>>>>
    public List<GameObject> showListContent;

    protected int numberOfObjectsToEat = 10;
    protected int nextNumberOfObjectsToEat = 3;


    [SerializeField] protected int health;


    protected override void Start()
    {
        nearestDistanceToTargetToEat = 2f;

        targetObjectsList = GameManager.mainFishObjectsList; // May be overridden in the child classes

        timeBeforeGettingHungry = 0f;
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
        // Handle the target object interaction if the object is close enough to eat
        if (lastNearestTargetObject != null && IsCloseEnoughToEat())
        {
            HandleTargetObjectInteraction(lastNearestTargetObject);
        }

        //lastPosition = transform.position;
        return lastNearestTargetObject.transform.position;
    }


    protected virtual void DetectAndDestroyNearestObjects()
    {
        Vector3 nearestTargetPosition = GetNearestObjectPosition();

        float distanceSqr = (nearestTargetPosition - transform.position).sqrMagnitude;
        if (distanceSqr <= nearestDistanceToTargetToEat * nearestDistanceToTargetToEat)
        {
            HandleTargetObjectInteraction(lastNearestTargetObject);
        }
    }

    protected override void HandleTargetObjectInteraction(GameObject targetObject)
    {
        numberOfEatenObjects++;

        Destroy(lastNearestTargetObject);

        if (numberOfEatenObjects >= numberOfObjectsToEat)
        {
            timeBeforeGettingHungry = 0f;
            numberOfObjectsToEat += nextNumberOfObjectsToEat;
            hungerStartingTime = Random.Range(6f, 15f); // Hunger starting duration is between 6 and 15 seconds
        }
    }

    protected override void HungerHandler()
    {
        timeBeforeGettingHungry += Time.deltaTime;
    }


    protected override void OnDestroy()
    {
        if (!this.gameObject.scene.isLoaded) return;

        Instantiate(moneyPrefab, transform.position, Quaternion.identity);
        moneyPrefab.GetComponent<Collectable>().moneyConfig = moneyTypes[currentMoneyIndex];

        GameManager.enemiesTargetObjectsList.Remove(gameObject);
    }

}