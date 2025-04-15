using UnityEngine;

public class HybridSwimmingMovementStrategy : IMovementStrategy
{
    #region References
    private readonly MovementController movementController;
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

    #region Constructor
    public HybridSwimmingMovementStrategy(
        MovementController _movementController
        )
    {
        movementController = _movementController;
        movementProperties = movementController.movementProperties;
        boundsManager = movementController.boundsManager;

        InitializeRandomMovement();
    }
    #endregion


    // 2 different movement strategies based on the existence of a target
    public void Move(Rigidbody rb)
    {
        target = movementController.CurrentTarget;

        if (target != null)
        {
            // Move towards the target
            Vector3 positionDifference = target.position - rb.position;
            Vector3 directionToTarget = positionDifference.normalized;
            float targetFollowingVelocity = movementProperties.maxFollowingDesiredVelocity;

            rb.velocity = Vector3.Lerp(
                rb.velocity,
                directionToTarget * targetFollowingVelocity,
                Time.fixedDeltaTime
                );
        }
        else
        {
            // Move randomly
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

            if (positionDifference.sqrMagnitude <= movementProperties.minDistanceTowardsRandomTarget * movementProperties.minDistanceTowardsRandomTarget)
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
    }

    // Initialize the random movement strategy of the object
    private void InitializeRandomMovement()
    {
        randomTargetPosition = boundsManager.GenerateRandomClampedPosition();

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
}