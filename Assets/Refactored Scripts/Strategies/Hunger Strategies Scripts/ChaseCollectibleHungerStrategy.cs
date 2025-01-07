public class ChaseCollectibleHungerStrategy : IHungerStrategy
{
    private bool isHungry = false;

    public void HandleHungerState()
    {
        if (GameManager.currentActiveCollectiblesList.Count > 0)
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
