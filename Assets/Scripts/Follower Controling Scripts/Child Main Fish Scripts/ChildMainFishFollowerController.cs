using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildMainFishFollowerController : BaseFollowerController
{

    // <<<<<<Will be deleted later>>>>>
    // <<<<<<Will be deleted later>>>>>
    // <<<<<<Will be deleted later>>>>>
    public List<GameObject> GetList;


    protected override void Start()
    {
        timeBeforeGettingHungry = 0f;
        hungerStartingTime = 8f;
        timeBeforeDying = 15f;

        base.Start();

        StartCoroutine(SpawnMoney());
    }


    void Update()
    {
        // <<<<<<Will be deleted later>>>>>
        // <<<<<<Will be deleted later>>>>>
        // <<<<<<Will be deleted later>>>>>
        GetList = foodTargetObjectsList;

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