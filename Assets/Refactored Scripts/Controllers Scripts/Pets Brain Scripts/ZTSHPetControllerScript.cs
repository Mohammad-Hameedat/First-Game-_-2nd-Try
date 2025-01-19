using System.Collections.Generic;
using UnityEngine;

#region ZTSH Pet Controller - Required Components
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(TargetingSystem))]
[RequireComponent(typeof(HungerSystem))]
[RequireComponent(typeof(BoundsAndPositioningManager))]
#endregion
public class ZTSHPetControllerScript : MonoBehaviour
{
    #region Components References
    private MovementController movementController;
    private StateMachine stateMachine;
    private TargetingSystem targetingSystem;
    private HungerSystem hungerSystem;

    private GameManager gameManager;

    public FollowerSettings followerProperties;

    #endregion


    #region ZTSH Pet - Required Variables
    private List<GameObject> targetObjectsList = new();

    #endregion


    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();

        // Get references to the components
        stateMachine = GetComponent<StateMachine>();
        targetingSystem = GetComponent<TargetingSystem>();
        movementController = GetComponent<MovementController>();
        hungerSystem = GetComponent<HungerSystem>();


        // Assign the properties to the components
        movementController.movementProperties = followerProperties.movementProperties;

        // Set the targets list equal to the current active enemy objects list
        targetObjectsList = GameManager.currentActiveEnemyObjectsList;
    }


    private void Start()
    {
        targetingSystem.targetingStrategy = new ProximityTargetTargetingStrategy();
        targetingSystem.SetTargetObjectsList(targetObjectsList);


        /* Explaining Hunger System functionality for Zorf The Sea Horse (PTMF) Pet
         * 
         * Zorf The Sea Horse (ZTMF) Pet is an peaceful pet that spawns foods
         * Whenever an enemy instance is detected, PTMF Pet will stop spawning foods
         * And this controller will handle switching between states
         */
        hungerSystem.SetHungerBehavior(new EnemySensitivePetHungerStrategy());
    }

    private void Update()
    {
        if (!hungerSystem.IsHungry())
        {
            if (stateMachine.currentState is not ZTSHPetIdleState)
            {
                stateMachine.ChangeState(new ZTSHPetIdleState(
                    gameObject,
                    movementController,
                    followerProperties.spawnProperties,
                    hungerSystem,
                    gameManager
                    ));
            }
        }
        else
        {
            if (stateMachine.currentState is not ZTSHPetAggressiveState)
            {
                stateMachine.ChangeState(new ZTSHPetAggressiveState(
                    gameObject,
                    gameManager,
                    targetingSystem,
                    movementController,
                    followerProperties.spawnProperties
                    ));
            }
        }
    }
}
