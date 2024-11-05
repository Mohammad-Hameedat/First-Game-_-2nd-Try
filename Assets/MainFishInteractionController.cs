using UnityEngine;

public class MainFishInteractionController : InteractionController
{
    public int currentNumberofEatenObjects = 0;
    private Hunger hunger;


    protected override void Awake()
    {
        base.Awake();
        hunger = GetComponent<Hunger>();
    }


    protected override void InteractWithTarget(GameObject target)
    {
        if (GameManager.currentActiveFoodTargetObjectsList.Contains(target))
        {
            currentNumberofEatenObjects++;

            FoodProperties foodConfig = target.GetComponent<Food>().foodConfig;
            hunger.hungerStartingTime = foodConfig.staminaTime;
            hunger.destructionTime = foodConfig.destructionTime;

            // Consume the target
            Destroy(target);

            // Reset hunger
            hunger?.ResetHunger();
        }
    }
}
