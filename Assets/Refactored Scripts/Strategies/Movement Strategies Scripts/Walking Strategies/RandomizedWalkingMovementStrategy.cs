using UnityEngine;

public class RandomizedWalkingMovementStrategy : IMovementStrategy
{
    private MovementController movementController;
    private MovementProperties movementProperties;
    private BoundsAndPositioningManager boundsManager;

    private Vector3 randomTargetPosition;
    private float desiredVelocity;
    private float AccelerationDuration;
    private float timeBeforeChangingVelocity;


    public RandomizedWalkingMovementStrategy(MovementController _movementController)
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
            desiredVelocity = Random.Range(movementProperties.minRandomDesiredVelocity, movementProperties.maxRandomDesiredVelocity);
            AccelerationDuration = Random.Range(movementProperties.minAccelerationDuration, movementProperties.maxAccelerationDuration);
            timeBeforeChangingVelocity = 0f;
        }

        Vector3 positionDifference = randomTargetPosition - rb.position;

        // Zero out Y and Z for pure X-axis movement
        positionDifference.y = 0f;
        positionDifference.z = 0f;

        if (positionDifference.sqrMagnitude <= movementProperties.minDistanceTowardsRandomTarget * movementProperties.minDistanceTowardsRandomTarget)
        {
            randomTargetPosition = boundsManager.GetNewRandomPosition();
        }

        Vector3 directionToRandomTarget = positionDifference.normalized;

        // Calculate the interpolated velocity
        Vector3 calculatedXVelocity = Vector3.Lerp(rb.velocity, directionToRandomTarget * desiredVelocity, Time.fixedDeltaTime / 2f);
        // Force the object to only move along X-axis
        calculatedXVelocity = new(calculatedXVelocity.x, 0f, 0f);

        // Change the velocity of the object smoothly while moving towards the target position
        rb.velocity = calculatedXVelocity;
    }


    // Initialize the movement strategy (Random Movement) of the object
    private void InitializeMovement()
    {
        movementProperties = movementController.movementProperties;
        boundsManager = movementController.boundsManager;

        randomTargetPosition = boundsManager.GetNewRandomPosition();
        desiredVelocity = Random.Range(movementProperties.minRandomDesiredVelocity, movementProperties.maxRandomDesiredVelocity);
        AccelerationDuration = Random.Range(movementProperties.minAccelerationDuration, movementProperties.maxAccelerationDuration);
    }
}
