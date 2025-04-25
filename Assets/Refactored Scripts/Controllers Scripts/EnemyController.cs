using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#region Enemy Controller - Required Components
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(TargetingSystem))]
[RequireComponent(typeof(HungerSystem))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(InteractionController))]
#endregion
public class EnemyController : MonoBehaviour
{
    private MovementController movementController;
    private StateMachine stateMachine;
    private TargetingSystem targetingSystem;
    private InteractionController interactionController;
    private Health health;
    private HungerSystem hungerSystem;


    //public List<GameObject> showListsContent = new();

    private static List<GameObject> targetObjectsList = new();
    private static IEnumerable<GameObject> combinedTargetsList = new List<GameObject>();

    public FollowerSettings followerProperties;
    public EnemyProperties enemyProperties;

    public EnemyType enemyType;

    private void Awake()
    {
        // Get references to the components
        movementController = GetComponent<MovementController>();
        targetingSystem = GetComponent<TargetingSystem>();
        interactionController = GetComponent<InteractionController>();
        health = GetComponent<Health>();
        hungerSystem = GetComponent<HungerSystem>();
        stateMachine = GetComponent<StateMachine>();


        // Assign the properties to the components
        movementController.movementProperties = followerProperties.movementProperties;
        health.maxHealth = enemyProperties.maxHealth;


        // Determine target list based on the type of enemy
        if (enemyType == EnemyType.FishesAndFoodsEater)
        {
            targetObjectsList = GameManager.currentActiveMainFishObjectsList;

            // Combine the target lists to target both main fishes and foods
            combinedTargetsList = targetObjectsList.Concat(
                GameManager.currentActiveFoodTargetObjectsList
                );

            targetingSystem.SetTargetObjectsList(combinedTargetsList);
        }
        else
        {
            // Enemy targets only main fishes
            targetObjectsList = GameManager.currentActiveMainFishObjectsList;

            targetingSystem.SetTargetObjectsList(targetObjectsList);
        }
    }

    private void Start()
    {
        targetingSystem.targetingStrategy = new ProximityTargetTargetingStrategy();


        // Register the enemy in the list of active enemies || Register in GameManager
        hungerSystem.SetHungerBehavior(new IntermittentHungerStrategy(
            followerProperties.hungerProperties.hungerStartingTime,
            followerProperties.hungerProperties.hungerDuration
            ));

        // Set the interaction strategy for the enemy
        interactionController.SetInteractionStrategy(new EnemyInteractionStrategy(
            gameObject,
            targetingSystem
            ));


        // Set the initial state of the enemy || Initialize state
        stateMachine.ChangeState(new EnemyHuntingState(
            movementController,
            hungerSystem
            ));
    }


    private void Update()
    {
        //showListsContent = combinedTargetsList.ToList();
    }


    private void OnDestroy()
    {
        // Return if the game stoped in the editor
        if (!this.gameObject.scene.isLoaded)
            return;

        // Unregister the enemy from the list of active enemies || Unregister from GameManager
        GameManager.currentActiveEnemyObjectsList.Remove(gameObject);

        // Spawn collectable if the enemy is destroyed
        GameObject collectableInstance = Instantiate(followerProperties.spawnProperties.collectablePrefab, transform.position, Quaternion.identity);
        collectableInstance.GetComponent<CollectibleScript>().collectibleProperties = followerProperties.spawnProperties.collectibleProperties[0];

    }
}
