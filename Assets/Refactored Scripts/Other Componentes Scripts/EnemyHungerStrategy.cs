using System.Collections;
using UnityEngine;

public class EnemyHungerStrategy : IHungerStrategy
{
    private float idleDuration;   // Time spent in idle state
    private float hungerDuration; // Time spent in hungry state

    [SerializeField] private float stateTimer = 0f;
    private bool isHungry = false;

    private float currentCycleDuration;

    public EnemyHungerStrategy(float _idleDuration,float _hungerDuration)
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

    public bool IsHungry() => isHungry;

    public void SetHungerValues(float _hungerStartingTime,float _destructionTime)
    {
        MonoBehaviour.print("Setting Hunger Values");

        idleDuration = _hungerStartingTime;
        hungerDuration = _destructionTime;
    }
}
