public interface IHungerStrategy
{
    void HandleHungerState();
    bool IsHungry();
    void ResetHunger();
    void SetHungerValues(float _hungerStartingTime,float _destructionTime);
}