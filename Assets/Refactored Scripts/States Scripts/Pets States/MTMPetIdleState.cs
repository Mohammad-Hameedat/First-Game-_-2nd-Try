public class MTMPetIdleState : IState
{
    private MovementController movementController;

    public MTMPetIdleState(MovementController _movementController)
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