using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class FollowerMovement : MonoBehaviour
{
    #region Follower Controller References
    [Header("Follower Controller References")]

    FollowerController followerController;
    #endregion

    #region Utility Controllers
    [Header("Utility Controllers")]

    private Rigidbody rb;
    int numberOfTargets;

    [SerializeField] Vector3 targetPosition;
    #endregion

    #region Movement Controllers
    [Header("Movement controllers")]

    float minDistance = 1f;

    float desiredVelocity;

    [SerializeField] float movementSpeed;
    [SerializeField] float accelerationDuration;
    #endregion

    float timeBeforeChangingVelocity = 0f;
    // A variable that will be later deleted
    public float currentSpeed;


    private void Start()
    {
        followerController = GetComponent<FollowerController>();
        rb = GetComponent<Rigidbody>();
        desiredVelocity = .5f;
        accelerationDuration = Random.Range(2f, 4f);
        targetPosition = GetNewRandomPosition();
    }

    private void Update()
    {
        numberOfTargets = FollowerController.GetNumberOfTargetObjects();
        //CheckDistance();
        MovementSpeed();

        currentSpeed = rb.velocity.magnitude;


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

        //MovingInRandomDirection();
    }



    #region Moving Direction Controllers

    void MovingTowardsTarget()
    {
        Vector3 positionDifference = followerController.CheckNearestTargetObject() - transform.position;

        Vector3 directionToTarget = positionDifference.normalized;

        movementSpeed = desiredVelocity;

        rb.velocity = Vector3.Lerp(rb.velocity, directionToTarget * movementSpeed, Time.fixedDeltaTime / 2f);

    }

    void MovingInRandomDirection()
    {
        // Get the difference between the target position and the object position OR return a new random target position
        Vector3 positionDifference = (targetPosition - transform.position).sqrMagnitude >= minDistance * minDistance ? targetPosition - transform.position : targetPosition = GetNewRandomPosition();

        // Get the direction to the target
        Vector3 directionToTarget = positionDifference.normalized;

        // If the squared distance is less than the acceleration distance, move the object towards the target with a speed of 20f
        movementSpeed = desiredVelocity;

        // Move the object towards the random target
        rb.velocity = Vector3.Lerp(rb.velocity, directionToTarget * movementSpeed, Time.fixedDeltaTime);

    }

    Vector3 GetNewRandomPosition()
    {
        targetPosition = new Vector3(Random.Range(-17f, 17f), Random.Range(1f, 17f), -2f);
        return targetPosition;
    }

    #endregion


    #region Movement Speed Controller
    void MovementSpeed()
    {
        if (numberOfTargets == 0)
        {
            timeBeforeChangingVelocity += Time.deltaTime;
            if (timeBeforeChangingVelocity > accelerationDuration)
            {
                desiredVelocity = Random.Range(.5f, 2f);
                accelerationDuration = Random.Range(2f, 4f);
                timeBeforeChangingVelocity = 0f;
            }
        }
        else if (followerController.IsHungry())
        {
            desiredVelocity = 4f;
        }
    }

    #endregion



    // Draw a line to see the movement direction of the object
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, targetPosition);
    }
}
