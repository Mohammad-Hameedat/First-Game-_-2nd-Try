using UnityEngine;

public class Hunger : MonoBehaviour
{
    public float hungerStartingTime;
    public float destructionTime;

    [SerializeField] private float hungerTimeCounter = 0f;
    private bool isHungry = false;

    /* Enable this if you want to enable/disable Awake Function
    private bool hungerEnabled = true;
    */

    /* Awake Function is not needed yet
    private void Awake()
    {
        hungerEnabled = hungerStartingTime > 0 && destructionTime > 0;
    }
    */

    private void Update()
    {
        HandleHungerState();
    }

    // Handle the hunger state of the object
    private void HandleHungerState()
    {
        /* Enable this if you want to enable/disable Awake Function
        if (!hungerEnabled) return; // If hunger is not enabled, do nothing
        */

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

/* This a modified version of the original script that is not needed yet

using UnityEngine;

public class Hunger : MonoBehaviour
{
    public float hungerStartingTime;
    public float destructionTime;

    [SerializeField] private float hungerTimeCounter = 0f;
    private bool isHungry = false;
    private bool hungerEnabled = true;

    MovementController movementController;

    private void Start()
    {
        movementController = GetComponent<MovementController>();
        hungerStartingTime = movementController.properties.hungerStartingTime;
        destructionTime = movementController.properties.destructionTime;
    }

    private void Update()
    {
        if (hungerStartingTime > movementController.properties.hungerStartingTime)
        {
            hungerEnabled = true;
        }
        else
        {
            hungerEnabled = false;
        }

        HandleHungerState();
    }

    // Handle the hunger state of the object
    private void HandleHungerState()
    {
        if (!hungerEnabled) return; // If hunger is not enabled, do nothing

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
*/