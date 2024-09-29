using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Button spawnObject;
    public Button upgradeFood;
    public TMPro.TextMeshProUGUI currenctAvailableMoney;
    public TMPro.TextMeshProUGUI numberOfMainFishesInScene;

    int moneyAmount = 0;
    int mainFishesNumber = 0;

    void GetMoneyValue(int coinsValue)
    {
        moneyAmount = coinsValue;
        currenctAvailableMoney.text = moneyAmount.ToString() + "$";
    }

    void GetMainFishesNumber(int mainFishesValue)
    {
        mainFishesNumber = mainFishesValue;
        numberOfMainFishesInScene.text = mainFishesNumber.ToString();
    }

    private void OnEnable()
    {
        GameEvents.EventsChannelInstance.OnRefreshInGameSceneMoney += GetMoneyValue;

        GameEvents.EventsChannelInstance.OnRefreshMainFishesNumber += GetMainFishesNumber;

        spawnObject.onClick.AddListener(() => GameEvents.EventsChannelInstance.SpawnObjects(1));
        upgradeFood.onClick.AddListener(() => GameEvents.EventsChannelInstance.UpgradeFood());
    }


    private void OnDisable()
    {
        GameEvents.EventsChannelInstance.OnRefreshInGameSceneMoney -= GetMoneyValue;
        GameEvents.EventsChannelInstance.OnRefreshMainFishesNumber -= GetMainFishesNumber;

        // Unsubscribe all listeners from the buttons
        spawnObject.onClick.RemoveAllListeners();
        upgradeFood.onClick.RemoveAllListeners();
    }


}
