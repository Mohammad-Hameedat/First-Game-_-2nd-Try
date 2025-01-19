using UnityEngine;

public class EnemyInteractionStrategy : IInteractionStrategy
{
    private TargetingSystem targetingSystem;
    private Health health;

    public EnemyInteractionStrategy(GameObject interactor, TargetingSystem _targetingSystem)
    {
        health = interactor.GetComponent<Health>();
        targetingSystem = _targetingSystem;
    }

    public Transform IdentifyNearestObject()
    {
        Transform nearestTarget = targetingSystem.GetNearestTarget();

        return nearestTarget;
    }

    public void Interact(GameObject interactor, GameObject target)
    {
        if (target.activeSelf)
        {
            if (GameManager.currentActiveFoodTargetObjectsList.Contains(target))
            {
                // Target is food
                Food foodInstance = target.GetComponent<Food>();
                health.TakeDamage(foodInstance.foodConfig.damage);
                Object.Destroy(target);
            }
            else if (GameManager.currentActiveMainFishObjectsList.Contains(target) && target.activeInHierarchy == true)
            {
                // Target is main fish
                Object.Destroy(target);
            }
        }

        if (health.GetCurrentHealth() <= 0)
        {
            Object.Destroy(interactor);
        }
    }

    public int GetInteractedTargetsCount()
    {
        return 0;
    }
}