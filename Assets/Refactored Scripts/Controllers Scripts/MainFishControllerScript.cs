using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Main Fish Controller - Required Components
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(TargetingSystem))]
[RequireComponent(typeof(HungerSystem))]
[RequireComponent(typeof(InteractionController))]
[RequireComponent(typeof(BoundsAndPositioningManager))]
#endregion
public class MainFishControllerScript : MonoBehaviour
{
    #region Components References
    private MovementController movementController;
    private StateMachine stateMachine;
    private TargetingSystem targetingSystem;
    private HungerSystem hungerSystem;
    private InteractionController interactionController;


    private Rigidbody rb;
    public FollowerSettings followerProperties;

    #endregion


    #region Main Fish Controller - Required Variables
    private List<GameObject> targetObjectsList = new();

    private int currentFoodIndex = 0;

    private bool isBoosting = false;
    private int boostIterations;
    private float boostWaitTime = 1f;

    public int totalEatenObjectsCount = 0;

    #endregion


    private void Awake()
    {
        // Get references to the components
        movementController = GetComponent<MovementController>();
        stateMachine = GetComponent<StateMachine>();
        targetingSystem = GetComponent<TargetingSystem>();
        hungerSystem = GetComponent<HungerSystem>();
        interactionController = GetComponent<InteractionController>();

        rb = GetComponent<Rigidbody>();

        // Assign the properties to the components
        movementController.movementProperties = followerProperties.movementProperties;
        targetObjectsList = GameManager.currentActiveFoodTargetObjectsList;
    }


    private void OnEnable()
    {
        // Register this object in the list of active main fishes || Add to GameManager list
        if (!GameManager.currentActiveMainFishObjectsList.Contains(gameObject))
        {
            GameManager.currentActiveMainFishObjectsList.Add(gameObject);
        }

        if (!GameManager.currentActiveDistractibleObjectsList.Contains(gameObject))
        {
            GameManager.currentActiveDistractibleObjectsList.Add(gameObject);
        }

        rb.velocity = Vector3.zero;

        if (GameManager.currentActiveEnemyObjectsList.Count > 0)
        {
            hungerSystem.enabled = false;

            stateMachine.ChangeState(new ThreatenedSwimmingState(
                movementController
                ));
        }

        GameEvents.EventsChannelInstance.OnBoostSpawningCollectibles += ToggleBoostSpawningCollectibles;
    }


    private void Start()
    {
        targetingSystem.targetingStrategy = new FrameBasedTargetingStrategy();

        // Initialize target list
        targetingSystem.SetTargetObjectsList(targetObjectsList);

        // Register the object in the list of active main fishes || Add to GameManager list
        hungerSystem.SetHungerBehavior(new MainFishHungerStrategy(
            gameObject,
            targetingSystem,
            followerProperties.hungerProperties.hungerStartingTime,
            followerProperties.hungerProperties.destructionTime
            ));


        // Set the interaction strategy
        interactionController.SetInteractionStrategy(new MainFishInteractionStrategy(
            hungerSystem,
            targetingSystem
            ));


        // Start spawning money
        StartCoroutine(CollectibleSpawnRoutine());
    }


    private void Update()
    {
        // Handle state transitions based on existing enemies
        if (GameManager.currentActiveEnemyObjectsList.Count == 0)
        {
            if (stateMachine.currentState is not NoThreatSwimmingState)
            {
                hungerSystem.enabled = true;

                // Switch to NoDangerState
                stateMachine.ChangeState(new NoThreatSwimmingState(
                movementController,
                hungerSystem
                ));
            }
        }
        else
        {
            if (stateMachine.currentState is not ThreatenedSwimmingState)
            {
                hungerSystem.enabled = false;

                stateMachine.ChangeState(new ThreatenedSwimmingState(
                    movementController
                    ));
            }
        }

        totalEatenObjectsCount = interactionController.interactionStrategy.GetInteractedTargetsCount();
    }



    #region Collectibles Spawners
    private IEnumerator CollectibleSpawnRoutine()
    {
        yield return new WaitUntil(() => totalEatenObjectsCount > 2);

        while (true)
        {
            if (!isBoosting)
            {
                if (totalEatenObjectsCount >= 20)
                    currentFoodIndex = 2;
                else if (totalEatenObjectsCount >= 10)
                    currentFoodIndex = 1;
                else if (totalEatenObjectsCount >= 3)
                    currentFoodIndex = 0;
            }


            float waitTime = isBoosting
                ? boostWaitTime
                : Random.Range(
                    followerProperties.spawnProperties.minCollectableSpwanTime,
                    followerProperties.spawnProperties.maxCollectableSpwanTime
                    );


            float elapsedTime = 0f;

            while (elapsedTime < waitTime && !isBoosting)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
                continue;
            }

            if (isBoosting)
            {
                elapsedTime = 0f;
                /* Explaining the logic of the collectibles spawning booster:
                 * 
                 * The booster will spawn "X" collectibles in a row.
                 * Each collectible will be spawned 1 second after the previous one.
                 * 
                 * "X" = Spawning booster iterations (Set by MTM Pet).
                 * "S" = Spawning booster wait time (Set In MTM Pet's Controller Spawner Booster Coroutine).
                 */
                for (int iterationIndex = 0; iterationIndex < boostIterations; iterationIndex++)
                {
                    SpawnCollectible(currentFoodIndex);
                    yield return new WaitForSeconds(boostWaitTime);
                }
            }
            else
            {
                SpawnCollectible(currentFoodIndex);
            }

            yield return null;
        }
    }

    private void SpawnCollectible(int collectibleIndex)
    {
        // Instantiate the collectable prefab at the current position
        GameObject collectableInstance = Instantiate(
            followerProperties.spawnProperties.collectablePrefab,
            transform.position,
            Quaternion.identity);

        // Set the collectable configuration
        collectableInstance.GetComponent<CollectibleScript>().collectibleProperties =
            followerProperties.spawnProperties.collectibleProperties[collectibleIndex];
    }

    #endregion


    #region Collectibles Spawning Boosters
    private void ToggleBoostSpawningCollectibles(bool _isBoosting, int _boostIterations)
    {
        isBoosting = _isBoosting;
        boostIterations = _boostIterations;
    }

    #endregion



    private void OnDisable()
    {
        // Unregister the object from the list of active main fishes || Remove from GameManager list
        //GameManager.currentActiveMainFishObjectsList.Remove(gameObject);
        //GameManager.currentActiveDistractibleObjectsList.Remove(gameObject);

        // Refresh the number of main fishes in the game
        //GameEvents.EventsChannelInstance.RefresheMainFishesNumber(GameManager.currentActiveMainFishObjectsList.Count);

        GameEvents.EventsChannelInstance.OnBoostSpawningCollectibles -= ToggleBoostSpawningCollectibles;
    }


    private void OnDestroy()
    {
        // Return if the game stoped in the editor
        if (!this.gameObject.scene.isLoaded)
            return;

        // Unregister the object from the list of active main fishes || Remove from GameManager list
        GameManager.currentActiveMainFishObjectsList.Remove(gameObject);
        GameManager.currentActiveDistractibleObjectsList.Remove(gameObject);

        // Refresh the number of main fishes in the game
        GameEvents.EventsChannelInstance.RefresheMainFishesNumber(GameManager.currentActiveMainFishObjectsList.Count);
    }
}