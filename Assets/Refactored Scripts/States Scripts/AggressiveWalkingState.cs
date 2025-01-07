public class AggressiveWalkingState : IState
{
    private TargetingSystem targetingSystem;
    private MovementController movementController;


    private IMovementStrategy movementStrategy;

    public AggressiveWalkingState(
        MovementController _movementController,
        TargetingSystem _targetingSystem
        )
    {
        movementController = _movementController;
        targetingSystem = _targetingSystem;
    }


    public void Enter()
    {
        movementStrategy = new HybridWalkingMovementStrategy(
            movementController,
            targetingSystem);

        movementController.SetMovementStrategy(movementStrategy);

        /* A different movement strategy can set later.
         
                movementStrategy = new RandomizedSwimmingMovementStrategy(
                    movementController);

                movementController.SetMovementStrategy(movementStrategy);
        */
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
