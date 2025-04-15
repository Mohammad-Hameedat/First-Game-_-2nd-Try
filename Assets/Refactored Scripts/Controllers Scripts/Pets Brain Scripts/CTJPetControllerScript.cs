using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region CTJ Pet - Required Components
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(TargetingSystem))]
[RequireComponent(typeof(HungerSystem))]
[RequireComponent(typeof(BoundsAndPositioningManager))]
[RequireComponent(typeof(InteractionController))]
#endregion
public class CTJPetControllerScript : MonoBehaviour
{
    #region Components References
    private MovementController movementController;
    private StateMachine stateMachine;
    private TargetingSystem targetingSystem;
    private HungerSystem hungerSystem;
    private InteractionController interactionController;

    public FollowerSettings followerProperties;

    #endregion


    #region CTJ Pet - Required Variables
    private List<GameObject> targetObjectsList = new();

    #endregion


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
        targetObjectsList = GameManager.currentActiveCollectiblesList;
    }

    private void Start()
    {
        // Initialize target list
        targetingSystem.SetTargetObjectsList(targetObjectsList);


        targetingSystem.targetingStrategy = new ProximityTargetTargetingStrategy();

        hungerSystem.SetHungerBehavior(new CheckForTargetHungerStrategy(
            targetObjectsList
            ));

        interactionController.SetInteractionStrategy(new CollectCollectibleInteractionStrategy(
            targetingSystem
        ));

        // Switch to NoDangerState
        stateMachine.ChangeState(new NoThreatSwimmingState(
        movementController,
        hungerSystem
        ));
    }
}
