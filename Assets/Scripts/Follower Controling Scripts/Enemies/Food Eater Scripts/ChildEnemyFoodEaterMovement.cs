using System.Collections;

public class ChildEnemyFoodEaterMovement : ChildEnemyMovement
{
    protected override void Start()
    {
        base.Start();
        FollowerControllerSetter = GetComponent<ChildEnemyFoodEaterController>();
    }

    protected override void Update()
    {
        base.Update();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    protected override void MovingTowardsTargetDirection()
    {
        base.MovingTowardsTargetDirection();
    }

    protected override void MovingInRandomDirection()
    {
        base.MovingInRandomDirection();
    }

    protected override void MovingTowardsTargetSpeed()
    {
        base.MovingTowardsTargetSpeed();
    }

    protected override void RandomMovementSpeed()
    {
        base.RandomMovementSpeed();
    }
    protected override IEnumerator CheckPosition()
    {
        return base.CheckPosition();
    }
}
