using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoundsAndPositioningManager))]
public class MovementController : MonoBehaviour
{
    public MovementProperties movementProperties;
    public BoundsAndPositioningManager boundsManager { get; private set; }
    public Rigidbody rb;


    public IMovementStrategy movementStrategy;
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
