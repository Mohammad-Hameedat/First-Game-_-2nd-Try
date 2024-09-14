using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChildEnemyController : BaseFollowerController
{
    // <<<<<<Will be deleted later>>>>>
    // <<<<<<Will be deleted later>>>>>
    // <<<<<<Will be deleted later>>>>>
    public List<GameObject> showListContent;

    protected int numberOfObjectsToEat = 3;
    protected int nextNumberOfObjectsToEat = 3;


    protected int health;


    protected override void Start()
    {
        positionChangeThreshold = .5f;
        inRangeThreshold = 1.75f;

        targetObjectsList = GameManager.mainFishiesObjectsList; // May be overridden in the child classes

        timeBeforeGettingHungry = 0f;
        hungerStartingTime = Random.Range(0f, 5f);

        base.Start();
    }


    protected virtual void Update()
    {

        // <<<<<<Will be deleted later>>>>>
        // <<<<<<Will be deleted later>>>>>
        // <<<<<<Will be deleted later>>>>>
        showListContent = targetObjectsList;

        DetectAndDestroyNearestObjects();

        HungerHandler();
    }


    protected virtual void DetectAndDestroyNearestObjects()
    {
        Vector3 nearestPosition = GetNearestObjectPosition();
        float distanceSqr = (transform.position - nearestPosition).sqrMagnitude;

        float inRangeThresholdSqr = inRangeThreshold * inRangeThreshold;

        if (distanceSqr <= inRangeThresholdSqr)
        {
            HandleTargetObjectInteraction(lastNearestObject);
        }


        //if ((transform.position - GetNearestObjectPosition()).magnitude <= inRangeThreshold)
        //{
        //    HandleTargetObjectInteraction(lastNearestObject);
        //}
    }

    protected override void HandleTargetObjectInteraction(GameObject targetObject)
    {
        numberOfEatenObjects++;

        Destroy(lastNearestObject);

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