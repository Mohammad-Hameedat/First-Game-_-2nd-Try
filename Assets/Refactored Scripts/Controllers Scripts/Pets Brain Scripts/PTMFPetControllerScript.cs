using UnityEngine;

#region Main Fish Controller - Required Components
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(HungerSystem))]
[RequireComponent(typeof(BoundsAndPositioningManager))]
#endregion
public class PTMFPetControllerScript : MonoBehaviour
{
    #region Components References
    private MovementController movementController;
    private StateMachine stateMachine;
    private HungerSystem hungerSystem;


    public FollowerSettings followerProperties;

    #endregion

    private void Awake()
    {
        // Get references to the components
        stateMachine = GetComponent<StateMachine>();
        movementController = GetComponent<MovementController>();
        hungerSystem = GetComponent<HungerSystem>();


        // Assign the properties to the components
        movementController.movementProperties = followerProperties.movementProperties;
    }


    private void Start()
    {
        /* Explaining Hunger System functionality for Pergo The Momma Fish (PTMF) Pet
        * 
        * Pergo The Momma Fish (PTMF) Pet is an peaceful pet that spawns main fishes
        * Whenever an enemy instance is detected, PTMF Pet will stop spawning main fishes
        * And this controller will handle switching between states
        */
        hungerSystem.SetHungerBehavior(new EnemySensitivePetHungerStrategy());
    }


    private void Update()
    {
        if (!hungerSystem.IsHungry())
        {
            if (stateMachine.currentState is not PTMFPetIdleState)
            {
                stateMachine.ChangeState(new PTMFPetIdleState(
                    gameObject,
                    movementController,
                    followerProperties.spawnProperties,
                    hungerSystem
                    ));
            }
        }
        else
        {
            return;
        }
    }
}