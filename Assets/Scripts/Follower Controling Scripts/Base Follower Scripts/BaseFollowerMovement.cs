using System.Collections;
using UnityEngine;

public class BaseFollowerMovement : MonoBehaviour
{
    #region Follower Controller References
    [Header("Follower Controller References")]
    protected BoundsAndPositioningManager boundsManager;


    /* You have to get to this variable from the child class
     * and assign it to the correct controller in the child class Start method,
     * like this: baseFollowerController = GetComponent<ChildFollowerController>();
     */
    protected virtual BaseFollowerController followerControllerSetter { get; set; }
    #endregion

    #region Utility Controllers
    [Header("Utility Controllers")]
    protected Rigidbody rb;
    protected int numberOfTargetsInList;
    #endregion

    #region Movement Controllers
    [Header("Movement controllers")]
    protected float minDistance = 1f;
    protected float desiredVelocity;
    [SerializeField] protected float movementSpeed;
    [SerializeField] protected float accelerationDuration;
    protected float timeBeforeChangingVelocity = 0f;
    [SerializeField] protected Vector3 targetPosition;
    #endregion

    protected virtual void Start()
    {
        boundsManager = GetComponent<BoundsAndPositioningManager>();
        rb = GetComponent<Rigidbody>();

        desiredVelocity = .5f;
        accelerationDuration = Random.Range(2f, 4f);

        targetPosition = boundsManager.GetNewRandomPosition();

        StartCoroutine(CheckPosition());
    }

    protected virtual void Update()
    {
        if (numberOfTargetsInList != followerControllerSetter.GetNumberOfTargetObjectsInList())
        {
            numberOfTargetsInList = followerControllerSetter.GetNumberOfTargetObjectsInList();
        }

        transform.position = boundsManager.ClampPositionWithInView(transform.position);
    }


    #region Moving Direction Controllers


    // Move the object towards the target object
    protected virtual void MovingTowardsTarget()
    {
        MovementSpeed();
        Vector3 positionDifference = followerControllerSetter.CheckTargetDirection() - transform.position;
        Vector3 movementDirection = positionDifference.normalized;
        movementSpeed = desiredVelocity;
        rb.velocity = Vector3.Lerp(rb.velocity, movementDirection * movementSpeed, Time.fixedDeltaTime);
    }

    // Randomize the movement direction of the object
    protected virtual void MovingInRandomDirection()
    {
        RandomMovementSpeed();
        Vector3 positionDifference;
        if ((targetPosition - transform.position).magnitude >= minDistance * minDistance)
        {
            positionDifference = targetPosition - transform.position;
        }
        else
        {
            targetPosition = boundsManager.GetNewRandomPosition();
            positionDifference = targetPosition - transform.position;
        }
        Vector3 directionToTarget = positionDifference.normalized;
        movementSpeed = desiredVelocity;
        rb.velocity = Vector3.Lerp(rb.velocity, directionToTarget * movementSpeed, Time.fixedDeltaTime);
    }

    #endregion

    #region Movement Speed Controllers

    // Set the movement speed of the object
    protected virtual void MovementSpeed()
    {
        if (timeBeforeChangingVelocity < accelerationDuration)
        {
            timeBeforeChangingVelocity = accelerationDuration + 1f;
        }
        desiredVelocity = 4f;
    }

    // Randomize the movement speed of the object
    protected virtual void RandomMovementSpeed()
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


    // Check the position of the object and get a new random position if the object is close to the edge of the movement-area+
    protected virtual IEnumerator CheckPosition()
    {
        while (true)
        {
            yield return new WaitUntil(() => boundsManager.PositionCheck());
            targetPosition = boundsManager.GetNewRandomPosition();
            yield return new WaitForSeconds(2f);
        }
    }
}
