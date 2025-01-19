using System.Collections;
using UnityEngine;

public class WTWPetIdleState : IState
{
    private GameObject petObject;
    private MovementController movementController;

    private Coroutine reactivationCoroutine;

    public WTWPetIdleState(
        GameObject _petObject,
        MovementController _movementController
        )
    {
        petObject = _petObject;
        movementController = _movementController;
    }


    public void Enter()
    {
        reactivationCoroutine = petObject.GetComponent<MonoBehaviour>().StartCoroutine(ReactivateProtectedObjects());

        movementController.SetMovementStrategy(new RandomizedSwimmingMovementStrategy(
            movementController
            ));
    }


    public void Execute()
    {

    }


    public void Exit()
    {
        movementController.SetMovementStrategy(null);

        if (reactivationCoroutine != null)
        {
            petObject.GetComponent<MonoBehaviour>().StopCoroutine(reactivationCoroutine);
            reactivationCoroutine = null;
        }
    }



    private IEnumerator ReactivateProtectedObjects()
    {
        // Note: Activate the Idle animation here (((AGAIN))). || Will be implemented later.

        foreach (GameObject protectedObject in GameManager.currentActiveMainFishObjectsList)
        {
            if (!protectedObject.activeSelf)
            {
                protectedObject.transform.position = petObject.transform.position;
                protectedObject.GetComponent<Rigidbody>().velocity = Vector3.zero;


                protectedObject.SetActive(true);

                yield return new WaitForSeconds(0.01f);
            }
        }

        // Note: Deactivate the Idle animation here. || Will be implemented in later.
    }
}