using UnityEngine;

public class ChildEnemyClorpController : ChildEnemyController
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }

    protected override void DetectAndDestroyNearestObjects()
    {
        base.DetectAndDestroyNearestObjects();
    }

    public override Vector3 CheckTargetDirection()
    {
        return base.CheckTargetDirection();
    }

    protected override Vector3 GetNearestObjectPosition()
    {
        return base.GetNearestObjectPosition();
    }

    protected override void HandleTargetObjectInteraction(GameObject targetObject)
    {
        base.HandleTargetObjectInteraction(targetObject);
    }

    protected override void HungerHandler()
    {
        base.HungerHandler();
    }

    public override int GetNumberOfTargetObjectsInList()
    {
        return base.GetNumberOfTargetObjectsInList();
    }

    public override bool IsHungry()
    {
        return base.IsHungry();
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
