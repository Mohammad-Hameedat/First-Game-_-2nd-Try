using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Main Fish Controller - Required Components
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(TargetingSystem))]
[RequireComponent(typeof(MainFishInteractionController))]
#endregion
public class MainFishController : MonoBehaviour
{
    private MovementController movementController;
    private StateMachine stateMachine;
    private TargetingSystem targetingSystem;
    private MainFishInteractionController mainFishInteractionController;
    private Hunger hunger;

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
        hunger = GetComponent<Hunger>();

        // Assign the properties to the components
        movementController.properties = followerProperties;
        if (hunger != null)
        {
            // Assign hunger properties if the object has a hunger component
            hunger.hungerStartingTime = followerProperties.hungerStartingTime;
            hunger.destructionTime = followerProperties.destructionTime;
        }

        targetObjectsList = GameManager.currentActiveFoodTargetObjectsList;

        // Initialize target list
        targetingSystem.SetEatableTargetsList(targetObjectsList);
    }


    private void Start()
    {
        // Set the movement strategy of the object || Initialize state
        stateMachine.ChangeState(new NoRiskState(movementController));

        // Start spawning money
        StartCoroutine(SpawnMoney());
    }


    private void Update()
    {
        // Handle state transitions based on existing enemies
        if (GameManager.currentActiveEnemyObjectsList.Count == 0)
        {
            /* State = NoRiskState.
             * Handle state transitions based on hunger and if no enemies are present 
             */

            if (hunger != null && hunger.IsHungry())
            {
                if (movementController.movementStrategy is not HungryMovementStrategy)
                {
                    stateMachine.ChangeState(new NoRiskState(
                        movementController,
                        targetingSystem
                        ));
                }
            }
            else
            {
                if (movementController.movementStrategy is not RandomMovementStrategy)
                {
                    stateMachine.ChangeState(new NoRiskState(movementController));
                }
            }
        }
        else
        {

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
                GameObject collectableInstance = Instantiate(followerProperties.collectablePrefab, transform.position, Quaternion.identity);

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
        if (!this.gameObject.scene.isLoaded) return;

        // Unregister the object from the list of active main fishes || Remove from GameManager list
        GameManager.currentActiveMainFishObjectsList.Remove(gameObject);

        // Refresh the number of main fishes in the game
        GameEvents.EventsChannelInstance.RefresheMainFishesNumber(GameManager.currentActiveMainFishObjectsList.Count);
    }
}
