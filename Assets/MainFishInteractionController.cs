using UnityEngine;

public class MainFishInteractionController : InteractionController
{
    public int currentNumberofEatenObjects = 0;
    //private MainFishHungerBehavior hungerSystem;

    public HungerSystem hungerSystem;


    protected override void Awake()
    {
        base.Awake();
        //hungerSystem = GetComponent<HungerSystem>();
    }


    protected override void InteractWithTarget(GameObject target)
    {
        if (GameManager.currentActiveFoodTargetObjectsList.Contains(target))
        {
            currentNumberofEatenObjects++;

            FoodProperties foodConfig = target.GetComponent<Food>().foodConfig;

            hungerSystem.hungerStrategy.SetHungerValues(
                 foodConfig.staminaTime,
              foodConfig.destructionTime
              );

            // Reset hunger
            hungerSystem.hungerStrategy?.ResetHunger();

            // Consume the target
            Destroy(target);
        }
    }
}
