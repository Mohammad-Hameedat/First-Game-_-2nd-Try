using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button spawnObject;
    public Button upgradeFood;
    public Button upgradeWeapon;
    public Button UpgradeEgg_NextLevel;

    [Header("UI Texts")]
    public TextMeshProUGUI currentInSceneMoneyText;
    public TextMeshProUGUI currentInSceneMainFishesText;
    public TextMeshProUGUI eggCostText;


    void RefreshMoneyValue(int coinsValue)
    {
        currentInSceneMoneyText.text = coinsValue.ToString() + "$";
    }

    void RefreshMainFishesCounter(int mainFishesCounter)
    {
        currentInSceneMainFishesText.text = mainFishesCounter.ToString();
    }

    void RefreshEggCost(int eggCost)
    {
        eggCostText.text = eggCost.ToString() + "$";
    }


    private void OnEnable()
    {
        GameEvents.EventsChannelInstance.OnRefreshInSceneMoney += RefreshMoneyValue;
        GameEvents.EventsChannelInstance.OnRefreshMainFishesCounter += RefreshMainFishesCounter;
        GameEvents.EventsChannelInstance.OnRefreshEggCost += RefreshEggCost;

        spawnObject.onClick.AddListener(() => GameEvents.EventsChannelInstance.SpawnObjects(1));

        upgradeFood.onClick.AddListener(() => GameEvents.EventsChannelInstance.UpgradeFood());
        upgradeWeapon.onClick.AddListener(() => GameEvents.EventsChannelInstance.UpgradeWeapon());
        UpgradeEgg_NextLevel.onClick.AddListener(() => GameEvents.EventsChannelInstance.UpgradeEgg());
    }


    private void OnDisable()
    {
        GameEvents.EventsChannelInstance.OnRefreshInSceneMoney -= RefreshMoneyValue;
        GameEvents.EventsChannelInstance.OnRefreshMainFishesCounter -= RefreshMainFishesCounter;
        GameEvents.EventsChannelInstance.OnRefreshEggCost -= RefreshEggCost;

        // Unsubscribe all listeners from the buttons
        spawnObject.onClick.RemoveAllListeners();
        upgradeFood.onClick.RemoveAllListeners();
        upgradeWeapon.onClick.RemoveAllListeners();
        UpgradeEgg_NextLevel.onClick.RemoveAllListeners();
    }


}
