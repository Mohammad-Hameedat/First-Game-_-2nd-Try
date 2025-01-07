using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region RTFC Pet Controller - Required Components
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(TargetingSystem))]
[RequireComponent(typeof(HungerSystem))]
[RequireComponent(typeof(BoundsAndPositioningManager))]
[RequireComponent(typeof(InteractionController))]
#endregion
public class RTFCPetControllerScript : MonoBehaviour
{
    private MovementController movementController;
    private StateMachine stateMachine;
    private TargetingSystem targetingSystem;
    private HungerSystem hungerSystem;
    private InteractionController interactionController;

    private List<GameObject> targetObjectsList = new();

    [Tooltip("Assign the main property settings (Scriptable Object) to the follower")]
    public FollowerSettings followerProperties;

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
        targetObjectsList = GameManager.currentActiveEnemyObjectsList;
    }

    private void Start()
    {
        targetingSystem.targetingStrategy = new ProximityEnemyTargetingStrategy();
        targetingSystem.SetEatableTargetsList(targetObjectsList);

        /* Explaining Hunger System functionality for Rufus The Fiddler Crab (RTFC) Pet
         * 
         * The RTFC Pet is a pet that attacks enemies
         * Whenever an enemy instance is detected, RTFC Pet will become hungry
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
            if (stateMachine.currentState is not AggressiveWalkingState)
            {
                stateMachine.ChangeState(new AggressiveWalkingState(
                movementController,
                targetingSystem
                ));
            }
        }
        else
        {
            if (stateMachine.currentState is not NoThreatWalkingState)
            {
                stateMachine.ChangeState(new NoThreatWalkingState(
                    movementController,
                    targetingSystem,
                    hungerSystem
                    ));
            }
        }
    }



}