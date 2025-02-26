using UnityEngine;

public class MainFishHungerStrategy : IHungerStrategy
{
    private GameObject gameObject;
    private TargetingSystem targetingSystem;

    private GameObject hungerCanvasObject;
    private HungerArrow hungerArrowScript;

    private float hungerStartingTime;
    private float destructionTime;
    private float hungerTimeCounter = 0f;

    private bool isHungry = false;


    public MainFishHungerStrategy(
        GameObject _gameObject,
        TargetingSystem _targetingSystem,
        float _hungerStartingTime,
        float _destructionTime
        )
    {
        gameObject = _gameObject;
        targetingSystem = _targetingSystem;
        hungerStartingTime = _hungerStartingTime;
        destructionTime = _destructionTime;

        InitializeHungerArrow();
    }

    public void HandleHungerState()
    {
        hungerTimeCounter += Time.deltaTime;

        // If the object is not hungry and the hunger time counter is greater than the hunger starting time
        if (!isHungry && hungerTimeCounter >= hungerStartingTime)
        {
            if (GameManager.cAPPetsDictionary.ContainsKey(PetType.BTPPet))
            {
                hungerCanvasObject.SetActive(true);
            }

            // Set the object as hungry
            isHungry = true;

            /* Important Note!!
             * An animation or Color change must be added here to indicate that the object is hungry
             */
        }
        // If the object is hungry and the hunger time counter is greater than the destruction time
        else if (isHungry)
        {
            hungerArrowScript.UpdateColorBasedOnHunger(hungerTimeCounter - hungerStartingTime);

            targetingSystem.GetNearestTarget();

            if (hungerTimeCounter >= destructionTime)
            {
                // Destroy the object ---> This must be replaced with disabling the object when adding the reviver pet object.
                Object.Destroy(gameObject);
            }
        }
    }

    public bool GetHungerStatus()
    {
        return isHungry;
    }

    public void SetHungerValues(float _hungerStartingTime, float _destructionTime)
    {
        hungerStartingTime = _hungerStartingTime;
        destructionTime = _destructionTime;

        hungerArrowScript.colorMaxValue = destructionTime - hungerStartingTime;
    }

    public void ResetHunger()
    {
        hungerTimeCounter = 0f;
        isHungry = false;

        hungerCanvasObject.SetActive(false);
        hungerArrowScript.RevertToInitialColor();
    }


    private void InitializeHungerArrow()
    {
        hungerArrowScript = gameObject.GetComponentInChildren<HungerArrow>();
        hungerArrowScript.colorMaxValue = destructionTime - hungerStartingTime;
        hungerArrowScript.RevertToInitialColor();

        //hungerArrowObject = hungerArrowScript.gameObject;
        //hungerArrowObject.SetActive(false);

        hungerCanvasObject = hungerArrowScript.GetComponentInParent<Canvas>().gameObject;
        hungerCanvasObject.SetActive(false);
    }
}



//using UnityEngine;

//public class MainFishHungerStrategy : IHungerStrategy
//{
//    private GameObject gameObject; // This game object.

//    private GameObject hungerArrowObject; // The sub object that will be used to indicate the hunger state of the object.
//    private HungerArrow hungerArrow;

//    private float hungerStartingTime;
//    private float destructionTime;
//    private float hungerTimeCounter = 0f;

//    private bool isHungry = false;


//    public MainFishHungerStrategy(GameObject _gameObject, float _hungerStartingTime, float _destructionTime)
//    {
//        gameObject = _gameObject;
//        hungerStartingTime = _hungerStartingTime;
//        destructionTime = _destructionTime;

//        InitializeHungerArrow();
//    }

//    public void HandleHungerState()
//    {
//        hungerTimeCounter += Time.deltaTime;

//        // If the object is not hungry and the hunger time counter is greater than the hunger starting time
//        if (!isHungry && hungerTimeCounter >= hungerStartingTime)
//        {
//            hungerArrowObject.SetActive(true);

//            // Set the object as hungry
//            isHungry = true;

//            /* Important Note!!
//             * An animation or Color change must be added here to indicate that the object is hungry
//             */
//        }
//        // If the object is hungry and the hunger time counter is greater than the destruction time
//        else if (isHungry && hungerTimeCounter >= destructionTime)
//        {
//            // Destroy the object ---> This must be replaced with disabling the object when adding the reviver pet object.
//            Object.Destroy(gameObject);
//        }
//    }

//    public bool GetHungerStatus()
//    {
//        return isHungry;
//    }

//    public void SetHungerValues(float _hungerStartingTime, float _destructionTime)
//    {
//        hungerStartingTime = _hungerStartingTime;
//        destructionTime = _destructionTime;


//        hungerArrowObject.SetActive(false);
//        hungerArrow.maxHealth = destructionTime - hungerStartingTime;

//        hungerArrow.SetInitialColor();
//    }

//    public void ResetHunger()
//    {
//        hungerTimeCounter = 0f;
//        isHungry = false;
//    }


//    private void InitializeHungerArrow()
//    {
//        hungerArrow = gameObject.GetComponentInChildren<HungerArrow>();
//        hungerArrow.maxHealth = destructionTime - hungerStartingTime;
//        hungerArrow.SetInitialColor();

//        hungerArrowObject = hungerArrow.gameObject;
//        hungerArrowObject.SetActive(false);
//    }
//}
