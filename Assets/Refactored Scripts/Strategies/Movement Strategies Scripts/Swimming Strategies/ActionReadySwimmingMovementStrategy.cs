using UnityEngine;

public class ActionReadySwimmingMovementStrategy : IMovementStrategy
{
    #region References
    private readonly MovementController movementController;
    private readonly TargetingSystem targetingSystem;
    private readonly BoundsAndPositioningManager boundsManager;

    private readonly MovementProperties movementProperties;
    #endregion


    #region Targets And Targeing Variables
    private Transform targetTransform;

    private ScreenCorner currentCorner, nextCorenr;
    private Vector3 nextCorenerWorldPosition;
    #endregion


    #region Movement Scaling Variables and properties
    private float movementInterpolation = 1f;
    private float movementSpeedScale = 1.5f;

    private bool isMoving = false; // Track if the object actively moving to a corner.
    private const float cornerArrivalDistance = 10f;
    #endregion


    #region Constructor
    public ActionReadySwimmingMovementStrategy(
        MovementController _movementController,
        TargetingSystem _targetingSystem
        )
    {
        movementController = _movementController;
        targetingSystem = _targetingSystem;
        movementProperties = movementController.movementProperties;
        boundsManager = movementController.boundsManager;


        Initialize();
    }

    #endregion

    public void GetTarget()
    {
        targetTransform = targetingSystem.GetNearestTarget();
    }


    public void Move(Rigidbody rb)
    {
        if (targetTransform == null)
        {
            return;
        }


        if (!isMoving)
        {
            nextCorenr = boundsManager.FindBestNextCorner(currentCorner, targetTransform.position);

            nextCorenerWorldPosition = boundsManager.CornerToWorldPosition(nextCorenr);

            isMoving = true;
        }


        Vector3 movementDirection = ( nextCorenerWorldPosition - rb.position ).normalized;
        float targetVelocity = movementProperties.maxFollowingDesiredVelocity * movementSpeedScale;

        float distanceToNextCorner = Vector3.Distance(rb.position, nextCorenerWorldPosition);

        if (distanceToNextCorner < cornerArrivalDistance)
        {
            currentCorner = nextCorenr;
            isMoving = false;
        }

        if (distanceToNextCorner > movementInterpolation)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, movementDirection * targetVelocity, Time.fixedDeltaTime * 2f);
        }
        else if (distanceToNextCorner <= movementInterpolation)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.fixedDeltaTime * 2f);
        }
    }


    private void Initialize()
    {
        currentCorner = boundsManager.GetNearestCorner(movementController.transform.position);
    }
}