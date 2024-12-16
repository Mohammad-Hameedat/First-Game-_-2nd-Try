using UnityEngine;

public class SnailPetInteractionStrategy : IInteractionStrategy
{
    private TargetingSystem targetingSystem;

    public SnailPetInteractionStrategy(TargetingSystem _targetingSystem)
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
        if (GameManager.activeCollectablesList.Contains(target))
        {
            int collectable = target.GetComponent<Collectable>().collectableConfig.collectableValue;

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