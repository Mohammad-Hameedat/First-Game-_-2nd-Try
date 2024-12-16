using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Main Fish Controller - Required Components
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(TargetingSystem))]
[RequireComponent(typeof(HungerSystem))]
[RequireComponent(typeof(BoundsAndPositioningManager))]
[RequireComponent(typeof(InteractionController))]
#endregion
public class MainFishController : MonoBehaviour
{
    private MovementController movementController;
    private StateMachine stateMachine;
    private TargetingSystem targetingSystem;
    private HungerSystem hungerSystem;
    private InteractionController interactionController;

    private List<GameObject> targetObjectsList = new();
    public FollowerSettings followerProperties;

    public int currentNumberofEatenObjects = 0;


    private void Awake()
    {
        // Get references to the components
        movementController = GetComponent<MovementController>();
        stateMachine = GetComponent<StateMachine>();
        targetingSystem = GetComponent<TargetingSystem>();
        hungerSystem = GetComponent<HungerSystem>();
        interactionController = GetComponent<InteractionController>();

        // Assign the properties to the components
        movementController.movementProperties = followerProperties.movementProperties;

        targetObjectsList = GameManager.currentActiveFoodTargetObjectsList;
    }


    private void Start()
    {

        targetingSystem.targetingStrategy = new FrameBasedTargetingStrategy();

        // Initialize target list
        targetingSystem.SetEatableTargetsList(targetObjectsList);

        // Register the object in the list of active main fishes || Add to GameManager list
        hungerSystem.SetHungerBehavior(new MainFishHungerStrategy(
            gameObject,
            followerProperties.hungerProperties.hungerStartingTime,
            followerProperties.hungerProperties.destructionTime));

        // Set the interaction strategy
        interactionController.SetInteractionStrategy(new MainFishInteractionStrategy(
            hungerSystem,
            targetingSystem
            ));


        // Start spawning money
        StartCoroutine(SpawnMoney());
    }


    private void Update()
    {
        // Handle state transitions based on existing enemies
        if (GameManager.currentActiveEnemyObjectsList.Count == 0)
        {
            if (stateMachine.currentState is not NoThreatSwimmingState)
            {
                GetComponent<HungerSystem>().enabled = true;

                // Switch to NoDangerState
                stateMachine.ChangeState(new NoThreatSwimmingState(
                movementController,
                targetingSystem,
                hungerSystem
                ));
            }
        }
        else
        {
            if (stateMachine.currentState is not ThreatenedSwimmingState)
            {
                GetComponent<HungerSystem>().enabled = false;
                // Switch to InDangerMovementState
                stateMachine.ChangeState(new ThreatenedSwimmingState(
                    movementController));
            }
        }

        currentNumberofEatenObjects = interactionController.interactionStrategy.GetEatenObjectsCount();
    }


    IEnumerator SpawnMoney()
    {
        while (true)
        {
            if (currentNumberofEatenObjects >= 3)
            {
                // Instantiate the collectable prefab at the current position
                GameObject collectableInstance = Instantiate(
                    followerProperties.spawnProperties.collectablePrefab,
                    transform.position,
                    Quaternion.identity);

                // Set the collectable configuration
                collectableInstance.GetComponent<Collectable>().collectableConfig = followerProperties.spawnProperties.collectableConfigs[0];
            }

            float randomTimeBeforeNextCollectableSpawn = Random.Range(7f, 15f);
            yield return new WaitForSeconds(randomTimeBeforeNextCollectableSpawn);
        }
    }


    private void OnDestroy()
    {
        // Return if the game stoped in the editor
        if (!this.gameObject.scene.isLoaded)
            return;

        // Unregister the object from the list of active main fishes || Remove from GameManager list
        GameManager.currentActiveMainFishObjectsList.Remove(gameObject);

        // Refresh the number of main fishes in the game
        GameEvents.EventsChannelInstance.RefresheMainFishesNumber(GameManager.currentActiveMainFishObjectsList.Count);
    }
}
