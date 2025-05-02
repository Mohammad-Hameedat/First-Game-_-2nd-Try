using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Required Components
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(TargetingSystem))]
[RequireComponent(typeof(HungerSystem))]
[RequireComponent(typeof(InteractionController))]
[RequireComponent(typeof(DeathAndRevivalSystem))]
[RequireComponent(typeof(BoundsAndPositioningManager))]
#endregion


public class MainFishControllerScript : MonoBehaviour
{
    #region Serialized Settings
    [Header("Follower Settings")]
    public FollowerSettings followerProperties;
    #endregion


    #region Cached Component References
    private MovementController movementController;
    private StateMachine stateMachine;
    private TargetingSystem targetingSystem;
    private HungerSystem hungerSystem;
    private InteractionController interactionController;
    private DeathAndRevivalSystem deathAndRevivalSystem;

    private Rigidbody rb;
    #endregion


    #region StateAndRuntimeData
    Vector3 bottomPosition;

    private List<GameObject> targetObjectsList = new();

    private int currentFoodIndex = 0;
    private bool isBoosting = false;
    private int boostIterations = 0;
    private const float boostWaitTime = 1f;

    public int totalEatenObjectsCount = 0;
    #endregion


    #region UNITY LIFECYCLE
    private void Awake()
    {
        // Cache required components
        movementController = GetComponent<MovementController>();
        stateMachine = GetComponent<StateMachine>();
        targetingSystem = GetComponent<TargetingSystem>();
        hungerSystem = GetComponent<HungerSystem>();
        interactionController = GetComponent<InteractionController>();
        deathAndRevivalSystem = GetComponent<DeathAndRevivalSystem>();
        rb = GetComponent<Rigidbody>();

        // Pass settings to sub-systems
        movementController.movementProperties = followerProperties.movementProperties;
        targetObjectsList = GameManager.currentActiveFoodTargetObjectsList;
    }


    private void OnEnable()
    {
        if (deathAndRevivalSystem.corpseStrategy != null
            && deathAndRevivalSystem.corpseStrategy.IsDead)
        {
            deathAndRevivalSystem.corpseStrategy.TriggerRevivalState();
        }

        rb.velocity = Vector3.zero;

        if (GameManager.currentActiveEnemyObjectsList.Count > 0)
        {
            hungerSystem.enabled = false;
            stateMachine.ChangeState(new ThreatenedSwimmingState(movementController));
        }

        GameEvents.EventsChannelInstance.OnBoostSpawningCollectibles += ToggleBoostSpawningCollectibles;
    }


    private void Start()
    {
        deathAndRevivalSystem.corpseStrategy = new MainFishDeathStrategy(
            gameObject,
            this,
            true,
            GameManager.currentActiveMainFishObjectsList,
            GameManager.currentActiveDistractibleObjectsList,
            GameManager.currentActiveCorpsedObjectsList
            );

        deathAndRevivalSystem.corpseStrategy.TriggerRevivalState();

        GameEvents.EventsChannelInstance.RefresheMainFishesNumber(
            GameManager.currentActiveMainFishObjectsList.Count);

        bottomPosition = movementController.boundsManager.CornerToWorldPosition(ScreenCorner.BottomCenter);


        targetingSystem.targetingStrategy = new FrameBasedTargetingStrategy();
        targetingSystem.SetTargetObjectsList(targetObjectsList);

        hungerSystem.SetHungerBehavior(new MainFishHungerStrategy(
            gameObject,
            deathAndRevivalSystem,
            targetingSystem,
            followerProperties.hungerProperties.hungerStartingTime,
            followerProperties.hungerProperties.destructionTime));

        interactionController.SetInteractionStrategy(new MainFishInteractionStrategy(
            hungerSystem,
            targetingSystem
            ));


        StartCoroutine(MainFishLifeCycle());

        StartCoroutine(CollectibleSpawnRoutine());
    }


    private IEnumerator MainFishLifeCycle()
    {
        while (true)
        {
            /*   ALIVE BRANCH   */
            while (!deathAndRevivalSystem.corpseStrategy.IsDead)
            {
                // THREATENED  =>  NOT-THREATENED
                if (stateMachine.currentState is not NoThreatSwimmingState)
                {
                    // Stay in the threatened state *until* no enemies are left,
                    // then switch to the peaceful state.
                    yield return new WaitUntil(() => GameManager.currentActiveEnemyObjectsList.Count == 0);

                    hungerSystem.enabled = true;

                    stateMachine.ChangeState(new NoThreatSwimmingState(
                        movementController,
                        hungerSystem
                        ));
                }
                // NOT-THREATENED  =>  THREATENED
                else
                {
                    // Stay in the peaceful state *until* at least one enemy appears,
                    // then switch to the threatened state.
                    yield return new WaitUntil(() => GameManager.currentActiveEnemyObjectsList.Count > 0);

                    hungerSystem.enabled = false;

                    stateMachine.ChangeState(new ThreatenedSwimmingState(
                        movementController
                        ));
                }

                yield return null;
            }


            /*   DEAD / FALLING BRANCH   */
            // Ensure we are in DeadState once the corpse flag is set.
            if (stateMachine.currentState is not DeadState)
            {
                stateMachine.ChangeState(new DeadState(movementController));
            }

            // Wait until the corpse reaches the bottom, then destroy and end.
            yield return new WaitUntil(() =>
            {
                HandleFallingDeath();

                return transform.position.y <= bottomPosition.y || !deathAndRevivalSystem.corpseStrategy.IsDead;
            });

            if (transform.position.y <= bottomPosition.y)
            {
                Destroy(gameObject);

                yield break;
            }

            yield return null;
        }
    }

