using UnityEngine;

public class ZTSHPetIdleState : IState
{
    #region References
    private GameObject petGameObject;
    private GameManager gameManager;

    private MovementController movementController;
    private HungerSystem hungerSystem;

    private Rigidbody rb;

    // The collectible properties is used to spawn the main fish prefabs instead of collectibles.
    private CollectibleSpawnProperties foodSpawnProperties;

    #endregion


    #region Variables
    private float elapsedTime = 0f;
    private float foodSpawnInterval;

    #endregion


    public ZTSHPetIdleState(
        GameObject _petGameObject,
        MovementController _movementController,
        CollectibleSpawnProperties _foodSpawnProperties,
        HungerSystem _hungerSystem,
        GameManager _gameManager
        )
    {
        petGameObject = _petGameObject;
        movementController = _movementController;
        foodSpawnProperties = _foodSpawnProperties;
        hungerSystem = _hungerSystem;
        gameManager = _gameManager;

        InitializeReferences();
    }



    public void Enter()
    {
        foodSpawnInterval = Random.Range(
            foodSpawnProperties.minCollectableSpwanTime,
            foodSpawnProperties.maxCollectableSpwanTime
            );

        movementController.SetMovementStrategy(new RandomizedSwimmingMovementStrategy(
            movementController
            ));
    }

    public void Execute()
    {
        if (!hungerSystem.IsHungry())
        {
            SpawnFood();
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


    /* Explaining the functionality of spawning food prefabs by Zorf The Sea Horse (ZTMF) Pet
    * 
    * The food prefab is spawned at random intervals
    * The time interval is randomized between the min and max spawn time of the collectible spawn properties
    */
    private void SpawnFood()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= foodSpawnInterval)
        {
            // Spawn the main fish prefab
            GameObject spawnedInstanceObject = Object.Instantiate(
                foodSpawnProperties.collectablePrefab,
                 petGameObject.transform.position,
                 Quaternion.identity
                 );

            spawnedInstanceObject.GetComponent<Food>().foodConfig =
                gameManager.levelData.foodQualityTypes[2];

            Rigidbody spawnedInstanceRigidbody = spawnedInstanceObject.GetComponent<Rigidbody>();

            if (rb.velocity.x >= 0f)
            {
                // Add Instant force (like punch) to the right direction.
                spawnedInstanceRigidbody.AddForce(Vector3.right * 4f, ForceMode.VelocityChange);
            }
            else
            {
                // Add Instant force (like punch) to the left direction.
                spawnedInstanceRigidbody.AddForce(Vector3.left * 4f, ForceMode.VelocityChange);
            }

            foodSpawnInterval = Random.Range(
                foodSpawnProperties.minCollectableSpwanTime,
                foodSpawnProperties.maxCollectableSpwanTime
                );
            elapsedTime = 0f;
        }
    }



    private void InitializeReferences()
    {
        rb = petGameObject.GetComponent<Rigidbody>();
    }
}