using UnityEngine;

public class InteractionController : MonoBehaviour
{
    private MovementController movementController;
    public IInteractionStrategy interactionStrategy;

    [SerializeField]
    private float interactionRange;


    public void SetInteractionStrategy(IInteractionStrategy _interactionStrategy)
    {
        interactionStrategy = _interactionStrategy;
    }

    private void Start()
    {
        movementController = GetComponent<MovementController>();
        interactionRange = movementController.movementProperties.nearestDistanceToEatATarget;
    }

    private void Update()
    {
        InteractWithNearestObject();
    }

    // Detect the nearest object and interact with it
    private void InteractWithNearestObject()
    {
        Transform nearestTarget = interactionStrategy?.IdentifyNearestObject();

        // Return if no target object is found
        if (nearestTarget == null)
            return;

        movementController.CurrentTarget = nearestTarget;

        // Check if the nearest target object is within the interaction range
        float distanceToTargetSqr = ( nearestTarget.position - transform.position ).magnitude;

        if (distanceToTargetSqr <= interactionRange)
        {
            // Interact with the target object
            interactionStrategy.Interact(gameObject, nearestTarget.gameObject);

            // Reset the nearest target object
            nearestTarget = null;
        }
    }
}
