public class NoThreatWalkingState : IState
{
    private TargetingSystem targetingSystem;
    private MovementController movementController;
    private HungerSystem hungerSystem;


    private IMovementStrategy movementStrategy;

    public NoThreatWalkingState(
        MovementController _movementController,
        TargetingSystem _targetingSystem,
        HungerSystem _hungerStrategy
        )
    {
        movementController = _movementController;
        targetingSystem = _targetingSystem;
        hungerSystem = _hungerStrategy;
    }


    public void Enter()
    {
        //throw new System.NotImplementedException();
    }

    public void Execute()
    {
        if (hungerSystem.IsHungry() && movementStrategy is not HybridWalkingMovementStrategy)
        {
            movementStrategy = new HybridWalkingMovementStrategy(
                movementController,
                targetingSystem
                );

            movementController.SetMovementStrategy(movementStrategy);
        }
        else if (!hungerSystem.IsHungry() && movementStrategy is not RandomizedWalkingMovementStrategy)
        {
            movementStrategy = new RandomizedWalkingMovementStrategy(
                movementController
                );

            movementController.SetMovementStrategy(movementStrategy);
        }
    }

    public void Exit()
    {
        movementController.SetMovementStrategy(null);
    }
}
