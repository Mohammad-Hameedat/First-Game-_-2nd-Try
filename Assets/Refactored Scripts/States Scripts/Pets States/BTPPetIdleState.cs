public class BTPPetIdleState : IState
{
    private MovementController movementController;


    public BTPPetIdleState(MovementController _movementController)
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
        // May execute animations here
    }

    public void Exit() { }
}