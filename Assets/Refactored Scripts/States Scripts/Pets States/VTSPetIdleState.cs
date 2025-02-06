public class VTSPetIdleState : IState
{
    private MovementController movementController;

    public VTSPetIdleState(MovementController _movementController)
    {
        movementController = _movementController;
    }

    public void Enter()
    {
        movementController.SetMovementStrategy(new RandomizedSwimmingMovementStrategy(
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