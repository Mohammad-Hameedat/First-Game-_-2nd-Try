using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GTAPetIdleState : IState
{
    private MovementController movementController;

    public GTAPetIdleState(MovementController _movementController)
    {
        movementController = _movementController;
    }

    public void Enter()
    {
        movementController.SetMovementStrategy(new RandomizedSwimmingMovementStrategy(
            movementController
            ));
    }

    public void Execute() { }

    public void Exit()
    {
        movementController.SetMovementStrategy(null);
    }
}
