using UnityEngine;

public class GTSHungerStrategy : IHungerStrategy
{
    private TargetingSystem targetingSystem;

    private float hungerStartingTime;
    private float hungerTimeCounter = 0f;



    public GTSHungerStrategy(
        TargetingSystem _targetingSystem,
        float _hungerStartingTime
        )
    {
        targetingSystem = _targetingSystem;
        hungerStartingTime = _hungerStartingTime;
    }

    public void HandleHungerState()
    {
        hungerTimeCounter += Time.deltaTime;

        if (hungerTimeCounter >= hungerStartingTime)
        {
            targetingSystem.GetNearestTarget();

            /* Important Note!!
             * An animation or Color change must be added here to indicate that the object is hungry
             */
        }
    }

    public bool GetHungerStatus()
    {
        return hungerTimeCounter >= hungerStartingTime ? true : false;
    }

    public void ReconfigureHungerTimingSettings(float _hungerStartingTime, float _destructionTime)
    {
        hungerStartingTime = _hungerStartingTime;
    }

    public void ResetHunger()
    {
        hungerTimeCounter = 0f;
    }
}
