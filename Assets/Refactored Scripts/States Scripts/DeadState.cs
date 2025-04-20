public class DeadState : IState
{
    private MovementController movementController;

    public DeadState(
        MovementController _movementController
        )
    {
        movementController = _movementController;
    }

    public void Enter()
    {
        // Change the MeshRenderer to the dead object to indicate that the object is dead.

        movementController.SetMovementStrategy(new DeadMovementStrategy(
            movementController
            ));
    }

    public void Execute() { }

    public void Exit()
    {
        movementController.SetMovementStrategy(null);
    }
}
