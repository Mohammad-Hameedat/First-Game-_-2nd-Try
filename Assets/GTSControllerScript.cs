using System.Collections.Generic;
using UnityEngine;

#region GTH Controller - Required Components
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(TargetingSystem))]
[RequireComponent(typeof(HungerSystem))]
[RequireComponent(typeof(InteractionController))]
[RequireComponent(typeof(BoundsAndPositioningManager))]
#endregion
public class GTSControllerScript : MonoBehaviour
{
    #region Components References
    private MovementController movementController;
    private StateMachine stateMachine;
    private TargetingSystem targetingSystem;
    private HungerSystem hungerSystem;
    private InteractionController interactionController;

    public FollowerSettings followerProperties;
    #endregion


    #region GTS - Required Variables
    private List<GameObject> targetedFishObjectsList = new();

    private List<GameObject> targetedEnemyObjectsList = new();
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

        targetedFishObjectsList = GameManager.currentActiveMainFishObjectsList;
        targetedEnemyObjectsList = GameManager.currentActiveEnemyObjectsList;
    }


    private void Start()
    {
        targetingSystem.targetingStrategy = new FrameBasedTargetingStrategy();

        /* Explaining Hunger System functionality for GTH Pet
         * 
         * The GTH Pet is an aggressive pet that attacks enemies
         * Whenever an enemy instance is detected, GTH Pet will become hungry
         * And this controller script will handle switching between states.
         */
        hungerSystem.SetHungerBehavior(new GTSHungerStrategy(
            targetingSystem,
            followerProperties.hungerProperties.hungerStartingTime
            ));


    }


    private void Update()
    {
        if (GameManager.currentActiveEnemyObjectsList.Count > 0)
        {
            if (stateMachine.currentState is not AggressiveSwimmingState)
            {
                hungerSystem.enabled = false;

                targetingSystem.SetTargetObjectsList(targetedEnemyObjectsList);

                interactionController.SetInteractionStrategy(new AggressiveInteractionStrategy(
                    targetingSystem,
                    followerProperties.attackProperties
                    ));

                stateMachine.ChangeState(new AggressiveSwimmingState(
                    movementController
                    ));
            }
        }
        else
        {
            if (stateMachine.currentState is not GTSIdleState)
            {
                hungerSystem.enabled = true;

                targetingSystem.SetTargetObjectsList(targetedFishObjectsList);

                // Set the interaction strategy
                interactionController.SetInteractionStrategy(new GeneralInteractionStrategy(
                    hungerSystem,
                    targetingSystem,
                    targetedFishObjectsList
                    ));

                stateMachine.ChangeState(new GTSIdleState(
                    movementController
                    ));
            }
        }
    }
}