public class ChildEnemyFoodEaterMovement : ChildEnemyMovement
{
    protected override void Start()
    {
        base.Start();
        FollowerControllerSetter = GetComponent<ChildEnemyFoodEaterController>();
    }
}
