using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region ATAF Controller - Required Components
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(TargetingSystem))]
[RequireComponent(typeof(HungerSystem))]
[RequireComponent(typeof(InteractionController))]
[RequireComponent(typeof(BoundsAndPositioningManager))]
#endregion
public class ATAFControllerScript : MonoBehaviour
{
    #region Components References
    private MovementController movementController;
    private StateMachine stateMachine;
    private TargetingSystem targetingSystem;
    private HungerSystem hungerSystem;
    private InteractionController interactionController;

    public FollowerSettings followerProperties;
    #endregion

    #region ATAF - Required Variables
    private List<GameObject> activeCorpseTargetsList = new();
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
        activeCorpseTargetsList = GameManager.currentActiveCorpsedObjectsList;
    }

    private void Start()
    {
        hungerSystem.SetHungerBehavior(new GeneralCheckHungerStrategy(
            movementController,
            targetingSystem,
            activeCorpseTargetsList
            ));

        targetingSystem.targetingStrategy = new ProximityTargetTargetingStrategy();
        // Set the target objects list for the targeting system
        targetingSystem.SetTargetObjectsList(activeCorpseTargetsList);


        // Set the interaction strategy for the interaction controller
        interactionController.SetInteractionStrategy(new RevivalInteractionStrategy(
            movementController,
            targetingSystem
            ));

        StartCoroutine(ControllerLifeCicle());
    }

    private IEnumerator ControllerLifeCicle()
    {
        float elapsedTime = 0f;
        float dashDuration = 5f;
        float dashActivationTime = 20f;

        while (true)
        {
            stateMachine.ChangeState(new ATAFIdleState(
                movementController
                ));

            while (elapsedTime < dashActivationTime)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            elapsedTime = 0f;

            stateMachine.ChangeState(new ATAFDashState(
                movementController
                ));

            Debug.Log("Dashing...");

            yield return new WaitForSeconds(dashDuration);
        }
    }

    //private void Update()
    //{
    //    if (!isDashing)
    //    {
    //        if (stateMachine.currentState is not ATAFIdleState)
    //        {
    //            stateMachine.ChangeState(new ATAFIdleState(
    //                movementController
    //                ));
    //        }
    //    }
    //    else
    //    {
    //        if (stateMachine.currentState is not ATAFDashState)
    //        {
    //            stateMachine.ChangeState(new ATAFDashState(
    //                movementController
    //                ));
    //        }
    //    }
    //}

}
