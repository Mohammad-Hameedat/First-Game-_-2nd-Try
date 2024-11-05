using UnityEngine;

public class EnemyInteractionController : InteractionController
{
    private Health health;
    private Hunger hunger;
    private EnemyProperties enemyProperties;
    private int currentNumberofEatenObjects = 0;
    private int numberOfObjectsToEat;
    private int nextNumberOfObjectsToEat;

    protected override void Awake()
    {
        base.Awake();
        health = GetComponent<Health>();
        hunger = GetComponent<Hunger>();
        enemyProperties = GetComponent<EnemyController>().enemyProperties;
        numberOfObjectsToEat = enemyProperties.numberOfObjectsToEat;
        nextNumberOfObjectsToEat = enemyProperties.nextNumberOfObjectsToEat;
    }

    protected override void InteractWithTarget(GameObject target)
    {
        if (GameManager.currentActiveFoodTargetObjectsList.Contains(target))
        {
            // Target is food
            Food foodInstance = target.GetComponent<Food>();
            health.TakeDamage(foodInstance.foodConfig.damage);
            Destroy(target);
        }
        else if (GameManager.currentActiveMainFishObjectsList.Contains(target))
        {
            // Target is main fish
            currentNumberofEatenObjects++;
            Destroy(target);
        }


        if (currentNumberofEatenObjects >= numberOfObjectsToEat)
        {
            // Enemy has eaten enough objects, adjust behavior
            numberOfObjectsToEat += nextNumberOfObjectsToEat;
            if (hunger != null)
            {
                hunger.ResetHunger();
            }
        }


        if (health.GetCurrentHealth() <= 0)
        {
            Destroy(gameObject);
        }
    }
}
