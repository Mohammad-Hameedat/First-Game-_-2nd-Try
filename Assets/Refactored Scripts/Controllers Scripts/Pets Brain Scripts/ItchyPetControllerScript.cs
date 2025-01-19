using System.Collections.Generic;
using UnityEngine;

#region Itchy Pet Controller - Required Components
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(TargetingSystem))]
[RequireComponent(typeof(HungerSystem))]
[RequireComponent(typeof(BoundsAndPositioningManager))]
[RequireComponent(typeof(InteractionController))]
#endregion
public class ItchyPetControllerScript : MonoBehaviour
{
    #region Components References
    private MovementController movementController;
    private StateMachine stateMachine;
    private TargetingSystem targetingSystem;
    private HungerSystem hungerSystem;
    private InteractionController interactionController;

    public FollowerSettings followerProperties;

    #endregion

    #region Itchy Pet - Required Variables
    private List<GameObject> targetObjectsList = new();

    #endregion

    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        stateMachine = GetComponent<StateMachine>();
        targetingSystem = GetComponent<TargetingSystem>();
        hungerSystem = GetComponent<HungerSystem>();
        interactionController = GetComponent<InteractionController>();

        // Assign the properties to the components
        movementController.movementProperties = followerProperties.movementProperties;
        targetObjectsList = GameManager.currentActiveEnemyObjectsList;
    }

    private void Start()
    {
        targetingSystem.targetingStrategy = new ProximityTargetTargetingStrategy();
        targetingSystem.SetTargetObjectsList(targetObjectsList);

        /* Explaining Hunger System functionality for Itchy Pet
         * 
         * The Itchy Pet is an aggressive pet that attacks enemies
         * Whenever an enemy instance is detected, Itchy Pet will become hungry
         * And this controller script will handle switching between states
         */
        hungerSystem.SetHungerBehavior(new EnemySensitivePetHungerStrategy());

        interactionController.SetInteractionStrategy(new AggressiveInteractionStrategy(
            targetingSystem,
            followerProperties.attackProperties
            ));
    }

    private void Update()
    {
        if (hungerSystem.IsHungry())
        {
            if (stateMachine.currentState is not AggressiveSwimmingState)
            {
                stateMachine.ChangeState(new AggressiveSwimmingState(
                    movementController,
                    targetingSystem
                    ));
            }
        }
        else
        {
            if (stateMachine.currentState is not NoThreatSwimmingState)
            {
                stateMachine.ChangeState(new NoThreatSwimmingState(
                    movementController,
                    targetingSystem,
                    hungerSystem
                    ));
            }
        }
    }
}