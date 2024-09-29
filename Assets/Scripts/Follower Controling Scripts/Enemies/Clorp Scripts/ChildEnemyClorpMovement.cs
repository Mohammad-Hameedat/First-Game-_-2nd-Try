using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildEnemyClorpMovement : ChildEnemyMovement
{

    protected override void Start()
    {
        base.Start();
        FollowerControllerSetter = GetComponent<ChildEnemyClorpController>();
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
