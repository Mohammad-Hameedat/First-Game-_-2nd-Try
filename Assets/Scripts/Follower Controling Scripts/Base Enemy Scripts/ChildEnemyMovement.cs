using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildEnemyMovement : BaseFollowerMovement
{
    protected bool coroutineRunning = false;

    protected override void Start()
    {
        followerControllerSetter = GetComponent<ChildEnemyController>();
        base.Start();
    }

    private void FixedUpdate()
    {

        if ((numberOfTargetsInList > 0 && followerControllerSetter.IsHungry()))
        {
            MovingTowardsTarget();
        }
        else
        {
            MovingInRandomDirection();
        }

    }

    protected override void MovementSpeed()
    {
        if (timeBeforeChangingVelocity < accelerationDuration)
        {
            timeBeforeChangingVelocity = accelerationDuration + 1f;
        }
        desiredVelocity = 4.5f;
    }


    // Draw a line to see the movement direction of the object
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, targetPosition);


        //Gizmos.color = Color.green;
        //Gizmos.DrawLine(transform.position, followerController.CheckNearestTargetObject());
    }



}
