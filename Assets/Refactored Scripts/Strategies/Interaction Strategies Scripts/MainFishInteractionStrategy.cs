using UnityEngine;

public class MainFishInteractionStrategy : IInteractionStrategy
{
    private TargetingSystem targetingSystem;
    public HungerSystem hungerSystem;

    private int currentNumberofEatenObjects = 0;

    public MainFishInteractionStrategy(HungerSystem _hungerSystem, TargetingSystem _targetingSystem)
    {
        hungerSystem = _hungerSystem;
        targetingSystem = _targetingSystem;
    }


    public Transform IdentifyNearestObject()
    {
        Transform nearestTarget = targetingSystem.GetlastNearestTarget();

        return nearestTarget;
    }

    public void Interact(GameObject interator, GameObject target)
    {
        if (GameManager.currentActiveFoodTargetObjectsList.Contains(target))
        {
            currentNumberofEatenObjects++;

            FoodProperties foodConfig = target.GetComponent<Food>().foodConfig;

            hungerSystem.hungerStrategy.SetHungerValues(
                 foodConfig.staminaTime,
              foodConfig.destructionTime
              );

            // Reset hunger
            hungerSystem.hungerStrategy?.ResetHunger();

            // Consume the target
            Object.Destroy(target);
        }
    }

    public int GetInteractedTargetsCount()
    {
        return currentNumberofEatenObjects;
    }
}