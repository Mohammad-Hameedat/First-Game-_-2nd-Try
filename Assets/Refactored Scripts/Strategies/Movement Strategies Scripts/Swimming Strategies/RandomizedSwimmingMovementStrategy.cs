using UnityEngine;

public class RandomizedSwimmingMovementStrategy : IMovementStrategy
{
    private MovementController movementController;
    private MovementProperties movementProperties;
    private BoundsAndPositioningManager boundsManager;

    private Vector3 randomTargetPosition;
    private float desiredVelocity;
    private float AccelerationDuration;
    private float timeBeforeChangingVelocity;


    public RandomizedSwimmingMovementStrategy(MovementController _movementController)
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
            desiredVelocity = Random.Range(
                movementProperties.minRandomDesiredVelocity,
                movementProperties.maxRandomDesiredVelocity
                );

            AccelerationDuration = Random.Range(
                movementProperties.minAccelerationDuration,
                movementProperties.maxAccelerationDuration
                );

            timeBeforeChangingVelocity = 0f;
        }

        Vector3 positionDifference = randomTargetPosition - rb.position;

        float minDistanceToRandomTarget = movementProperties.minDistanceTowardsRandomTarget;

        if (positionDifference.sqrMagnitude <= minDistanceToRandomTarget * minDistanceToRandomTarget)
        {
            randomTargetPosition = boundsManager.GenerateRandomClampedPosition();
        }

        Vector3 directionToRandomTarget = positionDifference.normalized;

        // Change the velocity of the object smoothly while moving towards the target position
        rb.velocity = Vector3.Lerp(
            rb.velocity,
            directionToRandomTarget * desiredVelocity,
            Time.fixedDeltaTime
            );
    }


    // Initialize the movement strategy (Random Movement) of the object
    private void InitializeMovement()
    {
        movementProperties = movementController.movementProperties;

        boundsManager = movementController.boundsManager;

        randomTargetPosition = boundsManager.GenerateRandomClampedPosition();


        desiredVelocity = Random.Range(
            movementProperties.minRandomDesiredVelocity,
            movementProperties.maxRandomDesiredVelocity
            );

        AccelerationDuration = Random.Range(
            movementProperties.minAccelerationDuration,
            movementProperties.maxAccelerationDuration
            );
    }
}
