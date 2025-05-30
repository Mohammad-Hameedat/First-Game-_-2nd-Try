public class EnemySensitivePetHungerStrategy : IHungerStrategy
{
    private bool isHungry = false;

    public void HandleHungerState()
    {
        if (GameManager.currentActiveEnemyObjectsList.Count > 0)
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
    public void ReconfigureHungerTimingSettings(float _hungerStartingTime, float _destructionTime)
    {
        throw new System.NotImplementedException("Pet Set Hunger Values Exception");
    }

    public void ResetHunger()
    {
        throw new System.Exception("Pet Reset Hunger Exception");
    }
    #endregion
}
