using System.Collections;
using UnityEngine;

public class BaseFollowerMovement : MonoBehaviour
{
    #region Follower Controller References
    [Header("Follower Controller References")]
    protected BoundsAndPositioningManager boundsManager;


    /* You have to set to this variable from the child class
     * and assign it to the correct controller in child's movment script in the Start method,
     * like this: baseFollowerController = GetComponent<ChildFollowerController>();
     */
    protected virtual BaseFollowerController FollowerControllerSetter { get; set; }
    #endregion

    #region Utility Controllers
    [Header("Utility Controllers")]
    protected Rigidbody rb;
    protected int numberOfTargetsInList;
    #endregion

    #region Movement Controllers
    [Header("Movement controllers")]

    protected float desiredVelocity;
    [SerializeField] protected float accelerationDuration;
    [SerializeField] protected float movementSpeed;


    [SerializeField] protected float timeBeforeChangingVelocity = 0f;
    [SerializeField] protected Vector3 randomTargetPosition;
    #endregion


    // These variables are for testing purposes
    #region Testing Variables
    [Header("Testing Variables")]
    public float currentSpeed;
    #endregion

    protected virtual void Start()
    {
        boundsManager = GetComponent<BoundsAndPositioningManager>();
        rb = GetComponent<Rigidbody>();

        desiredVelocity = 0.5f;
        accelerationDuration = Random.Range(2f, 4f);

        randomTargetPosition = boundsManager.GetNewRandomPosition();
        StartCoroutine(CheckPosition());
    }

    protected virtual void Update()
    {
        numberOfTargetsInList = FollowerControllerSetter.GetNumberOfTargetObjectsInList(); // Get the number of target objects in the list from the follower controller script
        transform.position = boundsManager.ClampPositionWithInView(transform.position); // Clamp the position of the object to the camera view
    }


    #region Moving Direction Controllers
    // Move the object towards the target object
    protected virtual void MovingTowardsTargetDirection()
    {
        MovingTowardsTargetSpeed();
        Vector3 directionTowardsTargetThreshold = new(0, 0.5f, 0); // Set a threshold for the direction towards the target object because the target object also moves at the same time (downwards)
        Vector3 positionDifference = (FollowerControllerSetter.CheckTargetDirection() - transform.position) - directionTowardsTargetThreshold;
        Vector3 movementDirection = positionDifference.normalized;
        movementSpeed = desiredVelocity;
        rb.velocity = Vector3.Lerp(rb.velocity, movementDirection * movementSpeed, Time.fixedDeltaTime);
    }

    // Randomize the movement direction of the object
    protected virtual void MovingInRandomDirection()
    {
        RandomMovementSpeed();
        Vector3 positionDifference = randomTargetPosition - transform.position;
        if (positionDifference.sqrMagnitude <= FollowerControllerSetter.followerProperties.minDistanceTowardsRandomTarget * FollowerControllerSetter.followerProperties.minDistanceTowardsRandomTarget)
        {
            randomTargetPosition = boundsManager.GetNewRandomPosition();
        }
        Vector3 directionToTarget = positionDifference.normalized;
        movementSpeed = desiredVelocity;
        rb.velocity = Vector3.Lerp(rb.velocity, directionToTarget * movementSpeed, Time.fixedDeltaTime);
    }
    #endregion


    #region Movement Speed Controllers
    // Set the movement speed of the object and prevent it from moving too fast after eating the target object
    protected virtual void MovingTowardsTargetSpeed()
    {
        if (timeBeforeChangingVelocity < accelerationDuration) // This to prevent the object from moving too fast after it eats the target object
        {
            timeBeforeChangingVelocity = accelerationDuration + 1f;
        }

        desiredVelocity = FollowerControllerSetter.followerProperties.maxFollowingDesiredVelocity;
    }

    // Randomize the movement speed of the object
    protected virtual void RandomMovementSpeed()
    {
        timeBeforeChangingVelocity += Time.fixedDeltaTime;
        if (timeBeforeChangingVelocity >= accelerationDuration)
        {
            desiredVelocity = Random.Range(FollowerControllerSetter.followerProperties.minRandomDesiredVelocity, FollowerControllerSetter.followerProperties.maxRandomDesiredVelocity);
            accelerationDuration = Random.Range(FollowerControllerSetter.followerProperties.minAccelerationDuration, FollowerControllerSetter.followerProperties.maxAccelerationDuration);

            timeBeforeChangingVelocity = 0f;
        }
    }
    #endregion


    // Check the position of the object and get a new random position if the object is close to the edge of the movement-area+
    protected virtual IEnumerator CheckPosition()
    {
        while (true)
        {
            yield return new WaitUntil(() => boundsManager.PositionCheck()); // Wait until the object is close to the edge of the movement-area to avoid getting random positions too frequently
            randomTargetPosition = boundsManager.GetNewRandomPosition();
            yield return new WaitForSeconds(2f); // Wait for 2 seconds before checking the position again
        }
    }
}
