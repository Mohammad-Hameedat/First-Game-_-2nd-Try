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
    protected virtual BaseFollowerController baseFollowerController { get; set; }
    #endregion

    #region Utility Controllers
    [Header("Utility Controllers")]
    protected Rigidbody rb;
    protected int numberOfTargets;
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
        numberOfTargets = baseFollowerController.GetNumberOfTargetObjects();
        transform.position = boundsManager.ClampPositionWithInView(transform.position);
    }


    #region Moving Direction Controllers

    protected virtual void MovingTowardsTarget()
    {
        MovementSpeed();
        Vector3 positionDifference = baseFollowerController.CheckTargetDirection() - transform.position;
        Vector3 movementDirection = positionDifference.normalized;
        movementSpeed = desiredVelocity;
        rb.velocity = Vector3.Lerp(rb.velocity, movementDirection * movementSpeed, Time.fixedDeltaTime);
    }

    protected virtual void MovingInRandomDirection()
    {
        RandomMovementSpeed();
        Vector3 positionDifference;
        if ((targetPosition - transform.position).sqrMagnitude >= minDistance * minDistance)
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

    protected virtual void MovementSpeed()
    {
        if (timeBeforeChangingVelocity < accelerationDuration)
        {
            timeBeforeChangingVelocity = accelerationDuration + 1f;
        }
        desiredVelocity = 4f;
    }

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
