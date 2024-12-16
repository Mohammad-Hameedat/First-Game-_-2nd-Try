public interface IHungerStrategy
{
    void HandleHungerState();
    bool GetHungerStatus();
    void ResetHunger();
    void SetHungerValues(float _hungerStartingTime,float _destructionTime);
}