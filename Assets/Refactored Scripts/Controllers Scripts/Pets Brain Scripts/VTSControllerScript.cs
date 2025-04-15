using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region VTS Pet Controller - Required Components
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(HungerSystem))]
[RequireComponent(typeof(BoundsAndPositioningManager))]
#endregion
public class VTSControllerScript : MonoBehaviour
{
    #region Components References
    private MovementController movementController;
    private StateMachine stateMachine;

    public FollowerSettings petProperties;

    #endregion


    #region VTS Pet - Required Variables
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
        stateMachine.ChangeState(new VTSPetIdleState(
            movementController
            ));


        // Start spawning money
        StartCoroutine(SpawnMoney());
    }


    IEnumerator SpawnMoney()
    {
        float elapsedTime = 0f;

        while (true)
        {
            if (targetObjectsList.Count > 0)
            {
                // Stop spawning money temporarily
                yield return null;
            }
            else
            {
                elapsedTime += Time.deltaTime;

                // Set collectible's configuration
                float nextCollectibleSpawnTime = elapsedTime < 180f
                    ? petProperties.spawnProperties.minCollectableSpwanTime
                    : petProperties.spawnProperties.maxCollectableSpwanTime;

                yield return new WaitForSeconds(nextCollectibleSpawnTime);

                // Instantiate the collectable prefab at the current position
                GameObject collectibleInstance = Instantiate(
                        petProperties.spawnProperties.collectablePrefab,
                        transform.position,
                        Quaternion.identity);

                int collectibleIndex = elapsedTime < 180f ? 0 : 1;
                collectibleInstance.GetComponent<CollectibleScript>().collectibleProperties =
                    petProperties.spawnProperties.collectibleProperties[collectibleIndex];
            }
        }
    }
}