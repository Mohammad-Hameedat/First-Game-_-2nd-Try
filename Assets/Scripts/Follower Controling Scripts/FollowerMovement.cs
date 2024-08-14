using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class FollowerMovement : MonoBehaviour
{
    #region Follower Controller References
    [Header("Follower Controller References")]

    FollowerController followerController;

    BoundsAndPositioningManager boundsManager;

    #endregion

    #region Utility Controllers
    [Header("Utility Controllers")]

    private Rigidbody rb;
    int numberOfTargets;

    #endregion

    #region Movement Controllers
    [Header("Movement controllers")]

    float minDistance = 1f;
    float desiredVelocity;

    [SerializeField] float movementSpeed;
    [SerializeField] float accelerationDuration;
    float timeBeforeChangingVelocity = 0f;

    [SerializeField] Vector3 targetPosition;
    #endregion

    // A variable that will be later deleted
    public float currentSpeed;


    private void Start()
    {
        followerController = GetComponent<FollowerController>();
        boundsManager = GetComponent<BoundsAndPositioningManager>();
        rb = GetComponent<Rigidbody>();


        desiredVelocity = .5f;
        accelerationDuration = Random.Range(2f, 4f);


        targetPosition = boundsManager.GetNewRandomPosition();
    }

    private void Update()
    {
        numberOfTargets = followerController.GetNumberOfTargetObjects();

        currentSpeed = rb.velocity.magnitude;

        transform.position = boundsManager.ClampPositionWithInView(transform.position);

    }

    private void FixedUpdate()
    {
        if (numberOfTargets > 0 && followerController.IsHungry())
        {
            MovingTowardsTarget();
        }
        else
        {
            MovingInRandomDirection();
        }

    }



    #region Moving Direction Controllers

    void MovingTowardsTarget()
    {
        MovementSpeed();

        Vector3 positionDifference = followerController.CheckTargetDirection() - transform.position;

        Vector3 directionToTarget = positionDifference.normalized;

        movementSpeed = desiredVelocity;

        rb.velocity = Vector3.Lerp(rb.velocity, directionToTarget * movementSpeed, Time.fixedDeltaTime);
    }

    void MovingInRandomDirection()
    {
        RandomMovementSpeed();

        // Get the difference between the target position and the object position OR return a new random target position
        Vector3 positionDifference = (targetPosition - transform.position).sqrMagnitude >= minDistance * minDistance ? targetPosition - transform.position : targetPosition = boundsManager.GetNewRandomPosition();

        // Get the direction to the target
        Vector3 directionToTarget = positionDifference.normalized;

        // If the squared distance is less than the acceleration distance, move the object towards the target with a speed of 20f
        movementSpeed = desiredVelocity;

        // Move the object towards the random target
        rb.velocity = Vector3.Lerp(rb.velocity, directionToTarget * movementSpeed, Time.fixedDeltaTime);
    }

    #endregion


    #region Movement Speed Controllers
    private void MovementSpeed()
    {
        if (timeBeforeChangingVelocity < accelerationDuration)
        {
            timeBeforeChangingVelocity = accelerationDuration + 1f;
        }
        desiredVelocity = 4f;
    }

    private void RandomMovementSpeed()
    {
        timeBeforeChangingVelocity += Time.deltaTime;
        if (timeBeforeChangingVelocity >= accelerationDuration)
        {
            desiredVelocity = Random.Range(.5f, 2f);
            accelerationDuration = Random.Range(2f, 4f);
            timeBeforeChangingVelocity = 0f;
        }
    }

    #endregion



    // Draw a line to see the movement direction of the object
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, targetPosition);


        //Gizmos.color = Color.green;
        //Gizmos.DrawLine(transform.position, followerController.CheckNearestTargetObject());
    }


}
