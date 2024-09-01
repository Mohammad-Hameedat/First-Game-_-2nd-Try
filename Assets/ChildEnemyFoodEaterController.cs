using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildEnemyFoodEaterController : BaseFollowerController
{
    //public new List<GameObject> foodTargetObjectsList = new List<GameObject>();

    public List<GameObject> foodTargetsListInEnemyScript;

    protected override void Start()
    {
        timeBeforeGettingHungry = 0f;
        hungerStartingTime = 8f;
        timeBeforeDying = 15f;

        base.Start();
    }

    protected void Update()
    {

        foodTargetsListInEnemyScript = foodTargetObjectsList;

    }

}
