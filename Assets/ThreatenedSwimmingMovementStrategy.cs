using UnityEngine;

public class ThreatenedSwimmingMovementStrategy : IMovementStrategy
{

    #region References
    private readonly MovementController movementController;

    private readonly MovementProperties movementProperties;
    #endregion

    private Transform target = null;


    #region Constructor
    public ThreatenedSwimmingMovementStrategy(
        MovementController _movementController,
        Transform protectivePetTarget
        )
    {
        movementController = _movementController;
        movementProperties = movementController.movementProperties;

        target = protectivePetTarget;
    }
    #endregion


    public void GetTarget()
    {

    }

    
    public void Move(Rigidbody rb)
    {
        if (target != null)
        {
            // Move towards the target
            Vector3 positionDifference = target.position - rb.position;
            Vector3 directionToTarget = positionDifference.normalized;
            float targetFollowingVelocity = movementProperties.maxFollowingDesiredVelocity;
            rb.velocity = Vector3.Lerp(rb.velocity, directionToTarget * targetFollowingVelocity, Time.fixedDeltaTime);
        }

    }
}
