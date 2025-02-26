public class NoThreatSwimmingState : IState
{
    private MovementController movementController;
    private HungerSystem hungerSystem;


    private IMovementStrategy movementStrategy;

    public NoThreatSwimmingState(
        MovementController _movementController,
        HungerSystem _hungerStrategy
        )
    {
        movementController = _movementController;
        hungerSystem = _hungerStrategy;
    }


    public void Enter() { }

    public void Execute()
    {
        if (hungerSystem.IsHungry() && movementStrategy is not HybridSwimmingMovementStrategy)
        {
            movementStrategy = new HybridSwimmingMovementStrategy(
                movementController
                );

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
