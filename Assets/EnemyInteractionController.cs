using UnityEngine;

public class EnemyInteractionController : InteractionController
{
    private Health health;

    protected override void Awake()
    {
        base.Awake();
        health = GetComponent<Health>();
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
            //currentNumberofEatenObjects++;
            Destroy(target);
        }


        if (health.GetCurrentHealth() <= 0)
        {
            Destroy(gameObject);
        }
    }
}
