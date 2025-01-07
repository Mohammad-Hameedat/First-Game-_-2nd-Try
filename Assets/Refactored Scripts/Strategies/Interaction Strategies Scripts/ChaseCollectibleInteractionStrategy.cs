using UnityEngine;

public class ChaseCollectibleInteractionStrategy : IInteractionStrategy
{
    private TargetingSystem targetingSystem;

    public ChaseCollectibleInteractionStrategy(TargetingSystem _targetingSystem)
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
        if (GameManager.currentActiveCollectiblesList.Contains(target))
        {
            int collectable = target.GetComponent<CollectibleScript>().collectibleProperties.collectableValue;

            GameManager.AddCoins(collectable);

            // Consume the target
            Object.Destroy(target);
        }
    }

    public int GetEatenObjectsCount()
    {
        throw new System.NotImplementedException("Snail pet does not eat objects");
    }
}