public class ATAFDashState : IState
{
    private MovementController movementController;


    public ATAFDashState(MovementController _movementController)
    {
        movementController = _movementController;
    }


    public void Enter()
    {
        movementController.SetMovementStrategy(new DashingMovementStrategy(
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