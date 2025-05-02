using Unity.VisualScripting;
using UnityEngine;

public class RevivalInteractionStrategy : IInteractionStrategy
{
    private MovementController movementController;
    private TargetingSystem targetingSystem;

    private int currentNumberofEatenObjects = 0;

    public RevivalInteractionStrategy(
        MovementController _movementController,
        TargetingSystem _targetingSystem
        )
    {
        movementController = _movementController;
        targetingSystem = _targetingSystem;
    }


    public int GetInteractedTargetsCount()
    {
        return currentNumberofEatenObjects;
    }

    public Transform IdentifyNearestObject()
    {
        Transform nearestTarget = targetingSystem.GetlastNearestTarget();

        if (nearestTarget != null && nearestTarget.GetComponent<DeathAndRevivalSystem>().corpseStrategy.IsDead)
        {
            return nearestTarget;
        }
        
        movementController.CurrentTarget = null;
        return null;
    }

    public void Interact(GameObject interactor, GameObject target)
    {
        if (GameManager.currentActiveCorpsedObjectsList.Contains(target))
        {
            currentNumberofEatenObjects++;

            // Reset target hunger timing.
            target.GetComponent<HungerSystem>().hungerStrategy.ResetHunger();

            target.GetComponent<DeathAndRevivalSystem>().corpseStrategy.TriggerRevivalState();

            target = null;
        }
    }
}
