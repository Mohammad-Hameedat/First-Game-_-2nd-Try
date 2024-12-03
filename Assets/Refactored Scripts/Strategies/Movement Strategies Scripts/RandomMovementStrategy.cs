using UnityEngine;

public class RandomMovementStrategy : IMovementStrategy
{
    private MovementController movementController;
    private FollowerProperties properties;
    private BoundsAndPositioningManager boundsManager;

    private Vector3 randomTargetPosition;
    private float desiredVelocity;
    private float AccelerationDuration;
    private float timeBeforeChangingVelocity;


    public RandomMovementStrategy(MovementController _movementController)
    {
        movementController = _movementController;

        InitializeMovement();
    }


    // Move the object to a random target position
    public void Move(Rigidbody rb)
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


    // Initialize the movement strategy (Random Movement) of the object
    private void InitializeMovement()
    {
        properties = movementController.properties;
        boundsManager = movementController.boundsManager;


        randomTargetPosition = boundsManager.GetNewRandomPosition();
        desiredVelocity = Random.Range(properties.minRandomDesiredVelocity, properties.maxRandomDesiredVelocity);
        AccelerationDuration = Random.Range(properties.minAccelerationDuration, properties.maxAccelerationDuration);
    }
}
