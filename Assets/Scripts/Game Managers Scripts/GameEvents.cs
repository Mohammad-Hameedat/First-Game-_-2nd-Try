using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    // Reference to the instance of the GameEvents class
    public static GameEvents EventsChannelInstance;

    // Events
    public event Action OnUpgradeFood;
    public event Action OnUpgradeWeapon;
    public event Action<int> OnSpawnObject;
    public event Action<int> OnRefreshInGameSceneMoney;
    public event Action<int> OnRefreshMainFishesNumber;


    private void Awake()
    {
        if (EventsChannelInstance == null)
        {
            EventsChannelInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpgradeFood()
    {
        OnUpgradeFood?.Invoke();
    }

    public void UpgradeWeapon()
    {
        OnUpgradeWeapon?.Invoke();
    }

    // Invoking the event
    public void SpawnObjects(int objectType)
    {
        // If there are subscribers to the event, invoke the event
        OnSpawnObject?.Invoke(obj: objectType);
    }

    public void UpdateInGameSceneMoney(int coins)
    {
        OnRefreshInGameSceneMoney?.Invoke(coins);
    }

    public void RefresheMainFishesNumber(int mainFishesNumber)
    {
        OnRefreshMainFishesNumber?.Invoke(mainFishesNumber);
    }


    private void OnDisable()
    {
        OnUpgradeFood = null;
        OnUpgradeWeapon = null;
        OnSpawnObject = null;
        OnRefreshInGameSceneMoney = null;
        OnRefreshMainFishesNumber = null;
    }

}
