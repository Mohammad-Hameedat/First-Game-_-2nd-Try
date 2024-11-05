using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(TargetingSystem))]
[RequireComponent(typeof(InteractionController))]
[RequireComponent(typeof(Health))]
public class EnemyController : MonoBehaviour
{
    private MovementController movementController;
    private StateMachine stateMachine;
    private TargetingSystem targetingSystem;
    private InteractionController interactionController;
    private Health health;
    private Hunger hunger; // If aplicable. >> If not, remove this line
    public EnemyProperties enemyProperties;
    public FollowerProperties followerProperties;
    private List<GameObject> targetObjectsList = new();

    //private int currentNumberofEatenObjects = 0;
    private int numberOfObjectsToEat;
    private int nextNumberOfObjectsToEat;

    public EnemyType enemyType;

    private void Awake()
    {
        // Get references to the components
        movementController = GetComponent<MovementController>();
        stateMachine = GetComponent<StateMachine>();
        targetingSystem = GetComponent<TargetingSystem>();
        interactionController = GetComponent<InteractionController>();
        health = GetComponent<Health>();
        hunger = GetComponent<Hunger>(); // If hunger is optional. >> If not, remove this line

        // Assign the properties to the components
        movementController.properties = followerProperties;
        health.maxHealth = followerProperties.maxHealth;

        // Initialize the number of objects to eat before destroying the enemy
        numberOfObjectsToEat = enemyProperties.numberOfObjectsToEat;
        nextNumberOfObjectsToEat = enemyProperties.nextNumberOfObjectsToEat;

        // Determine target list based on the type of enemy
        //if (this is ChildEnemyFoodEaterController)
        if (enemyType == EnemyType.MainFishAndFood)
        {
            // Enemy targets both main fishes and food
            targetObjectsList = new();
            targetObjectsList.AddRange(GameManager.currentActiveMainFishObjectsList);
            targetObjectsList.AddRange(GameManager.currentActiveFoodTargetObjectsList);
        }
        else
        {
            // Enemy targets only main fishes
            targetObjectsList = GameManager.currentActiveMainFishObjectsList;
        }

        targetingSystem.SetEatableTargetsList(targetObjectsList);
    }

    private void Start()
    {
        // Set the initial state of the enemy || Initialize state
        stateMachine.ChangeState(new IdleState(movementController));

        // Register the enemy to the list of active enemies || Register to GameManager
        GameManager.currentActiveEnemyObjectsList.Add(gameObject);
    }


    private void Update()
    {
        // Handle state transitions based on hunger
        if (hunger != null && hunger.IsHungry())
        {
            if (!(stateMachine.currentState is NoRiskState))
            {
                stateMachine.ChangeState(new NoRiskState(movementController, targetingSystem));
            }
        }


        // Additional logic if needed
    }


    private void OnDestroy()
    {
        // Unregister the enemy from the list of active enemies || Unregister from GameManager
        GameManager.currentActiveEnemyObjectsList.Remove(gameObject);

        // Spawn collectable if the enemy is destroyed
        GameObject collectableInstance = Instantiate(followerProperties.collectablePrefab, transform.position, Quaternion.identity);
        collectableInstance.GetComponent<Collectable>().collectableConfig = followerProperties.collectableConfigs[0];

    }
}
