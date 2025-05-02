using UnityEngine;

public class DashingMovementStrategy : IMovementStrategy
{

    #region References
    private readonly MovementController movementController;
    private readonly BoundsAndPositioningManager boundsManager;

    private readonly MovementProperties movementProperties;
    #endregion

    private Transform target = null;

    private float targetFollowingVelocity = 0f;

    private bool isDashing = false;
    private float dashingVelocity = 10f;
    private float directionalVelocityAdjustmentSpeed = 5f;

    #region Random Movement Variables
    private Vector3 randomTargetPosition;
    private float desiredVelocity;
    private float AccelerationDuration;
    private float timeBeforeChangingVelocity;
    #endregion

    #region Constructor
    public DashingMovementStrategy(
        MovementController _movementController
        )
    {
        movementController = _movementController;
        movementProperties = movementController.movementProperties;
        targetFollowingVelocity = movementProperties.maxFollowingDesiredVelocity;

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
            isDashing = true;

            // Move towards the target
            Vector3 positionDifference = target.position - rb.position;
            Vector3 directionToTarget = positionDifference.normalized;

            rb.velocity = Vector3.Lerp(
                rb.velocity,
                directionToTarget * targetFollowingVelocity * dashingVelocity,
                Time.fixedDeltaTime * directionalVelocityAdjustmentSpeed
                );

            /* Note:
             * Since the interaction with the target is depending on the distance in this game,
             * then replace the interaction with the target with the following code,
             * and make sure you replace the mechanism of the interaction system.
            */
            /* if (positionDifference.magnitude <= movementProperties.nearestDistanceToEatATarget)
            {
                
            }
            */
        }
        else
        {

            if (isDashing)
            {
                rb.velocity = Vector3.Lerp(
                    rb.velocity,
                    rb.velocity.normalized * ( movementProperties.minRandomDesiredVelocity - 0.5f ),
                    Time.fixedDeltaTime * directionalVelocityAdjustmentSpeed * 1.5f
                    );

                _ = rb.velocity.magnitude <= movementProperties.minRandomDesiredVelocity
                    ? isDashing = false
                    : isDashing = true;
            }

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