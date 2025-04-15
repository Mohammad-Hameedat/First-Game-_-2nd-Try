using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region STRF Pet Controller - Required Components
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(HungerSystem))]
[RequireComponent(typeof(BoundsAndPositioningManager))]
#endregion
public class STRFControllerScript : MonoBehaviour
{
    #region Components References
    private MovementController movementController;
    private StateMachine stateMachine;

    public FollowerSettings petProperties;

    #endregion


    #region STRF Pet - Required Variables
    private List<GameObject> targetObjectsList = new();

    #endregion


    private void Awake()
    {
        // Get references to the components
        movementController = GetComponent<MovementController>();
        stateMachine = GetComponent<StateMachine>();


        // Assign the properties to the components
        movementController.movementProperties = petProperties.movementProperties;

        // Set the targets list equal to the current active enemy objects list
        targetObjectsList = GameManager.currentActiveEnemyObjectsList;
    }


    private void Start()
    {
        /* Read the following note before you do anything inside the following strategy script:
         * 
         * The following strategy script is the Idle state of another pet ogject
         * that named "Vert The Skeleton (VTS)" pet.
         */
        stateMachine.ChangeState(new VTSPetIdleState(
            movementController
            ));


        // Start spawning money
        StartCoroutine(SpawnMoney());
    }


    IEnumerator SpawnMoney()
    {
        while (true)
        {
            // Check if there are monsters in the game.
            if (targetObjectsList.Count > 0)
            {
                // Stop spawning money temporarily
                yield return null;
                continue;
            }
            else
            {
                // Set collectible's configuration
                float nextCollectibleSpawnTime = Random.Range(
                    petProperties.spawnProperties.minCollectableSpwanTime,
                    petProperties.spawnProperties.maxCollectableSpwanTime
                    );

                yield return new WaitForSeconds(nextCollectibleSpawnTime);

                // Instantiate the collectable prefab at the current position
                GameObject collectibleInstance = Instantiate(
                        petProperties.spawnProperties.collectablePrefab,
                        transform.position,
                        Quaternion.identity
                        );

                collectibleInstance.GetComponent<CollectibleScript>().collectibleProperties =
                    petProperties.spawnProperties.collectibleProperties[0];

                collectibleInstance.AddComponent<BombScript>();
            }
        }
    }
}