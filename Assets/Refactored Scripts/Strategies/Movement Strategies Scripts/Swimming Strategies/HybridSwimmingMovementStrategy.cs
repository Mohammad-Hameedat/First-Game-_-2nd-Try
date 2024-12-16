using UnityEngine;

public class HybridSwimmingMovementStrategy : IMovementStrategy
{

    #region Movement Managers
    private readonly MovementController movementController;
    private readonly TargetingSystem targetingSystem;
    private readonly BoundsAndPositioningManager boundsManager;

    private readonly MovementProperties movementProperties;
    #endregion

    #region Random Movement Variables
    private Vector3 randomTargetPosition;
    private float desiredVelocity;
    private float AccelerationDuration;
    private float timeBeforeChangingVelocity;
    #endregion

    #region Constructor
    public HybridSwimmingMovementStrategy(MovementController _movementController, TargetingSystem _targetingSystem)
    {
        movementController = _movementController;
        targetingSystem = _targetingSystem;
        movementProperties = movementController.movementProperties;
        boundsManager = movementController.boundsManager;

        InitializeRandomMovement();
    }
    #endregion

    // 2 different movement strategies based on the existence of a target
    public void Move(Rigidbody rb)
    {
        Transform target = targetingSystem.GetNearestTarget();

        if (target != null)
        {
            // Move towards the target
            Vector3 positionDifference = target.position - rb.position;
            Vector3 directionToTarget = positionDifference.normalized;
            float targetFollowingVelocity = movementProperties.maxFollowingDesiredVelocity;
            rb.velocity = Vector3.Lerp(rb.velocity, directionToTarget * targetFollowingVelocity, Time.fixedDeltaTime);
        }
        else
        {
            // Move randomly
            timeBeforeChangingVelocity += Time.fixedDeltaTime;

            if (timeBeforeChangingVelocity >= AccelerationDuration)
            {
                desiredVelocity = Random.Range(movementProperties.minRandomDesiredVelocity, movementProperties.maxRandomDesiredVelocity);
                AccelerationDuration = Random.Range(movementProperties.minAccelerationDuration, movementProperties.maxAccelerationDuration);
                timeBeforeChangingVelocity = 0f;
            }

            Vector3 positionDifference = randomTargetPosition - rb.position;

            if (positionDifference.sqrMagnitude <= movementProperties.minDistanceTowardsRandomTarget * movementProperties.minDistanceTowardsRandomTarget)
            {
                randomTargetPosition = boundsManager.GetNewRandomPosition();
            }
            Vector3 directionToRandomTarget = positionDifference.normalized;

            // Change the velocity of the object smoothly while moving towards the target position
            rb.velocity = Vector3.Lerp(rb.velocity, directionToRandomTarget * desiredVelocity, Time.fixedDeltaTime);
        }
    }

    // Initialize the random movement strategy of the object
    private void InitializeRandomMovement()
    {
        randomTargetPosition = boundsManager.GetNewRandomPosition();
        desiredVelocity = Random.Range(movementProperties.minRandomDesiredVelocity, movementProperties.maxRandomDesiredVelocity);
        AccelerationDuration = Random.Range(movementProperties.minAccelerationDuration, movementProperties.maxAccelerationDuration);
        timeBeforeChangingVelocity = 0f;
    }
}