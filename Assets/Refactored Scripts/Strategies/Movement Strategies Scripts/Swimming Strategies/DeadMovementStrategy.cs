using UnityEngine;

public class DeadMovementStrategy : IMovementStrategy
{
    private MovementController movementController;

    public DeadMovementStrategy(MovementController _movementController)
    {
        movementController = _movementController;
    }

    public void Move(Rigidbody rb)
    {
        if (rb.velocity.x != 0)
        {
            Vector3 smoothedVelocity = Vector3.Lerp(
                rb.velocity,
                new Vector3(0f, -1f, rb.velocity.z),
                Time.deltaTime);

            rb.velocity = smoothedVelocity;
        }
        else
        {
            rb.velocity = Vector3.down;
        }
    }
}