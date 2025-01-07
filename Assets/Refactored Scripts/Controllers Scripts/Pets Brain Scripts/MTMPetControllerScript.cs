using System.Collections;
using UnityEngine;

#region MTM Controller - Required Components
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(HungerSystem))]
[RequireComponent(typeof(BoundsAndPositioningManager))]
#endregion
public class MTMPetControllerScript : MonoBehaviour
{

    #region Components References
    private MovementController movementController;
    private StateMachine stateMachine;
    private HungerSystem hungerSystem;

    public FollowerSettings followerProperties;

    #endregion

    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        stateMachine = GetComponent<StateMachine>();
        hungerSystem = GetComponent<HungerSystem>();


        // Assign the properties to the components
        movementController.movementProperties = followerProperties.movementProperties;
    }

    private void Start()
    {
        /* Explaining Hunger System functionality for MTM pet:
         * 
         * The MTM is a pet that is not aggressive and does not attack enemies
         * Whenever an enemy instance is detected, MTM will not become hungry
         * And this controller script will handle the change of states
         */
        hungerSystem.SetHungerBehavior(new EnemySensitivePetHungerStrategy());

        stateMachine.ChangeState(new MTMPetIdleState(
            movementController
            ));

        StartCoroutine(SpawnersBoosters());
    }


    private IEnumerator SpawnersBoosters()
    {
        float spawnCycle = 40f;
        float elapsedTime = 0f;

        while (true)
        {
            while (elapsedTime < spawnCycle && !hungerSystem.IsHungry())
            {
                elapsedTime += Time.deltaTime;
                yield return null;
                continue;
            }

            if (hungerSystem.IsHungry())
            {
                GameEvents.EventsChannelInstance.BoostSpawningCollectibles(false, 0);
                yield return new WaitUntil(() => !hungerSystem.IsHungry());
            }
            else if (!hungerSystem.IsHungry())
            {
                int boostIterations = Random.Range(3, 6);
                GameEvents.EventsChannelInstance.BoostSpawningCollectibles(true, boostIterations);

                yield return new WaitForSeconds(1f);

                GameEvents.EventsChannelInstance.BoostSpawningCollectibles(false, boostIterations);
                elapsedTime = 0f;
            }
        }
    }
}