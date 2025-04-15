using UnityEngine;

public class NTMRPetIdleState : IState
{
    #region State References
    private MovementController movementController;

    #endregion

    public NTMRPetIdleState(
        MovementController _movementController
        )
    {
        movementController = _movementController;
    }

    public void Enter()
    {
        // Switch to a "random idle movement" strategy
        movementController.SetMovementStrategy(new HybridSwimmingMovementStrategy(
            movementController
        ));
    }

    public void Execute()
    {
        // Could play idle animations here
    }

    public void Exit()
    {
        // Clean up
        movementController.SetMovementStrategy(null);
    }
}
