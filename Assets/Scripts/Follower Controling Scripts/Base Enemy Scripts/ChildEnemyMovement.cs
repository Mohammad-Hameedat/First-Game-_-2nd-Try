using UnityEngine;

public class ChildEnemyMovement : BaseFollowerMovement
{
    protected bool coroutineRunning = false;

    protected override void Start()
    {
        FollowerControllerSetter = GetComponent<ChildEnemyController>();
        base.Start();
    }

    protected virtual void FixedUpdate()
    {
        if (numberOfTargetsInList > 0 && FollowerControllerSetter.IsHungry())
        {
            MovingTowardsTargetDirection();
        }
        else
        {
            MovingInRandomDirection();
        }
    }





    protected override void MovingTowardsTargetSpeed()
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
        Gizmos.DrawLine(transform.position, randomTargetPosition);


        //Gizmos.color = Color.green;
        //Gizmos.DrawLine(transform.position, followerController.CheckNearestTargetObject());
    }



}
