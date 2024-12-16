public class EnemyHuntingState : IState
{
    private TargetingSystem targetingSystem;
    private MovementController movementController;
    private HungerSystem hungerSystem;

    private IMovementStrategy movementStrategy;


    public EnemyHuntingState(MovementController _movementController,TargetingSystem _targetingSystem,HungerSystem _hungerSystem)
    {
        movementController = _movementController;
        targetingSystem = _targetingSystem;
        hungerSystem = _hungerSystem;
    }

    public void Enter()
    {
        //movementStrategy = new RandomMovementStrategy(movementController);

        //movementController.SetMovementStrategy(movementStrategy);
    }

    public void Execute()
    {
        if (hungerSystem.hungerStrategy.GetHungerStatus() && movementStrategy is not HybridSwimmingMovementStrategy)
        {
            //MonoBehaviour.print("Hunting");
            movementStrategy = new HybridSwimmingMovementStrategy(
                movementController,
                targetingSystem
                );

            movementController.SetMovementStrategy(movementStrategy);
        }
        else if (!hungerSystem.hungerStrategy.GetHungerStatus() && movementStrategy is not RandomizedSwimmingMovementStrategy)
        {
            movementStrategy = new RandomizedSwimmingMovementStrategy(
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
