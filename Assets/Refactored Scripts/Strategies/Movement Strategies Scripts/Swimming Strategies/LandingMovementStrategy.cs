using UnityEngine;

public class LandingMovementStrategy : IMovementStrategy
{
    // Refs
    private readonly MovementController movementController;
    private readonly BoundsAndPositioningManager boundsManager;

    // Motion vars
    private Vector3 currentVelocity;
    private Vector3 startPosition;
    private Vector3 groundPosition;
    private Vector3 landingPosition;

    private float gravity = 9.81f;
    private float initialVerticalVelocity;
    private float initialHorizontalVelocity;

    private float elapsedTime = 0f;
    private bool hasLanded = false;

    // Ctor
    public LandingMovementStrategy(MovementController _movementController)
    {
        movementController = _movementController;
        boundsManager = movementController.boundsManager;

        InitializeMovementParameters();
        landingPosition = CalculateLandingPoint();  // Initial prediction
    }

    // The "Move" method is called via the "FixedUpdate" method of the MovementController.
    public void Move(Rigidbody rb)
    {
        if (hasLanded)
            return;

        // Recalculate landing each frame (using current velocity)
        landingPosition = CalculateLandingPoint();

        float slowMotionFactor = ComputeSlowMotionFactor(rb);
        elapsedTime += Time.fixedDeltaTime * slowMotionFactor;

        UpdatePosition(rb);
        UpdateVelocity(rb, slowMotionFactor);
    }

    // Init movement from Rigidbody
    private void InitializeMovementParameters()
    {
        Rigidbody rb = movementController.rb;

        // Start velocity & position
        currentVelocity = rb.velocity;
        startPosition = rb.position;

        // Separate velocity components
        initialVerticalVelocity = currentVelocity.y;
        initialHorizontalVelocity = currentVelocity.x;

        // Ground pos (bottom-center)
        groundPosition = boundsManager.CornerToWorldPosition(ScreenCorner.BottomCenter);
    }

    // Slow time based on horizontal fraction
    private float ComputeSlowMotionFactor(Rigidbody rb)
    {
        float totalDistance = Mathf.Abs(landingPosition.x - startPosition.x);
        if (totalDistance < Mathf.Epsilon)
            return 0.1f;

        float distanceTraveled = Mathf.Abs(rb.position.x - startPosition.x);
        float progressPercentage = Mathf.Clamp01(distanceTraveled / totalDistance);

        // Smooth deceleration, clamp to 0.1x
        return Mathf.Max(0.5f * ( 1f - progressPercentage * progressPercentage ), 0.1f);
    }

    // Update position using ballistic path
    private void UpdatePosition(Rigidbody rb)
    {
        float xPosition = startPosition.x + ( initialHorizontalVelocity * elapsedTime );
        float yPosition = startPosition.y + ( initialVerticalVelocity * elapsedTime )
                          - 0.5f * gravity * elapsedTime * elapsedTime;

        // Clamp Y to ground
        if (yPosition <= groundPosition.y)
        {
            yPosition = groundPosition.y;
            hasLanded = true;
        }

        rb.position = new Vector3(xPosition, yPosition, rb.position.z);
    }

    // Update velocity with gravity and slow-mo
    private void UpdateVelocity(Rigidbody rb, float slowMotionFactor)
    {
        if (!hasLanded)
        {
            float instantaneousVerticalVelocity = initialVerticalVelocity - gravity * elapsedTime;
            rb.velocity = new Vector3(
                initialHorizontalVelocity * slowMotionFactor,
                instantaneousVerticalVelocity * slowMotionFactor,
                0f
            );
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    // Calculate landing ignoring slow-mo
    private Vector3 CalculateLandingPoint()
    {
        Rigidbody rb = movementController.rb;

        // Optional: use latest velocity for recalculation
        currentVelocity = rb.velocity; // Uncomment if desired

        float fallAcceleration = -0.5f * gravity;
        float height = startPosition.y - groundPosition.y;
        float discriminant = initialVerticalVelocity * initialVerticalVelocity
                            - 4f * fallAcceleration * height;
        float sqrtDisc = Mathf.Sqrt(discriminant);

        // Two possible times; pick the larger
        float timeToFall1 = ( -initialVerticalVelocity + sqrtDisc ) / ( 2f * fallAcceleration );
        float timeToFall2 = ( -initialVerticalVelocity - sqrtDisc ) / ( 2f * fallAcceleration );
        float actualTimeToFall = Mathf.Max(timeToFall1, timeToFall2);

        // Use ballistic X formula here
        float landingX = startPosition.x + ( initialHorizontalVelocity * actualTimeToFall );

        // Original code doubled final X
        return new Vector3(landingX * 2f, groundPosition.y, startPosition.z);
    }
}