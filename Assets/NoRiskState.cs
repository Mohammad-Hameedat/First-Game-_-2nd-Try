public class NoRiskState : IState
{
    private MovementController movementController;
    private TargetingSystem targetingSystem;
    private IMovementStrategy movementStrategy;

    public NoRiskState(MovementController _movementController, TargetingSystem _targetingSystem)
    {
        movementController = _movementController;
        targetingSystem = _targetingSystem;
    }

    public NoRiskState(MovementController _movementController)
    {
        movementController = _movementController;
    }


    public void Enter()
    {
        if (targetingSystem != null)
        {
            movementStrategy = new HungryMovementStrategy(movementController, targetingSystem);
        }
        else
        {
            movementStrategy = new RandomMovementStrategy(movementController);
        }

        movementController.SetMovementStrategy(movementStrategy);
    }

    public void Execute()
    {
        // Additional logic if needed
        // For example, if the object is not hungry anymore, change the state to IdleState
    }

    public void Exit()
    {
        movementController.SetMovementStrategy(null);
        // Clean up if necessary
        // For example, reset the hunger state of the object
    }
}
