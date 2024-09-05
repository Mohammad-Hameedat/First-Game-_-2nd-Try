using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildMainFishFollowerController : BaseFollowerController
{
    // <<<<<<Will be deleted later>>>>>
    // <<<<<<Will be deleted later>>>>>
    // <<<<<<Will be deleted later>>>>>
    public List <GameObject> GetList1;

    protected override void Start()
    {
        targetObjectsList = GameManager.foodTargetObjectsList;

        //timeBeforeGettingHungry = 0f;
        hungerStartingTime = 8f;
        timeBeforeDestruction = 15f;

        base.Start();

        StartCoroutine(SpawnMoney());
    }


    void Update()
    {
        // <<<<<<Will be deleted later>>>>>
        // <<<<<<Will be deleted later>>>>>
        // <<<<<<Will be deleted later>>>>>
        GetList1 = targetObjectsList;


        HungerHandler();
    }


    private IEnumerator SpawnMoney()
    {
        while (true)
        {
            if (numberOfEatenObjects >= 3)
            {
                float randomTime = Random.Range(moneyTypes[currentMoneyIndex].defaultTimeToInitiate, moneyTypes[currentMoneyIndex].defaultTimeToInitiate + 3f);
                yield return new WaitForSeconds(randomTime);
                Instantiate(moneyPrefab, transform.position, Quaternion.identity);
                moneyPrefab.GetComponent<Collectable>().moneyConfig = moneyTypes[currentMoneyIndex];
            }
            yield return null;
        }
    }
}