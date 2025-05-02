public class ATAFIdleState : IState
{
    private MovementController movementController;

    public ATAFIdleState(MovementController _movementController)
    {
        movementController = _movementController;
    }

    public void Enter()
    {
        movementController.SetMovementStrategy(new HybridSwimmingMovementStrategy(
            movementController
        ));
    }

    public void Execute()
    {

    }

    public void Exit()
    {
        movementController.SetMovementStrategy(null);
    }
}
