using UnityEngine;

public class ThreatenedSwimmingState : IState
{
    private MovementController movementController;

    private IMovementStrategy movementStrategy;

    GameObject protectivePetObject;

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
        if (GameManager.cAPPetsDictionary.ContainsKey(ProtectivePetType.WTW))
        {
            protectivePetObject = GameManager.cAPPetsDictionary[ProtectivePetType.WTW];

            GameObject wtwPet = GameManager.cAPPetsDictionary[ProtectivePetType.WTW];

            IMovementStrategy wtwMovementStrategy = wtwPet.GetComponent<MovementController>().movementStrategy;

            if (wtwMovementStrategy == null)
            {
                movementStrategy = new ThreatenedSwimmingMovementStrategy(
                    movementController,
                    protectivePetObject.transform
                    );
            }
            else
            {
                movementStrategy = new RandomizedSwimmingMovementStrategy(
                movementController
                );
            }
        }
        else
        {
            // Find the best protective pet object.
            foreach (GameObject pet in GameManager.cAPPetsDictionary.Values)
            {
                if (pet.activeSelf)
                {
                    protectivePetObject = pet;

                    // Set the movement strategy depending on the pet type

                    break;
                }
            }

            /* Read the following note when you add new protective pet types
             * 
             * The following random movement strategy is a temporary solution
             * because there is no other protective pet types created yet.
             * 
             * Once you add new protective pet types,
             * you should create or use the appropriate movement strategy.
             */
            movementStrategy = new RandomizedSwimmingMovementStrategy(
                      movementController
                      );
        }
    }
    #endregion
}