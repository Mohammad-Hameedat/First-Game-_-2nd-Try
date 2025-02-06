using UnityEngine;

public class AggressiveInteractionStrategy : IInteractionStrategy
{
    private TargetingSystem targetingSystem;
    private AttackProperties attackProperties;

    private float elapsedTime = 0f;

    public AggressiveInteractionStrategy(TargetingSystem _targetingSystem, AttackProperties _attackProperties)
    {
        targetingSystem = _targetingSystem;
        attackProperties = _attackProperties;
    }


    public Transform IdentifyNearestObject()
    {
        Transform nearestTarget = targetingSystem.GetlastNearestTarget();

        return nearestTarget;
    }

    public void Interact(GameObject interator, GameObject target)
    {
        if (GameManager.currentActiveEnemyObjectsList.Contains(target))
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime < attackProperties.nextAttackHitTime)
                return;

            elapsedTime = 0f;
            Health targetHealth = target.GetComponent<Health>();
            targetHealth.TakeDamage(attackProperties.attackDamage);
        }
    }


    #region Not Implemented
    public int GetInteractedTargetsCount()
    {
        return 0;
    }
    #endregion
}