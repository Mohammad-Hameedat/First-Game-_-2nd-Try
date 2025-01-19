using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour
{
    //public FollowerProperties properties;
    public MovementProperties movementProperties;
    public BoundsAndPositioningManager boundsManager { get; private set; }


    public IMovementStrategy movementStrategy;
    public Rigidbody rb;


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

    private void Update()
    {
        movementStrategy?.GetTarget();
    }

    // Set the movement strategy of the object
    public void SetMovementStrategy(IMovementStrategy strategy)
    {
        movementStrategy = strategy;
    }
}
