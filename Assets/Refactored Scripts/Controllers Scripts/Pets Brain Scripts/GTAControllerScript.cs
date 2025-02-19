using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#region WTW Pet Controller - Required Components
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(TargetingSystem))]
[RequireComponent(typeof(HungerSystem))]
[RequireComponent(typeof(BoundsAndPositioningManager))]
#endregion
public class GTAControllerScript : MonoBehaviour
{
    #region Components References
    private MovementController movementController;
    private StateMachine stateMachine;
    private TargetingSystem targetingSystem;
    private HungerSystem hungerSystem;

    public FollowerSettings followerProperties;
    #endregion

    #region GTA Pet - Required Variables
    public PetType PetType;

    private List<GameObject> targetObjectsList = new();
    #endregion

    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        stateMachine = GetComponent<StateMachine>();
        targetingSystem = GetComponent<TargetingSystem>();
        hungerSystem = GetComponent<HungerSystem>();

        // Assign the properties to the components
        movementController.movementProperties = followerProperties.movementProperties;
        targetObjectsList = GameManager.currentActiveEnemyObjectsList;
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
         * The GTA Pet is not aggressive and does not attack enemies
         * Whenever an enemy instance is detected, GTA Pet will become hungry
         * and GTA Pet will start moving towards the farthest corner of the screen
         * away from the ((((existing enemy object)))).
         */
        hungerSystem.SetHungerBehavior(new EnemySensitivePetHungerStrategy());
    }

    private void Update()
    {
        if (!hungerSystem.IsHungry())
        {
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
            if (stateMachine.currentState is not GTAPetEnemySensitiveState)
            {
                stateMachine.ChangeState(new GTAPetEnemySensitiveState(
                    movementController,
                    targetingSystem
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
