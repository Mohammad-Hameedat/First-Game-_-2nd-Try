using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildMainFishFollowerController : BaseFollowerController
{

    // <<<<<<Will be deleted later>>>>>
    // <<<<<<Will be deleted later>>>>>
    // <<<<<<Will be deleted later>>>>>
    public List<GameObject> GetList1;

    protected override void Start()
    {
        targetObjectsList = GameManager.currentActiveFoodTargetObjectsList;

        currentMoneyIndex = 0;

        hungerStartingTime = followerProperties.hungerStartingTime;
        destructionTime = followerProperties.destructionTime;


        StartCoroutine(SpawnMoney());
    }


    protected override void Update()
    {
        // <<<<<<Will be deleted later>>>>>
        // <<<<<<Will be deleted later>>>>>
        // <<<<<<Will be deleted later>>>>>
        GetList1 = targetObjectsList;


        HungerHandler();
    }



    protected override void HungerHandler()
    {
        if (GameManager.currentActiveEnemyObjectsList.Count > 0)
        {
            return;
        }
        else
        {
            hungerTimeCounter += Time.deltaTime;

            if (hungerTimeCounter >= destructionTime)
            {
                Destroy(gameObject);
            }
        }
    }


    private IEnumerator SpawnMoney()
    {
        while (true)
        {
            if (currentNumberOfEatenObjects >= 3)
            {
                GameObject collectableInstance = Instantiate(collectablePrefab, transform.position, Quaternion.identity); // Spawn the money prefab

                /*
                 * Set the money configuration - Will be changed later
                 * Note: Must be changed when the follower eats a number of target objects that is over than 10
                 * Note: can be changed be setting If conditions to check the number of eaten objects
                 */
                collectableInstance.GetComponent<Collectable>().collectableConfig = followerProperties.collectableConfigs[currentMoneyIndex]; // Set the money configuration
            }

            float randomTimeBeforeNextMoneySpawn = Random.Range(7f, 15f); // Randomize the time before the next money spawn
            yield return new WaitForSeconds(randomTimeBeforeNextMoneySpawn); // Wait for the random time before the next money spawn
        }
    }


    protected override void OnDisable()
    {
        if (!this.gameObject.scene.isLoaded) return;
        GameManager.currentActiveMainFishObjectsList.Remove(gameObject);

        GameEvents.EventsChannelInstance.RefresheMainFishesNumber(GameManager.currentActiveMainFishObjectsList.Count);
    }

}