using System.Collections;

public class ChildSpecialPetMovement : BaseFollowerMovement
{
    protected override BaseFollowerController FollowerControllerSetter
    {
        get => base.FollowerControllerSetter;
        set => base.FollowerControllerSetter = value;
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }


    protected override void MovingTowardsTargetDirection()
    {
        base.MovingTowardsTargetDirection();
    }

    protected override void MovingInRandomDirection()
    {
        base.MovingInRandomDirection();
    }

    protected override void RandomMovementSpeed()
    {
        base.RandomMovementSpeed();
    }


    protected override void MovingTowardsTargetSpeed()
    {
        base.MovingTowardsTargetSpeed();
    }

    protected override IEnumerator CheckPosition()
    {
        return base.CheckPosition();
    }
}
