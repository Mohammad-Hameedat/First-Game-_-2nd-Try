using UnityEngine;

public class HybridWalkingMovementStrategy : IMovementStrategy
{
    #region Movement Managers
    private readonly MovementController movementController;
    private readonly TargetingSystem targetingSystem;
    private readonly BoundsAndPositioningManager boundsManager;

    private readonly MovementProperties movementProperties;
    #endregion

    private Transform target = null;

    #region Random Movement Variables
    private Vector3 randomTargetPosition;
    private float desiredVelocity;
    private float AccelerationDuration;
    private float timeBeforeChangingVelocity;
    #endregion

    public HybridWalkingMovementStrategy(
        MovementController _movementController,
        TargetingSystem _targetingSystem
        )
    {
        movementController = _movementController;
        targetingSystem = _targetingSystem;
        movementProperties = movementController.movementProperties;
        boundsManager = movementController.boundsManager;

        InitializeRandomMovement();
    }


    // 2 different movement strategies based on the existence of a target
    public void Move(Rigidbody rb)
    {
        target = movementController.CurrentTarget;

        if (target != null)
        {
            // Move towards the target only on the x-axis
            Vector3 positionDifference = target.position - rb.position;

            // Zero out Y and Z to ensure pure X-axis movement
            positionDifference.y = 0f;
            positionDifference.z = 0f;


            Vector3 directionToTarget = positionDifference.normalized;

            float targetFollowingVelocity = movementProperties.maxFollowingDesiredVelocity;

            // Calculate the interpolated velocity
            Vector3 calculatedXVelocity = Vector3.Lerp(rb.velocity, directionToTarget * targetFollowingVelocity, Time.fixedDeltaTime / 2f);
            // Force the object to only move along X-axis
            calculatedXVelocity = new(calculatedXVelocity.x, 0f, 0f);

            // Change the velocity of the object smoothly while moving towards the target position only on the x-axis
            rb.velocity = calculatedXVelocity;
        }
        else
        {
            // Move randomly only on the x-axis
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
                randomTargetPosition = boundsManager.GenerateRandomClampedPosition();
            }
            Vector3 directionToRandomTarget = positionDifference.normalized;

            // Calculate the interpolated velocity
            Vector3 calculatedXVelocity = Vector3.Lerp(rb.velocity, directionToRandomTarget * desiredVelocity, Time.fixedDeltaTime / 2f);
            // Force the object to only move along X-axis
            calculatedXVelocity = new(calculatedXVelocity.x, 0f, 0f);

            // Change the velocity of the object smoothly while moving towards the target position
            rb.velocity = calculatedXVelocity;
        }
    }

    // Initialize the random movement strategy of the object
    private void InitializeRandomMovement()
    {
        randomTargetPosition = boundsManager.GenerateRandomClampedPosition();
        desiredVelocity = Random.Range(movementProperties.minRandomDesiredVelocity, movementProperties.maxRandomDesiredVelocity);
        AccelerationDuration = Random.Range(movementProperties.minAccelerationDuration, movementProperties.maxAccelerationDuration);
        timeBeforeChangingVelocity = 0f;
    }
}
