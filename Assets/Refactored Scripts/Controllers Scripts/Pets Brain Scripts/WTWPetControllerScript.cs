using System.Collections.Generic;
using UnityEngine;

#region WTW Pet Controller - Required Components
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(TargetingSystem))]
[RequireComponent(typeof(HungerSystem))]
[RequireComponent(typeof(InteractionController))]
[RequireComponent(typeof(BoundsAndPositioningManager))]
#endregion
public class WTWPetControllerScript : MonoBehaviour
{
    #region Components References
    private MovementController movementController;
    private StateMachine stateMachine;
    private TargetingSystem targetingSystem;
    private HungerSystem hungerSystem;
    private InteractionController interactionController;

    public FollowerSettings followerProperties;
    #endregion


    #region WTW Pet - Required Variables
    public PetType PetType;

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
        targetObjectsList = GameManager.currentActiveMainFishObjectsList;
    }

    private void OnEnable()
    {
        if (!GameManager.cAPPetsDictionary.ContainsKey(PetType))
        {
            GameManager.cAPPetsDictionary.Add(PetType, gameObject);
        }
    }

    private void Start()
    {
        targetingSystem.targetingStrategy = new ProximityTargetTargetingStrategy();
        targetingSystem.SetTargetObjectsList(targetObjectsList);

        /* Explaining Hunger System functionality for WTW Pet
         * 
         * The WTW Pet is not aggressive and does not attack enemies
         * Whenever an enemy instance is detected, WTW Pet will become hungry
         * and WTW Pet will pull all the main fishes into its mouth.
         */
        hungerSystem.SetHungerBehavior(new EnemySensitivePetHungerStrategy());

        interactionController.SetInteractionStrategy(
            new ProtectiveGatheringInteractionStrategy(
            targetingSystem
            ));
    }


    private void Update()
    {
        if (!hungerSystem.IsHungry())
        {
            // Disable the interaction controller to prevent unnecessary interactions with the main fish.
            //interactionController.enabled = false;


            // Set new interaction strategy to reset the interaction strategy to the default values.
            interactionController.SetInteractionStrategy(
               new ProtectiveGatheringInteractionStrategy(
               targetingSystem
               ));


            if (stateMachine.currentState is not WTWPetIdleState)
            {
                stateMachine.ChangeState(new WTWPetIdleState(
                    gameObject,
                    movementController
                    ));
            }
        }
        else
        {
            if (stateMachine.currentState is not WTWPetEnemySensitiveState)
            {
                //interactionController.enabled = true;

                /* NOTE: The following code is commented because its for testing purposes
                if (!GameManager.cAPPetsDictionary.ContainsKey(PetType))
                {
                    GameManager.cAPPetsDictionary.Add(PetType, gameObject);
                }
                /*

                /* Read the following note when you back to continue the implementation
                 * 
                 * The interaction controller might not work as expected in the current state
                 * when reactivating the fish objects while there is an enemy object in the scene.
                 * 
                 * So try to fix this issue by controlling the interaction controller status in the state strategy.
                 */
                stateMachine.ChangeState(new WTWPetEnemySensitiveState(
                    gameObject,
                    movementController,
                    targetingSystem,
                    interactionController
                    ));
            }
        }
    }

    private void OnDisable()
    {
        if (GameManager.cAPPetsDictionary.ContainsKey(PetType))
        {
            GameManager.cAPPetsDictionary.Remove(PetType);
        }
    }
}