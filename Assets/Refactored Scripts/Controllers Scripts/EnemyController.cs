using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#region Enemy Controller - Required Components
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(TargetingSystem))]
[RequireComponent(typeof(HungerSystem))]
[RequireComponent(typeof(Health))]
#endregion
public class EnemyController : MonoBehaviour
{
    private MovementController movementController;
    private StateMachine stateMachine;
    private TargetingSystem targetingSystem;
    private Health health;
    private HungerSystem hungerSystem;


    //public List<GameObject> showListsContent = new();

    private static List<GameObject> targetObjectsList = new();
    public static IEnumerable<GameObject> combinedTargetsList = new List<GameObject>();

    public FollowerProperties followerProperties;
    public EnemyProperties enemyProperties;

    public EnemyType enemyType;

    private void Awake()
    {
        // Get references to the components
        movementController = GetComponent<MovementController>();
        stateMachine = GetComponent<StateMachine>();
        targetingSystem = GetComponent<TargetingSystem>();
        health = GetComponent<Health>();
        hungerSystem = GetComponent<HungerSystem>();


        // Assign the properties to the components
        movementController.properties = followerProperties;
        health.maxHealth = enemyProperties.maxHealth;


        // Determine target list based on the type of enemy
        if (enemyType == EnemyType.FishesAndFoodsEater)
        {
            targetObjectsList = GameManager.currentActiveMainFishObjectsList;

            // Combine the target lists to target both main fishes and foods
            combinedTargetsList = targetObjectsList.Concat(
                GameManager.currentActiveFoodTargetObjectsList
                );

            targetingSystem.SetEatableTargetsList(combinedTargetsList);
        }
        else
        {
            // Enemy targets only main fishes
            targetObjectsList = GameManager.currentActiveMainFishObjectsList;

            targetingSystem.SetEatableTargetsList(targetObjectsList);
        }
    }

    private void Start()
    {
        // Register the enemy in the list of active enemies || Register in GameManager
        hungerSystem.SetHungerBehavior(new EnemyHungerStrategy(
            followerProperties.hungerStartingTime,
            followerProperties.hungerDuration
            ));


        // Set the initial state of the enemy || Initialize state
        stateMachine.ChangeState(new EnemyHuntingState(
            movementController,
            targetingSystem,
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
        GameObject collectableInstance = Instantiate(followerProperties.collectablePrefab, transform.position, Quaternion.identity);
        collectableInstance.GetComponent<Collectable>().collectableConfig = followerProperties.collectableConfigs[0];

    }
}
