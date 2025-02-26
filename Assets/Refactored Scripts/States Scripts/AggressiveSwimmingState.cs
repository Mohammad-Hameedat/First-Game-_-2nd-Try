public class AggressiveSwimmingState : IState
{
    private MovementController movementController;


    private IMovementStrategy movementStrategy;

    public AggressiveSwimmingState(
        MovementController _movementController
        )
    {
        movementController = _movementController;
    }


    public void Enter()
    {
        movementStrategy = new HybridSwimmingMovementStrategy(
            movementController
            );

        movementController.SetMovementStrategy(movementStrategy);
    }

    public void Execute()
    {
        // This will handle animations and other stuff related to this state.
    }

    public void Exit()
    {
        movementController.SetMovementStrategy(null);
    }
}
