using UnityEngine;

public class SingleTargetFollowingMovementStrategy : IMovementStrategy
{

    #region References
    private readonly MovementController movementController;
    private readonly MovementProperties movementProperties;
    #endregion

    private Transform target = null;
    private float randomDispersionFactor;
    private float targetFollowingVelocity;

    #region Constructor
    public SingleTargetFollowingMovementStrategy(
        MovementController _movementController,
        Transform protectivePetTarget
        )
    {
        movementController = _movementController;
        movementProperties = movementController.movementProperties;
        targetFollowingVelocity = movementProperties.maxFollowingDesiredVelocity + 1f;

        target = protectivePetTarget;
        randomDispersionFactor = Random.Range(-1f, 1f);
    }
    #endregion


    public void Move(Rigidbody rb)
    {
        if (target != null)
        {
            // Move towards the target
            Vector3 dispersionOffset = new Vector2(randomDispersionFactor, randomDispersionFactor);
            Vector3 positionDifference = ( target.position - rb.position ) + dispersionOffset;
            Vector3 directionToTarget = positionDifference.normalized;
            rb.velocity = Vector3.Lerp(rb.velocity, directionToTarget * targetFollowingVelocity, Time.fixedDeltaTime);
        }
    }
}
