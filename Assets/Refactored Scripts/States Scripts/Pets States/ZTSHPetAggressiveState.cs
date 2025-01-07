using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ZTSHPetAggressiveState : IState
{
    #region References
    private GameObject petGameObject;
    private GameManager gameManager;

    private TargetingSystem targetingSystem;
    private MovementController movementController;

    // The collectible properties is used to spawn the main fish prefabs instead of collectibles.
    private CollectibleSpawnProperties foodSpawnProperties;
    #endregion

    /* A reference to the coroutine that spawns the food prefabs
     * to avoid multiple coroutines running at the same time
     * when the state is exited
     */
    private Coroutine spawnCoroutine;


    public ZTSHPetAggressiveState(
        GameObject _petGameObject,
        GameManager _gameManager,
        TargetingSystem _targetingSystem,
        MovementController _movementController,
        CollectibleSpawnProperties _foodSpawnProperties
        )
    {
        petGameObject = _petGameObject;
        gameManager = _gameManager;
        targetingSystem = _targetingSystem;
        movementController = _movementController;
        foodSpawnProperties = _foodSpawnProperties;
    }


    public void Enter()
    {
        movementController.SetMovementStrategy(new RandomizedSwimmingMovementStrategy(
            movementController
            ));

        spawnCoroutine = petGameObject.GetComponent<MonoBehaviour>().StartCoroutine(SpawnFood());
    }

    public void Execute()
    {
        targetingSystem.GetNearestTarget();
    }

    public void Exit()
    {
        movementController.SetMovementStrategy(null);

        if (spawnCoroutine != null)
        {
            petGameObject.GetComponent<MonoBehaviour>().StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }


    /* Explaining the functionality of spawning food prefabs by Zorf The Sea Horse (ZTSH) Pet
    * 
    * If an enemy instance of type (Fishes And Foods Eater "FAFE") is detected,
    * ZTSH Pet will start throwing 2 food prefabs every some period of time towards the enemy object.
    */
    public IEnumerator SpawnFood()
    {
        float spawnedFoodCounter = 0;
        while (true)
        {
            Transform target = targetingSystem.GetlastNearestTarget();

            if (target == null)
            {
                yield return null;
                continue;
            }
            else
            {
                EnemyController currentEnemyController = target.GetComponent<EnemyController>();

                if (currentEnemyController != null && currentEnemyController.enemyType == EnemyType.FishesAndFoodsEater)
                {
                    yield return new WaitForSeconds(foodSpawnProperties.minCollectableSpwanTime);

                    if (!target.IsDestroyed())
                    {
                        GameObject spawnedInstanceObject = Object.Instantiate(
                            foodSpawnProperties.collectablePrefab,
                            petGameObject.transform.position,
                            Quaternion.identity
                            );


                        Food spawnedFoodInstance = spawnedInstanceObject.GetComponent<Food>();
                        spawnedFoodInstance.foodConfig = gameManager.levelData.foodQualityTypes[2];


                        Rigidbody spawnedInstanceRigidbody = spawnedInstanceObject.GetComponent<Rigidbody>();
                        Vector3 throwDirection = ( target.position - petGameObject.transform.position ).normalized;

                        // Add Instant force (like punch) to the right direction.
                        spawnedInstanceRigidbody.AddForce(throwDirection * 25f, ForceMode.VelocityChange);
                        spawnedFoodCounter++;
                    }
                }
            }
            yield return null;
        }
    }
}