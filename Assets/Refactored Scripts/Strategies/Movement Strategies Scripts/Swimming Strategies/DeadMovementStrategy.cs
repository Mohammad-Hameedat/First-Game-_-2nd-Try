using UnityEngine;

public class DeadMovementStrategy : IMovementStrategy
{
    private MovementController movementController;

    private Vector3 bottomPosition;

    public DeadMovementStrategy(MovementController _movementController)
    {
        movementController = _movementController;

        bottomPosition = movementController.boundsManager.CornerToWorldPosition(ScreenCorner.BottomCenter);
    }

    public void Move(Rigidbody rb)
    {
        if (rb.position.y > bottomPosition.y)
        {
            movementController.rb.velocity = Vector3.down;
        }
    }
}
