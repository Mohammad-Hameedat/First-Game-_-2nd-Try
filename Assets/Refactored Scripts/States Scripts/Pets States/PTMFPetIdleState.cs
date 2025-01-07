using UnityEngine;

public class PTMFPetIdleState : IState
{
    #region References
    private GameObject petGameObject;

    private MovementController movementController;
    private HungerSystem hungerSystem;

    // The collectible properties is used to spawn the main fish prefabs instead of collectibles.
    private CollectibleSpawnProperties collectibleSpawnProperties;

    #endregion


    #region Variables
    private IMovementStrategy movementStrategy;

    private float elapsedTime = 0f;
    private float timeToSpawnMainFish;

    #endregion


    public PTMFPetIdleState(
        GameObject _petGameObject,
        MovementController _movementController,
        CollectibleSpawnProperties _mainFishSpawnProperties,
        HungerSystem _hungerSystem
        )
    {
        petGameObject = _petGameObject;
        movementController = _movementController;
        collectibleSpawnProperties = _mainFishSpawnProperties;
        hungerSystem = _hungerSystem;
    }


    public void Enter()
    {
        timeToSpawnMainFish = Random.Range(
            collectibleSpawnProperties.minCollectableSpwanTime,
            collectibleSpawnProperties.maxCollectableSpwanTime
            );

        movementController.SetMovementStrategy(new RandomizedSwimmingMovementStrategy(
            movementController
            ));
    }


    public void Execute()
    {
        if (!hungerSystem.IsHungry())
        {
            SpawnMainFish();
        }
        else
        {
            return;
        }
    }


    public void Exit()
    {
        movementController.SetMovementStrategy(null);
    }


    /* Spawn the main fish prefab
     * 
     * The main fish prefab is spawned at random intervals
     * The time interval is randomized between the min and max spawn time of the collectible spawn properties
     */
    private void SpawnMainFish()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= timeToSpawnMainFish)
        {
            // Spawn the main fish prefab
            GameObject mainFishInstance = Object.Instantiate(collectibleSpawnProperties.collectablePrefab,
                 petGameObject.transform.position,
                 Quaternion.identity
                 );

            timeToSpawnMainFish = Random.Range(
                collectibleSpawnProperties.minCollectableSpwanTime,
                collectibleSpawnProperties.maxCollectableSpwanTime
                );
            elapsedTime = 0f;
        }
    }
}