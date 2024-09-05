using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    // Reference to the instance of the GameEvents class
    public static GameEvents eventsChannelInstance;

    // Events
    public event Action<int> onSpawnObject;

    public event Action<int> onUpdateCoins;

    public event Action onUpgradeFood;


    private void Awake()
    {
        if (eventsChannelInstance == null)
        {
            eventsChannelInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Invoking the event
    public void SpawnObjects(int objectType)
    {
        // If there are subscribers to the event, invoke the event
        onSpawnObject?.Invoke(obj: objectType);
    }

    public void UpdateInGameSceneMoney(int coins)
    {
        onUpdateCoins?.Invoke(coins);
    }

    public void UpgradeFood()
    {
        onUpgradeFood?.Invoke();
    }
}
