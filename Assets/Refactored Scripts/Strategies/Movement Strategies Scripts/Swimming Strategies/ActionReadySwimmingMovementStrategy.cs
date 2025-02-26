using UnityEngine;


/* Explanation of the useage of this movement strategy:
 * 
 * This movement strategy depends on the BoundsAndPositioningManager script
 * to find the best corner to move to, and then move to that corner.
 * 
 * Note: The corners's positions are pre-defined in the BoundsAndPositioningManager script.
 */
[RequireComponent(typeof(BoundsAndPositioningManager))]
public class ActionReadySwimmingMovementStrategy : IMovementStrategy
{
    #region References
    private readonly MovementController movementController;
    private readonly TargetingSystem targetingSystem;
    private readonly BoundsAndPositioningManager boundsManager;

    private readonly MovementProperties movementProperties;
    #endregion


    #region Targets And Targeing Variables
    private Transform target;

    private ScreenCorner currentCorner, nextCorenr;
    private Vector3 nextCorenerWorldPosition;
    #endregion


    #region Movement Scaling Variables and properties
    private float movementInterpolation = 1f; // The interpolation value to move the object to the next corner.
    private float movementSpeedScale = 1.5f; // The speed scale of the object movement.

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


    public void Move(Rigidbody rb)
    {
        target = movementController.CurrentTarget;

        if (target == null)
        {
            return;
        }


        if (!isMoving)
        {
            nextCorenr = boundsManager.FindBestNextCorner(currentCorner, target.position);

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
        /* Explaining the use of the following line:
         * 
         * Set a starting corner for the object to move to
         * so that it can determine the next corner as the object will always be moving to a corner.
         */
        currentCorner = boundsManager.GetNearestCorner(movementController.transform.position);
    }
}