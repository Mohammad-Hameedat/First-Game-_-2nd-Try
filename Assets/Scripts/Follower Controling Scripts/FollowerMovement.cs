using System.Collections;
using UnityEngine;

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

        StartCoroutine(CheckPosition());
    }

    private void Update()
    {
        numberOfTargets = followerController.GetNumberOfTargetObjects();

        // <<<<<<<<<>>>>>>>>>> A variable that will be later deleted <<<<<<<<<>>>>>>>>>>
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

        Vector3 positionDifference;

        // Check if the squared distance is greater than or equal to the squared minimum distance
        if ((targetPosition - transform.position).sqrMagnitude >= minDistance * minDistance)
        {
            // If the distance is sufficient, use the difference between the target position and the current position
            positionDifference = targetPosition - transform.position;
        }
        else
        {
            // If the distance is too small, generate a new random target position
            targetPosition = boundsManager.GetNewRandomPosition();

            // Use the difference between the new target position and the current position
            positionDifference = targetPosition - transform.position;
        }

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


    private IEnumerator CheckPosition()
    {
        while (true)
        {
            yield return new WaitUntil(() => boundsManager.PositionCheck());

            targetPosition = boundsManager.GetNewRandomPosition();

            yield return new WaitForSeconds(2f);
        }
    }


    // Draw a line to see the movement direction of the object
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, targetPosition);


        //Gizmos.color = Color.green;
        //Gizmos.DrawLine(transform.position, followerController.CheckNearestTargetObject());
    }


}
