using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeneralInteractionStrategy : IInteractionStrategy
{
    private TargetingSystem targetingSystem;
    public HungerSystem hungerSystem;

    private IEnumerable<GameObject> targetObjectsList;
    private int currentNumberofEatenObjects = 0;

    public GeneralInteractionStrategy(
        HungerSystem _hungerSystem,
        TargetingSystem _targetingSystem,
        IEnumerable<GameObject> _targetObjectsList
        )
    {
        hungerSystem = _hungerSystem;
        targetingSystem = _targetingSystem;
        targetObjectsList = _targetObjectsList;
    }


    public Transform IdentifyNearestObject()
    {
        Transform nearestTarget = targetingSystem.GetlastNearestTarget();

        return nearestTarget;
    }

    public void Interact(GameObject interator, GameObject target)
    {
        /* Note:
         * Automatically define the target objects list
         * based on the type of targets that the interactor can interact with
        */
        if (targetObjectsList.Contains(target))
        {
            currentNumberofEatenObjects++;

            hungerSystem.hungerStrategy.ResetHunger();

            // Consume the target
            Object.Destroy(target);
        }
    }

    public int GetInteractedTargetsCount()
    {
        return currentNumberofEatenObjects;
    }
}