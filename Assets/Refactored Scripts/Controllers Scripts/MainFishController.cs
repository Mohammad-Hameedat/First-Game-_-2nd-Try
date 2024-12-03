using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Main Fish Controller - Required Components
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(TargetingSystem))]
[RequireComponent(typeof(HungerSystem))]
[RequireComponent(typeof(MainFishInteractionController))]
#endregion
public class MainFishController : MonoBehaviour
{
    private MovementController movementController;
    private StateMachine stateMachine;
    private TargetingSystem targetingSystem;
    private MainFishInteractionController mainFishInteractionController;
    private HungerSystem hungerSystem;

    private List<GameObject> targetObjectsList = new();
    public FollowerProperties followerProperties;

    public int currentNumberofEatenObjects = 0;


    private void Awake()
    {
        // Get references to the components
        movementController = GetComponent<MovementController>();
        stateMachine = GetComponent<StateMachine>();
        targetingSystem = GetComponent<TargetingSystem>();
        mainFishInteractionController = GetComponent<MainFishInteractionController>();
        hungerSystem = GetComponent<HungerSystem>();

        // Assign the properties to the components
        movementController.properties = followerProperties;

        targetObjectsList = GameManager.currentActiveFoodTargetObjectsList;

        // Initialize target list
        targetingSystem.SetEatableTargetsList(targetObjectsList);
    }


    private void Start()
    {
        hungerSystem.SetHungerBehavior(new MainFishHungerStrategy(
            gameObject,
            followerProperties.hungerStartingTime,
            followerProperties.destructionTime));

        mainFishInteractionController.hungerSystem = this.hungerSystem;


        // Start spawning money
        StartCoroutine(SpawnMoney());
    }


    private void Update()
    {
        // Handle state transitions based on existing enemies
        if (GameManager.currentActiveEnemyObjectsList.Count == 0)
        {
            if (stateMachine.currentState is not NoDangerState)
            {
                GetComponent<HungerSystem>().enabled = true;

                // Switch to NoDangerState
                stateMachine.ChangeState(new NoDangerState(
                movementController,
                targetingSystem,
                hungerSystem
                ));
            }
        }
        else
        {
            if (stateMachine.currentState is not InDangerMovementState)
            {
                GetComponent<HungerSystem>().enabled = false;
                // Switch to InDangerMovementState
                stateMachine.ChangeState(new InDangerMovementState(
                    movementController));
            }
        }

        currentNumberofEatenObjects = mainFishInteractionController.currentNumberofEatenObjects;
    }


    IEnumerator SpawnMoney()
    {
        while (true)
        {
            if (currentNumberofEatenObjects >= 3)
            {
                // Instantiate the collectable prefab at the current position
                GameObject collectableInstance = Instantiate(
                    followerProperties.collectablePrefab, 
                    transform.position, 
                    Quaternion.identity);

                // Set the collectable configuration
                collectableInstance.GetComponent<Collectable>().collectableConfig = followerProperties.collectableConfigs[0];
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
