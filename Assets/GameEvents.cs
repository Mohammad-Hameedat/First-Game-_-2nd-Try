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
    public void SpawnObject(int objectType)
    {
        // If there are subscribers to the event, invoke the event
        onSpawnObject?.Invoke(objectType);
    }

}
