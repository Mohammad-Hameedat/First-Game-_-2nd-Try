public class GTAPetEnemySensitiveState : IState
{
    private MovementController movementController;
    private TargetingSystem targetingSystem;


    public GTAPetEnemySensitiveState(
        MovementController _movementController, 
        TargetingSystem _targetingSystem
        )
    {
        movementController = _movementController;
        targetingSystem = _targetingSystem;
    }

    public void Enter()
    {
        movementController.SetMovementStrategy(new ActionReadySwimmingMovementStrategy(
            movementController,
            targetingSystem
            ));
    }

    public void Execute() { }

    public void Exit()
    {
        movementController.SetMovementStrategy(null);
    }
}
