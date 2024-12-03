using UnityEngine;

public class MainFishHungerBehavior : MonoBehaviour
{
    public float hungerStartingTime;
    public float destructionTime;
    [SerializeField] private float hungerTimeCounter = 0f;

    public bool isHungerEnabled = true;
    private bool isHungry = false;


    private void Update()
    {
        HandleHungerState();
    }

    // Handle the hunger state of the object
    private void HandleHungerState()
    {
        if (!isHungerEnabled) return; // If hunger is not enabled, do nothing

        hungerTimeCounter += Time.deltaTime;
        if (!isHungry && hungerTimeCounter >= hungerStartingTime)
        {
            isHungry = true;
        }
        else if (isHungry && hungerTimeCounter >= destructionTime)
        {
            Destroy(gameObject);
        }
    }

    // Set the hunger time and destruction time of the object if the object eats something
    public void ResetHunger()
    {
        hungerTimeCounter = 0f;
        isHungry = false;
    }

    // Inform the controller if the object is hungry
    public bool IsHungry()
    {
        return isHungry;
    }
}