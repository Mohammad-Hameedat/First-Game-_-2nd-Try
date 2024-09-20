using UnityEngine;

public class ChildMainFishFollowerMovement : BaseFollowerMovement
{
    protected override void Start()
    {
        FollowerControllerSetter = GetComponent<ChildMainFishFollowerController>();
        base.Start();
    }

    private void FixedUpdate()
    {
        if (numberOfTargetsInList > 0 && FollowerControllerSetter.IsHungry() && GameManager.enemiesTargetObjectsList.Count == 0)
        {
            MovingTowardsTargetDirection();
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
        Gizmos.DrawLine(transform.position, randomTargetPosition);


        //Gizmos.color = Color.green;
        //Gizmos.DrawLine(transform.position, followerController.CheckNearestTargetObject());
    }
}
