using UnityEngine;

public class ChildSpecialPetController : BaseFollowerController
{
    protected override void Start()
    {
        throw new System.NotImplementedException();
    }


    protected override void Update()
    {
        throw new System.NotImplementedException();
    }


    protected override Vector3 GetNearestObjectPosition()
    {
        return base.GetNearestObjectPosition();
    }


    public override int GetNumberOfTargetObjectsInList()
    {
        return base.GetNumberOfTargetObjectsInList();
    }


    public override Vector3 CheckTargetDirection()
    {
        return base.CheckTargetDirection();
    }


    public override bool IsHungry()
    {
        return base.IsHungry();
    }


    protected override void HandleTargetObjectInteraction(GameObject targetObject)
    {
        base.HandleTargetObjectInteraction(targetObject);
    }


    protected override void HungerHandler()
    {
        base.HungerHandler();
    }


    protected override void OnDisable()
    {
        throw new System.NotImplementedException();
    }

}
