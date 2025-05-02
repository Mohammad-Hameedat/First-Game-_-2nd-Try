using System.Collections.Generic;
using UnityEngine;

public class GeneralCheckHungerStrategy : IHungerStrategy
{
    private MovementController movementController;
    private TargetingSystem targetingSystem;

    private List<GameObject> targetObjectsList = new();

    private bool isHungry = false;

    public GeneralCheckHungerStrategy(
        MovementController _movementController,
        TargetingSystem _targetingSystem,
        IEnumerable<GameObject> _targetObjectsList
        )
    {
        movementController = _movementController;
        targetingSystem = _targetingSystem;
        targetObjectsList = _targetObjectsList as List<GameObject>;
    }

    public bool GetHungerStatus()
    {
        return isHungry;
    }

    public void HandleHungerState()
    {
        if (targetObjectsList.Count > 0)
        {
            targetingSystem.GetNearestTarget();
            isHungry = true;
        }
        else
        {
            movementController.CurrentTarget = null;
            isHungry = false;
        }
    }


    #region Not Implemented
    public void ReconfigureHungerTimingSettings(float _hungerStartingTime, float _destructionTime) { }

    public void ResetHunger() { }
    #endregion

}
