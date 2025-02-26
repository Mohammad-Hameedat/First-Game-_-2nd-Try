public class EnemyHuntingState : IState
{
    private MovementController movementController;
    private HungerSystem hungerSystem;

    private IMovementStrategy movementStrategy;


    public EnemyHuntingState(MovementController _movementController, HungerSystem _hungerSystem)
    {
        movementController = _movementController;
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
                movementController
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
