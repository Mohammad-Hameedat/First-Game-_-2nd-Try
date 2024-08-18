using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    // Singleton
    public static GameEvents eventsChannelInstance;

    // Events
    public event Action<int> onSpawnObject;
    public event Action<float> onUpdateCoins;

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

    public void UpdateInGameSceneMoney(float coins)
    {
        onUpdateCoins?.Invoke(coins);
    }

}
