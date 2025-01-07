using System.Collections.Generic;
using UnityEngine;


#region Snail Pet Controller - Required Components
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(TargetingSystem))]
[RequireComponent(typeof(HungerSystem))]
[RequireComponent(typeof(BoundsAndPositioningManager))]
[RequireComponent(typeof(InteractionController))]
#endregion
public class SnailPetControllerScript : MonoBehaviour
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
        targetObjectsList = GameManager.currentActiveCollectiblesList;
    }

    private void Start()
    {
        // Initialize target list
        targetingSystem.SetEatableTargetsList(targetObjectsList);


        targetingSystem.targetingStrategy = new ProximityEnemyTargetingStrategy();

        hungerSystem.SetHungerBehavior(new ChaseCollectibleHungerStrategy());

        interactionController.SetInteractionStrategy(new ChaseCollectibleInteractionStrategy(
            targetingSystem
        ));

        // Switch to NoDangerState
        stateMachine.ChangeState(new NoThreatWalkingState(
        movementController,
        targetingSystem,
        hungerSystem
        ));
    }
}