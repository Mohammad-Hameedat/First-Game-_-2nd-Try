using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    // Reference to the instance of the GameEvents class
    public static GameEvents EventsChannelInstance;

    // Events
    public event Action OnUpgradeFood;
    public event Action OnUpgradeWeapon;
    public event Action OnUpgradeEgg;

    public event Action<int> OnSpawnObject;

    public event Action<int> OnRefreshInSceneMoney;
    public event Action<int> OnRefreshMainFishesCounter;
    public event Action<int> OnRefreshEggCost;


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

    public void UpgradeEgg()
    {
        OnUpgradeEgg?.Invoke();
    }



    // Invoking the event
    public void SpawnObjects(int objectType)
    {
        // If there are subscribers to the event, invoke the event
        OnSpawnObject?.Invoke(objectType);
    }

    public void UpdateInGameSceneMoney(int inSceneMoney)
    {
        OnRefreshInSceneMoney?.Invoke(inSceneMoney);
    }

    public void RefresheMainFishesNumber(int mainFishesNumber)
    {
        OnRefreshMainFishesCounter?.Invoke(mainFishesNumber);
    }

    public void RefreshEggCost(int eggCost)
    {
        OnRefreshEggCost?.Invoke(eggCost);
    }


    private void OnDisable()
    {
        OnUpgradeFood = null;
        OnUpgradeWeapon = null;
        OnSpawnObject = null;
        OnRefreshInSceneMoney = null;
        OnRefreshMainFishesCounter = null;
        OnRefreshEggCost = null;
    }

}
