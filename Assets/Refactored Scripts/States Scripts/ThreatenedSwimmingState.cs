using UnityEngine;

public class ThreatenedSwimmingState : IState
{
    private MovementController movementController;

    private IMovementStrategy movementStrategy;

    public ThreatenedSwimmingState(MovementController _movementController)
    {
        movementController = _movementController;
    }

    // StateMachine methods
    public void Enter()
    {
        DetermineMovementStrategy();


        // Set the movement strategy of the object
        movementController.SetMovementStrategy(movementStrategy);
    }

    public void Execute()
    {
        // Could add a feature where support pets can help the main fish escape from the threat here.
        // A logic will be added here later.
    }

    public void Exit()
    {
        movementController.SetMovementStrategy(null);
    }


    #region Extra methods for cleaning up the state
    private void DetermineMovementStrategy()
    {
        PetType protectivePetType = GameManager.IdentifyProtectivePet();


        /* Read the following note when you add new protective pet types
         * 
         * The following random movement strategy is a temporary solution
         * because there is no other protective pet types created yet.
         * 
         * Once you add new protective pet types,
         * you should create or use the appropriate movement strategy.
         */
        if (protectivePetType == PetType.WTWPet || protectivePetType == PetType.GTAPet)
        {
            GameObject protectivePetObject = GameManager.cAPPetsDictionary[protectivePetType];

            movementStrategy = new SingleTargetFollowingMovementStrategy(
                movementController,
                protectivePetObject.transform
                );
        }
        else
        {
            /* Read the following note when you add new protective pet types
             * 
             * Here you should add the movement strategy depending on the protective pet type.
             */

            movementStrategy = new RandomizedSwimmingMovementStrategy(
                movementController
                );
        }
    }

    #endregion
}