using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region ATEE Pet Controller - Required Components
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(BoundsAndPositioningManager))]
#endregion
public class ATEEPetControllerScript : MonoBehaviour
{
    #region Components References
    private MovementController movementController;

    public FollowerSettings followerProperties;
    #endregion

    #region ATEE Pet Controller - Required Variables
    private List<GameObject> targetObjectsList = new();

    [SerializeField] private float elapsedTime = 0f;
    private float boostWaitTime = 75f;

    private int electrificationTriggerIndex = 3;


    private int currentClickIndex = 0;

    #endregion


    #region Properties
    public int CurrentClickIndex
    {
        get => currentClickIndex;
        set
        {
            if (elapsedTime >= boostWaitTime)
            {
                currentClickIndex = value;
            }
        }
    }

    #endregion

    private void Awake()
    {
        // Get references to the components
        movementController = GetComponent<MovementController>();

        // Assign the properties to the components
        movementController.movementProperties = followerProperties.movementProperties;
        targetObjectsList = GameManager.currentActiveMainFishObjectsList;
    }

    private void Start()
    {
        movementController.SetMovementStrategy(new RandomizedSwimmingMovementStrategy(
            movementController
            ));

        StartCoroutine(ElectrifyTankCoroutine());
    }


    private IEnumerator ElectrifyTankCoroutine()
    {
        while (true)
        {
            while (elapsedTime < boostWaitTime)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Make the pet to do something to informe the player that this object is ready to electrify the tank.

            yield return new WaitUntil(() => currentClickIndex == electrificationTriggerIndex);

            foreach (GameObject target in targetObjectsList)
            {
                if (target != null)
                {
                    target.GetComponent<MainFishControllerScript>().ElectrificationTrigger();
                }
            }

            /* Important Note:
             * 
             * => After the electrification activation, a new fish must be spawned in the tank.
             */

            currentClickIndex = 0;
            elapsedTime = 0f;

            yield return null;
        }
    }
}