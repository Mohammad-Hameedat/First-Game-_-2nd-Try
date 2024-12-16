public class NoThreatSwimmingState : IState
{
    private TargetingSystem targetingSystem;
    private MovementController movementController;
    //private MainFishHungerBehavior hungerStrategy;
    private HungerSystem hungerSystem;


    private IMovementStrategy movementStrategy;

    public NoThreatSwimmingState(MovementController _movementController, TargetingSystem _targetingSystem, HungerSystem _hungerStrategy)
    {
        movementController = _movementController;
        targetingSystem = _targetingSystem;
        hungerSystem = _hungerStrategy;
    }


    public void Enter()
    {
        //throw new System.NotImplementedException("Enter Func No Threat Swimming State");
    }

    public void Execute()
    {
        if (hungerSystem.IsHungry() && movementStrategy is not HybridSwimmingMovementStrategy)
        {
            movementStrategy = new HybridSwimmingMovementStrategy(
                movementController,
                targetingSystem);

            movementController.SetMovementStrategy(movementStrategy);
        }
        else if (!hungerSystem.IsHungry() && movementStrategy is not RandomizedSwimmingMovementStrategy)
        {
            movementStrategy = new RandomizedSwimmingMovementStrategy(
                movementController);

            movementController.SetMovementStrategy(movementStrategy);
        }
    }

    public void Exit()
    {
        movementController.SetMovementStrategy(null);
    }
}
