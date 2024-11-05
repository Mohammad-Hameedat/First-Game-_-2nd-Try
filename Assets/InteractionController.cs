using UnityEngine;

public abstract class InteractionController : MonoBehaviour
{
    public float interactionRange;
    protected TargetingSystem targetingSystem;
    protected FollowerProperties properties;

    protected virtual void Awake()
    {
        targetingSystem = GetComponent<TargetingSystem>();
        properties = GetComponent<MovementController>().properties;
        interactionRange = properties.nearestDistanceToEatATarget;
    }

    private void Update()
    {
        DetectionAndInteractionWithNearestObject();
    }

    // Detect the nearest object and interact with it
    private void DetectionAndInteractionWithNearestObject()
    {
        // Get the last nearest target object
        Transform nearestTarget = targetingSystem.GetlastNearestTarget();
        // If there is no nearest target object, return
        if (nearestTarget == null) return;


        // Check if the nearest target object is within the interaction range
        float distanceToTargetSqr = (nearestTarget.position - transform.position).magnitude;

        if (distanceToTargetSqr <= interactionRange)
        {
            InteractWithTarget(nearestTarget.gameObject);
        }
    }

    // Interact with the target object
    protected abstract void InteractWithTarget(GameObject target);
}
