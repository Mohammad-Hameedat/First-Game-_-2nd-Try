using UnityEngine;

public class ProtectiveGatheringInteractionStrategy : IInteractionStrategy
{
    private TargetingSystem targetingSystem;

    private int protectedTargetsCount = 0;

    public ProtectiveGatheringInteractionStrategy(
        TargetingSystem _targetingSystem
        )
    {
        targetingSystem = _targetingSystem;
    }


    public Transform IdentifyNearestObject()
    {
        Transform nearestTarget = targetingSystem.GetlastNearestTarget();

        return nearestTarget;
    }

    public void Interact(GameObject interator, GameObject target)
    {
        // NOTE: the (interator) parameter is not used in this strategy.

        if (protectedTargetsCount == GameManager.currentActiveMainFishObjectsList.Count)
        {
            return;
        }
        else if (GameManager.currentActiveMainFishObjectsList.Contains(target) && target.activeSelf)
        {
            //Debug.Log(message: "Protecting target: " + target.name);

            target.SetActive(false);
            protectedTargetsCount++;
        }
    }


    // Get the number of interacted targets.
    public int GetInteractedTargetsCount()
    {
        return protectedTargetsCount;
    }
}