using System.Collections.Generic;
using UnityEngine;

public class CheckForTargetHungerStrategy : IHungerStrategy
{
    private bool isHungry = false;

    private List<GameObject> targetObjectsList = new();

    public CheckForTargetHungerStrategy(IEnumerable<GameObject> _targetObjectsList)
    {
        targetObjectsList = _targetObjectsList as List<GameObject>;
    }

    public void HandleHungerState()
    {
        if (targetObjectsList.Count > 0)
        {
            isHungry = true;
        }
        else
        {
            isHungry = false;
        }
    }

    public bool GetHungerStatus()
    {
        return isHungry;
    }

    #region Not Implemented
    public void SetHungerValues(float _hungerStartingTime, float _destructionTime)
    {
        throw new System.NotImplementedException("Pet Set Hunger Values Exception");
    }

    public void ResetHunger()
    {
        throw new System.Exception("Pet Reset Hunger Exception");
    }
    #endregion
}