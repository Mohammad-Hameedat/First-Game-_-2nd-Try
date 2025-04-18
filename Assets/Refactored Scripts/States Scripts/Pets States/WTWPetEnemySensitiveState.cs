using System.Collections;
using UnityEngine;

public class WTWPetEnemySensitiveState : IState
{
    private GameObject petObject;
    private MovementController movementController;
    private TargetingSystem targetingSystem;
    private InteractionController interactionController;

    private Coroutine threatenedBehaviorCoroutine;

    public WTWPetEnemySensitiveState(
        GameObject _petObject,
        MovementController _movementController,
        TargetingSystem _targetingSystem,
        InteractionController _interactionController
        )
    {
        petObject = _petObject;
        movementController = _movementController;
        targetingSystem = _targetingSystem;
        interactionController = _interactionController;
    }


    public void Enter()
    {
        // >>>>> Note: Activate the Idle animation here. || Will be implemented later.

        interactionController.enabled = true;

        movementController.SetMovementStrategy(null);

        threatenedBehaviorCoroutine = petObject.GetComponent<MonoBehaviour>().StartCoroutine(HandleThreatenedBehavior());
    }


    public void Execute() { }


    public void Exit()
    {
        movementController.SetMovementStrategy(null);

        if (threatenedBehaviorCoroutine != null)
        {
            //Debug.Log("Stopping the threatened behavior coroutine");

            petObject.GetComponent<MonoBehaviour>().StopCoroutine(threatenedBehaviorCoroutine);

            threatenedBehaviorCoroutine = null;
        }
    }


    /* Explaining the behavior of the WTW Pet when it is threatened by an enemy object:
     * 
     * The WTW pet will stop moving till all existing Main-Fish objects are protected.
     * All the Main-Fish objects will start moving towards the WTW pet and get protected (deactivated).
     * The WTW pet will start moving randomly and while keeping the Main-Fish objects protected for 20 secounds,
     * And then re-activate all the Main-Fish objects, adding some challenge to the game.
     */
    private IEnumerator HandleThreatenedBehavior()
    {
        while (interactionController.interactionStrategy.GetInteractedTargetsCount() < GameManager.currentActiveMainFishObjectsList.Count)
        {
            targetingSystem.GetNearestTarget();

            movementController.rb.velocity = Vector3.Lerp(movementController.rb.velocity, Vector3.zero, Time.deltaTime);

            yield return null;
        }

        interactionController.enabled = false;

        // >>>>> Note: Deactivate the Idle animation here. || Will be implemented in later.

        movementController.SetMovementStrategy(new RandomizedSwimmingMovementStrategy(
            movementController
            ));

        Debug.Log($"Setting {GameManager.fishesProtectionDuration} secounds count down timer befor re-activating all Main-Fish objects");

        /* NOTE: Set a timer before re-activate all the Main-Fish objects.
         * 
         * This is to give the Main-Fish objects a chance to escape from the threat,
         * And to add some challenge to the game.
         */
        float protectionDuration = GameManager.fishesProtectionDuration;

        yield return new WaitForSeconds(protectionDuration);

        GameManager.canBeProtectedByWTWPet = false;

        yield return ReactivateProtectedObjects();

        yield return null;
    }


    private IEnumerator ReactivateProtectedObjects()
    {
        // >>>>> Note: Activate the Idle animation here (((AGAIN))). || Will be implemented later.

        foreach (GameObject protectedObject in GameManager.currentActiveMainFishObjectsList)
        {
            if (!protectedObject.activeSelf)
            {
                protectedObject.transform.position = petObject.transform.position;

                protectedObject.SetActive(true);

                // Activate 100 objects per second
                yield return new WaitForSeconds(0.01f);
            }
        }

        // >>>>> Note: Deactivate the Idle animation here. || Will be implemented in later.
    }
}
