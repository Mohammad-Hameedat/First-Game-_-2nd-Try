using UnityEngine;

public class HungerSystem : MonoBehaviour
{
    public IHungerStrategy hungerStrategy { get; private set; }

    public void SetHungerBehavior(IHungerStrategy newHungerStrategy)
    {
        hungerStrategy = newHungerStrategy;
    }

    private void Update()
    {
        hungerStrategy?.HandleHungerState();
    }


    public bool IsHungry()
    {
        return hungerStrategy.GetHungerStatus();
    }

}