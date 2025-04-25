public interface IHungerStrategy
{
    void HandleHungerState();
    bool GetHungerStatus();
    void ResetHunger();
    void ReconfigureHungerTimingSettings(float _hungerStartingTime,float _destructionTime);
}