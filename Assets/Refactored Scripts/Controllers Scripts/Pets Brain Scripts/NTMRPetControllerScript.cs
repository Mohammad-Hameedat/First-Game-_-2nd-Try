using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#region NTMR Pet Controller - Required Components
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(TargetingSystem))]
[RequireComponent(typeof(InteractionController))]
[RequireComponent(typeof(BoundsAndPositioningManager))]
#endregion
public class NTMRPetControllerScript : MonoBehaviour
{
    #region Components References
    private MovementController movementController;
    private StateMachine stateMachine;
    private TargetingSystem targetingSystem;
    private InteractionController interactionController;

    [Tooltip("Assign the main property settings (Scriptable Object) to the follower")]
    public FollowerSettings followerProperties;
    #endregion

    #region NTMR Pet - Required Variables
    private static IEnumerable<GameObject> targetObjectsList = new List<GameObject>();
    #endregion

    #region Special Skill Variables
    private float elapsedTime = 0f;

    private float specialSkillActivationTime = 60f;
    private float specialSkillCooldownTime = 10f;
    #endregion


    private void Awake()
    {
        // Get references to the components
        movementController = GetComponent<MovementController>();
        stateMachine = GetComponent<StateMachine>();
        targetingSystem = GetComponent<TargetingSystem>();
        interactionController = GetComponent<InteractionController>();

        // Assign the properties to the MovementController, etc.
        movementController.movementProperties = followerProperties.movementProperties;

        targetObjectsList = GameManager.currentActiveFoodTargetObjectsList.Concat(
            GameManager.currentActiveCollectiblesList
            );
    }

    private void Start()
    {
        // Initialize target list
        targetingSystem.targetingStrategy = new FrameBasedTargetingStrategy();
        targetingSystem.SetTargetObjectsList(targetObjectsList);


        interactionController.SetInteractionStrategy(new NTMRPetInteractionStrategy(
            targetingSystem,
            movementController.boundsManager
            ));
    }


    private void Update()
    {
        // If no enemies -> switch to idle if not already in it
        if (GameManager.currentActiveEnemyObjectsList.Count == 0)
        {
            // Increment elapsed time
            elapsedTime += Time.deltaTime;

            if (specialSkillCooldownTime <= 0f)
            {
                elapsedTime = 0f;
                specialSkillCooldownTime = 10f;
            }
            else if (elapsedTime >= specialSkillActivationTime)
            {
                specialSkillCooldownTime -= Time.deltaTime;

                if (stateMachine.currentState is not NTMRPetSpecialSkillState)
                {
                    stateMachine.ChangeState(new NTMRPetSpecialSkillState(
                        movementController
                    ));
                }
            }
            else if (stateMachine.currentState is not NTMRPetIdleState)
            {
                stateMachine.ChangeState(new NTMRPetIdleState(
                    movementController
                    ));
            }
        }
        else
        {
            // If there are enemies, pick some Danger state, etc.
            if (stateMachine.currentState is not NTMRPetThreatenedState)
            {
                stateMachine.ChangeState(new NTMRPetThreatenedState(
                    movementController
                    ));
            }
        }
    }
}