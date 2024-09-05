using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildEnemyFoodEaterController : BaseFollowerController
{
    // <<<<<<Will be deleted later>>>>>
    // <<<<<<Will be deleted later>>>>>
    // <<<<<<Will be deleted later>>>>>
    public List<GameObject> getList;


    protected override void Start()
    {
        targetObjectsList = GameManager.mainFishiesObjectsList;


        timeBeforeGettingHungry = 0f;
        hungerStartingTime = 8f;
        timeBeforeDestruction = 15f;

        base.Start();
    }

    protected void Update()
    {
        // <<<<<<Will be deleted later>>>>>
        // <<<<<<Will be deleted later>>>>>
        // <<<<<<Will be deleted later>>>>>
        getList = GameManager.mainFishiesObjectsList;
    }
}
