using UnityEngine;

public class MainFishHungerStrategy : IHungerStrategy
{
    private float hungerStartingTime;
    private float destructionTime;
    private float hungerTimeCounter = 0f;

    private bool isHungry = false;
    //private bool isHungerEnabled = true;

    private GameObject gameObject;


    public MainFishHungerStrategy(GameObject _gameObject,float _hungerStartingTime,float _destructionTime)
    {
        gameObject = _gameObject;
        hungerStartingTime = _hungerStartingTime;
        destructionTime = _destructionTime;
    }

    public void HandleHungerState()
    {
        hungerTimeCounter += Time.deltaTime;

        // If the object is not hungry and the hunger time counter is greater than the hunger starting time
        if (!isHungry && hungerTimeCounter >= hungerStartingTime)
        {
            // Set the object as hungry
            isHungry = true;
        }
        // If the object is hungry and the hunger time counter is greater than the destruction time
        else if (isHungry && hungerTimeCounter >= destructionTime)
        {
            // Destroy the object
            Object.Destroy(gameObject);
        }
    }

    public bool GetHungerStatus()
    {
        return isHungry;
    }

    public void SetHungerValues(float _hungerStartingTime,float _destructionTime)
    {
        hungerStartingTime = _hungerStartingTime;
        destructionTime = _destructionTime;
    }

    public void ResetHunger()
    {
        hungerTimeCounter = 0f;
        isHungry = false;
    }
}
