using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour
{
    //public FollowerProperties properties;
    public MovementProperties movementProperties;
    public BoundsAndPositioningManager boundsManager { get; private set; }


    public IMovementStrategy movementStrategy;
    public Rigidbody rb;

    /* NOTE: a shared property that can be set by the InteractionController.
     * The movement strategies will read from this to move the fish.
     */
    public Transform CurrentTarget { get; set; } = null;

    private void Awake()
    {
        boundsManager = GetComponent<BoundsAndPositioningManager>();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        movementStrategy?.Move(rb);
        transform.position = boundsManager.ClampPositionWithInView(transform.position);
    }

    // Set the movement strategy of the object
    public void SetMovementStrategy(IMovementStrategy strategy)
    {
        movementStrategy = strategy;
    }
}
