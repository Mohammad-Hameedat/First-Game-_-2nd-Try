using UnityEngine;

public class NTMRPetInteractionStrategy : IInteractionStrategy
{
    private TargetingSystem targetingSystem;
    private BoundsAndPositioningManager boundsAndPositioningManager;

    public NTMRPetInteractionStrategy(
        TargetingSystem _targetingSystem,
        BoundsAndPositioningManager _boundsAndPositioningManager
        )
    {
        targetingSystem = _targetingSystem;
        boundsAndPositioningManager = _boundsAndPositioningManager;
    }


    public Transform IdentifyNearestObject()
    {
        /* Note:
         * If the NTMR pet unique ability is active, return null
         * this will prevent the pet from interacting with any object
         * and improve the performance of the game.
         */
        return !GameManager.NTMRpetUniqueAbility ? targetingSystem.GetNearestTarget() : null;
    }

    public void Interact(GameObject interator, GameObject target)
    {
        if (!GameManager.NTMRpetUniqueAbility)
        {
            CollectibleScript collectibleScript = target.GetComponent<CollectibleScript>();

            if (collectibleScript != null)
            {
                collectibleScript.isPushed = true;
            }
            else
            {
                Food foodScript = target.GetComponent<Food>();

                if (foodScript != null)
                {
                    foodScript.isPushed = true;
                }
            }
        }
    }

    public int GetInteractedTargetsCount() => 0;
}