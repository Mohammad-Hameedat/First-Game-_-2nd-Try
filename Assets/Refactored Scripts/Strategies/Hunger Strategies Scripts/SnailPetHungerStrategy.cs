public class SnailPetHungerStrategy : IHungerStrategy
{
    private bool isHungry = false;
    //private bool isHungerEnabled = true;

    public void HandleHungerState()
    {
        if (GameManager.activeCollectablesList.Count > 0)
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
