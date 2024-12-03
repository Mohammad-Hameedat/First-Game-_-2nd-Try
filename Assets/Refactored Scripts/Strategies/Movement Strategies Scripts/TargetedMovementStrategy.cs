using UnityEngine;

public class TargetedMovementStrategy : IMovementStrategy
{
    private readonly MovementController movementController;
    private readonly TargetingSystem targetingSystem;
    private readonly FollowerProperties properties;
    private readonly BoundsAndPositioningManager boundsManager;

    // Variables for random movement
    private Vector3 randomTargetPosition;
    private float desiredVelocity;
    private float AccelerationDuration;
    private float timeBeforeChangingVelocity;


    public TargetedMovementStrategy(MovementController _movementController, TargetingSystem _targetingSystem)
    {
        movementController = _movementController;
        targetingSystem = _targetingSystem;
        properties = movementController.properties;
        boundsManager = movementController.boundsManager;

        InitializeRandomMovement();
    }

    // 2 different movement strategies based on the state of the object
    public void Move(Rigidbody rb)
    {
        Transform target = targetingSystem.GetNearestTarget();

        if (target != null)
        {
            // Move towards the target
            Vector3 positionDifference = target.position - rb.position;
            Vector3 movementDirection = positionDifference.normalized;
            float desiredVelocity = properties.maxFollowingDesiredVelocity;
            rb.velocity = Vector3.Lerp(rb.velocity, movementDirection * desiredVelocity, Time.fixedDeltaTime);
        }
        else
        {
            timeBeforeChangingVelocity += Time.fixedDeltaTime;

            if (timeBeforeChangingVelocity >= AccelerationDuration)
            {
                desiredVelocity = Random.Range(properties.minRandomDesiredVelocity, properties.maxRandomDesiredVelocity);
                AccelerationDuration = Random.Range(properties.minAccelerationDuration, properties.maxAccelerationDuration);
                timeBeforeChangingVelocity = 0f;
            }

            Vector3 positionDifference = randomTargetPosition - rb.position;

            if (positionDifference.sqrMagnitude <= properties.minDistanceTowardsRandomTarget * properties.minDistanceTowardsRandomTarget)
            {
                randomTargetPosition = boundsManager.GetNewRandomPosition();
            }
            Vector3 directionToTarget = positionDifference.normalized;
            rb.velocity = Vector3.Lerp(rb.velocity, directionToTarget * desiredVelocity, Time.fixedDeltaTime); // Change the velocity of the object smoothly while moving towards the target position
        }
    }

    // Initialize the random movement strategy of the object
    private void InitializeRandomMovement()
    {
        randomTargetPosition = boundsManager.GetNewRandomPosition();
        desiredVelocity = Random.Range(properties.minRandomDesiredVelocity, properties.maxRandomDesiredVelocity);
        AccelerationDuration = Random.Range(properties.minAccelerationDuration, properties.maxAccelerationDuration);
        timeBeforeChangingVelocity = 0f;
    }
}
