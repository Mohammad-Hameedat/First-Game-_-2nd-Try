public class IdleState : IState
{
    private MovementController movementController;
    private IMovementStrategy movementStrategy;


    public IdleState(MovementController _movementController)
    {
        movementController = _movementController;
    }

    // StateMachine methods
    public void Enter()
    {
        movementStrategy = new RandomMovementStrategy(movementController);
        movementController.SetMovementStrategy(movementStrategy); // Set the movement strategy of the object
    }

    public void Execute()
    {
        // Additional logic if needed
        // For example, if the object is hungry, change the state to HungryState
    }

    public void Exit()
    {
        // Clean up if necessary
        // For example, reset the hunger state of the object

    }
}
