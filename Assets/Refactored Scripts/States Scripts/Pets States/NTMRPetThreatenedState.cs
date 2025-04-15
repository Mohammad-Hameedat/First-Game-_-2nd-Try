using UnityEngine.Experimental.Rendering;

public class NTMRPetThreatenedState : IState
{
    private MovementController movementController;

    public NTMRPetThreatenedState(
        MovementController _movementController
        )
    {
        movementController = _movementController;
    }

    public void Enter()
    {
        movementController.SetMovementStrategy(new LandingMovementStrategy(
            movementController
            ));
    }

    public void Execute()
    {

    }

    public void Exit()
    {
        movementController.SetMovementStrategy(null);
    }
}