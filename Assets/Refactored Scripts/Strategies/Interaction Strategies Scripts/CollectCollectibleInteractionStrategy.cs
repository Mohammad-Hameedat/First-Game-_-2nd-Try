using UnityEngine;

public class CollectCollectibleInteractionStrategy : IInteractionStrategy
{
    private TargetingSystem targetingSystem;

    public CollectCollectibleInteractionStrategy(TargetingSystem _targetingSystem)
    {
        targetingSystem = _targetingSystem;
    }


    public Transform IdentifyNearestObject()
    {
        Transform nearestTarget = targetingSystem.GetNearestTarget();

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

    public int GetInteractedTargetsCount()
    {
        //throw new System.NotImplementedException("Snail pet does not eat objects");

        return 0;
    }
}