    private void Update()
    {
        /*
                if (!deathAndRevivalSystem.corpseStrategy.IsDead)
                {
                    HandleThreatStates();

                    totalEatenObjectsCount = 
                        interactionController.interactionStrategy.GetInteractedTargetsCount();
                }
                else
                {
                    HandleFallingDeath();
                }
        */

        totalEatenObjectsCount =
            interactionController.interactionStrategy.GetInteractedTargetsCount();
    }


    private void OnDisable()
    {
        GameEvents.EventsChannelInstance.OnBoostSpawningCollectibles -= ToggleBoostSpawningCollectibles;
    }


    private void OnDestroy()
    {
#if UNITY_EDITOR
        // Return if the game stoped in the editor
        if (!this.gameObject.scene.isLoaded)
            return;
#endif

        deathAndRevivalSystem.corpseStrategy.ClearFromAllLists();

        GameEvents.EventsChannelInstance.RefresheMainFishesNumber(
            GameManager.currentActiveMainFishObjectsList.Count);
    }
    #endregion



    #region UPDATE HELPERS

    private void HandleThreatStates()
    {
        bool enemiesExist = GameManager.currentActiveEnemyObjectsList.Count > 0;

        if (!enemiesExist && stateMachine.currentState is not NoThreatSwimmingState)
        {
            hungerSystem.enabled = true;
            stateMachine.ChangeState(new NoThreatSwimmingState(movementController, hungerSystem));
        }
        else if (enemiesExist && stateMachine.currentState is not ThreatenedSwimmingState)
        {
            hungerSystem.enabled = false;
            stateMachine.ChangeState(new ThreatenedSwimmingState(movementController));
        }
    }


    private void HandleFallingDeath()
    {
        if (stateMachine.currentState is not DeadState)
        {
            stateMachine.ChangeState(new DeadState(
                movementController
                ));
        }

        if (transform.position.y <= bottomPosition.y)
        {
            Destroy(gameObject);
        }
    }

    #endregion



    #region COLLECTIBLES
    private IEnumerator CollectibleSpawnRoutine()
    {
        yield return new WaitUntil(() => totalEatenObjectsCount > 2);

        while (true)
        {

            /* Explaining the logic of setting the current food index/type:
             * 
             * The current food index will be set based on the total eaten objects count,
             * which will determine the type of collectible to spawn, but only if the booster
             * is not active.
             */
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
            }

            if (isBoosting)
            {
                elapsedTime = 0f;
                /* Explaining the logic of the collectibles spawning booster:
                 * 
                 * The booster will spawn "X" collectibles in a row.
                 * Each collectible will be spawned 1 second after the previous one.
                 * 
                 * "boostIterations" = Spawning booster iterations (Set by MTM Pet).
                 * "boostWaitTime" = Spawning booster wait time (Set In MTM Pet's Controller Spawner Booster Coroutine).
                 */
                for (int iterationIndex = 0; iterationIndex < boostIterations; iterationIndex++)
                {
                    SpawnCollectible(currentFoodIndex);
                    yield return new WaitForSeconds(boostWaitTime);
                }
            }

            /* Important Note:
             * This condition helps to keep the performance stable and high
             * on each platform by limiting the number of collectibles in the scene.
             */
            else if (
#if UNITY_STANDALONE || UNITY_EDITOR
                GameManager.currentActiveCollectiblesList.Count < 400
#elif UNITY_ANDROID || UNITY_IOS
                GameManager.currentActiveCollectiblesList.Count < 200
#endif
                )
            {
                SpawnCollectible(currentFoodIndex);
                yield return null;
            }
            yield return null;
        }
    }


    private void SpawnCollectible(int collectibleIndex)
    {
        GameObject instance = Instantiate(
            followerProperties.spawnProperties.collectablePrefab,
            transform.position,
            Quaternion.identity);

        instance.GetComponent<CollectibleScript>().collectibleProperties =
            followerProperties.spawnProperties.collectibleProperties[collectibleIndex];
    }


    private void ToggleBoostSpawningCollectibles(bool _isBoosting, int _boostIterations)
    {
        isBoosting = _isBoosting;
        boostIterations = _boostIterations;
    }
    #endregion


    // External electrification kills fish and drops high-value collectible
    public void ElectrificationTrigger()
    {
        GameObject collectibleInstance = Instantiate(
            followerProperties.spawnProperties.collectablePrefab,
            transform.position,
            Quaternion.identity
            );

        collectibleInstance.GetComponent<CollectibleScript>().collectibleProperties =
            followerProperties.spawnProperties.collectibleProperties[2];

        deathAndRevivalSystem.corpseStrategy.TriggerDeathState();
    }
}
