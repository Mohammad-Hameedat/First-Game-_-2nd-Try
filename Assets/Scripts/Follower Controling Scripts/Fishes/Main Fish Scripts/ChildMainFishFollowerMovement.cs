using System.Collections;

public class ChildMainFishFollowerMovement : BaseFollowerMovement
{
    protected override void Start()
    {
        FollowerControllerSetter = GetComponent<ChildMainFishFollowerController>();
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    private void FixedUpdate()
    {
        if (numberOfTargetsInList > 0 && FollowerControllerSetter.IsHungry() && GameManager.currentActiveEnemyObjectsList.Count == 0)
        {
            MovingTowardsTargetDirection();
        }
        else
        {
            MovingInRandomDirection();
        }
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




    /*
    // Draw a line to see the movement direction of the object
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 currentPosition = transform.position;
        float closerTargetsDetectionRangeThresholdSqr = FollowerControllerSetter.followerProperties.closeTargetsRangeThreshold * FollowerControllerSetter.followerProperties.closeTargetsRangeThreshold;

        // Check if the last nearest target object is still in the closer detection range of the follower
        if (FollowerControllerSetter.lastNearestTargetObject != null && (FollowerControllerSetter.lastNearestTargetObject.transform.position - currentPosition).sqrMagnitude <= closerTargetsDetectionRangeThresholdSqr)
        {
            Gizmos.DrawLine(transform.position, FollowerControllerSetter.lastNearestTargetObject.transform.position);
        }

        //Gizmos.color = Color.green;
        //Gizmos.DrawLine(transform.position, followerController.CheckNearestTargetObject());
    }
    */
}
