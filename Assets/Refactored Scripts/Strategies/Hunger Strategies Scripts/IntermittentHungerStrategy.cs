using UnityEngine;

public class IntermittentHungerStrategy : IHungerStrategy
{
    private float idleDuration;   // Time spent in idle state
    private float hungerDuration; // Time spent in hungry state

    [SerializeField] private float stateTimer = 0f;
    private bool isHungry = false;

    private float currentCycleDuration;

    public IntermittentHungerStrategy(float _idleDuration,float _hungerDuration)
    {
        idleDuration = _idleDuration;
        hungerDuration = _hungerDuration;

        currentCycleDuration = isHungry ? hungerDuration : idleDuration;
    }

    public void HandleHungerState()
    {
        stateTimer += Time.deltaTime;

        if (stateTimer >= currentCycleDuration)
        {
            ResetHunger();
        }
    }

    public void ResetHunger()
    {
        stateTimer = 0f;
        isHungry = !isHungry;
        currentCycleDuration = isHungry ? hungerDuration : idleDuration;
    }

    public bool GetHungerStatus() => isHungry;

    public void ReconfigureHungerTimingSettings(float _hungerStartingTime,float _destructionTime)
    {
        idleDuration = _hungerStartingTime;
        hungerDuration = _destructionTime;
    }
}
