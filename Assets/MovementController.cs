using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour
{
    public FollowerProperties properties;
    public BoundsAndPositioningManager boundsManager { get; private set; }
    public StateMachine stateMachine { get; private set; }


    public IMovementStrategy movementStrategy;
    private Rigidbody rb;


    private void Awake()
    {
        boundsManager = GetComponent<BoundsAndPositioningManager>();
        stateMachine = GetComponent<StateMachine>();
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
