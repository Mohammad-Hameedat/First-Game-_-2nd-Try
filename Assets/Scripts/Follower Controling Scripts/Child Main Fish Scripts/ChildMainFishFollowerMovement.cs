using UnityEngine;

public class ChildMainFishFollowerMovement : BaseFollowerMovement
{
    protected override void Start()
    {
        baseFollowerController = GetComponent<ChildMainFishFollowerController>();
        base.Start();
    }

    private void FixedUpdate()
    {
        if (numberOfTargets > 0 && baseFollowerController.IsHungry())
        {
            MovingTowardsTarget();
        }
        else
        {
            MovingInRandomDirection();
        }
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
