public class ThreatenedSwimmingState : IState
{
    private MovementController movementController;

    private IMovementStrategy movementStrategy;


    public ThreatenedSwimmingState(MovementController _movementController)
    {
        movementController = _movementController;
    }

    // StateMachine methods
    public void Enter()
    {
        movementStrategy = new RandomizedSwimmingMovementStrategy(movementController);
        movementController.SetMovementStrategy(movementStrategy); // Set the movement strategy of the object
    }

    public void Execute()
    {
        // Additional logic if needed
        // For example, if the object is hungry, change the state to HungryState
    }

    public void Exit()
    {
        movementController.SetMovementStrategy(null);

        // Clean up if necessary
        // For example, reset the hunger state of the object
    }
}
