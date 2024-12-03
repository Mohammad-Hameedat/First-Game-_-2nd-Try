using UnityEngine;

public class NoDangerState : IState
{
    private TargetingSystem targetingSystem;
    private MovementController movementController;
    //private MainFishHungerBehavior hungerStrategy;
    private HungerSystem hungerSystem;


    private IMovementStrategy movementStrategy;

    public NoDangerState(MovementController _movementController,TargetingSystem _targetingSystem,HungerSystem _hungerStrategy)
    {
        movementController = _movementController;
        targetingSystem = _targetingSystem;
        hungerSystem = _hungerStrategy;
    }


    public void Enter()
    {
        //hungerStrategy.isHungerEnabled = true;

        //movementStrategy = new RandomMovementStrategy(movementController);
        //movementController.SetMovementStrategy(movementStrategy);
    }

    public void Execute()
    {
        if (hungerSystem.hungerStrategy.IsHungry() && movementStrategy is not TargetedMovementStrategy)
        {
            movementStrategy = new TargetedMovementStrategy(
                movementController,
                targetingSystem);

            movementController.SetMovementStrategy(movementStrategy);
        }
        else if (!hungerSystem.hungerStrategy.IsHungry() && movementStrategy is not RandomMovementStrategy)
        {
            movementStrategy = new RandomMovementStrategy(
                movementController);

            movementController.SetMovementStrategy(movementStrategy);
        }
    }

    public void Exit()
    {
        //hungerStrategy.isHungerEnabled = false;

        

        movementController.SetMovementStrategy(null);
    }
}
